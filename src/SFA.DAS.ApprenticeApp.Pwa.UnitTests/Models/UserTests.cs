using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Models
{
    public class UserTests
    {
        [Test, MoqAutoData]
        public async Task Then_The_User_JsonProperties_Are_Valid(User user)
        {
            string userObject = JsonConvert.SerializeObject(user);

            using (new AssertionScope())
            {
                userObject.Should().Contain("sub");
                userObject.Should().Contain("phone_number_verified");
                userObject.Should().Contain("phone_number");
                userObject.Should().Contain("email_verified");
                userObject.Should().Contain("email");
            }
        }
    }
}
