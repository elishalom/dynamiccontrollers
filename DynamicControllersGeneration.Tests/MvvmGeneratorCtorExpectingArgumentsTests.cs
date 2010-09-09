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
            public Model Model { get; private set; }

            protected ViewModel(Service service, [Model]Model model)
            {
                Service = service;
                Model = model;
            }

            public abstract object Prop { get; set; }
        }

        [Test]
        public void Generate_ViewModelExpectingSerivceInCtor_RecieveServiceInCtor()
        {
            var viewModelGenerator = new ViewModelGenerator();

            var model = new Model();
            var service = new Service();
            ViewModel viewModel = viewModelGenerator.Generate<ViewModel>(service, model);

            Assert.That(viewModel.Service, Is.SameAs(service));
        }
        
        [Test]
        public void Generate_ViewModelExpectingModelAsSecondArgumentInCtor_RecieveModelInCtor()
        {
            var viewModelGenerator = new ViewModelGenerator();

            var model = new Model();
            var service = new Service();
            ViewModel viewModel = viewModelGenerator.Generate<ViewModel>(service, model);

            Assert.That(viewModel.Model, Is.SameAs(model));
        }
    
        [Test]
        public void Generate_ViewModelExpectingModelAsSecondArgumentInCtorAndPropertySet_ModelUpdated()
        {
            var viewModelGenerator = new ViewModelGenerator();

            var model = new Model();
            var service = new Service();
            ViewModel viewModel = viewModelGenerator.Generate<ViewModel>(service, model);
            var expected = new object();
            viewModel.Prop = expected;

            Assert.That(model.Prop, Is.SameAs(expected));
        }
    }
}