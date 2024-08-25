/**
Thanks to:
https://stackoverflow.com/a/19034501 
*/
namespace CSharpLox.Src;
public sealed class ThankYou
{
    private ThankYou() { }
    private readonly static ThankYou bye = new();
    public static ThankYou Bye { get { return bye; } }
}