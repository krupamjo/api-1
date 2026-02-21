using Microsoft.EntityFrameworkCore;

namespace api_1
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Pet> Pets { get; set; }
    }
}
