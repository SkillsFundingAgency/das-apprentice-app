using Newtonsoft.Json;

namespace SFA.DAS.ApprenticeApp.Pwa.Models
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("id_token")]
        public string? IdToken { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }
    }
}