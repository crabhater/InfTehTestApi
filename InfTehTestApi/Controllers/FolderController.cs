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
                Icon = "M14,2L20,8V20A2,2 0 0,1 18,22H6A2,2 0 0,1 4,20V4A2,2 0 0,1 6,2H14M18,20V9H13V4H6V20H18M17,13V19H7L12,14L14,16M10,10.5A1.5,1.5 0 0,1 8.5,12A1.5,1.5 0 0,1 7,10.5A1.5,1.5 0 0,1 8.5,9A1.5,1.5 0 0,1 10,10.5Z"
            };
            var fileType2 = new FileType
            {
                TypeName = "docx",
                Icon = "M14,2L20,8V20A2,2 0 0,1 18,22H6A2,2 0 0,1 4,20V4A2,2 0 0,1 6,2H14M18,20V9H13V4H6V20H18M17,13V19H7L12,14L14,16M10,10.5A1.5,1.5 0 0,1 8.5,12A1.5,1.5 0 0,1 7,10.5A1.5,1.5 0 0,1 8.5,9A1.5,1.5 0 0,1 10,10.5Z"
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
        public async Task<ActionResult<FolderViewModel>> GetFolderFiles(int folderId = 1)
        {
            var content = await _context.Folders.Where(f => f.Id == folderId)
                                                .Include(f => f.Files)
                                                .ThenInclude(f => f.FileType)
                                                .Select(f => new FolderViewModel()
                                                {
                                                    Child = new List<IBaseVm>
                                                    (
                                                        f.Files.Select(file => new FolderFileViewModel
                                                        {
                                                            Id= file.Id,
                                                            FolderId= file.FolderId,
                                                            Name = file.FileName,
                                                            Description = file.Description,
                                                            Content = file.Content,
                                                            FileTypeName = file.FileType.TypeName,
                                                            Icon = file.FileType.Icon
                                                        })
                                                    )
                                                }).FirstOrDefaultAsync();

            return  Ok(content.Child);
        }
        [HttpGet("{folderId}")]
        public async Task<ActionResult<FolderViewModel>> GetFolderFolders(int folderId = 1)
        {
            var content = await _context.Folders.Where(f => f.Id == folderId)
                                                .Include(f => f.Folders)
                                                .Select(f => new FolderViewModel()
                                                {
                                                    Child = new List<IBaseVm>
                                                    (
                                                        f.Folders.Select(folder => new FolderViewModel
                                                        {
                                                            Id = folder.Id,
                                                            FolderId = folder.FolderId,
                                                            Name = folder.FolderName,
                                                        })
                                                    )
                                                }).FirstOrDefaultAsync (); 
            return Ok(content.Child);
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
                .Select(f => new FolderFileViewModel
                {
                    Id = f.Id,
                    Name = f.FileName,
                    Description = f.Description,
                    FileTypeName = f.FileType.TypeName,
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
        public async Task<IActionResult> AddFile(FolderFileViewModel file)
        {
            var fileType = await _context.FileTypes.Where(e => e.TypeName == file.FileTypeName)
                                                   .FirstOrDefaultAsync();
            if (fileType == null)
            {
                fileType = new FileType
                {
                    Icon = file.Icon,
                    TypeName = file.FileTypeName
                };
                await _context.FileTypes.AddAsync(fileType);
                await _context.SaveChangesAsync();
            }

            var newFile = new FolderFile
            {
                FolderId = file.FolderId,
                FileName = file.Name,
                FileTypeId = fileType.Id,
                Content = file.Content,
                Description = file.Description
            };

            await _context.FolderFiles.AddAsync(newFile);

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddFolder(FolderViewModel folder)
        {
            var newFolder = new Folder()
            {
                FolderId = folder.FolderId,
                FolderName = folder.Name,
            };
            await _context.Folders.AddAsync(newFolder);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("filetype")]
        public async Task<ActionResult<FileType>> AddFileType(FileType fileType)
        {
            await _context.FileTypes.AddAsync(fileType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFolders), new { id = fileType.Id }, fileType);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFile(FolderFileViewModel file)
        {
            var fileType = await _context.FileTypes.Where(e => e.TypeName == file.FileTypeName)
                                                   .FirstOrDefaultAsync();
            if (fileType == null)
            {
                fileType = new FileType
                {
                    Icon = file.Icon,
                    TypeName = file.FileTypeName
                };
                await _context.FileTypes.AddAsync(fileType);
                await _context.SaveChangesAsync();
            }


            var newFile = new FolderFile()
            {
                FileName = file.Name,
                Id = file.Id,
                FolderId = file.FolderId,
                FileTypeId = fileType.Id,
                Content = file.Content,
                Description = file.Description,
            };

            _context.Entry(newFile).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFolder(FolderViewModel folder)
        {
            var newFolder = new Folder()
            {
                FolderName = folder.Name,
                Id = folder.Id,
                FolderId = folder.FolderId,
            };

            _context.Entry(newFolder).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
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

        [HttpDelete("{id}")]
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
