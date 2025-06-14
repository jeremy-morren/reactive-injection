﻿//HintName: Tree.Models.ViewModelFactory.g.cs
// <auto-generated/>
// This file was automatically generated by the ReactiveInjection source generator.
// Do not edit this file manually since it will be automatically overwritten.
// ReSharper disable All
#nullable enable

using Microsoft.Extensions.Logging;

#nullable disable warnings
namespace Tree.Models
{
    [global::System.Diagnostics.DebuggerStepThroughAttribute()]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
    partial class ViewModelFactory
    {
        private readonly ILogger _logger;

        private readonly global::Tree.Models.Service _service0;
        private readonly global::System.Collections.Generic.List<int> _service1;
        private readonly global::System.Collections.Generic.List<int[]> _service2;
        private readonly global::Tree.Models.ViewModel3 _service3;
        private readonly global::System.IServiceProvider _service4;

        private readonly global::Tree.Models.SharedState _state0 = new global::Tree.Models.SharedState();
        private readonly global::System.Collections.ObjectModel.ObservableCollection<global::Tree.Models.SharedState> _state1 = new global::System.Collections.ObjectModel.ObservableCollection<global::Tree.Models.SharedState>();

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public ViewModelFactory(global::Tree.Models.Service service0, global::System.Collections.Generic.List<int> service1, global::System.Collections.Generic.List<int[]> service2, global::Tree.Models.ViewModel3 service3, global::System.IServiceProvider service4, ILoggerFactory? loggerFactory = null)
        {
            this._logger = (loggerFactory ?? global::Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance).CreateLogger(this.GetType());

            this._service0 = service0;
            this._service1 = service1;
            this._service2 = service2;
            this._service3 = service3;
            this._service4 = service4;
        }

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public global::Tree.Models.ViewModel1 ViewModel1(global::Tree.Models.Model model, global::Tree.Models.ViewModel1 owner)
        {
            try
            {
                return new global::Tree.Models.ViewModel1(this._state0, model, owner, this._service0, this._service1, this._service2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error constructing {ViewModel}", args: new object[]{typeof(global::Tree.Models.ViewModel1)});
                throw;
            }
        }

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public async global::System.Threading.Tasks.Task<global::Tree.Models.ViewModel1> LoadViewModel1Async(string id, CancellationToken ct)
        {
            try
            {
                return await global::Tree.Models.ViewModel1.Load(id, this._service3, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling loader {LoaderMethod} for {ViewModel}", args: new object[]{"Load", typeof(global::Tree.Models.ViewModel1)});
                throw;
            }
        }

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public global::Tree.Models.ViewModel2 ViewModel2()
        {
            try
            {
                return new global::Tree.Models.ViewModel2(this._state0, this, this._service0, this._service4);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error constructing {ViewModel}", args: new object[]{typeof(global::Tree.Models.ViewModel2)});
                throw;
            }
        }

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public global::Tree.Models.ViewModel3 ViewModel3(string str, string str2)
        {
            try
            {
                return new global::Tree.Models.ViewModel3(str, this._state1, str2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error constructing {ViewModel}", args: new object[]{typeof(global::Tree.Models.ViewModel3)});
                throw;
            }
        }

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public global::Tree.Models.ViewModel4 ViewModel4()
        {
            try
            {
                return new global::Tree.Models.ViewModel4(this._state0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error constructing {ViewModel}", args: new object[]{typeof(global::Tree.Models.ViewModel4)});
                throw;
            }
        }

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public global::Tree.Models.ViewModel5 ViewModel5(global::System.Collections.ObjectModel.ReadOnlyObservableCollection<global::Tree.Models.SharedState> parameter)
        {
            try
            {
                return new global::Tree.Models.ViewModel5(parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error constructing {ViewModel}", args: new object[]{typeof(global::Tree.Models.ViewModel5)});
                throw;
            }
        }

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public async global::System.Threading.Tasks.Task<global::Tree.Models.ViewModel5> LoadViewModel5Async(uint? param, CancellationToken ct)
        {
            try
            {
                return await global::Tree.Models.ViewModel5.LoadByFactory(param);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling loader {LoaderMethod} for {ViewModel}", args: new object[]{"LoadByFactory", typeof(global::Tree.Models.ViewModel5)});
                throw;
            }
        }

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public async global::System.Threading.Tasks.Task<global::Tree.Models.ViewModel5> LoadViewModel5Async(string? param, CancellationToken ct)
        {
            try
            {
                return await global::Tree.Models.ViewModel5.LoadByFactory(param);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling loader {LoaderMethod} for {ViewModel}", args: new object[]{"LoadByFactory", typeof(global::Tree.Models.ViewModel5)});
                throw;
            }
        }

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public async global::System.Threading.Tasks.Task<global::Tree.Models.ViewModel5> LoadViewModel5Async(CancellationToken ct)
        {
            try
            {
                return await global::Tree.Models.ViewModel5.Load();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling loader {LoaderMethod} for {ViewModel}", args: new object[]{"Load", typeof(global::Tree.Models.ViewModel5)});
                throw;
            }
        }

        [global::System.Diagnostics.DebuggerStepThroughAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ReactiveInjection.SourceGenerator", "1.0.0.0")]
        public global::Tree.Models.ViewModel6 ViewModel6()
        {
            try
            {
                return new global::Tree.Models.ViewModel6();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error constructing {ViewModel}", args: new object[]{typeof(global::Tree.Models.ViewModel6)});
                throw;
            }
        }
    }
}
