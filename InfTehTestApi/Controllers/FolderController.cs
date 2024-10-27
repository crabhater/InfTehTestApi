using InfTehTestApi.DB;
using InfTehTestApi.Models;
using InfTehTestApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace InfTehTestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FolderController : ControllerBase
    {
        private readonly FolderContext _context;
        public FolderController(FolderContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var baseFolder = new Folder()
            {
                FolderName = "Base"
            };

            var folder1 = new Folder
            {
                FolderName = "folder1",
                FolderId = 1,
            };
            var folder2 = new Folder
            {
                FolderName = "folder2",
                FolderId = 1,
            };

            _context.Folders.AddRange(baseFolder, folder1, folder2);
            await _context.SaveChangesAsync();

            var folder3 = new Folder
            {
                FolderId = folder1.Id,
                FolderName = "folder3"
            };

            _context.Folders.Add(folder3);
            await _context.SaveChangesAsync();
            
            var fileType1 = new FileType
            {
                TypeName = "txt",
                Icon = "\\file.png"
            };
            var fileType2 = new FileType
            {
                TypeName = "docx",
                Icon = "\\file.png"
            };

            _context.FileTypes.AddRange(fileType1, fileType2);
            await _context.SaveChangesAsync();

            var file1 = new FolderFile
            {
                FolderId = folder1.Id,
                Content = "Hello, World!",
                Description = "Привет, мир!",
                FileName = "file1",
                FileTypeId = fileType1.Id,
            };
            var file2 = new FolderFile
            {
                FolderId = folder1.Id,
                Content = "Goodbye, Cruel World!",
                Description = "Привет, мир!",
                FileName = "file2",
                FileTypeId = fileType1.Id,
            };
            var file3 = new FolderFile
            {
                FolderId = folder3.Id,
                Content = "Don't know what to say..",
                Description = "Привет, мир!",
                FileName = "file3",
                FileTypeId = fileType2.Id,
            };

            _context.FolderFiles.AddRange(file1, file2, file3);
            await _context.SaveChangesAsync();


            return Ok();
        }

    

        [HttpGet]
        public async Task<ActionResult<List<FolderViewModel>>> GetFolders(int parentFolderId = 1)
        {
            //var folders = await _context.Folders
            //    .Where(f => f.FolderId == parentFolderId)
            //    .Select(f => new FolderViewModel
            //    {
            //        Id = f.Id,
            //        Name = f.FolderName,
            //        ParentFolderId = f.FolderId,
            //        Child = f.Files.Select(file => new FolderFileViewModel
            //        {
            //            Id = file.Id,
            //            Name = file.FileName,
            //            Description = file.Description,
            //            FileTypeId = file.FileTypeId,
            //            FileTypeName = file.FileType.TypeName,
            //            Icon = file.FileType.Icon,
            //            FolderId = file.FolderId
            //        }).ToList()
            //    })
            //    .ToListAsync();
            return Ok();//folders);
        }

        [HttpGet("{folderId}")]
        public async Task<ActionResult<FolderFileViewModel>> GetFolderContent(int folderId = 1)
        {
            var content = await _context.Folders.Where(f => f.Id == folderId)
                                                .Include(f => f.Folders)
                                                .Include(f => f.Files)
                                                .ThenInclude(f => f.FileType)
                                                .Select(f => new TreeViewVM
                                                {
                                                    Id = f.Id,
                                                    Name = f.FolderName,
                                                    TypeId = 0,
                                                    Child = new ObservableCollection<TreeViewVM>(new List<TreeViewVM>().Concat( 
                                                    f.Files.Select(file => new TreeViewVM
                                                    {
                                                        Id = file.Id,
                                                        TypeId = file.FileTypeId,
                                                        Name = file.FileName,
                                                        Description = file.Description,
                                                        TypeName = file.FileType.TypeName,
                                                        Icon = file.FileType.Icon,
                                                        FolderId = file.FolderId,
                                                    }).ToList()).Concat(
                                                     f.Folders.Select(subFolder => new TreeViewVM
                                                    {
                                                        Id = subFolder.Id,
                                                        TypeId = 0,
                                                        Name = subFolder.FolderName,
                                                        Icon = "\\folder.png",
                                                        FolderId = folderId,
                                                    }).ToList()))
                                                }).FirstOrDefaultAsync();

            return  Ok(content);
        }

        [HttpGet("{folderId}")]
        public async Task<ActionResult<List<FolderFileViewModel>>> GetFiles(int folderId = 1)
        {
            var files = await _context.FolderFiles
                .Where(f => f.FolderId == folderId)
                .Include(f => f.FileType)
                .Select(f => new FolderFileViewModel
                {
                    Id = f.Id,
                    Name = f.FileName,
                    Description = f.Description,
                    FileTypeId = f.FileTypeId,
                    FileTypeName = f.FileType.TypeName,
                    Icon = f.FileType.Icon,
                    FolderId = f.FolderId
                })
                .ToListAsync();
            return Ok(files);
        }

        [HttpGet("{fileId}")]
        public async Task<ActionResult<FolderFileViewModel>> GetFileContent(int fileId)
        {
            var file = await _context.FolderFiles
                .Where(f => f.Id == fileId)
                .Include(f => f.FileType)
                .Select(f => new TreeViewVM
                {
                    Id = f.Id,
                    Name = f.FileName,
                    Description = f.Description,
                    TypeId = f.FileTypeId,
                    TypeName = f.FileType.TypeName,
                    Icon = f.FileType.Icon,
                    Content = f.Content
                })
                .FirstOrDefaultAsync();

            if (file == null)
            {
                return NotFound();
            }

            return Ok(file);
        }

        [HttpPost]
        public async Task<ActionResult<FolderFile>> AddFile(FolderFile file)
        {
            await _context.FolderFiles.AddAsync(file);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFileContent), new { id = file.Id }, file);
        }

        [HttpPost]
        public async Task<ActionResult<Folder>> AddFolder(Folder folder)
        {
            await _context.Folders.AddAsync(folder);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFolders), new { id = folder.Id }, folder);
        }

        [HttpPost("filetype")]
        public async Task<ActionResult<FileType>> AddFileType(FileType fileType)
        {
            await _context.FileTypes.AddAsync(fileType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFolders), new { id = fileType.Id }, fileType);
        }

        [HttpPut("file/{id}")]
        public async Task<IActionResult> UpdateFile(int id, FolderFile file)
        {
            if (id != file.Id)
            {
                return BadRequest();
            }

            _context.Entry(file).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("folder/{id}")]
        public async Task<IActionResult> UpdateFolder(int id, Folder folder)
        {
            if (id != folder.Id)
            {
                return BadRequest();
            }

            _context.Entry(folder).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("file/{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var file = await _context.FolderFiles.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            _context.FolderFiles.Remove(file);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("folder/{id}")]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            var folder = await _context.Folders.FindAsync(id);
            if (folder == null)
            {
                return NotFound();
            }

            _context.Folders.Remove(folder);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
