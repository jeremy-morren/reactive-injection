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
        
        public static Task<ViewModel1> Load(string id, [FromServices] ViewModel3 service, [SharedState] List<double> state, CancellationToken ct)
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
    }
    
    public class ViewModel3
    {
        public ViewModel3(string str,
            [SharedState] ObservableCollection<SharedState> state,
            string str2)
        {
        }
    }
    
    public class ViewModel4
    {
        public ViewModel4([SharedState] SharedState state) {}   
    }
    
    public class ViewModel5
    {
        public ViewModel5(ReadOnlyObservableCollection<SharedState> parameter) {}   
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