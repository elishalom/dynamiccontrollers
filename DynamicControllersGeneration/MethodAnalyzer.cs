using System.Reflection;

namespace DynamicControllersGeneration
{
    internal class MethodAnalyzer
    {
        private const string GETTER_PREFIX = "get_";
        private const string SETTER_PREFIX = "set_";
        private readonly MethodInfo method;

        public MethodAnalyzer(MethodInfo method)
        {
            this.method = method;
        }

        public bool IsGetter
        {
            get { return IsPropertyAccessor(GETTER_PREFIX); }
        }

        public bool IsSetter
        {
            get { return IsPropertyAccessor(SETTER_PREFIX); }
        }

        public bool IsIndexer
        {
            get { return method.Name == "get_Item"; }
        }

        public PropertyInfo MatchingProperty { get { return method.DeclaringType.GetProperty(method.Name.Substring(4)); } }

        private bool IsPropertyAccessor(string accessorPrefix)
        {
            var hasGetterPrefix = method.Name.StartsWith(accessorPrefix);
            if (!hasGetterPrefix)
            {
                return false;
            }
                
            var expectedPropertyName = method.Name.Substring(accessorPrefix.Length);
            return HasMatchingProperty(expectedPropertyName);
        }

        private bool HasMatchingProperty(string expectedPropertyName)
        {
            var matchingProperty = method.DeclaringType.GetProperty(expectedPropertyName);
            return matchingProperty != null;
        }
    }
}