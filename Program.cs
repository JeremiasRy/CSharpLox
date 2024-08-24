using static CSharpLox.Src.Lox;

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