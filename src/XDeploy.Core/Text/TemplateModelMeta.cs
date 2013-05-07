using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XDeploy.Text
{
    public class TemplateModelMeta
    {
        public Type ModelType { get; set; }

        public IList<TemplateModelPropertyMeta> Properties { get; private set; }

        public TemplateModelMeta()
        {
            Properties = new List<TemplateModelPropertyMeta>();
        }

        public static TemplateModelMeta ReadFrom(Type type)
        {
            var meta = new TemplateModelMeta
            {
                ModelType = type
            };

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                meta.Properties.Add(TemplateModelPropertyMeta.ReadFrom(type, prop));
            }

            return meta;
        }
    }
}
