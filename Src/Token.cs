namespace CSharpLox.Src;

public class Token
{
    public required TokenType Type { get; init; }
    public required string Lexeme { get; init; }
    public required object Literal { get; init; }
    public required int Line { get; init; }

    public Token(TokenType type, string lexeme, object literal, int line)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
    }

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }

}

public enum TokenType
{
    // Single-character tokens
    LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
    COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,
    // One or two character tokens
    BANG, BANG_EQUAL, EQUAL, EQUAL_EQUAL,
    GREATER, GREATER_EQUAL, LESS, LESS_EQUAL,
    // Literals
    IDENTIFIER, STRING, NUMBER,
    // Keywords
    AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR,
    PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

    EOF
}