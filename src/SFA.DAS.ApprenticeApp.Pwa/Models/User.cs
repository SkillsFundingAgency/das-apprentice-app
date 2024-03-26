using Newtonsoft.Json;

namespace SFA.DAS.ApprenticeApp.Pwa.Models
{
    public class User
    {
        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("phone_number_verified")]
        public bool PhoneNumberVerified { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

    }
}