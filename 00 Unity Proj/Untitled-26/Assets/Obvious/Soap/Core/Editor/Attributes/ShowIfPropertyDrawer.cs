using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Attributes
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        private bool _showField = true;
        // Cache for custom drawers
        private static readonly Dictionary<Type, PropertyDrawer> _drawerCache = new Dictionary<Type, PropertyDrawer>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var showIfAttribute = (ShowIfAttribute) this.attribute;
            var conditionField = property.serializedObject.FindProperty(showIfAttribute.conditionFieldName);
            
            if (conditionField == null)
            {
                ShowError(position, label, "Field "+ showIfAttribute.conditionFieldName + " does not exist." );
                return;
            }

            _showField = ShouldShowField(conditionField, showIfAttribute);

            if (_showField)
            {
                EditorGUI.indentLevel++;
                if (TryGetCustomDrawer(out var customDrawer))
                {
                    customDrawer.OnGUI(position, property, label);
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
                EditorGUI.indentLevel--;
            }
        }
        private bool ShouldShowField(SerializedProperty conditionField, ShowIfAttribute showIfattribute)
        {
            try
            {
                switch (conditionField.propertyType)
                {
                    case SerializedPropertyType.Boolean:
                        bool comparisonBool = showIfattribute.comparisonValue == null || (bool)showIfattribute.comparisonValue;
                        return conditionField.boolValue == comparisonBool;
                    case SerializedPropertyType.Enum:
                        if (showIfattribute.comparisonValue == null)
                        {
                            Debug.LogError("Comparison value is required for enum types.");
                            return false;
                        }
                        int enumValue = conditionField.enumValueIndex;
                        int comparisonEnumValue = (int)showIfattribute.comparisonValue;
                        return enumValue == comparisonEnumValue;

                    default:
                        Debug.LogError($"Unsupported field type: {conditionField.propertyType}. Must be bool or enum.");
                        return false;
                }
            }
            catch
            {
                Debug.LogError("Invalid comparison value type.");
                return false;
            }
        }
        
        
        private void ShowError(Rect position, GUIContent label, string errorText)
        {
            EditorGUI.LabelField(position, label, new GUIContent(errorText));
            _showField = true;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return _showField ? EditorGUI.GetPropertyHeight(property, label) : 0;
        }
        
        private bool TryGetCustomDrawer(out PropertyDrawer customDrawer)
        {
            customDrawer = null;
            var propertyType = fieldInfo.FieldType;
            
            if (!_drawerCache.TryGetValue(propertyType, out var originalDrawer))
            {
                // Find the correct custom drawer type for the property
                var drawerType = GetCustomDrawerType(propertyType);
                if (drawerType != null)
                {
                    // Dynamically create an instance of the correct derived class
                    originalDrawer = (PropertyDrawer)Activator.CreateInstance(drawerType);

                    // Use reflection to set the private backing field for 'fieldInfo'
                    var fieldInfoBackingField = typeof(PropertyDrawer)
                        .GetField("m_FieldInfo", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfoBackingField != null)
                    {
                        fieldInfoBackingField.SetValue(originalDrawer, fieldInfo);
                    }

                    _drawerCache[propertyType] = originalDrawer;
                }
            }

            customDrawer = originalDrawer;
            return customDrawer != null;
        }
        
        private Type GetCustomDrawerType(Type propertyType)
        {
            var allDrawerTypes = TypeCache.GetTypesWithAttribute<CustomPropertyDrawer>();
            Type bestMatch = null;
            int bestMatchDepth = int.MaxValue;

            foreach (var drawerType in allDrawerTypes)
            {
                var attributes = drawerType.GetCustomAttributes(typeof(CustomPropertyDrawer), inherit: false);
                foreach (CustomPropertyDrawer attr in attributes)
                {
                    // Use reflection to get the target type from the constructor argument
                    var targetTypeField = typeof(CustomPropertyDrawer)
                        .GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
            
                    if (targetTypeField != null)
                    {
                        var targetType = (Type)targetTypeField.GetValue(attr);
                        if (targetType == propertyType || propertyType.IsSubclassOf(targetType))
                        {
                            // Will return the most derived class that matches the property type, so we can draw the shortcut
                            int currentDepth = GetInheritanceDepth(propertyType, targetType);
                            if (currentDepth < bestMatchDepth)
                            {
                                bestMatch = drawerType;
                                bestMatchDepth = currentDepth;
                            }
                        }
                    }
                }
            }
            return bestMatch;
        }
        
        private int GetInheritanceDepth(Type propertyType, Type targetType)
        {
            int depth = 0;
            var currentType = propertyType;
    
            while (currentType != null && currentType != targetType)
            {
                depth++;
                currentType = currentType.BaseType;
            }
    
            return depth;
        }
    }
}