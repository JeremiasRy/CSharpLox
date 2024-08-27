using System.Collections;
using System.Diagnostics;

namespace CSharpLox.Src;

public class Scanner(string source)
{
    string _source = source;
    List<Token> _tokens = [];
    int _start;
    int _current;
    int _line;

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
            ScanToken();
        }
        _tokens.Add(new(TokenType.EOF, "", null, _line));
        return _tokens;
    }
    bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    char Advance()
    {
        return _source[_current++];
    }

    void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    void AddToken(TokenType type, object? literal)
    {
        var text = _source[_start.._current];
        _tokens.Add(new(type, text, literal, _line));
    }

    void ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '{': AddToken(TokenType.LEFT_BRACE); break;
            case '}': AddToken(TokenType.RIGHT_BRACE); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '.': AddToken(TokenType.DOT); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '*': AddToken(TokenType.STAR); break;

            case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
            case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
            case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
            case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
            case '?': AddToken(TokenType.QUESTION_MARK); break;
            case ':': AddToken(TokenType.COLON); break;

            case '/':
                {
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else if (Match('*'))
                    {
                        while (Peek() != '*' && PeekNext() != '/')
                        {
                            if (Peek() == '\n')
                            {
                                _line++;
                            }
                            Advance();
                        }
                        Advance();
                        Advance();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                }
                break;

            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                _line++;
                break;

            case '"': String(); break;

            default:
                {
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Console.WriteLine("Unexpected {0}", c);
                        Lox.Error(_line, "Unexpected character.");
                    }
                    break;
                }
        }
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }
        if (_source[_current] != expected)
        {
            return false;
        }

        _current++;
        return true;
    }

    private char Peek()
    {
        if (IsAtEnd())
        {
            return '\0';
        }
        return _source[_current];
    }

    private char PeekNext()
    {
        if (_current + 1 >= _source.Length)
        {
            return '\0';
        }
        return _source[_current + 1];
    }

    private static bool IsAlpha(char c)
    {
        return char.IsAsciiLetter(c);
    }

    private static bool IsAlphaNumeric(char c)
    {
        return char.IsAsciiLetterOrDigit(c);
    }

    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _line++;
            }
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(_line, "Unterminated string");
            return;
        }

        Advance();

        string value = _source[(_start + 1)..(_current - 1)];
        AddToken(TokenType.STRING, value);
    }

    private void Number()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();
        }
        while (IsDigit(Peek()))
        {
            Advance();
        }
        ReadOnlySpan<char> numberStr = _source.AsSpan(_start, _current - _start);

        if (double.TryParse(numberStr, out double number))
        {
            AddToken(TokenType.NUMBER, number);
        }
        else
        {
            Lox.Error(_line, $"Invalid number literal {numberStr}");
        }
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }
        string text = _source[_start.._current];
        if (_keywords.TryGetValue(text, out TokenType type))
        {
            AddToken(type);
            return;
        }

        AddToken(TokenType.IDENTIFIER);
    }

    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }
    static readonly Dictionary<string, TokenType> _keywords = new() {
        { "and", TokenType.AND },
        { "class", TokenType.CLASS },
        { "else", TokenType.ELSE },
        { "false", TokenType.FALSE },
        { "for", TokenType.FOR },
        { "fun", TokenType.FUN },
        { "if", TokenType.IF },
        { "nil", TokenType.NIL },
        { "or", TokenType.OR },
        { "print", TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super", TokenType.SUPER },
        { "this", TokenType.THIS },
        { "true", TokenType.TRUE },
        { "var", TokenType.VAR },
        { "while", TokenType.WHILE },
        { "break", TokenType.BREAK },
        { "continue", TokenType.CONTINUE },
    };
}