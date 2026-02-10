using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class Courses
    {
        public string? StandardUId { get; set; }
        
        public required string Title { get; set; }
    }
}
