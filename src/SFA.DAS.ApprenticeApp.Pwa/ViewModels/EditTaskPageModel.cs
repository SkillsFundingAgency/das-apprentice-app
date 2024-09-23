﻿using SFA.DAS.ApprenticeApp.Domain.Models;

namespace SFA.DAS.ApprenticeApp.Pwa.ViewModels
{
    public class EditTaskPageModel
    {
        public ApprenticeTask? Task { get; set; }
        public List<ApprenticeshipCategory>? Categories { get; set; }
        public List<ApprenticeKsbData>? KsbProgressData { get; set; }
        public string? LinkedKsbGuids { get; set; }
        public int? StatusId { get; set; }
    }
}