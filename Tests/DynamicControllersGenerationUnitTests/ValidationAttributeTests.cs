using System;
using DynamicControllersGeneration;
using NUnit.Framework;

namespace DynamicControllersGenerationUnitTests
{
    [TestFixture]
    public class ValidationAttributeTests
    {
        class FakeValidator : IValidator
        {
            internal static Func<object, string> ValidateAction;

            public string Validate(object value)
            {
                return ValidateAction.Invoke(value);
            }
        }

        class FakeValidatorA : IValidator
        {
            internal static Func<object, string> ValidateAction;

            public string Validate(object value)
            {
                return ValidateAction.Invoke(value);
            }
        }
        class FakeValidatorB : IValidator
        {
            internal static Func<object, string> ValidateAction;

            public string Validate(object value)
            {
                return ValidateAction.Invoke(value);
            }
        }

        [SetUp]
        public void Setup()
        {
            FakeValidator.ValidateAction = null;
        }

        [Test]
        public void Validate_HasSingleValidatorAndValidData_ReturnsNull()
        {
            FakeValidator.ValidateAction = data => null;

            var attribute = new ValidationAttribute(new[] { typeof(FakeValidator) });
            string validationResult = attribute.Validate(new object());

            Assert.That(validationResult, Is.Null);
        }

        [Test]
        public void Validate_HasSingleValidatorAndInvalidData_ReturnsError()
        {
            const string errorMessage = "error";

            FakeValidator.ValidateAction = data => errorMessage;

            var attribute = new ValidationAttribute(new[] { typeof(FakeValidator) });
            string validationResult = attribute.Validate(new object());

            Assert.That(validationResult, Is.EqualTo(errorMessage));
        }

        [Test]
        public void Validate_HasSingleValidator_VlidatorCalledOnce()
        {
            int validatorCallsCount = 0;
            var invalidData = new object();
            FakeValidator.ValidateAction = data =>
                                               {
                                                   validatorCallsCount++;
                                                   return null;
                                               };

            var attribute = new ValidationAttribute(new[] { typeof(FakeValidator) });
            attribute.Validate(invalidData);

            Assert.That(validatorCallsCount, Is.EqualTo(1));
        }

        [Test]
        public void Validate_HasSingleValidator_ValidateCalledWithCorrectData()
        {
            object validatedData = null;
            FakeValidator.ValidateAction = data =>
                                               {
                                                   validatedData = data;
                                                   return null;
                                               };

            var attribute = new ValidationAttribute(new[] { typeof(FakeValidator) });
            var dataToValidate = new object();
            attribute.Validate(dataToValidate);

            Assert.That(dataToValidate, Is.EqualTo(validatedData));
        }

        [Test]
        public void Validate_TwoValidatorsAndValidData_ReturnsNull()
        {
            FakeValidatorA.ValidateAction = data => null;
            FakeValidatorB.ValidateAction = data => null;

            var attribute = new ValidationAttribute(new[] { typeof(FakeValidatorA), typeof(FakeValidatorB) });
            string validationResult = attribute.Validate(new object());

            Assert.That(validationResult, Is.Null);
        }

        [Test]
        public void Validate_TwoValidatorsAndDataInvalidForFirst_ReturnsFirstError()
        {
            const string errorMessage = "error";
            FakeValidatorA.ValidateAction = data => errorMessage;
            FakeValidatorB.ValidateAction = data => null;

            var attribute = new ValidationAttribute(new[] { typeof(FakeValidatorA), typeof(FakeValidatorB) });
            string validationResult = attribute.Validate(new object());

            Assert.That(validationResult, Is.EqualTo(errorMessage));
        }

        [Test]
        public void Validate_TwoValidatorsAndDataInvalidForSecond_ReturnsSecondError()
        {
            const string errorMessage = "error";
            FakeValidatorA.ValidateAction = data => null;
            FakeValidatorB.ValidateAction = data => errorMessage;

            var attribute = new ValidationAttribute(new[] { typeof(FakeValidatorA), typeof(FakeValidatorB) });
            string validationResult = attribute.Validate(new object());

            Assert.That(validationResult, Is.EqualTo(errorMessage));
        }

        [Test]
        public void Validate_TwoValidatorsAndDataInvalidForBoth_ReturnsFirstError()
        {
            const string errorMessageA = "errorA";
            const string errorMessageB = "errorB";
            FakeValidatorA.ValidateAction = data => errorMessageA;
            FakeValidatorB.ValidateAction = data => errorMessageB;

            var attribute = new ValidationAttribute(new[] { typeof(FakeValidatorA), typeof(FakeValidatorB) });
            string validationResult = attribute.Validate(new object());

            Assert.That(validationResult, Is.EqualTo(errorMessageA));
        }
    }
}