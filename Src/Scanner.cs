using System.Dynamic;
using System.Runtime.CompilerServices;

namespace CSharpLox.Src;

public class Scanner
{
    string _source;
    List<Token> _tokens = [];
    int _start;
    int _current;
    int _line;

    public Scanner(string source)
    {
        _source = source;
    }

    List<Token> ScanTokens()
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
        var text = _source.Substring(_start, _current);
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
        }
    }
}