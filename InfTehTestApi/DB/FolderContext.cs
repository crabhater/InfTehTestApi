using InfTehTestApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InfTehTestApi.DB
{
    public class FolderContext : DbContext
    {
        public FolderContext(DbContextOptions<FolderContext> options):base(options) { }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<FolderFile> FolderFiles { get; set; }
        public DbSet<FileType> FileTypes { get; set; }
        public FolderContext() 
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
