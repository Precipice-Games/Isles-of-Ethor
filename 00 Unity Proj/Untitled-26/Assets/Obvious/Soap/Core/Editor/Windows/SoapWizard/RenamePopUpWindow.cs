using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public class RenamePopUpWindow : PopupWindowContent
    {
     
        private string _newName = "";
        private string _initialName = "";
        private readonly Rect _position;
        private readonly Vector2 _dimensions = new Vector2(250, 120);
        private readonly ScriptableBase _scriptableBase = null;

        public override Vector2 GetWindowSize() => _dimensions;

        public RenamePopUpWindow(Rect origin, ScriptableBase scriptableBase)
        {
            _position = origin;
            _scriptableBase = scriptableBase;
            _initialName = _scriptableBase.name;
            _newName = _initialName;
            SoapWizardWindow.IsPopupOpen = true;
        }
        
        public override void OnClose()
        {
            SoapWizardWindow.IsPopupOpen = false;
        }

        public override void OnGUI(Rect rect)
        {
            editorWindow.position = SoapInspectorUtils.CenterInWindow(editorWindow.position, _position);
            SoapInspectorUtils.DrawPopUpHeader(editorWindow, "Rename");
            GUILayout.BeginVertical(SoapInspectorUtils.Styles.PopupContent);
            
            GUILayout.FlexibleSpace();
            _newName = EditorGUILayout.TextField(_newName, EditorStyles.textField);
            GUILayout.FlexibleSpace();
            GUI.enabled = _newName != _initialName;
            if (SoapInspectorUtils.DrawCallToActionButton("Apply", SoapInspectorUtils.ButtonSize.Medium))
            {
                SoapEditorUtils.RenameAsset(_scriptableBase, _newName);
                editorWindow.Close();
            }
            GUI.enabled = true;
            GUILayout.EndVertical();
        }
    }
}