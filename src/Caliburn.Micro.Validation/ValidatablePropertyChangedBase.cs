using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Caliburn.Micro.Validation
{
    public abstract class ValidatablePropertyChangedBase : PropertyChangedBase, IDataErrorInfo
    {
        public virtual string Error
        {
            get
            {
                var errors = Validator.GetAllErrors(this);

                if (errors.Count > 0)
                {
                    return String.Join(Environment.NewLine, errors);
                }

                return String.Empty;
            }
        }

        public virtual string this[string propertyName]
        {
            get
            {
                var errors = Validator.GetPropertyErrors(this, propertyName);
                if (errors.Count > 0)
                {
                    return String.Join(Environment.NewLine, errors);
                }

                return String.Empty;
            }
        }

        public virtual bool HasErrors
        {
            get
            {
                return !String.IsNullOrEmpty(Error);
            }
        }
    }
}
