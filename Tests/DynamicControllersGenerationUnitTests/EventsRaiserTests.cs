using System;
using DynamicControllersGeneration;
using NUnit.Framework;

namespace DynamicControllersGenerationUnitTests
{
    [TestFixture]
    public class EventsRaiserTests
    {
        private const string EVENT_NAME = "Event";

        class ClassWithEvent
        {
            public event EventHandler Event;
        }

        class SonOfClassWithEvent : ClassWithEvent
        {
        }

        [Test]
        public void Raise_EventWithSingleSubscriber_SubscriberNotified()
        {
            var classWithEvent = new ClassWithEvent();
            bool wasRaised = false;
            classWithEvent.Event += (sender, args) => wasRaised = true;

            var eventsRaiser = new EventsRaiser(classWithEvent);
            eventsRaiser.Raise(EVENT_NAME, classWithEvent, EventArgs.Empty);

            Assert.That(wasRaised, Is.True);
        }

        [Test]
        public void Raise_EventWithSingleSubscriber_SubscriberNotifiedWithCorrectArguments()
        {
            var classWithEvent = new ClassWithEvent();
            object actualSender = null;
            object actualEventArgs = null;
            classWithEvent.Event += (sender, args) =>
                                        {
                                            actualSender = sender;
                                            actualEventArgs = args;
                                        };

            var eventsRaiser = new EventsRaiser(classWithEvent);
            eventsRaiser.Raise(EVENT_NAME, classWithEvent, EventArgs.Empty);

            var expectedArguments = new object[] { classWithEvent, EventArgs.Empty };
            var actualArguments = new[] { actualSender, actualEventArgs };
            Assert.That(actualArguments, Is.EqualTo(expectedArguments));
        }

        [Test]
        public void Raise_EventWithTwoSubscribers_FirstSubscriberNotified()
        {
            var classWithEvent = new ClassWithEvent();
            bool wasRaised = false;
            classWithEvent.Event += (sender, args) => wasRaised = true;
            classWithEvent.Event += (sender, args) => { };

            var eventsRaiser = new EventsRaiser(classWithEvent);
            eventsRaiser.Raise(EVENT_NAME, classWithEvent, EventArgs.Empty);

            Assert.That(wasRaised, Is.True);
        }

        [Test]
        public void Raise_EventWithTwoSubscribers_SecondSubscriberNotified()
        {
            var classWithEvent = new ClassWithEvent();
            bool wasRaised = false;
            classWithEvent.Event += (sender, args) => { };
            classWithEvent.Event += (sender, args) => wasRaised = true;

            var eventsRaiser = new EventsRaiser(classWithEvent);
            eventsRaiser.Raise(EVENT_NAME, classWithEvent, EventArgs.Empty);

            Assert.That(wasRaised, Is.True);
        }

        [Test]
        public void Raise_EventDefinedOnBase_SubscriberNotified()
        {
            var classWithEvent = new SonOfClassWithEvent();
            bool wasRaised = false;
            classWithEvent.Event += (sender, args) => wasRaised = true;

            var eventsRaiser = new EventsRaiser(classWithEvent);
            eventsRaiser.Raise(EVENT_NAME, classWithEvent, EventArgs.Empty);

            Assert.That(wasRaised, Is.True);
        }
    }
}