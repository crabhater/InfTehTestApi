﻿using System.Collections.ObjectModel;

namespace InfTehTestApi.ViewModel
{
    public class FolderViewModel : IBaseVm
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<IBaseVm>? Child { get; set; }
        //public List<FolderFileViewModel> Files { get; set; }
        public string? Icon { get; set; }
        public int? FolderId { get; set; }
        public string? FileTypeName { get; set; }
        public string? Description { get; set; }
    }
}
