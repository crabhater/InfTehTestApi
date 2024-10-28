using System.Collections.ObjectModel;

namespace InfTehTestApi.ViewModel
{
    public class TreeViewVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? TypeId { get; set; }
        public int FolderId { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public ObservableCollection<TreeViewVM>? Child { get; set; }
        public string? Icon { get; set; }
        public string? Content { get; set; }
    }
}
