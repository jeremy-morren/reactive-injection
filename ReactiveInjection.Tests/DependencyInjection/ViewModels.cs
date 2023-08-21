// ReSharper disable All

using ReactiveInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tree.Models
{
    public record Model(Guid Id);
    
    public class SharedState { }
    
    public class Service { }  
    
    public class ViewModel1
    {
        public ViewModel1([SharedStateAttribute] SharedState sharedState,
            Model model,
            [FromServicesAttribute] Service service,
            [FromServicesAttribute] List<int> values,
            [FromServicesAttribute] List<int[]> valuesArray)
        {
        }
    }
    
    public class ViewModel2
    {
        public ViewModel2([SharedStateAttribute] SharedState sharedState,
            ViewModelFactory factory,
            [FromServicesAttribute] Service service,
            [FromServicesAttribute] IServiceProvider services)
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