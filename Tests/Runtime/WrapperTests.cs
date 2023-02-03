using System;
using CommunityToolkit.Mvvm.Messaging;
using MVVMToolkit.Messaging;
using NUnit.Framework;
using Unity.Collections;

namespace MVVMToolkit.RuntimeTests
{
    public static class WrapperTests
    {
        private struct TestInt : IUnmanagedMessage
        {
            public int value;
        }

        [Test]
        public static void OneMessageTest()
        {
            var messenger = new StrongReferenceMessenger();
            var recipient = new object();
            using var wrapper = new UnmanagedWrapperReference();

            void Receive(object _, Wrapped<TestInt> message)
            {
                if (message.data.value != 62) throw new("Value differs");
            }

            messenger.Register<Wrapped<TestInt>>(recipient, Receive);

            wrapper.Wrapper.Send(new TestInt { value = 62 });

            wrapper.Wrapper.Unwrap(messenger);
        }

        [Test]
        public static void MultipleMessageTest()
        {
            var messenger = new StrongReferenceMessenger();
            var recipient1 = new object();
            var recipient2 = new object();
            using var wrapper = new UnmanagedWrapperReference();

            void Receive1(object _, Wrapped<TestInt> message)
            {
                if (message.data.value != 62) throw new("Value differs");
            }

            void Receive2(object _, Wrapped<TestInt> message)
            {
                if (message.data.value != -123) throw new("Value differs");
            }

            messenger.Register<Wrapped<TestInt>>(recipient1, Receive1);

            wrapper.Wrapper.Send(new TestInt { value = 62 });
            wrapper.Wrapper.Unwrap(messenger);

            messenger.UnregisterAll(recipient1);
            messenger.Register<Wrapped<TestInt>>(recipient2, Receive2);

            wrapper.Wrapper.Send(new TestInt { value = -123 });
            wrapper.Wrapper.Unwrap(messenger);
        }
    }
}