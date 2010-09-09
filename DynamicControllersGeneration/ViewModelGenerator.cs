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

        public T Generate<T>(params object[] model)  where T : class 
        {
            var proxyGenerationOptions = new ProxyGenerationOptions(propertiesFilter);
            var modelInterceptor = new ModelInterceptor(model);

            object[] constructorArguments = GetConstructorArguments<T>(model);
            var classProxy = (T) proxyGenerator.CreateClassProxy(typeof (T), proxyGenerationOptions, constructorArguments, modelInterceptor);
            modelInterceptor.RegisterProxy(classProxy);

            return classProxy;
        }

        private static object[] GetConstructorArguments<T>(object model)
        {
            var nonPrivateConstructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(constructor => !constructor.IsPrivate);
            var hasConstructorExpectingModel = nonPrivateConstructors
                .Any(constructor => IsContructorExpectingModel(constructor, model));

            if (hasConstructorExpectingModel)
            {
                return new[] {model};
            }
            
            return new object[0];
        }

        private static bool IsContructorExpectingModel(ConstructorInfo constructor, object model)
        {
            var constructorParameters = constructor.GetParameters();
            var isExpectingSingleArgument = constructorParameters.Length == 1;
            if (!isExpectingSingleArgument)
            {
                return false;
            }

            var isParametersMatchingModel = constructorParameters[0].ParameterType.IsAssignableFrom(model.GetType());
            return isParametersMatchingModel;
        }
    }
}
