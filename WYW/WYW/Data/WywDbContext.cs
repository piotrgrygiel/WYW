using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
