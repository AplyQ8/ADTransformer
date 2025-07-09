using CommandExecutor;

namespace ConsoleGUI;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to the Command Executor Console.");
        Console.WriteLine("Type your command (or 'exit' to quit):");

        var executor = new CommandExecutor.CommandExecutor();
        
        executor.StatusChanged += () =>
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"[Status] {executor.Status.StatusMessage}");
            Console.ResetColor();
        };

        executor.ProgressChanged += () =>
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[Progress] {executor.Status.Progress}%"); 
            Console.ResetColor();
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