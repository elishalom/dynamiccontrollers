using System;
using System.Diagnostics.Contracts;

namespace DynamicControllersGeneration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ValidationAttribute : Attribute
    {
        private readonly Type validatorType;
        private readonly object[] args;

        [ContractInvariantMethod]
        private void ContractInvariantMethod()
        {
            Contract.Invariant(validatorType != null);
        }

        public ValidationAttribute(Type validatorType, params object[] args )
        {
            Contract.Requires<ArgumentException>(validatorType != null);
            Contract.Requires<ArgumentException>(typeof(IValidator).IsAssignableFrom(validatorType));
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