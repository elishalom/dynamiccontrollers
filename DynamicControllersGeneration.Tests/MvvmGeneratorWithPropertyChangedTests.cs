using System.ComponentModel;
using NUnit.Framework;

namespace DynamicControllersGeneration.Tests
{
    [TestFixture]
    public class MvvmGeneratorWithPropertyChangedTests
    {
        public class Model
        {
            public object Prop { get; set; }
        }

        public abstract class ViewModel : INotifyPropertyChanged
        {
            public abstract object Prop { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;
        }

        [Test]
        public void Generate_SetPropertyValue_PropertyChangedRaised()
        {
            var viewModelGenerator = new ViewModelGenerator();
            var model = new Model();
            ViewModel generatedViewModel = viewModelGenerator.Generate<ViewModel>(model);

            bool wasRaised = false;
            generatedViewModel.PropertyChanged += (sender, args) => wasRaised = true;

            generatedViewModel.Prop = new object();

            Assert.That(wasRaised, Is.True);
        }

        [Test]
        public void Generate_PropertyValueSet_PropertyChangedRaisedWithCorrectSource()
        {
            var mvvmGenerator = new ViewModelGenerator();
            var model = new Model();
            ViewModel generatedViewModel = mvvmGenerator.Generate<ViewModel>(model);

            object source = null;
            generatedViewModel.PropertyChanged += (sender, args) => source = sender;

            generatedViewModel.Prop = new object();

            Assert.That(source, Is.SameAs(generatedViewModel));
        }

        [Test]
        public void Generate_PropertyValueSet_PropertyChangedRaisedWithCorrectPropertyName()
        {
            var mvvmGenerator = new ViewModelGenerator();
            var model = new Model();
            ViewModel generatedViewModel = mvvmGenerator.Generate<ViewModel>(model);

            string propertyName = null;
            generatedViewModel.PropertyChanged += (sender, args) => propertyName = args.PropertyName;

            generatedViewModel.Prop = new object();

            Assert.That(propertyName, Is.EqualTo("Prop"));
        }

        [Test]
        public void Generate_CalledPropertyGetter_PropertyChangedWasNotRaised()
        {
            var mvvmGenerator = new ViewModelGenerator();
            var model = new Model();
            ViewModel generatedViewModel = mvvmGenerator.Generate<ViewModel>(model);

            bool wasRaised = false;
            generatedViewModel.PropertyChanged += (sender, args) => wasRaised = true;

            var value = generatedViewModel.Prop;

            Assert.That(wasRaised, Is.False);
        }
    }
}