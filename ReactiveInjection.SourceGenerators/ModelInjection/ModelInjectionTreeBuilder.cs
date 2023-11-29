using ReactiveInjection.SourceGenerators.Framework;
using ReactiveInjection.SourceGenerators.Symbols;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

namespace ReactiveInjection.SourceGenerators.ModelInjection;

internal class ModelInjectionTreeBuilder
{
    private readonly ErrorLogWriter _log;
    
    public ModelInjectionTreeBuilder(IErrorLog log)
    {
        _log = new ErrorLogWriter(log);
    }

    public bool Build(IType viewmodel, out ModelInjectionTree tree)
    {
        tree = null!;
        if (!viewmodel.IsPartial)
        {
            _log.ViewModelIsNotPartial(viewmodel);
            return false;
        }
        
        //TODO: check multiple properties of same type
        
        var properties = viewmodel.Properties
            .Where(AttributeHelpers.HasBackingModelAttribute)
            .ToList();

        var models = new List<Model>();
        foreach (var model in properties)
        {
            if (!model.CanRead)
            {
                _log.PropertyNotReadable(viewmodel, model);
                continue;
            }

            if (model.IsStatic)
            {
                _log.PropertyIsStatic(viewmodel, model);
                continue;
            }

            if (!model.Type.IsReferenceType)
            {
                _log.ModelNotReferenceType(viewmodel, model);
                continue;
            }

            var exclude = new HashSet<string>(model.Attributes.First(AttributeHelpers.IsBackingModelAttribute).StringParams, StringComparer.Ordinal);
            models.Add(new Model()
            {
                VmProperty = model,
                //TODO: filter by attribute
                Properties = model.Type.Properties
                    .Where(p => p.Name != "EqualityContract")
                    .Where(p => !exclude.Contains(p.Name))
                    .Where(p => p.IsPublic && !p.IsStatic)
                    .ToList()
            });
        }

        if (models.Count != properties.Count)
            return false;

        tree = new ModelInjectionTree()
        {
            ViewModel = viewmodel,
            Models = models
        };
        return true;
    }
}