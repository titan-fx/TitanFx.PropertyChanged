//HintName: Root.IContainer0.Container1.Container2.MyModel.Implementation.g.cs
#nullable enable
namespace Root
{
    partial interface IContainer0
    {
        partial struct Container1
        {
            partial class Container2
            {
                partial class MyModel : global::System.ComponentModel.INotifyPropertyChanged, global::System.ComponentModel.INotifyPropertyChanging
                {
                    partial void LoadSender(ref object? sender);
                    public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
                    private void OnPropertyChanged(params global::System.ReadOnlySpan<global::System.String?> propertyNames)
                    {
                        global::System.Object? sender = this;
                        LoadSender(ref sender);
                        global::System.Exception? exception = null;
                        foreach (var propertyName in propertyNames)
                        {
                            try
                            {
                                PropertyChanged?.Invoke(sender, global::TitanFx.PropertyChanged.PropertyChangeEventArgs.Changed(propertyName));
                            }
                            catch(global::System.Exception ex)
                            {
                                exception = ex;
                            }
                        }
                        if (!global::System.Object.ReferenceEquals(exception, null))
                        {
                            global::System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw(exception);
                        }
                    }
                    public event global::System.ComponentModel.PropertyChangingEventHandler? PropertyChanging;
                    private void OnPropertyChanging(params global::System.ReadOnlySpan<global::System.String?> propertyNames)
                    {
                        global::System.Object? sender = this;
                        LoadSender(ref sender);
                        global::System.Exception? exception = null;
                        foreach (var propertyName in propertyNames)
                        {
                            try
                            {
                                PropertyChanging?.Invoke(sender, global::TitanFx.PropertyChanged.PropertyChangeEventArgs.Changing(propertyName));
                            }
                            catch(global::System.Exception ex)
                            {
                                exception = ex;
                            }
                        }
                        if (!global::System.Object.ReferenceEquals(exception, null))
                        {
                            global::System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw(exception);
                        }
                    }
                    private void Set<T>(ref T field, T value, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] global::System.String? propertyName = null)
                    {
                        if (!global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value))
                        {
                            try
                            {
                                OnPropertyChanging(propertyName);
                            }
                            finally
                            {
                                field = value;
                                OnPropertyChanged(propertyName);
                            }
                        }
                    }
                    private void Set<T>(bool condition, ref T field, T value, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] global::System.String? propertyName = null)
                        where T : allows ref struct
                    {
                        if (condition)
                        {
                            try
                            {
                                OnPropertyChanging(propertyName);
                            }
                            finally
                            {
                                field = value;
                                OnPropertyChanged(propertyName);
                            }
                        }
                    }
                    private void Set<T>(bool condition, T value, Action<T> assign, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] global::System.String? propertyName = null)
                        where T : allows ref struct
                    {
                        if (condition)
                        {
                            try
                            {
                                OnPropertyChanging(propertyName);
                            }
                            finally
                            {
                                assign(value);
                                OnPropertyChanged(propertyName);
                            }
                        }
                    }
                    private void Set<TValue, TState>(bool condition, in TState state, TValue value, global::TitanFx.PropertyChanged.Setter<TState, TValue> assign, [global::System.Runtime.CompilerServices.CallerMemberNameAttribute] global::System.String? propertyName = null)
                        where TValue : allows ref struct
                        where TState : allows ref struct
                    {
                        if (condition)
                        {
                            try
                            {
                                OnPropertyChanging(propertyName);
                            }
                            finally
                            {
                                assign(in state, value);
                                OnPropertyChanged(propertyName);
                            }
                        }
                    }
                    private void Set<T>(ref T field, T value, params global::System.ReadOnlySpan<global::System.String> propertyNames)
                    {
                        if (!global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value))
                        {
                            try
                            {
                                OnPropertyChanging(propertyNames);
                            }
                            finally
                            {
                                field = value;
                                OnPropertyChanged(propertyNames);
                            }
                        }
                    }
                    private void Set<T>(bool condition, ref T field, T value, params global::System.ReadOnlySpan<global::System.String> propertyNames)
                        where T : allows ref struct
                    {
                        if (condition)
                        {
                            try
                            {
                                OnPropertyChanging(propertyNames);
                            }
                            finally
                            {
                                field = value;
                                OnPropertyChanged(propertyNames);
                            }
                        }
                    }
                    private void Set<T>(bool condition, T value, Action<T> assign, params global::System.ReadOnlySpan<global::System.String> propertyNames)
                        where T : allows ref struct
                    {
                        if (condition)
                        {
                            try
                            {
                                OnPropertyChanging(propertyNames);
                            }
                            finally
                            {
                                assign(value);
                                OnPropertyChanged(propertyNames);
                            }
                        }
                    }
                    private void Set<TValue, TState>(bool condition, in TState state, TValue value, global::TitanFx.PropertyChanged.Setter<TState, TValue> assign, params global::System.ReadOnlySpan<global::System.String> propertyNames)
                        where TValue : allows ref struct
                        where TState : allows ref struct
                    {
                        if (condition)
                        {
                            try
                            {
                                OnPropertyChanging(propertyNames);
                            }
                            finally
                            {
                                assign(in state, value);
                                OnPropertyChanged(propertyNames);
                            }
                        }
                    }
                }
            }
        }
    }
}
