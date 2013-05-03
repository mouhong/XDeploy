using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Caliburn.Micro.Validation
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

        public static bool IsPropertyValid<TProperty>(object model, Expression<Func<TProperty>> property)
        {
            var name = property.GetMemberInfo().Name;
            var meta = ModelValidationMetaCache.Get(model.GetType());
            if (meta.ValidatedProperties.Any(x => x.Name == name))
            {
                var value = meta.GetPropertyValue(model, name);
                return meta.GetPropertyValidationAttributes(name).All(v => v.IsValid(value));
            }

            return true;
        }
    }
}
