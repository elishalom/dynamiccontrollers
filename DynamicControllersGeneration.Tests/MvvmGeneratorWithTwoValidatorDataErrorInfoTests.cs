using System.ComponentModel;
using NUnit.Framework;

namespace DynamicControllersGeneration.Tests
{
    [TestFixture]
    public class MvvmGeneratorWithTwoValidatorDataErrorInfoTests
    {
        public class Model
        {
            public object Prop { get; set; }
        }

        public abstract class ViewModel : IDataErrorInfo
        {
            protected ViewModel()
            {
                Prop = string.Empty;
            }

            public abstract string this[string columnName] { get; }
            public abstract string Error { get; }

            [Validation(typeof(RequiredAValidator), typeof(RequiredOneValidator))]
            public abstract string Prop { get; set; }
        }

        public class RequiredAValidator : IValidator
        {
            internal const string ERROR = "No A";

            public string Validate(object value)
            {
                return ((string)value).Contains("A") ? null : ERROR;
            }
        }

        public class RequiredOneValidator : IValidator
        {
            internal const string ERROR = "No 1";

            public string Validate(object value)
            {
                return ((string)value).Contains("1") ? null : ERROR;
            }
        }

        [Test]
        public void Generate_SetValidValueForBothValidators_PropertyErrorIsNull()
        {
            var viewModel = new ViewModelGenerator();
            var model = new Model();

            var generatedViewModel = viewModel.Generate<ViewModel>(model);
            const string validValue = "1A";
            generatedViewModel.Prop = validValue;

            Assert.That(generatedViewModel["Prop"], Is.Null);
        }

        [Test]
        public void Generate_SetInvalidValueForFirstValidator_PropertyErrorIsFirstValidatorError()
        {
            var viewModel = new ViewModelGenerator();
            var model = new Model();

            var generatedViewModel = viewModel.Generate<ViewModel>(model);
            const string validValueForSecondOnly = "1";
            generatedViewModel.Prop = validValueForSecondOnly;

            Assert.That(generatedViewModel["Prop"], Is.EqualTo(RequiredAValidator.ERROR));
        }

        [Test]
        public void Generate_SetInvalidValueForSecondValidator_PropertyErrorIsSecondValidatorError()
        {
            var viewModel = new ViewModelGenerator();
            var model = new Model();

            var generatedViewModel = viewModel.Generate<ViewModel>(model);
            const string validValueForFirstFirstOnly = "A";
            generatedViewModel.Prop = validValueForFirstFirstOnly;

            Assert.That(generatedViewModel["Prop"], Is.EqualTo(RequiredOneValidator.ERROR));
        }
    }
}