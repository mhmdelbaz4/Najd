using Microsoft.EntityFrameworkCore;
using NajdAPI.Models;

namespace NajdAPI.Data
{
    public class NajdDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public NajdDBContext(DbContextOptions options):base(options)
        {}

    }
}
