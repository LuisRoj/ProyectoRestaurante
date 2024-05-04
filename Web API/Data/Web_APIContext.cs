using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web_API.Models;

namespace Web_API.Data
{
    public class Web_APIContext : DbContext
    {
        public Web_APIContext (DbContextOptions<Web_APIContext> options)
            : base(options)
        {
        }

        public DbSet<Web_API.Models.Usuarios> Usuarios { get; set; } = default!;
    }
}
