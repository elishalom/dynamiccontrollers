using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace DynamicControllersGeneration
{
    internal class ModelInterceptor : IInterceptor
    {
        private readonly object model;
        private readonly Dictionary<string, string> errors = new Dictionary<string, string>();

        public ModelInterceptor(object model)
        {
            this.model = model;
        }

        public void RegisterProxy(object classProxy)
        {
            InitializeModelFields(classProxy);
            UpdatePropertiesErrors(classProxy);
        }

        private void InitializeModelFields(object classProxy)
        {
            var viewModelType = classProxy.GetType().BaseType;
            var viewModelFields = viewModelType.GetFields(BindingFlags.Instance|BindingFlags.NonPublic |BindingFlags.Public);
            var modelFields = viewModelFields
                .Where(fieldInfo =>fieldInfo.IsDefined(typeof(ModelAttribute), false))
                .Where(fieldInfo => fieldInfo.FieldType.IsAssignableFrom(model.GetType()));
            foreach (var modelField in modelFields)
            {
                modelField.SetValue(classProxy, model);
            }
        }

        public void Intercept(IInvocation invocation)
        {
            var methodAnalyzer = new MethodAnalyzer(invocation.Method);
            if (methodAnalyzer.IsIndexer)
            {
                var propertyName = (string) invocation.Arguments[0];
                if (errors.ContainsKey(propertyName))
                {
                    invocation.ReturnValue = errors[propertyName];
                }
                else
                {
                    invocation.Proceed();
                }
                
                return;
            }

            if (methodAnalyzer.IsSetter || methodAnalyzer.IsGetter)
            {
                ForwardCallToModel(invocation);
            }

            if (methodAnalyzer.IsSetter)
            {
                UpdateValidationErrors(invocation, methodAnalyzer);
                RaisePropertyChangedEvent(invocation, methodAnalyzer);
            }
        }

        private void UpdatePropertiesErrors(object classProxy)
        {
            foreach (var property in classProxy.GetType().BaseType.GetProperties()
                .Where(info => model.GetType().GetProperty(info.Name) != null))
            {
                var getter = property.GetGetMethod();
                if (getter.GetParameters().Length > 0)
                {
                    continue;
                }
                var value = getter.Invoke(classProxy, new object[0]);
                UpdatePropertyValidationErrors(property, value);
            }
        }

        private void UpdateValidationErrors(IInvocation invocation, MethodAnalyzer invocationMethodAnalyzer)
        {
            var property = invocationMethodAnalyzer.MatchingProperty;
            UpdatePropertyValidationErrors(property, invocation.Arguments[0]);
        }

        private void UpdatePropertyValidationErrors(PropertyInfo property, object value)
        {
            var validationAttributes = property
                .GetCustomAttributes(typeof (ValidationAttribute), true)
                .Cast<ValidationAttribute>();

            var propertyError = validationAttributes
                .Select(validationAttribute => validationAttribute.Validate(value))
                .FirstOrDefault(error => !string.IsNullOrEmpty(error));

            errors[property.Name] = propertyError;
        }

        private static void RaisePropertyChangedEvent(IInvocation invocation, MethodAnalyzer invocationMethodAnalyzer)
        {
            Type implementedInterface = invocation.TargetType.GetInterface(typeof (INotifyPropertyChanged).Name);
            var isImplementingNotifyPropertyChanged = implementedInterface == typeof (INotifyPropertyChanged);
            if(!isImplementingNotifyPropertyChanged)
            {
                return;
            }

            var changedPropertyName = invocationMethodAnalyzer.MatchingProperty.Name;
            var eventArgs = new PropertyChangedEventArgs(changedPropertyName);
            
            var eventsRaiser = new EventsRaiser(invocation.InvocationTarget);
            eventsRaiser.Raise("PropertyChanged", invocation.InvocationTarget, eventArgs);

            var relatedProperties = new MethodAnalyzer(invocation.Method).MatchingProperty.GetCustomAttributes(typeof(RelatedPropertyAttribute), true).Cast<RelatedPropertyAttribute>().Select(attribute => attribute.PropertyName);
            foreach (var relatedPropertyAttribute in relatedProperties)
            {
                var propertyChangedEventArgs = new PropertyChangedEventArgs(relatedPropertyAttribute);
                eventsRaiser.Raise("PropertyChanged", invocation.InvocationTarget, propertyChangedEventArgs);
            }
        }

        private void ForwardCallToModel(IInvocation invocation)
        {
            var methodInfo = model.GetType().GetMethod(invocation.Method.Name);
            invocation.ReturnValue = methodInfo.Invoke(model, invocation.Arguments);
        }
    }
}