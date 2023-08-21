using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveInjection.Framework;
using ReactiveInjection.Symbols;

// ReSharper disable InvertIf

namespace ReactiveInjection.ModelInjection;

internal static class InjectionImplementationWriter
{
    public static string Generate(ModelInjectionTree tree)
    {
        //We are aiming for early C# here
        
        var w = new IndentedWriter();
        w.WriteFileHeader("enable");
        
        w.WriteLine("using ReactiveUI;");
        w.WriteLine($"namespace {tree.ViewModel.Namespace}");
        w.WriteLineThenPush('{');
        
        w.WriteClassAttributes();
        w.WriteLine($"partial class {tree.ViewModel.Name}");
        w.WriteLineThenPush('{');

        foreach (var model in tree.Models)
        {
            if (!model.VmProperty.CanWrite || model.VmProperty.IsInitOnly)
                ReadOnlyModel(w, model);
            else
                WritableModel(w, model);
        }
        
        w.PopThenWriteLine('}');
        w.PopThenWriteLine('}');

        return w.ToString();
    }

    private static void VerifyModelNotNull(IndentedWriter writer, Model model)
    {
        writer.WriteLine($"if (this.{model.Name} == null!)");
        writer.WriteLineThenPush('{');
        writer.WriteLine($"throw new InvalidOperationException(\"Backing model {model.Name} is null\");");
        writer.PopThenWriteLine('}');
    }

    private static void WriteDocumentation(IndentedWriter writer, IProperty model)
    {
        if (string.IsNullOrEmpty(model.DocumentationXml)) return;
        
        var doc = model.DocumentationXml!.Replace(Environment.NewLine, $"\n{writer.Indent}///");
        writer.WriteRawLine($"\n{writer.Indent}///{doc}");
    }
    
    private static void ReadOnlyModel(IndentedWriter writer, Model model)
    {
        /*
         * If prop is nullable
         * int? Prop => Model?.Prop;
         *
         * 
         * 
         *
         * int Prop => Model
         * {
         *   get
         *   {
         *     If model is not nullable:
         *     if (Model == null) throw;
         *     
         *     return Model.Prop;
         *   }
         *   set
         *   {
         *     If (Model == null) throw;
         *     If prop is not nullable and model is nullable:
         *     if (value == null) throw;
         *     Model.Prop = value;
         * }
         * 
         */
        
        foreach (var prop in model.Properties)
        {
            WriteDocumentation(writer, prop);
            if (model.VmProperty.IsNullable)
            {
                var nullSymbol = !prop.Type.IsNullable ? "?" : null; //Add null symbol if property is not nullable
            
                writer.WriteLine($"public {prop.Type.CSharpName}{nullSymbol} {prop.Name}");
                writer.WriteLineThenPush('{');
                
                writer.WriteDebuggerAttributes();
                writer.WriteLine($"get => this.{model.Name}?.{prop.Name};");
            }
            else
            {
                writer.WriteLine($"public {prop.Type.CSharpName} {prop.Name}");
                writer.WriteLineThenPush('{');
            
                writer.WriteDebuggerAttributes();
                writer.WriteLine("get");
                writer.WriteLineThenPush('{');
                VerifyModelNotNull(writer, model);
                writer.WriteLine($"return this.{model.Name}.{prop.Name};");
                writer.PopThenWriteLine('}');
            }

            writer.PopThenWriteLine('}');
            writer.WriteLine();
        }
    }

    private static void WritableModel(IndentedWriter writer, Model model)
    {
        /*
         * If prop is nullable
         * int? Prop => Model?.Prop;
         * set
         * {
         *   init();
         *
         *   Set - see method below
         * }
         *
         *
         * If prop is not nullable:
         *
         * int Prop => Model
         * {
         *   get
         *   {
         *     if (Model == null) throw;
         *     return Model.Prop;
         *   }
         *   if property is writable (and not init-only)
         *   set
         *   {
         *     if (Model == null) throw;
         *     set - see method below
         *   }
         * }
         *
         */

        //The SetModel(Model value) method
        if (model.Properties.Any())
            WriteSetModelMethod(writer, model);
        
        if (model.VmProperty.IsNullable)
        {
            //the 'init()' method
            WriteModelFactory(writer, model.VmProperty);
            
            foreach (var prop in model.Properties)
            {
                WriteDocumentation(writer, prop);
            
                var nullSymbol = prop.Type.IsNullable ? string.Empty : "?"; //Add null symbol if needed

                writer.WriteLine($"public {prop.Type.CSharpName}{nullSymbol} {prop.Name}");
                writer.WriteLineThenPush('{');
                
                writer.WriteDebuggerAttributes();
                writer.WriteLine($"get => this.{model.Name}?.{prop.Name};");

                if (prop is { CanWrite: true, IsInitOnly: false })
                {
                    writer.WriteDebuggerAttributes();
                    writer.WriteLine("set");
                    writer.WriteLineThenPush('{');
                    writer.WriteLine($"init_{model.Name}();");
                    writer.WriteLine();
                    WriteSetProperty(writer, model.VmProperty, prop);
                    
                    writer.PopThenWriteLine('}');
                }
                
                writer.PopThenWriteLine('}');
                writer.WriteLine();
            }
        }
        else
        {
            foreach (var prop in model.Properties)
            {
                WriteDocumentation(writer, prop);
            
                writer.WriteLine($"public {prop.Type.CSharpName} {prop.Name}");
                writer.WriteLineThenPush('{');
                
                writer.WriteDebuggerAttributes();
                writer.WriteLine("get");
                writer.WriteLineThenPush('{');
                VerifyModelNotNull(writer, model);
                writer.WriteLine($"return this.{model.Name}.{prop.Name};");
                writer.PopThenWriteLine('}');

                if (prop is { CanWrite: true, IsInitOnly: false })
                {
                    writer.WriteDebuggerAttributes();
                    writer.WriteLine("set");
                    writer.WriteLineThenPush('{');

                    VerifyModelNotNull(writer, model);
                    writer.WriteLine();
                
                    WriteSetProperty(writer, model.VmProperty, prop);
                    writer.PopThenWriteLine('}');
                }

                writer.PopThenWriteLine('}');
                writer.WriteLine();
            }
        }
        
    }

    private static void WriteSetProperty(IndentedWriter writer, IProperty model, IProperty prop)
    {
        //NB: this.RaisePropertyChanging is from ReactiveUI
        
        /*
         * if (EqualityComparer<int>.Default.Equals(Model.Prop, value)) return;
         * 
         * this.RaisePropertyChanging("Model");
         * this.RaisePropertyChanging("Prop");
         * 
         * Model.Prop = value;
         * 
         * this.RaisePropertyChanged("Model");
         * this.RaisePropertyChanged("Prop");
         */
        
        writer.WriteLine($"if ({EqualityComparerEquals(prop.Type)}(this.{model.Name}.{prop.Name}, value)) {{ return; }}");

        WritePropertyChanging(writer, prop.Name);
        WritePropertyChanging(writer, model.Name);
        writer.WriteLine($"{model.Name}.{prop.Name} = value;");
        WritePropertyChanged(writer, prop.Name);
        WritePropertyChanged(writer, model.Name);
    }
    
    private static void WriteModelFactory(IndentedWriter writer, IProperty model)
    {
        writer.WriteDebuggerAttributes();
        writer.WriteLine(EditorBrowsableNever);
        writer.WriteLine(MethodSynchronized);
        writer.WriteLine($"private void init_{model.Name}()");
        writer.WriteLineThenPush('{');
        writer.WriteLine($"if (this.{model.Name} != null!) {{ return; }}");
        
        //Note: all properties/parameters are 'default!'
        
        //Get constructor with fewest parameters
        var constructor = model.Type.Constructors.OrderBy(c => c.Parameters.Count()).First();
        writer.Write($"this.{model.Name} = new {model.Type.CSharpName}(");

        var @params = constructor.Parameters.Select(p => $"default({p.Type.CSharpName})!");
        writer.WriteRawLine($"{string.Join(",", @params)})");
        writer.WriteLineThenPush('{');

        //Initialize all 'required' properties

        foreach (var prop in model.Type.Properties.Where(p => !p.IsStatic && p.IsRequired))
        {
            writer.WriteLine($"{prop.Name} = default({prop.Type.CSharpName})!,");
        }
        writer.PopThenWriteLine("};");

        writer.PopThenWriteLine('}');
        writer.WriteLine();
    }
    
    private static void WriteSetModelMethod(IndentedWriter writer, Model model)
    {
        /*
         * Generate a set model method that updates all properties as appropriate
         *
         * private void SetModel(object value)
         * {
         *   if nullable
         *   if (value == null)
         *     throw new ArgumentNullException("value");
         *   List<string> changed = new();
         *   if (!Equals(this.Model.Prop, value.Prop)) { OnPropertyChanging("Prop"); changed.Add("Prop") ; }
         *
         *   this.Model = value;
         *
         *   foreach (var prop in changed) { OnPropertyChanged(prop); }
         * }
         */
        writer.WriteDebuggerAttributes();
        writer.WriteLine(MethodSynchronized);
        writer.WriteLine($"protected void Set{model.Name}({model.CSharpName} value)");
        writer.WriteLineThenPush('{');

        if (!model.VmProperty.IsNullable)
            writer.WriteLine("if (value == null) { throw new ArgumentNullException(\"value\"); }");
        
        //If references are equal, then skip
        writer.WriteLine($"if (object.ReferenceEquals(this.{model.Name}, value)) {{ return; }}");
        
        //Note: the existing value may be null, so we ignore model nullability from here

        writer.WriteLine($"List<string> changed = new List<string>(capacity: {model.Properties.Count});");
        
        foreach (var prop in model.Properties)
        {
            //We need a nullable equality comparer, if property is not nullable
            var csharpName = $"{prop.Type.CSharpName}{(!prop.IsNullable ? "?" : null)}";

            writer.WriteLine(
                $"if (!{EqualityComparerEquals(csharpName)}(this.{model.Name}?.{prop.Name}, value.{prop.Name}))");
            writer.WriteLineThenPush('{');
            writer.WriteLine($"changed.Add(\"{prop.Name}\");");
            WritePropertyChanging(writer, prop.Name);
            writer.PopThenWriteLine('}');
        }

        WritePropertyChanging(writer, model.Name);
        writer.WriteLine($"this.{model.Name} = value;");
        WritePropertyChanged(writer, model.Name);

        writer.WriteLine("foreach (string prop in changed)");
        writer.WriteLineThenPush('{');
        writer.WriteLine("this.RaisePropertyChanged(propertyName: prop);");
        writer.PopThenWriteLine('}');

        writer.PopThenWriteLine('}');
        writer.WriteLine();
    }

    private static void WritePropertyChanging(IndentedWriter writer, string propertyName)
    {
        writer.WriteLine($"this.RaisePropertyChanging(propertyName: \"{propertyName}\");");
    }
    
    private static void WritePropertyChanged(IndentedWriter writer, string propertyName)
    {
        writer.WriteLine($"this.RaisePropertyChanged(propertyName: \"{propertyName}\");");
    }

    private static string EqualityComparerEquals(IType type) => EqualityComparerEquals(type.CSharpName);
    
    private static string EqualityComparerEquals(string csharpName) => 
        $"global::{typeof(EqualityComparer<>).Namespace}.EqualityComparer<{csharpName}>.Default.Equals";

    private static readonly string EditorBrowsableNever =
        $"[global::{typeof(EditorBrowsableAttribute).FullName}({typeof(EditorBrowsableState).FullName}.{nameof(EditorBrowsableState.Never)})]";
    
    private static readonly string MethodSynchronized =
        $"[global::{typeof(MethodImplAttribute).FullName}({typeof(MethodImplOptions).FullName}.{nameof(MethodImplOptions.Synchronized)})]";
}