using MVVMToolkit.Binding;
using NUnit.Framework;

namespace MVVMToolkit.RuntimeTests
{
    public class BindingTests
    {
        [Test]
        public static void TestFormatting()
        {
            var str = "Test {0} {Test} {@hello} {$'}";
            var result = BindingUtility.GetFormatKeys(str);

            CollectionAssert.AreEquivalent(result, new[] { "0", "Test", "@hello", "$'" });
        }

        [Test]
        public static void TestFormattingFail()
        {
            Assert.Throws<BindingUtility.StringParsingException>(() =>
            {
                var str = "Test {hello} {world welcome";
                BindingUtility.GetFormatKeys(str);
            });
        }
    }
}