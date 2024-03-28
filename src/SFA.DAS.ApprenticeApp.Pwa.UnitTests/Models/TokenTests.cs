using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Models
{
    public class TokenTests
    {
        [Test, MoqAutoData]
        public void Then_The_User_JsonProperties_Are_Valid(Token token)
        {
            string tokenObject = JsonConvert.SerializeObject(token);

            using (new AssertionScope())
            {
                tokenObject.Should().Contain("access_token");
                tokenObject.Should().Contain("id_token");
                tokenObject.Should().Contain("token_type");
            }
        }
    }
}
