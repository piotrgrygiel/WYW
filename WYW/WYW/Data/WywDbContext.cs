using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WYW.Data
{
    public class WywDbContext : IdentityDbContext
    {
        public WywDbContext(DbContextOptions<WywDbContext> options)
            : base(options)
        {
        }
    }
}
