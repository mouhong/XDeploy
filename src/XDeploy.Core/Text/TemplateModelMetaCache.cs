using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Text
{
    public static class TemplateModelMetaCache
    {
        static readonly Dictionary<Type, TemplateModelMeta> _metas = new Dictionary<Type, TemplateModelMeta>();

        public static TemplateModelMeta GetModelMeta(Type modelType)
        {
            TemplateModelMeta meta = null;
            if (_metas.TryGetValue(modelType, out meta))
            {
                return meta;
            }

            lock (_metas)
            {
                if (!_metas.TryGetValue(modelType, out meta))
                {
                    meta = TemplateModelMeta.ReadFrom(modelType);
                    _metas.Add(modelType, meta);
                }
            }

            return meta;
        }
    }
}
