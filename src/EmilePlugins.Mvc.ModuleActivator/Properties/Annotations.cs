using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annotations.WebPlugins.Mvc.ModuleActivator
{
    /// <summary>
    /// Indicates that the value of the marked element could be <c>null</c> sometimes,
    /// so the check for <c>null</c> is necessary before its usage
    /// </summary>
    /// <example><code>
    /// [CanBeNull] 
    /// public object Test() { return null; }
    /// 
    /// public void UseTest() 
    /// {
    ///     var p = Test();
    ///     var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
    /// }
    /// </code></example>
    [AttributeUsage(
      AttributeTargets.Method | AttributeTargets.Parameter |
      AttributeTargets.Property | AttributeTargets.Delegate |
      AttributeTargets.Field)]
    public sealed class CanBeNullAttribute : Attribute { }
    /// <summary>
    /// Indicates that the value of the marked element could never be <c>null</c>
    /// </summary>
    /// <example><code>
    /// [NotNull] 
    /// public object Foo() 
    /// {
    ///     return null; // Warning: Possible 'null' assignment
    /// }
    /// </code></example>
    [AttributeUsage(
      AttributeTargets.Method | AttributeTargets.Parameter |
      AttributeTargets.Property | AttributeTargets.Delegate |
      AttributeTargets.Field)]
    public sealed class NotNullAttribute : Attribute { }
}
