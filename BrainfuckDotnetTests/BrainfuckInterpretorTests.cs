using BrainfuckDotnet;

namespace BrainfuckDotnetTests
{
    [TestClass]
    public sealed class BrainfuckInterpretorTests
    {
        [TestMethod]
        [DataRow("[.]", "")]
        public void Interpret_UsingValidCommand_ReturnsExpectedString(string inputString, string expectedOutput)
        {
            var interpretor = new BrainfuckInterpretor();

            await interpretor.InterpretAsync(inputString);

            Assert.AreEqual(expectedOutput, output);
        }
    }
}
