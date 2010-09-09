using System;

namespace DynamicControllersGeneration
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ModelAttribute : Attribute { }
}