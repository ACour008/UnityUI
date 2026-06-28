using System;
using System.Diagnostics;
using System.Reflection;

public abstract class Singleton<T> : IClearOnLoad
    where T : new()
{
    [ClearOnReload(clearOnExitingPlayMode = true, clearOnExitingEditMode = true)]
    static T _instance;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static T instance => _instance ?? (_instance = new T());

    public static T instanceNoAlloc => _instance;

    public virtual bool ClearOnDomainReload(FieldInfo field) => true;
    public virtual bool ClearOnExitPlayMode(FieldInfo field) => true;
    public virtual bool ClearOnExitEditMode(FieldInfo field) => true;

    public static void ResetSingleton()
    {
        if (_instance is IDisposable disposable)
            disposable?.Dispose();
        _instance = default;
    }
    
}