// ReSharper disable All

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Threading;
using ReactiveInjection;
using StronglyTypedIds;

#nullable enable

namespace Tree.Models
{
    public record Model(Guid Id);
    
    public class SharedState { }
    
    public class Service { }  
    
    public class ViewModel1
    {
        public ViewModel1([SharedState] SharedState sharedState,
            Model model,
            ViewModel1 owner,
            [FromServices] Service service,
            [FromServices] List<int> values,
            [FromServices] List<int[]> valuesArray)
        {
        }
        
        public static Task<ViewModel1> Load(string id, [FromServices] ViewModel3 service, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
        
        [LoaderRoute("VM1/{id}")]
        public static Task<ViewModel1> Create(long id,
            [FromLoaderQuery] string name,
            [FromServices] ViewModel3 service,
            [FromParameters] ViewModel2 vm2,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
        
        [LoaderRoute("VM\"1/{id}")]
        public static Task<ViewModel1> Create2(bool id,
            [FromLoaderQuery("other\"")] string date,
            [FromServices] ViewModel3 service,
            [FromParameters] ISerializable owner,
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
    
    public class ViewModel2
    {
        public ViewModel2([SharedStateAttribute] SharedState sharedState,
            ViewModelFactory factory,
            [FromServices] Service service,
            [FromServices] IServiceProvider services)
        {
        }
        
        [LoaderRoute("VM2")]
        public static ViewModel2 CreateFromRoute(
            [FromParameters] ViewModel3 vm,
            [FromServices] ViewModel4 service, 
            CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
    
    public class ViewModel3
    {
        public ViewModel3(string str,
            [SharedState] ObservableCollection<SharedState> state,
            string str2)
        {
        }
        
        [LoaderRoute("VM3/{name}/{id?}")]
        public static Task<ViewModel3> Load(decimal? id, string name, [FromServices] IServiceProvider service, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
        
        [LoaderRoute("VM5/StrongId/{id}/{name}")]
        public static Task<ViewModel3> LoadStrongId(StrongId id, string name)
        {
            throw new NotImplementedException();
        }
    }
    
    public class ViewModel4
    {
        public ViewModel4([SharedState] SharedState state) {}   
        
        [LoaderRoute("VM4/{id}/{name}")]
        public static Task<ViewModel4> Load(int id, long name)
        {
            throw new NotImplementedException();
        }
        
        [LoaderRoute("VM4/{id}")]
        public static Task<ViewModel4> LoadGeneratedStrongId(GeneratedStrongId id)
        {
            throw new NotImplementedException();
        }
    }
    
    public class ViewModel5
    {
        public ViewModel5(ReadOnlyObservableCollection<SharedState> parameter) {}   
        
        [LoaderRoute("{Token?}")]
        public static Task<ViewModel5> Load(string token, [FromServices] ViewModel3 service, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
        
        [LoaderRoute("VM5/{Param2}/{Token?}")]
        public static Task<ViewModel5> Load(string param2, string? token)
        {
            throw new NotImplementedException();
        }
        
        [LoaderRoute("VM5/{Param?}")]
        public static Task<ViewModel5> Load(uint? param)
        {
            throw new NotImplementedException();
        }
        
        [LoaderRoute("VM5/StrongId/{id?}")]
        public static Task<ViewModel5> LoadStrongId(StrongId? id)
        {
            throw new NotImplementedException();
        }
        
        public static Task<ViewModel5> LoadByFactory(uint? param)
        {
            throw new NotImplementedException();
        }
        
        public static Task<ViewModel5> LoadByFactory(string? param)
        {
            throw new NotImplementedException();
        }
        
        public static Task<ViewModel5> Load()
        {
            throw new NotImplementedException();
        }
    }
    
    public class ViewModel6
    {
    }
    
    [ReactiveFactory(typeof(ViewModel1))]
    [ReactiveFactory(typeof(ViewModel2))]
    [ReactiveFactory(typeof(ViewModel3))]
    [ReactiveFactory(typeof(ViewModel4))]
    [ReactiveFactory(typeof(ViewModel5))]
    [ReactiveFactory(typeof(ViewModel6))]
    [Serializable] //Test random attribute
    public partial class ViewModelFactory
    {
    }

    public readonly record struct StrongId : IParsable<StrongId>
    {
        public static StrongId Parse(string s, IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out StrongId result)
        {
            throw new NotImplementedException();
        }
    }
    
    [StronglyTypedId(Template.String)]
    public partial struct GeneratedStrongId {}
}