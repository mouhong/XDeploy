using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace XDeploy.Text
{
    public static class Template
    {
        static readonly Regex _pattern = new Regex(@"\{(?<param>[^\}]+)\}", RegexOptions.Compiled);

        public static string Render(string template, object model)
        {
            if (String.IsNullOrEmpty(template) || model == null)
            {
                return template;
            }

            var meta = TemplateModelMetaCache.GetModelMeta(model.GetType());

            return _pattern.Replace(template, match =>
            {
                var param = match.Groups["param"].Value;
                var prop = meta.Properties.FirstOrDefault(x => x.Property.Name.Equals(param, StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    var value = prop.Property.GetValue(model, null);
                    return value == null ? null : value.ToString();
                }

                return param;
            });
        }
    }
}
