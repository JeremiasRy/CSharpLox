using System.Text;
namespace CSharpLox.Src;
public static class Lox
{
    private static bool _hadError = false;
    private static bool _hadRuntimeError = false;
    private readonly static Interpreter _interpreter = new();
    public static void RunFile(string pathToFile)
    {
        using var fs = new FileStream(pathToFile, FileMode.Open);
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes);
        Run(Encoding.UTF8.GetString(bytes));
        if (_hadError)
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
            _interpreter.SetToReplSession();
            Run(input);
            _hadError = false;
        }
    }

    public static void Run(string src)
    {
        Scanner scanner = new(src);
        List<Token> tokens = scanner.ScanTokens();
        Parser parser = new(tokens);
        List<Stmt> statements = parser.Parse();
        if (_hadError || _hadRuntimeError || statements.Count == 0)
        {
            return;
        }
        try
        {
            _interpreter.Interpret(statements);
        }
        catch (NullReferenceException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Couldn't execute given code.");
            Console.ForegroundColor = ConsoleColor.White;
        }

    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }
    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }
    public static void Report(int line, string where, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[Line: {0}] Error {1}: {2}", line, where, message);
        Console.ForegroundColor = ConsoleColor.White;
    }
    public static void RuntimeError(RuntimeError error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[line: " + error.Token.Line + "]: " + error.Message);
        Console.ForegroundColor = ConsoleColor.White;
        _hadRuntimeError = true;
    }
}