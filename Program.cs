using System.Text;
bool hadError = false;
if (args.Length > 1)
{
    Console.WriteLine("Usage CSharpLox [script]");
    return;
}
else if (args.Length == 1)
{
    RunFile(args[0]);
}
else
{
    RunPrompt();
}

void RunFile(string pathToFile)
{
    using (var fs = new FileStream(pathToFile, FileMode.Open))
    {
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes);
        Run(Encoding.UTF8.GetString(bytes));
        if (hadError)
        {
            return;
        }
    }
}

void RunPrompt()
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

void Run(string src)
{
    string[] tokens = src.Split(" ");
    Error(12, "Invalid Token!");
    foreach (var token in tokens)
    {
        Console.WriteLine(token);
    }
}

void Error(int line, string message)
{
    Report(line, "", message);
}

void Report(int line, string where, string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[Line: {0}] Error {1}: {2}", line, where, message);
    Console.ForegroundColor = ConsoleColor.White;
}