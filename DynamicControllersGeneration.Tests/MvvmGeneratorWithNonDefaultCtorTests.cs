using System.ComponentModel;
using NUnit.Framework;

namespace DynamicControllersGeneration.Tests
{
    [TestFixture]
    public class MvvmGeneratorWithNonDefaultCtorTests
    {
        public class Model
        {
            public object Prop { get; set; }
        }

        public abstract class ViewModel : INotifyPropertyChanged
        {
            public Model Model { get; private set; }

            public abstract object Prop { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;

            protected ViewModel(Model model)
            {
                Model = model;
            }
        }

        [Test]
        public void Generate_ViewModelExpectingModelInCtor_RecieveModelInCtor()
        {
            var viewModelGenerator = new ViewModelGenerator();

            var model = new Model();
            ViewModel viewModel = viewModelGenerator.Generate<ViewModel>(model);

            Assert.That(viewModel.Model, Is.SameAs(model));
        }
    }
}