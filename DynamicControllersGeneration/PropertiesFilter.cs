using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace DynamicControllersGeneration
{
    internal class PropertiesFilter : IProxyGenerationHook
    {
        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            return methodInfo.Name.StartsWith("get_") || methodInfo.Name.StartsWith("set_");
        }

        public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
        {
            return;
        }

        public void MethodsInspected()
        {
            return;
        }
    }
}