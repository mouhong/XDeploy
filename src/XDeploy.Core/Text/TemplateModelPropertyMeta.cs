using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XDeploy.Text
{
    public class TemplateModelPropertyMeta
    {
        public PropertyInfo Property { get; set; }

        public TemplateFieldAttribute FieldAttribute { get; set; }

        public static TemplateModelPropertyMeta ReadFrom(Type modelType, PropertyInfo property)
        {
            var meta = new TemplateModelPropertyMeta
            {
                Property = property
            };

            var fieldAttr = property.GetCustomAttributes(false).OfType<TemplateFieldAttribute>().FirstOrDefault();
            if (fieldAttr != null)
            {
                meta.FieldAttribute = fieldAttr;
            }

            return meta;
        }
    }
}
