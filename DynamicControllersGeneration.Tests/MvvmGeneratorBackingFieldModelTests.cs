using NUnit.Framework;

namespace DynamicControllersGeneration.Tests
{
    [TestFixture]
    public class MvvmGeneratorBackingFieldModelTests
    {
        public class MyModel
        {
            public object Prop { get; set; }
        }

        public abstract class ViewModel
        {
            [Model] private readonly MyModel model;

            public MyModel Model
            {
                get { return model; }
            }

            public abstract object Prop { get; set; }
        }

        [Test]
        public void Generate_ViewModelWithModelField_ModelFieldInitialized()
        {
            var viewModelGenerator = new ViewModelGenerator();
            var model = new MyModel();

            ViewModel generatedViewModel = viewModelGenerator.Generate<ViewModel>(model);

            Assert.That(generatedViewModel.Model, Is.Not.Null);
        }
        
        [Test]
        public void Generate_ViewModelWithModelField_ModelFieldIsInstatiatedModel()
        {
            var viewModelGenerator = new ViewModelGenerator();
            var model = new MyModel();

            ViewModel generatedViewModel = viewModelGenerator.Generate<ViewModel>(model);

            Assert.That(generatedViewModel.Model, Is.SameAs(model));
        }
    }
}