using System;

namespace DynamicControllersGeneration
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ModelAttribute : Attribute { }
}