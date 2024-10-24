using Microsoft.VisualBasic.FileIO;

namespace InfTehTestApi.Models
{
    public class FolderFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string? Description { get; set; }
        public int? FileTypeId { get; set; }
        public FileType FileType { get; set; }
        public int FolderId { get; set; }
        public string? Content { get; set; }
    }
}
