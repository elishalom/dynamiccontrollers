using DynamicControllersGeneration;
using NUnit.Framework;

namespace DynamicControllersGenerationUnitTests
{
    [TestFixture]
    public class MethodsAnalyzerTests
    {
        public class MyClass
        {
            public object Prop { get; set; }
            public void Method() { }
            
            public object get_NonPropertyAccessor() { return null; }
            public object set_NonPropertyAccessor() { return null; }
        }

        [Test]
        public void IsGetter_GetterMethod_True()
        {
            var getter = typeof(MyClass).GetProperty("Prop").GetGetMethod();
            var methodsAnalyzer = new MethodAnalyzer(getter);

            Assert.That(methodsAnalyzer.IsGetter, Is.True);
        }
    
        [Test]
        public void IsGetter_SetterMethod_False()
        {
            var setter = typeof(MyClass).GetProperty("Prop").GetSetMethod();
            var methodsAnalyzer = new MethodAnalyzer(setter);

            Assert.That(methodsAnalyzer.IsGetter, Is.False);
        }
    
        [Test]
        public void IsGetter_NonAccessorMethod_False()
        {
            var method = typeof (MyClass).GetMethod("Method");
            var methodsAnalyzer = new MethodAnalyzer(method);

            Assert.That(methodsAnalyzer.IsGetter, Is.False);
        }
        
        [Test]
        public void IsGetter_NonAccessorMethodStartsWithGetterPrefix_False()
        {
            var method = typeof(MyClass).GetMethod("get_NonPropertyAccessor");
            var methodsAnalyzer = new MethodAnalyzer(method);

            Assert.That(methodsAnalyzer.IsGetter, Is.False);
        }

        [Test]
        public void IsSetter_SetterMethod_True()
        {
            var setter = typeof(MyClass).GetProperty("Prop").GetSetMethod();
            var methodsAnalyzer = new MethodAnalyzer(setter);

            Assert.That(methodsAnalyzer.IsSetter, Is.True);
        }

        [Test]
        public void IsSetter_GetterMethod_False()
        {
            var getter = typeof(MyClass).GetProperty("Prop").GetGetMethod();
            var methodsAnalyzer = new MethodAnalyzer(getter);

            Assert.That(methodsAnalyzer.IsSetter, Is.False);
        }

        [Test]
        public void IsSetter_NonAccessorMethod_False()
        {
            var method = typeof(MyClass).GetMethod("Method");
            var methodsAnalyzer = new MethodAnalyzer(method);

            Assert.That(methodsAnalyzer.IsSetter, Is.False);
        }

        [Test]
        public void IsSetter_NonAccessorMethodStartsWithSetterPrefix_False()
        {
            var method = typeof(MyClass).GetMethod("set_NonPropertyAccessor");
            var methodsAnalyzer = new MethodAnalyzer(method);

            Assert.That(methodsAnalyzer.IsSetter, Is.False);
        }
    
        [Test]
        public void MatchingProperty_PropertyGetter_ReturnsCorrectProperty()
        {
            var propertyInfo = typeof (MyClass).GetProperty("Prop");
            var getter = propertyInfo.GetGetMethod();
            var methodsAnalyzer = new MethodAnalyzer(getter);

            Assert.That(methodsAnalyzer.MatchingProperty, Is.EqualTo(propertyInfo));
        }
    }
}