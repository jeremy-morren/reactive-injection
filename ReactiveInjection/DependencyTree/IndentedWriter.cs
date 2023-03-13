using System.Text;

namespace ReactiveInjection.DependencyTree;

public class IndentedWriter
{
    private readonly int _indentSize;
    
    public IndentedWriter(int indentSize = 4) => _indentSize = indentSize;
    
    private readonly StringBuilder _builder = new();

    private int _currentIndentLevel = 0;

    private void WriteIndent()
    {
        _builder.Append(new string(' ', _currentIndentLevel * 4));
    }

    /// <summary>
    /// Writes an indented line and increases the indent level by 1
    /// </summary>
    public void WriteLineAndPushIndent(string value)
    {
        WriteIndent();
        _builder.AppendLine(value);
        _currentIndentLevel++;
    }
    
    /// <summary>
    /// Writes a line without indent
    /// and increases the indent level by 1
    /// </summary>
    public void WriteLineWithoutIndentAndPushIndent(string value)
    {
        _builder.AppendLine(value);
        _currentIndentLevel++;
    }
    
    /// <summary>
    /// Writes an indented line and reduces the indent level by 1
    /// </summary>
    public void PopIndentAndWriteLine(string value)
    {
        if (_currentIndentLevel == 0)
            throw new InvalidOperationException("Indent level is already at 0");
        _currentIndentLevel--;
        WriteIndent();
        _builder.AppendLine(value);
    }

    /// <summary>
    /// Writes an indented line
    /// </summary>
    public void WriteLine(string value)
    {
        WriteIndent();
        _builder.AppendLine(value);
    }
    
    /// <summary>
    /// Writes an indented value (without a trailing newline)
    /// </summary>
    public void Write(string value)
    {
        WriteIndent();
        _builder.Append(value);
    }
    
    /// <summary>
    /// Writes value without any indent applied
    /// </summary>
    public void WriteRaw(string value)
    {
        _builder.Append(value);
    }
    
    /// <summary>
    /// Writes value without any indent applied
    /// </summary>
    public void WriteRawLine(string value)
    {
        _builder.AppendLine(value);
    }

    public void TrimEnd(int numChars)
    {
        if (numChars < 0)
            throw new ArgumentOutOfRangeException(nameof(numChars), numChars,
                "Value must be greater than or equal to 0");
        _builder.Length -= numChars;
    }

    public override string ToString() => _builder.ToString();
}