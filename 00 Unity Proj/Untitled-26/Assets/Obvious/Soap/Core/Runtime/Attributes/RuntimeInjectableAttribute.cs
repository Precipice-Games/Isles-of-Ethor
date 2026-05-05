using System;
using UnityEngine;

namespace Obvious.Soap.Attributes
{
    /// <summary>
    /// Marks fields to be injected with a runtime-created variable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class RuntimeInjectableAttribute : PropertyAttribute
    {
        public string Id { get; private set; }

        public RuntimeInjectableAttribute(string id)
        {
            Id = id;
        }
    }
}