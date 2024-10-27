using System.Collections.ObjectModel;

namespace InfTehTestApi.ViewModel
{
    public class FolderFileViewModel : TreeViewVM
    {
        public int Id { get; set; }
        public int? TypeId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? FileTypeId { get; set; }
        public string FileTypeName { get; set; }
        public string? Icon { get; set; }
        public int FolderId { get; set; }
        public string? Content { get; set; }
        public ObservableCollection<TreeViewVM> Child { get { return null; } set => throw new NotImplementedException(); }
    }
}
