using BlazorAuthDbPush.Data;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAuthDbPush
{
    public class WywDbContext : IdentityDbContext
    {
        public WywDbContext(DbContextOptions<WywDbContext> options)
            : base(options)
        {
        }

        public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }
    }
}
