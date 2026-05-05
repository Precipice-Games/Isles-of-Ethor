using UnityEngine;
using UnityEditor;
using Obvious.Soap.Attributes;
using Object = UnityEngine.Object;

namespace Obvious.Soap.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableBase), true)]
    public abstract class ScriptableBasePropertyDrawer : PropertyDrawer
    {
        protected UnityEditor.Editor _editor;
        private const float WidthRatioWhenNull = 0.82f;
        protected virtual float WidthRatio => 0.82f;
        protected bool? _canBeSubAsset;
        private bool CanBeSubAsset => _canBeSubAsset != null && _canBeSubAsset.Value;
        protected bool? IsReadOnly { get; private set; }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (!_canBeSubAsset.HasValue)
            {
                _canBeSubAsset = SoapEditorUtils.CanBeSubAsset(property, fieldInfo);
            }

            var targetObject = property.objectReferenceValue;
            if (targetObject == null)
            {
                DrawIfNull(position, property, label);
                return;
            }

            if (!IsReadOnly.HasValue)
            {
                IsReadOnly = fieldInfo?.DeclaringType != null &&
                             fieldInfo.DeclaringType.IsSubclassOf(typeof(ReadOnlyBase));
            }

            DrawIfNotNull(position, property, label, targetObject);

            EditorGUI.EndProperty();
        }

        protected void DrawIfNull(Rect position, SerializedProperty property, GUIContent label)
        {
            // Check if the type is generic (as new Unity version serialized Generic types)
            System.Type propertyType = GetPropertyType(property);
            var isCollection = SoapUtils.IsCollection(propertyType);
            if (propertyType?.IsGenericType == true && !isCollection)
            {
                //Display error message
                System.Type genericArgument = propertyType.GetGenericArguments()[0];
                var intrinsicType = SoapUtils.GetIntrinsicType(genericArgument.Name);
                var scriptableName = propertyType.Name.Split('`')[0];
                string fullGenericTypeName = $"{scriptableName}<{intrinsicType}>";

                var suggestedTypeName = scriptableName.Contains("Variable")
                    ? $"{intrinsicType.CapitalizeFirstLetter()}Variable"
                    : $"{scriptableName}{intrinsicType.CapitalizeFirstLetter()}";

                var message = $"Do not use {fullGenericTypeName} directly. " +
                              $"\nPlease use {suggestedTypeName} that inherit from it instead.";

                EditorGUILayout.HelpBox(message, MessageType.Error);
            }
            else
            {
                // Original logic for non-generic types
                var widthRatio = fieldInfo == null ? 1f : WidthRatioWhenNull;
                var rect = DrawCustomPropertyField(position, property, label, widthRatio);

                if (fieldInfo != null)
                {
                    var guiContent = new GUIContent("Create");
                    if (GUI.Button(rect, guiContent))
                    {
                        var soapSettings = SoapEditorUtils.GetOrCreateSoapSettings();
                        var prefix = soapSettings.GetPrefix(fieldInfo.FieldType);
                        var assetName = $"{prefix}{GetFieldName()}";
                        if (CanBeSubAsset)
                            SoapEditorUtils.CreateSubAsset(property, fieldInfo, assetName);
                        else
                        {
                            if (soapSettings.NamingOnCreationMode == ENamingCreationMode.Manual)
                            {
                                var popUpRect = new Rect(EditorWindow.focusedWindow.position);
                                PopupWindow.Show(new Rect(), new SoapAssetCreatorPopup(popUpRect,
                                    SoapAssetCreatorPopup.EOrigin.Inspector,
                                    fieldInfo, assetName, scriptableBase =>
                                    {
                                        property.objectReferenceValue = scriptableBase;
                                        property.serializedObject.ApplyModifiedProperties();
                                    }));
                            }
                            else
                            {
                                SoapEditorUtils.CreateSoapSoAtPath(property, fieldInfo, assetName);
                            }
                        }
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        // Helper method to get the type of the property
        private System.Type GetPropertyType(SerializedProperty property)
        {
            if (fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                return property.objectReferenceValue?.GetType() ?? typeof(UnityEngine.Object);
            }

            return null;
        }


        protected void DrawIfNotNull(Rect position, SerializedProperty property, GUIContent label,
            Object targetObject)
        {
            var rect = position;
            var labelRect = position;

            var offset = CanBeSubAsset ? EditorGUIUtility.singleLineHeight : 0f;
            labelRect.width = position.width * .4f - offset;

            property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, new GUIContent(""), true);
            if (property.isExpanded)
            {
                //To Handle Odin [HideLabel] attribute 
                if (CanBeSubAsset && !string.IsNullOrEmpty(label.text))
                {
                    label.image = SoapInspectorUtils.Icons.SubAsset;
                }

                if (IsReadOnly.HasValue && IsReadOnly.Value)
                {
                    label.image = SoapInspectorUtils.Icons.ReadOnly;
                }

                EditorGUI.PropertyField(rect, property, label);
                EditorGUI.indentLevel++;
                var cacheBgColor = GUI.backgroundColor;
                GUI.backgroundColor = SoapEditorUtils.SoapColor;
                GUILayout.BeginVertical(GUI.skin.box);
                DrawEmbeddedEditor(targetObject);
                GUI.backgroundColor = cacheBgColor;
                GUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
            else
            {
                DrawUnExpanded(position, property, label, targetObject);
            }
        }

        protected virtual void DrawEmbeddedEditor(Object targetObject)
        {
            if (_editor == null)
            {
                UnityEditor.Editor.CreateCachedEditor(targetObject, null, ref _editor);
            }

            _editor.OnInspectorGUI();
        }

        protected virtual string GetFieldName()
        {
            return fieldInfo.Name;
        }

        protected virtual void DrawUnExpanded(Rect position, SerializedProperty property, GUIContent label,
            Object targetObject)
        {
            var rect = DrawCustomPropertyField(position, property, label, WidthRatio);
            DrawShortcut(rect, property, targetObject);
        }

        protected virtual void DrawShortcut(Rect rect, SerializedProperty property, Object targetObject)
        {
        }

        private Rect DrawCustomPropertyField(Rect position, SerializedProperty property, GUIContent label,
            float widthRatio)
        {
            if (CanBeSubAsset && !string.IsNullOrEmpty(label.text))
            {
                label.text = SoapEditorUtils.CleanSubAssetName(label.text);
                label.image = SoapInspectorUtils.Icons.SubAsset;
            }

            if (IsReadOnly.HasValue && IsReadOnly.Value)
            {
                label.image = SoapInspectorUtils.Icons.ReadOnly;
            }

            var propertyRect = new Rect(position);
            propertyRect.width = position.width * widthRatio;
            //this sets the property rect position to the right of the label
            propertyRect = EditorGUI.PrefixLabel(propertyRect, label);

            if (CanBeSubAsset && property.objectReferenceValue != null)
            {
                //draw a small X button to delete the asset
                propertyRect.x -= EditorGUIUtility.singleLineHeight + 2f;
                var buttonRect = new Rect(propertyRect);
                buttonRect.width = EditorGUIUtility.singleLineHeight;
                var content = new GUIContent(GUIContent.none);
                content.image = SoapInspectorUtils.Icons.Cancel;
                if (GUI.Button(buttonRect, content))
                {
                    SoapEditorUtils.DeleteSubAsset(property.objectReferenceValue);
                    return position;
                }

                propertyRect.x += buttonRect.width + 2f;
            }

            var originalGuiEnabled = GUI.enabled;
            GUI.enabled = originalGuiEnabled && !CanBeSubAsset;
            EditorGUI.PropertyField(propertyRect, property, GUIContent.none);
            GUI.enabled = originalGuiEnabled;
            var rectPosition = new Rect(propertyRect);
            rectPosition.xMin = propertyRect.xMax + 5f;
            rectPosition.width = position.width * (1 - widthRatio) - 5f;
            return rectPosition;
        }

        private int GetTagIndexFromAttribute(SoapSettings soapSettings)
        {
            var tagAttribute = (AutoTag)System.Attribute.GetCustomAttribute(fieldInfo,
                typeof(AutoTag));

            if (tagAttribute == null)
                return 0;

            if (string.IsNullOrEmpty(tagAttribute.Tag))
            {
                return soapSettings.GetTagIndex(tagAttribute.TagIndex);
            }

            return soapSettings.GetTagIndex(tagAttribute.Tag);
        }
    }
}