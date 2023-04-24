using MVVMToolkit.Binding;
using NUnit.Framework;

namespace MVVMToolkit.RuntimeTests
{
    public static class CommandPathFormatterTests
    {
        [Test]
        public static void TestCommandFormatter0()
        {
            var path = "Test";
            CommandUtility.FormatCommandPath(ref path);
            Assert.IsTrue(path.Equals("TestCommand"));
        }

        [Test]
        public static void TestCommandFormatter1()
        {
            var path = "OnTest";
            CommandUtility.FormatCommandPath(ref path);
            Assert.IsTrue(path.Equals("TestCommand"));
        }

        [Test]
        public static void TestCommandFormatter2()
        {
            var path = "OnTestCommand";
            CommandUtility.FormatCommandPath(ref path);
            Assert.IsTrue(path.Equals("TestCommand"));
        }

        [Test]
        public static void TestCommandFormatter3()
        {
            var path = "Nesting.OnTest";
            CommandUtility.FormatCommandPath(ref path);
            Assert.IsTrue(path.Equals("Nesting.TestCommand"));
        }
    }
}