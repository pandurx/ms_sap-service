using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataModel
{
    public class SapDbContextFactory : IDesignTimeDbContextFactory<SapDbContext>
    {
        public SapDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SapDbContext>();
            var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
            return new SapDbContext(accessor);
            //return new SapDbContext();
        }
    }
}
