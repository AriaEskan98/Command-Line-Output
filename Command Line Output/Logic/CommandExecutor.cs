﻿using System;
using System.Diagnostics;

namespace CommandLineOutput.Logic.Logic
{
 
    /// Handles the execution of shell commands
    public class CommandExecutor
    {
       
        /// Event raised when output data is received
        public event EventHandler<CommandOutputEventArgs> OutputReceived;

       
        /// Event raised when error data is received
        public event EventHandler<CommandOutputEventArgs> ErrorReceived;

        
        /// Event raised when the process completes
        public event EventHandler<CommandCompletedEventArgs> CommandCompleted;

        private Process currentProcess;

       
        /// Executes a command with the given arguments 
        /// <param name="command">The command to execute</param>
        /// <param name="arguments">Arguments for the command</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task ExecuteCommandAsync(string command, string arguments)
        {
            try
            {
                // On Windows, we need to run commands through cmd.exe
                bool useShell = Environment.OSVersion.Platform == PlatformID.Win32NT;

                currentProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = useShell ? "cmd.exe" : command,
                        Arguments = useShell ? $"/c {command} {arguments}" : arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                // Set up event handlers for output and error streams
                currentProcess.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        OutputReceived?.Invoke(this, new CommandOutputEventArgs(args.Data));
                    }
                };

                currentProcess.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        ErrorReceived?.Invoke(this, new CommandOutputEventArgs(args.Data));
                    }
                };

                currentProcess.Start();
                currentProcess.BeginOutputReadLine();
                currentProcess.BeginErrorReadLine();

                await Task.Run(() => currentProcess.WaitForExit());

                // Notify that the command has completed
                CommandCompleted?.Invoke(this, new CommandCompletedEventArgs(currentProcess.ExitCode));
            }
            catch (Exception ex)
            {
                // Notify of the error as both an error message and completion with error code
                ErrorReceived?.Invoke(this, new CommandOutputEventArgs($"Error executing command: {ex.Message}"));
                CommandCompleted?.Invoke(this, new CommandCompletedEventArgs(-1));
            }
        }
    }

    
    /// Event arguments for command output
    public class CommandOutputEventArgs : EventArgs
    {
        public string Text { get; }

        public CommandOutputEventArgs(string text)
        {
            Text = text;
        }
    }


    /// Event arguments for command completion
    public class CommandCompletedEventArgs : EventArgs
    {
        public int ExitCode { get; }

        public CommandCompletedEventArgs(int exitCode)
        {
            ExitCode = exitCode;
        }
    }
}



