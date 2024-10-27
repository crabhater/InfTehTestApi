using InfTehTestApi.Controllers;
using InfTehTestApi.DB;
using InfTehTestApi.Models;
using InfTehTestApi.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfTehTestApi_tests
{
    public class FilesControllerTests
    {
        private readonly FolderController _controller;
        private readonly FolderContext _context;

        public FilesControllerTests()
        {
            var options = new DbContextOptionsBuilder<FolderContext>().UseNpgsql("Host=localhost;Port=5432;Database=InfTehDB;Username=postgres;Password=123")
                                                                      .Options;

            _context = new FolderContext(options);
            _controller = new FolderController(_context);
        }

        [Fact]
        public async Task GetFolders_ReturnsListOfFolders()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            // Arrange
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

            // Act
            var result = await _controller.GetFolders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<FolderViewModel>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetFiles_ReturnsListOfFiles()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            // Arrange

            var folder1 = new Folder
            {
                FolderName = "folder1",
            };

            _context.Folders.Add(folder1);
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

            _context.FolderFiles.AddRange(file1, file2);
            await _context.SaveChangesAsync();


            // Act
            var result = await _controller.GetFiles(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<FolderFileViewModel>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetFileContent_ReturnsFileContent()
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            // Arrange

            var folder1 = new Folder
            {
                FolderName = "folder1",
            };

            _context.Folders.Add(folder1);
            await _context.SaveChangesAsync();

            var file = new FolderFile {FileName = "file1", Content = "Sample Content", FolderId = folder1.Id};
            _context.FolderFiles.Add(file);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetFileContent(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<FolderFileViewModel>(okResult.Value);
            Assert.Equal("Sample Content", returnValue.Content);
        }
    }

}
