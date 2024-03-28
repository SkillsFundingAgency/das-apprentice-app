using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Models
{
    public class UserTests
    {
        [Test, MoqAutoData]
        public void Then_The_User_JsonProperties_Are_Valid(User user)
        {
            string userObject = JsonConvert.SerializeObject(user);

            using (new AssertionScope())
            {
                userObject.Should().Contain("sub");
                userObject.Should().Contain("email");
            }
        }
    }
}
