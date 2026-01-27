using System;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class DeleteTaskViewModel
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public DateTime? DueDate { get; set; }
        public int StatusId { get; set; }
    }
}