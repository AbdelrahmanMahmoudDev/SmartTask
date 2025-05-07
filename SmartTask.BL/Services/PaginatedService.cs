using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SmartTask.Bl.Helpers;
using SmartTask.Bl.IServices;
using SmartTask.DataAccess.Data;

namespace SmartTask.Bl.Services
{
    public class PaginatedService<T> : IPaginatedService<T> where T : class
    {
        private readonly SmartTaskContext _context;

        public PaginatedService(SmartTaskContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<T>> GetFiltered(Expression<Func<T, bool>>? filter, int page, int pageSize)
        {
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return PaginatedList<T>.Create(query, page, pageSize);
        }
    }
}
