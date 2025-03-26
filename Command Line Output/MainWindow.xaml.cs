using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CommandLineOutput.Logic.Logic;
using CommandLineOutput.Logic;
namespace CommandLineOutput
{
    public partial class MainWindow : Window
    {
        private StringBuilder outputBuffer = new StringBuilder();
        private CommandExecutor commandExecutor;

        public MainWindow()
        {
            InitializeComponent();
            StatusBar.Text = "Ready";
            StatusBar.Foreground = new SolidColorBrush(Colors.White);

            // Initialize the command executor and hook up events
            commandExecutor = new CommandExecutor();
            commandExecutor.OutputReceived += CommandExecutor_OutputReceived;
            commandExecutor.ErrorReceived += CommandExecutor_ErrorReceived;
            commandExecutor.CommandCompleted += CommandExecutor_CommandCompleted;
        }

        private void CommandExecutor_OutputReceived(object sender, CommandOutputEventArgs e)
        {
            Dispatcher.Invoke(() => {
                AppendToOutput(e.Text, Colors.LightGray);
            });
        }

        private void CommandExecutor_ErrorReceived(object sender, CommandOutputEventArgs e)
        {
            Dispatcher.Invoke(() => {
                AppendToOutput(e.Text, Colors.Red);
            });
        }

        private void CommandExecutor_CommandCompleted(object sender, CommandCompletedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                // Update status based on exit code
                Color bgColor = e.ExitCode == 0 ? Color.FromRgb(0, 100, 0) : Color.FromRgb(139, 0, 0);
                StatusBar.Text = $"Process exited with code: {e.ExitCode}";
                StatusBar.Foreground = new SolidColorBrush(Colors.White);
                StatusBar.Background = new SolidColorBrush(bgColor);
            });
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteCommand(sender, new RoutedEventArgs());
            }
        }

        private async void ExecuteCommand(object sender, RoutedEventArgs e)
        {
            string command = CommandInput.Text.Trim();
            if (string.IsNullOrEmpty(command))
            {
                StatusBar.Text = "Please enter a command";
                StatusBar.Background = new SolidColorBrush(Color.FromRgb(0, 122, 204)); // Reset to default blue
                return;
            }

            // Clear previous output
            OutputTextBox.Document.Blocks.Clear();
            outputBuffer.Clear();
            StatusBar.Text = "Executing...";
            StatusBar.Background = new SolidColorBrush(Color.FromRgb(0, 122, 204)); // Reset to default blue

            // Parse the command and arguments
            string[] commandParts = CommandParser.SplitCommandLine(command);
            if (commandParts.Length == 0)
                return;

            string executable = commandParts[0];
            string arguments = commandParts.Length > 1 ? string.Join(" ", commandParts, 1, commandParts.Length - 1) : "";

            // Execute the command asynchronously
            await commandExecutor.ExecuteCommandAsync(executable, arguments);
        }

        private void AppendToOutput(string text, Color color)
        {
            // Add the text to our buffer
            outputBuffer.AppendLine(text);

            // Create a paragraph with the specified text color
            Paragraph paragraph = new Paragraph();

            Run run = new Run(text)
            {
                Foreground = new SolidColorBrush(color)
            };
            paragraph.Inlines.Add(run);

            // Add the paragraph to the document
            OutputTextBox.Document.Blocks.Add(paragraph);

            // Auto-scroll to the bottom
            OutputTextBox.ScrollToEnd();
        }
    }
}