﻿//HintName: treeModelsViewModelFactory.g.cs
//This file was automatically generated by the ReactiveInjection source generator.
//Do not edit this file manually since it will be automatically overwritten.
#nullable disable
namespace Tree.Models
{
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection", "1.0.0.0")]
    partial class ViewModelFactory
    {
        private readonly global::Tree.Models.Service treeModelsService; //Dependency injected service
        private readonly global::System.IServiceProvider systemIServiceProvider; //Dependency injected service
        private readonly global::System.Collections.Generic.List<int> systemCollectionsGenericList_int; //Dependency injected service
        private readonly global::System.Collections.Generic.List<int[]> systemCollectionsGenericList_intArray; //Dependency injected service

        public ViewModelFactory(global::Tree.Models.Service treeModelsService, global::System.IServiceProvider systemIServiceProvider, global::System.Collections.Generic.List<int> systemCollectionsGenericList_int, global::System.Collections.Generic.List<int[]> systemCollectionsGenericList_intArray)
        {
            this.treeModelsService = treeModelsService;
            this.systemIServiceProvider = systemIServiceProvider;
            this.systemCollectionsGenericList_int = systemCollectionsGenericList_int;
            this.systemCollectionsGenericList_intArray = systemCollectionsGenericList_intArray;
        }

        private readonly global::Tree.Models.SharedState treeModelsSharedState = new global::Tree.Models.SharedState(); //Shared state

        public partial global::Tree.Models.ParentViewModel NewParent()
        {
            return new global::Tree.Models.ParentViewModel(this.treeModelsSharedState, this, this.treeModelsService, this.systemIServiceProvider);
        }

        public partial global::Tree.Models.ChildViewModel NewChild(Tree.Models.Model model)
        {
            return new global::Tree.Models.ChildViewModel(this.treeModelsSharedState, model, this.treeModelsService, this.systemCollectionsGenericList_int, this.systemCollectionsGenericList_intArray);
        }

        public partial global::Tree.Models.SimpleViewModel NewSimple(string str)
        {
            return new global::Tree.Models.SimpleViewModel(str);
        }
    }
}
