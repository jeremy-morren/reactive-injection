// ReSharper disable All

using ReactiveInjection;
using System;
using System.Collections.Generic;

namespace Tree
{
    public record Model();
    
    public class SharedState { }
    
    public class Service { }  
    
    public class ChildViewModel
    {
        public SharedState SharedState { get; }
        public Model Model { get; }
        public Service Service { get; }
        public List<int> Values { get; }

        public ChildViewModel(SharedState sharedState,
            Model model,
            [FromDI] Service service,
            [FromDI] List<int> values)
        {
            SharedState = sharedState;
            Model = model;
            Service = service;
            Values = values;
        }
    }
    
    public class ParentViewModel
    {
        public SharedState SharedState { get; }
        public ViewModelFactory Factory { get; }
        public Service Service { get; }
        public IServiceProvider Services { get; }

        public ParentViewModel(SharedState sharedState,
            ViewModelFactory factory,
            [FromDI] Service service,
            [FromDI] IServiceProvider services)
        {
            SharedState = sharedState;
            Factory = factory;
            Service = service;
            Services = services;
        }
    }
    
    public partial class ViewModelFactory
    {
        [ReactiveFactory]
        public partial ParentViewModel NewParent();
    
        [ReactiveFactory]
        public partial ChildViewModel NewChild(Model model);
    }
}