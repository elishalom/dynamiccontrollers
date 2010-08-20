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

        public T Generate<T>(object model)  where T : class 
        {
            var proxyGenerationOptions = new ProxyGenerationOptions(propertiesFilter);
            var modelInterceptor = new ModelInterceptor(model);

            var classProxy = proxyGenerator.CreateClassProxy<T>(proxyGenerationOptions, modelInterceptor);
            modelInterceptor.RegisterProxy(classProxy);

            return classProxy;
        }
    }
}
