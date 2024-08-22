using System.Text;
namespace CSharpLox.Src;
public static class Lox
{
    private static bool hadError = false;
    public static void RunFile(string pathToFile)
    {
        using var fs = new FileStream(pathToFile, FileMode.Open);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes);
        Run(Encoding.UTF8.GetString(bytes));
        if (hadError)
        {
            return;
        }
    }

    public static void RunPrompt()
    {

        Console.WriteLine("Write 'exit' to close the program");
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (input == null || input == "exit")
            {
                break;
            }
            Run(input);
            hadError = false;
        }
    }

    public static void Run(string src)
    {
        Scanner scanner = new(src);
        List<Token> tokens = scanner.ScanTokens();
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Report(int line, string where, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[Line: {0}] Error {1}: {2}", line, where, message);
        Console.ForegroundColor = ConsoleColor.White;
    }
}