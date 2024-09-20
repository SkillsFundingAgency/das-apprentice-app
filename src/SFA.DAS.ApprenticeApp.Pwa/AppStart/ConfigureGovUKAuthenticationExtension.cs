using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SFA.DAS.ApprenticeApp.Application;
using SFA.DAS.ApprenticeApp.AppStart;
using SFA.DAS.ApprenticeApp.Pwa.Configuration;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.ApprenticeApp.Pwa.Authentication;

namespace SFA.DAS.ApprenticeApp.Pwa.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class ConfigureGovUKAuthenticationExtension
    {
        public static ApplicationConfiguration _config { get; set; }

        public static void AddAndConfigureApprenticeAuthentication(this IServiceCollection services, ApplicationConfiguration config)
        {
            _config = config;
           
            bool.TryParse(_config.StubAuth, out var stubAuth);
            if(stubAuth)
            {
                services.AddApprenticeStubAuthentication("/signed-out","/Account/SignIn", "", "");
            }
            else
            {
                services.AddAuthentication(options =>
                {
                    ConfigureSharedAuthenticationOptions(options);
                })
                    .AddOpenIdConnect(options => { ConfigureOpenIdConnectOptions(options); })
                    .AddCookie(options => { ConfigureCookieOptions(options); });
            }
            services.AddAuthorization(o =>
            {
                o.AddPolicy(PolicyNames.Default, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(RoleNames.Default);
                });
            });
        }

        [ExcludeFromCodeCoverage]
        private static void ConfigureSharedAuthenticationOptions(AuthenticationOptions options)
        {
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        }

        [ExcludeFromCodeCoverage]
        private static void ConfigureOpenIdConnectOptions(OpenIdConnectOptions options)
        {
            options.ClientId = _config.ClientId;
            options.MetadataAddress = _config.MetadataAddress;
           // options.ResponseType = OpenIdConnectResponseType.Code;
            options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
            options.SignedOutRedirectUri = "/";
            options.SignedOutCallbackPath = "/signed-out";
            options.CallbackPath = "/sign-in";
            options.ResponseMode = string.Empty;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                AuthenticationType = "private_key_jwt",
                ValidateIssuerSigningKey = false,
                ValidateIssuer = true,
                ValidateAudience = true,
                SaveSigninToken = true
            };

            options.SaveTokens = true;
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("email");
            options.Scope.Add("phone");

            options.Events = new OpenIdConnectEvents
            {
                OnRemoteFailure = HandleRemoteFailure,
                OnAuthorizationCodeReceived = HandleAuthorizationCodeReceived,
                OnTokenValidated = PopulateAccountsClaim,
                OnSignedOutCallbackRedirect = HandleSignedOutCallbackRedirect
            };
        }

        [ExcludeFromCodeCoverage]
        private static void ConfigureCookieOptions(CookieAuthenticationOptions options)
        {
            options.AccessDeniedPath = "/error/403";
            options.ExpireTimeSpan = TimeSpan.FromHours(1);
            options.Cookie.Name = _config.CookieName;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.SlidingExpiration = true;
            options.Cookie.SameSite = SameSiteMode.None;
            options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
            options.LogoutPath = "/account/sign-out";
        }

        [ExcludeFromCodeCoverage]
        private static Task HandleRemoteFailure(RemoteFailureContext context)
        {
            if (context.Failure.Message.Contains("Correlation failed"))
            {
                context.Response.Redirect("/");
                context.HandleResponse();
            }

            return Task.CompletedTask;
        }

        [ExcludeFromCodeCoverage]
        private static Task HandleAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            try
            {
                var privateKey = LoadPrivateKey(_config.KeyDir);
                var signingCredentials = new SigningCredentials(new RsaSecurityKey(privateKey), SecurityAlgorithms.RsaSha256);

                var handler = new JwtSecurityTokenHandler();
                var jti = Guid.NewGuid().ToString();
                var value = handler.CreateJwtSecurityToken(_config.ClientId,
                    _config.TokenUri, new ClaimsIdentity(
                        new List<Claim>
                        {
                                    new ("sub",_config.ClientId),
                                    new ("jti", jti)

                        }),
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddMinutes(5),
                    DateTime.UtcNow,
                    signingCredentials
                   );

                var client = new HttpClient();

                var requestUri = $"{_config.TokenUri}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Headers =
                            {
                                Accept = { new MediaTypeWithQualityHeaderValue("*/*"), new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded") },
                                UserAgent = { new ProductInfoHeaderValue(_config.ProductInfoHeaderValue,"1") },
                            }
                };

                httpRequestMessage.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                        {
                            new ("grant_type","authorization_code"),
                            new ("code",context.TokenEndpointRequest.Code),
                            new ("redirect_uri",context.TokenEndpointRequest.RedirectUri),
                            new ("client_assertion_type","urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
                            new ("client_assertion",value.RawData),
                        });

                httpRequestMessage.Content.Headers.Clear();
                httpRequestMessage.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                var response = client.Send(httpRequestMessage);
                var valueString = response.Content.ReadAsStringAsync().Result;
                var content = JsonConvert.DeserializeObject<Token>(valueString);
                context.HandleCodeRedemption(content.AccessToken, content.IdToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Task.CompletedTask;
        }

        [ExcludeFromCodeCoverage]
        private static async Task PopulateAccountsClaim(TokenValidatedContext context)
        {
            var accessToken = context.TokenEndpointResponse.Parameters["access_token"];
            var client = new HttpClient();

            var requestUri = $"{_config.UserInfoUri}";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri)
            {
                Headers =
            {
                UserAgent = { new ProductInfoHeaderValue(_config.ProductInfoHeaderValue,"1") },
                Authorization = new AuthenticationHeaderValue("Bearer",accessToken)
            }
            };
            var response = await client.SendAsync(httpRequestMessage);
            var valueString = response.Content.ReadAsStringAsync().Result;
            var content = JsonConvert.DeserializeObject<User>(valueString);

            context.Principal?.Identities.First().AddClaim(new Claim("email", content.Email));
            context.Principal?.Identities.First().AddClaim(new Claim(Constants.ApprenticeIdClaimKey, $"fd0daf58-af19-440d-b52f-7e1d47267d3b"));
        }

        [ExcludeFromCodeCoverage]
        private static Task HandleSignedOutCallbackRedirect(RemoteSignOutContext context)
        {
            context.Response.Cookies.Delete(_config.CookieName);
            context.Response.Redirect("/Account/Login");
            context.HandleResponse();
            return Task.CompletedTask;
        }

        [ExcludeFromCodeCoverage]
        private static Task<string> AuthenticationCallback(string authority, string resource, string scope)
        {
            var azureResource = "https://vault.azure.net/.default";
            var chainedTokenCredential = new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new AzureCliCredential());

            var token = chainedTokenCredential.GetTokenAsync(
                new TokenRequestContext(scopes: new[] { azureResource })).Result;

            return Task.FromResult(token.Token);
        }

        [ExcludeFromCodeCoverage]
        private static RSA LoadPrivateKey(string filePath)
        {
            var privateKeyText = File.ReadAllText(filePath);
            var privateKey = RSA.Create();
            privateKey.ImportFromPem(privateKeyText.ToCharArray());
            return privateKey;
        }
    }
}