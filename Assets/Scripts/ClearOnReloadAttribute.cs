using System;
using System.Reflection;

// Originally adapted from com.jsteinhauer.unitydomainreloadhelper by Josh Steinhauer
//  - Added generic static field support

// Clears a static field when the domain is reloaded. If the class implements IDisposable,
// it will be disposed before the assembly reloads. If the class implements IClearOnLoad,
// its methods can override this behaviour.
[AttributeUsage(AttributeTargets.Field)]
public class ClearOnReloadAttribute : Attribute
{
    /// <summary>
    /// If true, field will also be cleared when exiting play mode.
    /// </summary>
    public bool clearOnExitingPlayMode;

    /// <summary>
    /// If true, field will also be cleared when exiting edit mode.
    /// </summary>
    public bool clearOnExitingEditMode;
}

// Optional interface that can be implemented by singletons, which can override
// the default clearing behaviour.
public interface IClearOnLoad
{
    // Return false to prevent dispose and field clearing after a domain reload.
    bool ClearOnDomainReload(FieldInfo field);

    // Return false to prevent dispose and field clearing after exiting play mode.
    bool ClearOnExitPlayMode(FieldInfo field);

    // Return false to prevent dispose and field clearing after exiting edit mode.
    bool ClearOnExitEditMode(FieldInfo field);
}

// [AttributeUsage(AttributeTargets.Field)]
// public class ClearOnReloadAttribute : Attribute
// {
//     public readonly object valueToAssign;
//     public readonly bool assignNewTypeInstance;

//     /// <summary>
//     ///     Marks field, property or event to be cleared on reload.
//     /// </summary>
//     public ClearOnReloadAttribute()
//     {
//         this.valueToAssign = null;
//         this.assignNewTypeInstance = false;
//     }
    
//     /// <summary>
//     ///     Marks field of property to be cleared and assigned given value on reload.
//     /// </summary>
//     /// <param name="valueToAssign">Explicit value which will be assigned to field/property on reload. Has to match field/property type. Has no effect on events.</param>
//     public ClearOnReloadAttribute(object valueToAssign)
//     {
//         this.valueToAssign = valueToAssign;
//         this.assignNewTypeInstance = false;
//     }

//     /// <summary>
//     ///     Marks field of property to be cleared or re-initialized on reload.
//     /// </summary>
//     /// <param name="assignNewTypeInstance">If true, field/property will be assigned a newly created object of its type on reload. Has no effect on events.</param>
//     public ClearOnReloadAttribute(bool assignNewTypeInstance = false)
//     {
//         this.valueToAssign = null;
//         this.assignNewTypeInstance = assignNewTypeInstance;
//     }
// }

// [AttributeUsage(AttributeTargets.Method)]
// public class ExecuteOnReloadAttribute : Attribute
// {
//     /// <summary>
//     ///     Marks method to be executed on reload.
//     /// </summary>
//     public ExecuteOnReloadAttribute() {}
// }