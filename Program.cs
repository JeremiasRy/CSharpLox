using System.Security.Principal;
using System.Text;

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
    }
}

void Run(string src)
{
    string[] tokens = src.Split(" ");
    foreach (var token in tokens)
    {
        Console.WriteLine(token);
    }
}