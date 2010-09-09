using NUnit.Framework;

namespace DynamicControllersGeneration.Tests
{
    [TestFixture]
    public class MvvmGeneratorCtorExpectingArgumentsTests
    {
        public class Model
        {
            public object Prop { get; set; }
        }

        public class Service {}

        public abstract class ViewModel
        {
            public Service Service { get; private set; }

            protected ViewModel(Model model, Service service)
            {
                Service = service;
            }
        }

        [Test]
        public void Generate_ViewModelExpectingModelInCtor_RecieveModelInCtor()
        {
            var viewModelGenerator = new ViewModelGenerator();

            var model = new Model();
            var service = new Service();
            ViewModel viewModel = viewModelGenerator.Generate<ViewModel>(model, service);

            Assert.That(viewModel.Service, Is.SameAs(service));
        }
    }
}