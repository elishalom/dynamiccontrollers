using System;

namespace DynamicControllersGeneration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ValidationAttribute : Attribute
    {
        private readonly Type validatorType;
        private readonly object[] args;

        public ValidationAttribute(Type validatorType, params object[] args )
        {
            this.validatorType = validatorType;
            this.args = args;
        }

        public string Validate(object value)
        {
            var validator = (IValidator)Activator.CreateInstance(validatorType, args);
            return validator.Validate(value);
        }
    }
}