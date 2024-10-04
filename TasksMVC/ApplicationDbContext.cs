using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TasksMVC.Entities;

namespace TasksMVC
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Tarea> Tasks { get; set; }
        public DbSet<Steps> Steps { get; set; }
        public DbSet<AttachedFile> AttachedFiles { get; set; }
    }
}
