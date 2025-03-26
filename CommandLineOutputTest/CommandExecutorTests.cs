using System;
using CommandLineOutput.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommandLineOutput.Logic.Logic;

namespace CommandLineOutput.Tests
{
    [TestClass]
    public class CommandParserTests
    {
        [TestMethod]
        public void SplitCommandLine_EmptyString_ReturnsEmptyArray()
        {
            // Arrange
            string command = "";

            // Act
            string[] result = CommandParser.SplitCommandLine(command);

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void SplitCommandLine_SimpleCommand_ReturnsCorrectParts()
        {
            // Arrange
            string command = "echo Hello World";

            // Act
            string[] result = CommandParser.SplitCommandLine(command);

            // Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("echo", result[0]);
            Assert.AreEqual("Hello", result[1]);
            Assert.AreEqual("World", result[2]);
        }

        [TestMethod]
        public void SplitCommandLine_QuotedArguments_HandlesQuotesProperly()
        {
            // Arrange
            string command = "echo \"Hello World\" test";

            // Act
            string[] result = CommandParser.SplitCommandLine(command);

            // Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("echo", result[0]);
            Assert.AreEqual("Hello World", result[1]); // Quotes removed, but text preserved as one arg
            Assert.AreEqual("test", result[2]);
        }

        [TestMethod]
        public void SplitCommandLine_MultipleSpaces_HandlesCorrectly()
        {
            // Arrange
            string command = "dir  /s   *.txt";

            // Act
            string[] result = CommandParser.SplitCommandLine(command);

            // Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("dir", result[0]);
            Assert.AreEqual("/s", result[1]);
            Assert.AreEqual("*.txt", result[2]);
        }

        [TestMethod]
        public void SplitCommandLine_MixedQuotesAndSpaces_HandlesCorrectly()
        {
            // Arrange
            string command = "  find \"Program Files\" -name \"*.exe\"  ";

            // Act
            string[] result = CommandParser.SplitCommandLine(command);

            // Assert
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("find", result[0]);
            Assert.AreEqual("Program Files", result[1]); // Quotes removed but space preserved
            Assert.AreEqual("-name", result[2]);
            Assert.AreEqual("*.exe", result[3]); // Quotes removed
        }
    }
}