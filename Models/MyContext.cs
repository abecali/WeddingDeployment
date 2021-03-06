using Microsoft.EntityFrameworkCore;

namespace wedding_planner.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        //   method? passing options? base options?

        public DbSet<User> Users { get; set; }
        public DbSet<Wedding> Weddings { get; set; }
        public DbSet<Association> Associations { get; set; }

    }

}