using CCG.AspNetCore.Business.Interface;
using DataModel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Queries
{
    public class GetCostCentresQuery : IQuery<string>
    {

    }

    public class GetCostCentresQueryHandler : IQueryHandler<GetCostCentresQuery, string>
    {
        private readonly SapDbContext _db;

        public GetCostCentresQueryHandler(SapDbContext db)
        {
            _db = db;
        }

        public Task<string> HandleAsync(GetCostCentresQuery query, CancellationToken cancellationToken = default)
        {
            Task<string> result = Task.FromResult("hello world");
            return result;
        }
    }
}
