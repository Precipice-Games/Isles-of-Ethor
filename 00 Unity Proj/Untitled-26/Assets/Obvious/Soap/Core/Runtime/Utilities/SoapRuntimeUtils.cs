using System;
using System.Collections.Generic;
using System.Reflection;
using Obvious.Soap.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Obvious.Soap
{
    public static class SoapRuntimeUtils
    {
        /// <summary>
        /// Creates a new instance of a ScriptableVariableBase subclass with the given name.
        /// </summary>
        /// <typeparam name="T">The type of the ScriptableVariableBase subclass to create.</typeparam>
        /// <param name="name">The name of the new ScriptableVariableBase instance.</param>
        /// <returns>The newly created ScriptableVariableBase instance.</returns>
        public static T CreateRuntimeInstance<T>(string name = "") where T : ScriptableVariableBase
        {
            var runtimeVariable = CreateInstance<T>(name);
            runtimeVariable.ResetTiming = ResetTiming.Never;
            return runtimeVariable;
        }
        
        /// <summary>
        /// Creates a new instance of a ScriptableObject subclass with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>(string name = "") where T : ScriptableObject
        {
            var instance = ScriptableObject.CreateInstance<T>();
            instance.name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
            return instance;
        }

        /// <summary>
        /// Creates a new instance of a ScriptableEvent subclass with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateRuntimeEventInstance<T>(string name = "") where T : ScriptableEventBase
        {
            return CreateInstance<T>(name);
        }

        /// <summary>
        /// Create a deep copy of the template
        /// </summary>
        /// <typeparam name="T">The type of the subclass to create.</typeparam>
        /// <param name="template">Template to copy.</param>
        /// <returns>The newly created or copied instance.</returns>
        public static T CreateCopy<T>(T template) where T : ScriptableObject
        {
            // Create a deep copy of the template
            T copy = Object.Instantiate(template);
            return copy;
        }

        private static readonly Dictionary<(Type, string), List<FieldInfo>> _cachedInjectableFields =
            new Dictionary<(Type, string), List<FieldInfo>>();

        public static void InjectInChildren<TVariable>(GameObject gameObject,
            TVariable runtimeVariable, string id, bool debugLogEnabled)
            where TVariable : ScriptableObject
        {
            var components = gameObject.GetComponentsInChildren<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component == null)
                    continue;

                var type = component.GetType();
                var cacheKey = (type, variableName: id);
                if (!_cachedInjectableFields.TryGetValue(cacheKey, out var fields))
                {
                    fields = new List<FieldInfo>();
                    foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                                         BindingFlags.Public))
                    {
                        if (Attribute.IsDefined(field, typeof(RuntimeInjectableAttribute)) &&
                            field.FieldType == typeof(TVariable))
                        {
                            // Check if the field has the correct variable name
                            var attribute = field.GetCustomAttribute<RuntimeInjectableAttribute>();
                            if (attribute.Id != id || string.IsNullOrEmpty(attribute.Id))
                                continue;

                            fields.Add(field);
                        }
                    }

                    _cachedInjectableFields[cacheKey] = fields;
                }

                foreach (var field in fields)
                {
                    field.SetValue(component, runtimeVariable);
                    if (debugLogEnabled)
                    {
                        Debug.Log(
                            $"<color=#f75369>[Injected]</color> {runtimeVariable.name} into {component.GetType().Name}.{field.Name}");
                    }
                }
            }
        }
    }
}