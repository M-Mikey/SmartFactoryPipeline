

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MachineData.Data
{
    public class AppDbContext:DbContext
    {
       
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }
    
            public DbSet<MachineInputs> MachineInputs { get; set; }

        public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
        {
            public AppDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseOracle("User Id=SYSTEM;Password=oracle123;Data Source=localhost:1521/orclpdb");

                return new AppDbContext(optionsBuilder.Options);
            }
        }
    }
}
