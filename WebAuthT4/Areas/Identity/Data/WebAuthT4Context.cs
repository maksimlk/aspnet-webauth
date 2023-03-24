
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WebAuthT4.Areas.Identity.Data;

namespace WebAuthT4.Data;

public class WebAuthT4Context : IdentityDbContext<WebAuthUser>
{
    public WebAuthT4Context(DbContextOptions<WebAuthT4Context> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WebAuthUser>()
            .HasAlternateKey(x => x.UserIndex).HasName("UserIndex");
    }
}
