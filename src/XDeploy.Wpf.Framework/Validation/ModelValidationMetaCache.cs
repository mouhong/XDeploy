using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Wpf.Framework.Validation
{
    public static class ModelValidationMetaCache
    {
        static ConcurrentDictionary<Type, ModelValidationMeta> _cache = new ConcurrentDictionary<Type, ModelValidationMeta>();

        public static ModelValidationMeta Get(Type modelType)
        {
            return _cache.GetOrAdd(modelType, type => ModelValidationMeta.Create(type));
        }
    }
}
