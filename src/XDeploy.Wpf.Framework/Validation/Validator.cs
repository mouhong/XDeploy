using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace XDeploy.Wpf.Framework.Validation
{
    public static class Validator
    {
        public static List<string> GetAllErrors(object model)
        {
            var errors = new List<string>();
            var meta = ModelValidationMetaCache.Get(model.GetType());

            foreach (var prop in meta.ValidatedProperties)
            {
                errors.AddRange(GetPropertyErrors(model, prop.Name));
            }

            return errors;
        }

        public static List<string> GetPropertyErrors(object model, string propertyName)
        {
            var meta = ModelValidationMetaCache.Get(model.GetType());
            if (meta.ValidatedProperties.Any(x => x.Name == propertyName))
            {
                var value = meta.GetPropertyValue(model, propertyName);
                var errors = (from validator in meta.GetPropertyValidationAttributes(propertyName)
                              where !validator.IsValid(value)
                              select validator.FormatErrorMessage(propertyName)).ToList();

                return errors;
            }

            return new List<string>();
        }
    }
}
