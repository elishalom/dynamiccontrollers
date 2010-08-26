using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;

namespace DynamicControllersGeneration.Tests
{
    [TestFixture]
    public class MvvmGeneratorRelatedPropertyTests
    {
        public class Model
        {
            public object Prop1 { get; set; }
            public object Prop2 { get; set; }
        }

        public abstract class ViewModel : INotifyPropertyChanged
        {
            [RelatedProperty("Prop2")]
            public abstract object Prop1 { get; set; }
            public abstract object Prop2 { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        [Test]
        public void Generate_SetPropertyValue_PropertyChangedRaisedOnRelatedProperty()
        {
            var viewModelGenerator = new ViewModelGenerator();
            var model = new Model();
            ViewModel generatedViewModel = viewModelGenerator.Generate<ViewModel>(model);

            var changedProperties = new List<string>();
            generatedViewModel.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);

            generatedViewModel.Prop1 = new object();

            Assert.Contains("Prop2", changedProperties);
        }
    }
}