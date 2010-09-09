using System;

namespace DynamicControllersGeneration
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ModelAttribute : Attribute { }
}