using Audit.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataModel
{
    public class SapDbContext : AuditDbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbConnection _connection;

        public SapDbContext(IHttpContextAccessor httpContextAccessor) : base()
        {
            // inject the context accessor
            //_connection = dbConnection;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
