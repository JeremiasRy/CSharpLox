namespace CSharpLox.Src;
interface ILoxCallable
{
    int Arity();
    object Call(Interpreter interpreter, List<object> arguments);
}