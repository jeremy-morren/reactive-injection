// ReSharper disable All

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using ReactiveInjection;

namespace Tree.Models
{
    public record Model(Guid Id);
    
    public class SharedState { }
    
    public class Service { }  
    
    public class ViewModel1
    {
        public ViewModel1([SharedState] SharedState sharedState,
            Model model,
            [FromServices] Service service,
            [FromServices] List<int> values,
            [FromServices] List<int[]> valuesArray)
        {
        }
        
        [NavigationRoute("VM1/{id}")]
        public static Task<ViewModel1> Load(string id, [FromServices] ViewModel3 service, CancellationToken ct)
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
        
        [NavigationRoute("VM2")]
        public static Task<ViewModel2> Load([FromServices] ViewModel3 service, CancellationToken ct)
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
        
        [NavigationRoute("VM3/{id:decimal}/{name}")]
        public static Task<ViewModel3> Load(decimal id, string name, [FromServices] IServiceProvider service, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
        
        [NavigationRoute("VM3/{b:bool}/{name}")]
        public static Task<ViewModel3> Load2(bool b, [FromServices] IServiceProvider service, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
    
    public class ViewModel4
    {
        public ViewModel4([SharedState] SharedState state) {}   
        
        [NavigationRoute("VM4/{id:int}/{name}")]
        public static Task<ViewModel4> Load(int id)
        {
            throw new NotImplementedException();
        }
    }
    
    public class ViewModel5
    {
        public ViewModel5(ReadOnlyObservableCollection<SharedState> parameter) {}   
        
        [NavigationRoute("{Token}")]
        public static Task<ViewModel5> Load(string token, [FromServices] ViewModel3 service, CancellationToken ct)
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
}