using Newtonsoft.Json;

namespace SFA.DAS.ApprenticeApp.Pwa.Models
{
    public class User
    {
        [JsonProperty("sub")]
        public string? Sub { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

    }
}