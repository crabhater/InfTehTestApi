using System.Collections.ObjectModel;

namespace InfTehTestApi.ViewModel
{
    public class FolderViewModel : TreeViewVM
    {
        public int Id { get; set; }
        public int? TypeId { get; set; }
        public string Name { get; set; }
        public ObservableCollection<TreeViewVM> Child { get; set; }
        public int? ParentFolderId { get; set; }
        public string Description { get => null; set => throw new NotImplementedException(); }
        public string Content { get => null; set => throw new NotImplementedException(); }
        public string Icon { get => "\\folder.png"; set => throw new NotImplementedException(); }
    }
}
