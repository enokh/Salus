using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace Salus.Data
{
    public class SalusDbContextFactory : IDesignTimeDbContextFactory<SalusDbContext>
    {
        public SalusDbContext CreateDbContext(string[] args)
        {
            var dbPath = Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "Salus", "salus.db");
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath)!);

            var optionsBuilder = new DbContextOptionsBuilder<SalusDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
            return new SalusDbContext(optionsBuilder.Options);
        }
    }
}
