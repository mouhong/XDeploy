using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace XDeploy.Validation
{
    public abstract class ValidatableScreen : Screen, IDataErrorInfo
	{
		/// <summary>
		/// Gets an error message indicating what is wrong with this object.
		/// </summary>
		/// <returns>
		/// An error message indicating what is wrong with this object. The default is an empty string ("").
		/// </returns>
		public virtual string Error
		{
			get
			{
                var errors = Validator.GetAllErrors(this);

                if (errors.Count > 0)
                {
                    return string.Join(Environment.NewLine, errors);
                }

                return String.Empty;
			}
		}

		/// <summary>
		/// Returns True if any of the property values generate a validation error
		/// </summary>
		public virtual bool HasErrors
		{
			get { return !string.IsNullOrEmpty(Error); }
		}

		/// <summary>
		/// Gets the error message for the property with the given name.
		/// </summary>
		/// <returns>
		/// The error message for the property. The default is an empty string ("").
		/// </returns>
		/// <param name="propertyName">The name of the property whose error message to get. </param>
		public virtual string this[string propertyName]
		{
			get
			{
                var errors = Validator.GetPropertyErrors(this, propertyName);

                if (errors.Count > 0)
                {
                    return String.Join(Environment.NewLine, errors);
                }

				return string.Empty;
			}
		}
	}
}