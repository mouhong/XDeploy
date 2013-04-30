using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NHibernate.Mapping.ByCode;

namespace XDeploy.Data.Mapping
{
    public static class IClassAttributesMapperExtensions
    {
        public static void HighLowId<TEntity, TProperty>(this IClassAttributesMapper<TEntity> mapper,
            Expression<Func<TEntity, TProperty>> idProperty,
            int maxLowValue = 1)
            where TEntity : class
        {
            mapper.Id(idProperty, m =>
            {
                m.Generator(Generators.HighLow, g =>
                {
                    g.Params(new
                    {
                        table = "HiValue",
                        column = "NextValue",
                        max_lo = maxLowValue,
                        where = "TableName = '" + typeof(TEntity).Name + "'"
                    });
                });
            });
        }
    }
}
