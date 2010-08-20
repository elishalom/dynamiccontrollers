using System;

namespace DynamicControllersGeneration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ValidationAttribute : Attribute
    {
        private readonly Type validatorType;

        public ValidationAttribute(Type validatorType)
        {
            this.validatorType = validatorType;
        }

        public string Validate(object value)
        {
            var validator = (IValidator)Activator.CreateInstance(validatorType);
            return validator.Validate(value);
        }
    }
}