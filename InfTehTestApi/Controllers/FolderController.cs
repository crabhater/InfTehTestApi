using InfTehTestApi.DB;
using InfTehTestApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                ParentFolderId = 1,
            };
            var folder2 = new Folder
            {
                FolderName = "folder2",
                ParentFolderId = 1,
            };

            _context.Folders.AddRange(folder1, folder2);
            await _context.SaveChangesAsync();

            var folder3 = new Folder
            {
                ParentFolderId = folder1.Id,
                FolderName = "folder3"
            };

            _context.Folders.Add(folder3);
            await _context.SaveChangesAsync();
            
            var fileType1 = new FileType
            {
                TypeName = "txt",
                Icon = "stub"
            };
            var fileType2 = new FileType
            {
                TypeName = "docx",
                Icon = "stub"
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

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Folder>>> GetFolders(int parentFolderId = 1)
        {
            var folders = await _context.Folders.Where(f => f.ParentFolderId == parentFolderId)
                                                .ToListAsync();

            return Ok(folders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<FolderFile>>> GetFiles(int folderId = 1)
        {
            var files = await _context.FolderFiles.Where(f => f.FolderId == folderId)
                                                  .Include(t => t.FileType)
                                                  .Select(f => new FolderFile
                                                  {
                                                      Id = f.Id,
                                                      FileName = f.FileName,
                                                      Description = f.Description,
                                                      FileTypeId = f.FileTypeId,
                                                      FileType = f.FileType,
                                                      FolderId = f.FolderId,
                                                      //Content = f.Content, //Пробуем не перегружать систему коллекцией тяжелых файлов
                                                  })
                                                  .ToListAsync();

            return Ok(files);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FolderFile>> GetFileContent(int fileId)
        {
            var file = await _context.FolderFiles.Where(f => f.Id == fileId)
                                                 .FirstOrDefaultAsync();

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
        public async Task<ActionResult<FolderFile>> AddFolder(Folder folder)
        {
            await _context.Folders.AddAsync(folder);
            await _context.SaveChangesAsync();

            return Ok();//TODO createdataction
        }
        [HttpPost]
        public async Task<ActionResult<FolderFile>> AddFileType(FileType ft)
        {
            await _context.FileTypes.AddAsync(ft);
            await _context.SaveChangesAsync();

            return Ok();//TODO createdataction
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFile(int id, FolderFile file)
        {
            _context.Entry(file).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFolder(int id, Folder folder)
        {
            _context.Entry(folder).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var file = await _context.FolderFiles.FindAsync(id);
            if(file == null)
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
