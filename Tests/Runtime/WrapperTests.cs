using CommunityToolkit.Mvvm.Messaging;
using MVVMToolkit.Messaging;
using NUnit.Framework;

// ReSharper disable NotAccessedField.Local

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
            using var wrapper = new WrapperReference(messenger);

            object result = null;

            void Receive(object _, Wrapped<TestInt> message)
            {
                result = message.data;
            }
            messenger.Register<Wrapped<TestInt>>(recipient, Receive);

            var value = new TestInt { value = 62 };
            wrapper.Wrapper.Send(value);

            wrapper.Unwrap();

            Assert.AreEqual(value, result);
        }

        [Test]
        public static void ReuseWrapperTest()
        {
            var messenger = new StrongReferenceMessenger();
            var recipient = new object();
            using var wrapper = new WrapperReference(messenger);

            object result = null;

            void Receive(object _, Wrapped<TestInt> message)
            {
                result = message.data;
            }

            messenger.Register<Wrapped<TestInt>>(recipient, Receive);


            var original = new TestInt { value = 62 };
            wrapper.Wrapper.Send(original);
            wrapper.Unwrap();
            Assert.AreEqual(original, result);

            original = new() { value = -123 };
            wrapper.Wrapper.Send(original);
            wrapper.Unwrap();
            Assert.AreEqual(original, result);
        }

        private struct TestFloat : IUnmanagedMessage
        {
            public float value;
        }

        private struct TestByte : IUnmanagedMessage
        {
            public byte value;
        }

        private struct TestMultiple : IUnmanagedMessage
        {
            public float floatValue;
            public long longValue;
            public double doubleValue;
            public byte byteValue;
        }

        [Test]
        public static void MultipleMessageTest()
        {
            var messenger = new StrongReferenceMessenger();
            using var wrapper = new WrapperReference(messenger);
            var recipient1 = new object();
            var recipient2 = new object();
            var recipient3 = new object();
            var recipient4 = new object();

            var results = new object[4];

            void Receive1(object _, Wrapped<TestInt> message)
            {
                results[0] = message.data;
            }

            messenger.Register<Wrapped<TestInt>>(recipient1, Receive1);
            messenger.Register<Wrapped<TestFloat>>(recipient2,
                (_, message) => { results[1] = message.data; });
            messenger.Register<Wrapped<TestByte>>(recipient3,
                (_, message) => { results[2] = message.data; });
            messenger.Register<Wrapped<TestMultiple>>(recipient4,
                (_, message) => { results[3] = message.data; });

            var value1 = new TestInt { value = 500 };
            var value2 = new TestFloat { value = 123f };
            var value3 = new TestByte { value = 113 };
            var value4 = new TestMultiple
            {
                floatValue = 1f,
                longValue = long.MaxValue,
                doubleValue = double.MinValue,
                byteValue = 100
            };


            wrapper.Wrapper.Send(value1);
            wrapper.Wrapper.Send(value2);
            wrapper.Wrapper.Send(value3);
            wrapper.Wrapper.Send(value4);

            wrapper.Unwrap();

            CollectionAssert.AreEquivalent(new object[] { value1, value2, value3, value4 }, results);
        }
    }
}