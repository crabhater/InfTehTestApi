using System.Collections.ObjectModel;

namespace InfTehTestApi.ViewModel
{
    public interface IBaseVm
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? FolderId { get; set; }
        public string? Icon { get; set; }
        public string? FileTypeName { get; set; }
        public string? Description { get; set; }
    }
}
