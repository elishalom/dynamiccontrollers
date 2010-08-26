﻿using System.ComponentModel;
using NUnit.Framework;

namespace DynamicControllersGeneration.Tests
{
    [TestFixture]
    public class MvvmGeneratorWithDataErrorInfoTests
    {
        public class Model
        {
            public object Prop { get; set; }
        }

        public abstract class ViewModel : IDataErrorInfo
        {
            public abstract string this[string columnName] { get; }
            public abstract string Error { get; }

            [Validation(typeof(DummyValidator))]
            public abstract object Prop { get; set; }
        }

        public class DummyValidator : IValidator
        {
            public string Validate(object value)
            {
                return "error";
            }
        }
        
        [Test]
        public void Generate_SetInvalidPropertyValue_PropertyErrorIsCorrect()
        {
            var viewModel = new ViewModelGenerator();
            var model = new Model();

            var generatedViewModel = viewModel.Generate<ViewModel>(model);

            generatedViewModel.Prop = new object();

            Assert.That(generatedViewModel["Prop"], Is.EqualTo("error"));
        }
    
        [Test]
        public void Generate_ValidatorReturnsErrorAndValueIsNotSet_PropertyHasError()
        {
            var mvvmGenerator = new ViewModelGenerator();
            var model = new Model();
            ViewModel generatedViewModel = mvvmGenerator.Generate<ViewModel>(model);

            Assert.That(generatedViewModel["Prop"], Is.Not.Null);
        }
    }
}