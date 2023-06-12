using System;
using System.Text;

namespace ReactiveInjection.DependencyTree;

/// <summary>
/// Provides an indented wrapper of <see cref="StringBuilder"/>
/// that always uses <c>LF</c> line endings
/// </summary>
public class IndentedWriter
{
    private readonly int _indentSize;
    
    public IndentedWriter(int indentSize = 4) => _indentSize = indentSize;

    private readonly StringBuilder _builder = new();

    private int _currentIndentLevel = 0;

    private void WriteIndent()
    {
        _builder.Append(new string(' ', _currentIndentLevel * _indentSize));
    }

    private void AppendLine(string value)
    {
        _builder.Append(value);
        _builder.Append('\n');
    }
    
    private void AppendLine()
    {
        _builder.Append('\n');
    }
    
    /// <summary>
    /// Increases the indent level by 1
    /// </summary>
    public void Push()
    {
        _currentIndentLevel++;
    }
    
    /// <summary>
    /// reduces indent level by 1
    /// </summary>
    public void Pop()
    {
        if (_currentIndentLevel == 0)
            throw new InvalidOperationException("Indent level is already at 0");
        _currentIndentLevel--;
    }

    /// <summary>
    /// Writes an indented line and increases the indent level by 1
    /// </summary>
    public void WriteLineAndPush(string value)
    {
        WriteIndent();
        AppendLine(value);
        _currentIndentLevel++;
    }
    
    /// <summary>
    /// Writes a line without indent
    /// and increases the indent level by 1
    /// </summary>
    public void WriteLineWithoutIndentAndPush(string value)
    {
        AppendLine(value);
        _currentIndentLevel++;
    }
    
    /// <summary>
    /// Writes a line without indent and reduces the indent level by 1
    /// </summary>
    public void WriteRawLineAndPop(string value)
    {
        if (_currentIndentLevel == 0)
            throw new InvalidOperationException("Indent level is already at 0");
        AppendLine(value);
        _currentIndentLevel--;
    }

    /// <summary>
    /// Writes an indented line and reduces the indent level by 1
    /// </summary>
    public void PopThenWriteLine(string value)
    {
        if (_currentIndentLevel == 0)
            throw new InvalidOperationException("Indent level is already at 0");
        _currentIndentLevel--;
        WriteIndent();
        AppendLine(value);
    }

    /// <summary>
    /// Writes an indented line
    /// </summary>
    public void WriteLine(char c)
    {
        WriteIndent();
        _builder.Append(c);
        AppendLine();
    }
    
    /// <summary>
    /// Writes an indented line
    /// </summary>
    public void WriteLine(string value)
    {
        WriteIndent();
        AppendLine(value);
    }
    
    /// <summary>
    /// Writes an empty
    /// </summary>
    public void WriteLine()
    {
        //Empty line, we don't need indent
        AppendLine();
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
    public void WriteRawLine(char c)
    {
        _builder.Append(c);
        AppendLine();
    }
    
    /// <summary>
    /// Writes value without any indent applied
    /// </summary>
    public void WriteRawLine(string value)
    {
        AppendLine(value);
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