using Microsoft.EntityFrameworkCore;

namespace SealAPI.Context
{
    public class FileContext : DbContext
    {
        public FileContext(DbContextOptions<FileContext> options) : base(options)
        { }
        public virtual DbSet<Models.File> Files { get; set; }
        public virtual DbSet<Models.Link> Links { get; set; }

        /// <summary>
        /// Used in place of real database. This will mimic a database and allow use to use EF as we normally would
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("InMemoryFileDatabase");
        }
    }
}
