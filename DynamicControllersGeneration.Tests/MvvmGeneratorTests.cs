using NUnit.Framework;

namespace DynamicControllersGeneration.Tests
{
    [TestFixture]
    public class MvvmGeneratorTests
    {
        public class Model
        {
            public object Prop { get; set; }
        }

        public abstract class ViewModel
        {
            public abstract object Prop { get; set; }
        }

        [Test]
        public void Generate_SetPropertyValue_ModelPropertyUpdated()
        {
            var viewModelGenerator = new ViewModelGenerator();
            var model = new Model();
            
            ViewModel generatedViewModel = viewModelGenerator.Generate<ViewModel>(model);

            object valueToAssign = new object();
            generatedViewModel.Prop = valueToAssign;

            Assert.That(model.Prop, Is.SameAs(valueToAssign));
        }

        [Test]
        public void Generate_CallPropertyGetter_ModelPropertyValueReturned()
        {
            var mvvmGenerator = new ViewModelGenerator();
            var model = new Model();
            ViewModel generatedViewModel = mvvmGenerator.Generate<ViewModel>(model);

            object assignedValue = new object();
            model.Prop = assignedValue;

            Assert.That(generatedViewModel.Prop, Is.SameAs(assignedValue));
        }
    }

    public class ForwardCallsExample
    {
        public class Model
        {
            public object Prop { get; set; }
        }

        public class ViewModel
        {
            private readonly Model model;

            public ViewModel(Model model)
            {
                this.model = model;
            }

            public object Prop
            {
                get { return model.Prop; }
                set { model.Prop = value; }
            }
        }
    }
}