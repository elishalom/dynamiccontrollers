using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace DynamicControllersGeneration
{
    public class ViewModelGenerator
    {
        private readonly ProxyGenerator proxyGenerator;
        private readonly PropertiesFilter propertiesFilter;

        public ViewModelGenerator()
        {
            proxyGenerator = new ProxyGenerator();
            propertiesFilter = new PropertiesFilter();
        }

        public T Generate<T>(params object[] factoryMethodArguments)  where T : class 
        {
            Contract.Requires(factoryMethodArguments != null);
            Contract.Requires(factoryMethodArguments.Length > 0);
            Contract.Ensures(Contract.Result<T>() != null);
            
            var proxyGenerationOptions = new ProxyGenerationOptions(propertiesFilter);

            ConstructorInfo matchingConstructor = GetMatchingConstructor(typeof(T), factoryMethodArguments);
            var hasMatchingNonDefaultCtor = matchingConstructor != null;

            ModelInterceptor modelInterceptor = CreateModelInterceptor(factoryMethodArguments, matchingConstructor, hasMatchingNonDefaultCtor);
            
            T classProxy = CreateViewModelProxy<T>(factoryMethodArguments, hasMatchingNonDefaultCtor, proxyGenerationOptions, modelInterceptor);
            modelInterceptor.RegisterProxy(classProxy);

            return classProxy;
        }

        private T CreateViewModelProxy<T>(object[] factoryMethodArguments, bool hasMatchingNonDefaultCtor, ProxyGenerationOptions proxyGenerationOptions, ModelInterceptor modelInterceptor)
        {
            object[] constructorArguments = GetConstructorArguments(factoryMethodArguments, hasMatchingNonDefaultCtor);
            return (T)proxyGenerator.CreateClassProxy(typeof(T), proxyGenerationOptions, constructorArguments, modelInterceptor);
        }

        private static ModelInterceptor CreateModelInterceptor(object[] factoryMethodArguments, ConstructorInfo matchingConstructor, bool hasMatchingNonDefaultCtor)
        {
            object model = GetModel(factoryMethodArguments, matchingConstructor, hasMatchingNonDefaultCtor);
            return new ModelInterceptor(model);
        }

        private static object[] GetConstructorArguments(object[] factoryMethodArguments, bool hasMatchingNonDefaultCtor)
        {
            if(hasMatchingNonDefaultCtor)
            {
                return factoryMethodArguments;
            }

            return new object[0];
        }

        private static object GetModel(object[] factoryMethodArguments, ConstructorInfo matchingConstructor, bool hasMatchingNonDefaultCtor)
        {
            Contract.Ensures(Contract.Result<object>() != null);
            if(hasMatchingNonDefaultCtor)
            {
                return GetModel(factoryMethodArguments, matchingConstructor);
            }
            
            return factoryMethodArguments[0];
        }

        private static ConstructorInfo GetMatchingConstructor(Type viewModelType, IEnumerable<object> factoryMethodArguments)
        {
            Contract.Requires(factoryMethodArguments != null);
            var nonPrivateCtors = viewModelType
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.NonPublic)
                .Where(info => !info.IsPrivate);
            var argumentsTypes = factoryMethodArguments.Select(argument => argument.GetType()).ToArray();
            return nonPrivateCtors
                .FirstOrDefault(info => IsMatch(info.GetParameters(), argumentsTypes));
        }

        private static object GetModel(object[] factoryMethodArguments, ConstructorInfo matchingConstructor)
        {
            var ctorParameters = matchingConstructor.GetParameters();
            for (int i = 0; i < ctorParameters.Length; i++)
            {
                if (ctorParameters[i].IsDefined(typeof (ModelAttribute), false))
                {
                    return factoryMethodArguments[i];
                }
            }

            if (ctorParameters.Length == 1 && factoryMethodArguments.Length == 1)
            {
                return factoryMethodArguments[0];
            }

            throw new NotImplementedException();
        }

        private static bool IsMatch(ParameterInfo[] parameters, Type[] arguments)
        {
            if(parameters.Length != arguments.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (!parameters[i].ParameterType.IsAssignableFrom(arguments[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
