using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Text
{
    public class TemplateFieldAttribute : Attribute
    {
        public string Description { get; set; }

        public TemplateFieldAttribute(string description)
        {
            Description = description;
        }
    }
}
