namespace InfTehTestApi.Models
{
    public class Folder
    {
        public int Id { get; set; }
        public List<FolderFile>? Files { get; set; }
        public string FolderName { get; set; }
        public int? ParentFolderId { get; set; }
    }
}
