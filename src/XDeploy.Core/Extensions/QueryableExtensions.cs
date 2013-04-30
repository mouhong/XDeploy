using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public static class QueryableExtensions
    {
        public static PagedQueryable<T> Paging<T>(this IQueryable<T> query, int pageSize)
        {
            Require.NotNull(query, "query");
            return new PagedQueryable<T>(query, pageSize);
        }
    }
}
