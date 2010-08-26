using System;

namespace DynamicControllersGeneration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RelatedPropertyAttribute : Attribute {
        public string PropertyName { get; private set; }

        public RelatedPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}