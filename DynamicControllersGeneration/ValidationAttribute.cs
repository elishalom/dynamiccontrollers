using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace DynamicControllersGeneration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ValidationAttribute : Attribute
    {
        private readonly IEnumerable<IValidator> validators;


        public ValidationAttribute(params Type[] validatorsTypes)
        {
            Contract.Requires<ArgumentException>(validatorsTypes != null);
            Contract.Requires<ArgumentException>(Contract.ForAll(validatorsTypes,
                                                                 validatorType =>
                                                                 typeof (IValidator).IsAssignableFrom(validatorType)));

            validators = validatorsTypes.Select(validatorType => (IValidator)Activator.CreateInstance(validatorType));
        }

        public string Validate(object value)
        {
            return validators.Select(validator => validator.Validate(value))
                .FirstOrDefault(validationResult => validationResult != null);
        }

        public ValidationMethod ValidationMethod { get; set; }
    }

    public enum ValidationMethod
    {
        FailOnFirst
    }
}