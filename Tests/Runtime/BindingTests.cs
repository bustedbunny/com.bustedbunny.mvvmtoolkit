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
            var result = ParsingUtility.GetFormatKeys(str);

            CollectionAssert.AreEquivalent(result, new[] { "0", "Test", "@hello", "$'" });
        }

        [Test]
        public static void TestFormattingFail()
        {
            Assert.Throws<ParsingUtility.StringParsingException>(() =>
            {
                var str = "Test {hello} {world welcome";
                ParsingUtility.GetFormatKeys(str);
            });
        }
    }
}