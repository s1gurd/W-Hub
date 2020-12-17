using System;

namespace SimpleContainer.Attributes
{
    [Obsolete("Use Container.RegisterAttribute<T>() instead")]
    public class InjectAttribute : Attribute { }
}
