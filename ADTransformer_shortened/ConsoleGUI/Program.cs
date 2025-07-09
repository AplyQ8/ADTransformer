using CommandExecutor;

namespace ConsoleGUI;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to the Command Executor Console.");
        Console.WriteLine("Type your command (or 'exit' to quit):");

        var executor = new CommandExecutor.CommandExecutor();
            
        //int statusLine = -1;
        executor.StatusChanged += () =>
        {
            // if (statusLine == -1)
            // {
            //     statusLine = Console.CursorTop;
            //     Console.WriteLine();
            // }
            //
            // int currentLeft = Console.CursorLeft;
            // int currentTop = Console.CursorTop;
            //
            // Console.SetCursorPosition(0, statusLine);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"[Status] {executor.Status.StatusMessage}");
            Console.ResetColor();

            //Console.SetCursorPosition(currentLeft, currentTop);
        };


        //int progressLine = -1;

        executor.ProgressChanged += () =>
        {
            // if (progressLine == -1)
            // {
            //     progressLine = Console.CursorTop;
            //     Console.WriteLine(); 
            // }
            //
            // int currentLeft = Console.CursorLeft;
            // int currentTop = Console.CursorTop;
            //
            // Console.SetCursorPosition(0, progressLine);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[Progress] {executor.Status.Progress}%"); 
            Console.ResetColor();

            //Console.SetCursorPosition(currentLeft, currentTop);
        };

        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();
        
            if (string.IsNullOrWhiteSpace(input))
                continue;
        
            if (input.Trim().ToLower() == "exit")
                break;
        
            try
            {
                await executor.Execute(input);
            }
            catch (CommandExecutorException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error] {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Unhandled Exception] {ex}");
                Console.ResetColor();
            }
        }
        


        Console.WriteLine("Goodbye.");
    }
}