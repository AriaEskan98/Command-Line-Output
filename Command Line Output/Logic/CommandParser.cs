using System;
using System.Text;

namespace CommandLineOutput.Logic
{
    /// <summary>
    /// Handles command line parsing logic separated from UI
    /// </summary>
    public class CommandParser
    {
        /// <summary>
        /// Splits a command line string into an array of arguments, handling quoted strings properly
        /// </summary>
        /// <param name="commandLine">The command line to parse</param>
        /// <returns>Array of command and arguments</returns>
        public static string[] SplitCommandLine(string commandLine)
        {
            if (string.IsNullOrEmpty(commandLine))
                return Array.Empty<string>();

            var result = new System.Collections.Generic.List<string>();
            var currentParam = new StringBuilder();
            bool inQuotes = false;

            foreach (char c in commandLine)
            {
                if (c == '"')
                {
                    // Toggle quote state but don't add the quote character
                    inQuotes = !inQuotes;
                }
                else if (c == ' ' && !inQuotes)
                {
                    // Space outside of quotes marks the end of an argument
                    if (currentParam.Length > 0)
                    {
                        result.Add(currentParam.ToString());
                        currentParam.Clear();
                    }
                }
                else
                {
                    // Add the character to the current parameter
                    currentParam.Append(c);
                }
            }

            // Add the last parameter if there is one
            if (currentParam.Length > 0)
            {
                result.Add(currentParam.ToString());
            }

            return result.ToArray();
        }
    }
}