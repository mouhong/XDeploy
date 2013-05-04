using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace XDeploy.Wpf.Framework.Validation
{
    public class ModelValidationMeta
    {
        private List<PropertyInfo> _validatedProperties = new List<PropertyInfo>();
        private Dictionary<string, ValidationAttribute[]> _propertyValidationAttributes = new Dictionary<string, ValidationAttribute[]>();

        public Type ModelType { get; private set; }

        public IEnumerable<PropertyInfo> ValidatedProperties
        {
            get
            {
                return _validatedProperties;
            }
        }

        public IEnumerable<ValidationAttribute> GetPropertyValidationAttributes(string propertyName)
        {
            return _propertyValidationAttributes[propertyName];
        }

        public object GetPropertyValue(object model, string propertyName)
        {
            var property = _validatedProperties.FirstOrDefault(x => x.Name == propertyName);
            if (property == null)
                throw new InvalidOperationException("Property \"" + propertyName + "\" was not found or not tagged with validation attributes.");

            return property.GetValue(model, null);
        }

        public static ModelValidationMeta Create(Type modelType)
        {
            var meta = new ModelValidationMeta
            {
                ModelType = modelType
            };

            foreach (var prop in modelType.GetProperties())
            {
                var validationAttributes = prop.GetCustomAttributes(true).OfType<ValidationAttribute>().ToList();
                if (validationAttributes.Count > 0)
                {
                    meta._validatedProperties.Add(prop);
                    meta._propertyValidationAttributes.Add(prop.Name, validationAttributes.ToArray());
                }
            }

            return meta;
        }
    }
}
