using Obvious.Soap.Editor;
using UnityEditor;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
#endif

namespace Obvious.Soap.Attributes
{
    #if ODIN_INSPECTOR
    public class RuntimeInjectableDrawer : OdinAttributeDrawer<RuntimeInjectableAttribute>
    {
        private GUIContent _iconContent;
        const float IconSize = 16f;
        private bool _hasId;
        private GUIStyle _guiStyle;

        protected override void Initialize()
        {
            var icon = SoapInspectorUtils.Icons.RuntimeInjectable; 
            _iconContent = new GUIContent(icon);
            _guiStyle = new GUIStyle();
            _guiStyle.padding = new RectOffset(0, 0, 2, 2);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var runtimeInjectableAttribute = this.Attribute;
            _hasId = !string.IsNullOrEmpty(runtimeInjectableAttribute.Id);
            GUILayout.BeginHorizontal(_guiStyle, GUILayout.ExpandWidth(false), 
                GUILayout.Height(EditorGUIUtility.singleLineHeight));
            
            if (!_hasId)
            {
                SirenixEditorGUI.ErrorMessageBox("Variable id required for injection.");
            }
            else if (!Application.isPlaying)
            {
                var iconRect = GUILayoutUtility.GetRect(IconSize, IconSize,
                    GUILayout.ExpandWidth(false));
                //iconRect.x -= IconSize;
                iconRect.y += (EditorGUIUtility.singleLineHeight - IconSize) ; 
                GUI.DrawTexture(iconRect, _iconContent.image, ScaleMode.ScaleToFit);
            }
            // Draw the property with disabled interaction
            GUIHelper.PushGUIEnabled(false);
            this.CallNextDrawer(label);
            GUIHelper.PopGUIEnabled();
            GUILayout.EndHorizontal(); 
        }
    }
    #else
    [CustomPropertyDrawer(typeof(RuntimeInjectableAttribute))]
    public class RuntimeInjectableDrawer : DecoratorDrawer
    {
        private readonly GUIContent _iconContent;
        const float IconSize = 16f;
        private bool _hasId;

        public RuntimeInjectableDrawer()
        {
            var icon = SoapInspectorUtils.Icons.RuntimeInjectable;
            _iconContent = new GUIContent(icon);
        }
        
        public override void OnGUI(Rect position)
        {
            var runtimeInjectableAttribute = (RuntimeInjectableAttribute) this.attribute;
            _hasId = !string.IsNullOrEmpty(runtimeInjectableAttribute.Id);
            if (_hasId == false)
            {
                EditorGUI.HelpBox(position, "Variable id required for injection.", MessageType.Error);
            }
            else if (!Application.isPlaying)
            {
                var xPos = position.x - IconSize;
                Rect iconRect = new Rect(xPos, position.y, IconSize, IconSize);
                GUI.DrawTexture(iconRect, _iconContent.image, ScaleMode.ScaleToFit);
            }
            EditorGUI.BeginDisabledGroup(true);
        }

        public override float GetHeight()
        {
            return _hasId ? 0 : EditorGUIUtility.singleLineHeight;
        }
    }
#endif
    
    
}