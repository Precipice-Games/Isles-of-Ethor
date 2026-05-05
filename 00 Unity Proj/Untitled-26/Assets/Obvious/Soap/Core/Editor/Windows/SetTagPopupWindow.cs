using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public class SetTagPopupWindow : PopupWindowContent
    {
        private int _selectedTagIndex = 0;
        private Vector2? _mousePosition;
        private readonly Vector2 _dimensions = new Vector2(250, 120);
        private readonly SoapSettings _soapSettings;
        private readonly ScriptableBase[] _scriptableBases;
        private Texture[] _icons;

        public override Vector2 GetWindowSize() => _dimensions;

        public SetTagPopupWindow(ScriptableBase[] scriptableBases)
        {
            _soapSettings = SoapEditorUtils.GetOrCreateSoapSettings();
            _scriptableBases = scriptableBases;
            if (_scriptableBases.Length == 1)
            {
                var scriptableBase = _scriptableBases[0];
                _selectedTagIndex = _soapSettings.GetTagIndex(scriptableBase);
            }
        }

        public override void OnGUI(Rect rect)
        {
            //Do this only once to cache the mouse position
            if (!_mousePosition.HasValue)
            {
                if (Event.current.type == EventType.Layout)
                {
                    var pos = Event.current.mousePosition;
                    _mousePosition = GUIUtility.GUIToScreenPoint(pos);
                    editorWindow.position = new Rect(_mousePosition.Value, _dimensions);
                }
            }
            SoapInspectorUtils.DrawPopUpHeader(editorWindow, "Set Tag");
            GUILayout.BeginVertical(SoapInspectorUtils.Styles.PopupContent);
            GUILayout.FlexibleSpace();
            var tags = _soapSettings.Tags.ToArray();
            _selectedTagIndex = EditorGUILayout.Popup(_selectedTagIndex, tags);
            GUILayout.FlexibleSpace();
            if (SoapInspectorUtils.DrawCallToActionButton("Apply", SoapInspectorUtils.ButtonSize.Medium))
            {
                foreach (var scriptableBase in _scriptableBases)
                    SoapEditorUtils.AssignTag(scriptableBase, _selectedTagIndex);
                
                editorWindow.Close();
            }
            GUILayout.EndVertical();
        }
    }
}