namespace SFA.DAS.ApprenticeApp.Domain.Models
{
    public class ApprenticeTaskFile
    {
        public int TaskId { get; set; }
        public int? TaskFileId { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public byte[] FileContents { get; set; }
    }
}
