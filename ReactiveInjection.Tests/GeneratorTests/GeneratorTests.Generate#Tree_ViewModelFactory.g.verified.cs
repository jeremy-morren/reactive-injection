//HintName: Tree_ViewModelFactory.g.cs
#nullable disable
namespace Tree
{
    partial class ViewModelFactory
    {
        private readonly global::Tree.Service Tree_Service; //Dependency injected service
        private readonly global::System.IServiceProvider System_IServiceProvider; //Dependency injected service
        private readonly global::System.Collections.Generic.List<int> System_Collections_Generic_List_int; //Dependency injected service

        public ViewModelFactory(global::Tree.Service Tree_Service, global::System.IServiceProvider System_IServiceProvider, global::System.Collections.Generic.List<int> System_Collections_Generic_List_int)
        {
            this.Tree_Service = Tree_Service;
            this.System_IServiceProvider = System_IServiceProvider;
            this.System_Collections_Generic_List_int = System_Collections_Generic_List_int;
        }

        private readonly global::Tree.SharedState Tree_SharedState = new global::Tree.SharedState(); //Shared state

        public partial global::Tree.ParentViewModel NewParent()
        {
            return new global::Tree.ParentViewModel(this.Tree_SharedState, this, this.Tree_Service, this.System_IServiceProvider);
        }

        public partial global::Tree.ChildViewModel NewChild(Tree.Model model)
        {
            return new global::Tree.ChildViewModel(this.Tree_SharedState, model, this.Tree_Service, this.System_Collections_Generic_List_int);
        }
    }
}
