using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public static class SoapMenuUtils
    {
        [MenuItem("Tools/Obvious/\ud83e\uddfc Soap/Delete Player Pref %#d", priority = 0)]
        public static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log($"<color={SoapEditorUtils.SoapColorHtml}>--Player Prefs deleted--</color>");
        }

        [MenuItem("Tools/Obvious/\ud83e\uddfc Soap/ToggleFastPlayMode %l", priority = 1)]
        public static void ToggleFastPlayMode()
        {
            EditorSettings.enterPlayModeOptionsEnabled = !EditorSettings.enterPlayModeOptionsEnabled;
            AssetDatabase.Refresh();
            var text = EditorSettings.enterPlayModeOptionsEnabled
                ? "<color=#55efc4>Enabled"
                : $"<color={SoapEditorUtils.SoapColorHtml}>Disabled";
            text += "</color>";
            Debug.Log("Fast Play Mode " + text);
        }

        [MenuItem("CONTEXT/ScriptableVariableBase/Reset Value", false, 2)]
        private static void ResetValue(MenuCommand command) => ResetValue(command.context);

        [MenuItem("CONTEXT/ScriptableCollection/Clear", false, 2)]
        private static void Clear(MenuCommand command) => ResetValue(command.context);

        private static void ResetValue(Object unityObject)
        {
            var reset = unityObject as IReset;
            reset.ResetValue();
        }

        [MenuItem("CONTEXT/ScriptableBase/Reset", false, 1)]
        private static void Reset(MenuCommand command) => Reset(command.context);

        private static void Reset(Object unityObject)
        {
            var scriptableBase = unityObject as ScriptableBase;
            scriptableBase.Reset();
            EditorUtility.SetDirty(unityObject);
        }

        [MenuItem("CONTEXT/ScriptableObject/Delete All SubAssets", false, 0)]
        private static void DeleteAllSubAssets(MenuCommand command) => DeleteAllSubAssets(command.context);

        [MenuItem("Assets/Soap/Delete All SubAssets")]
        private static void DeleteAllSubAssets() => DeleteAllSubAssets(Selection.activeObject);

        [MenuItem("CONTEXT/ScriptableObject/Delete All SubAssets", true)]
        private static bool ValidateDeleteAllSubAssets(MenuCommand command) => CanDeleteAllSubAssets(command.context);

        [MenuItem("Assets/Soap/Delete All SubAssets", true)]
        private static bool ValidateDeleteAllSubAssets() => CanDeleteAllSubAssets(Selection.activeObject);

        private static bool CanDeleteAllSubAssets(Object obj)
        {
            var isScriptable = obj is ScriptableObject;
            if (!isScriptable || AssetDatabase.IsSubAsset(obj))
                return false;
            var subAssets = SoapEditorUtils.GetAllSubAssets(Selection.activeObject);
            return subAssets.Count > 0;
        }

        private static void DeleteAllSubAssets(Object unityObject)
        {
            var subAssets = SoapEditorUtils.GetAllSubAssets(unityObject);
            foreach (var subAsset in subAssets)
                Object.DestroyImmediate(subAsset, true);

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(unityObject), ImportAssetOptions.ForceUpdate);
        }

        [MenuItem("CONTEXT/ScriptableObject/Delete SubAsset", false, 0)]
        private static void DeleteSubAsset(MenuCommand command) => DeleteSubAsset(command.context);

        [MenuItem("Assets/Soap/Delete SubAsset")]
        private static void DeleteSubAsset() => DeleteSubAsset(Selection.activeObject);

        [MenuItem("CONTEXT/ScriptableObject/Delete SubAsset", true)]
        private static bool ValidateDeleteSubAsset(MenuCommand command) => CanDeleteSubAsset(command.context);

        [MenuItem("Assets/Soap/Delete SubAsset", true)]
        private static bool ValidateDeleteSubAsset() => CanDeleteSubAsset(Selection.activeObject);

        private static bool CanDeleteSubAsset(Object obj)
        {
            var isScriptable = obj is ScriptableObject;
            if (!isScriptable)
                return false;
            return AssetDatabase.IsSubAsset(obj);
        }

        private static void DeleteSubAsset(Object unityObject)
        {
            SoapEditorUtils.DeleteSubAsset(unityObject);
        }

        [MenuItem("CONTEXT/ScriptableBase/\ud83d\udd0dFind References/In Scene and Project", false, 0)]
        private static void FindReferencesAll(MenuCommand command) =>
            FindReferenceFor(command.context, FindReferenceType.All);

        [MenuItem("CONTEXT/ScriptableBase/\ud83d\udd0dFind References/In Scene", false, 0)]
        private static void FindReferencesInScene(MenuCommand command) =>
            FindReferenceFor(command.context, FindReferenceType.Scene);

        [MenuItem("CONTEXT/ScriptableBase/\ud83d\udd0dFind References/In Project", false, 0)]
        private static void FindReferencesInProject(MenuCommand command) =>
            FindReferenceFor(command.context, FindReferenceType.Project);

        [MenuItem("Assets/Soap/\ud83d\udd0d Find References/In Scene and Project")]
        private static void FindReferencesAll() => FindReferenceFor(Selection.activeObject, FindReferenceType.All);

        [MenuItem("Assets/Soap/\ud83d\udd0d Find References/In Scene")]
        private static void FindReferencesInScene() =>
            FindReferenceFor(Selection.activeObject, FindReferenceType.Scene);

        [MenuItem("Assets/Soap/\ud83d\udd0d Find References/In Project")]
        private static void FindReferencesInProject() =>
            FindReferenceFor(Selection.activeObject, FindReferenceType.Project);


        [MenuItem("CONTEXT/ScriptableBase/Find References/In Scene and Project", true)]
        private static bool ValidateFindReferenceAll(MenuCommand command) => CanFindReferenceFor(command.context);

        [MenuItem("Assets/Soap/\ud83d\udd0d Find References/In Scene and Project", true)]
        private static bool ValidateFindReferenceAll() => CanFindReferenceFor(Selection.activeObject);

        [MenuItem("CONTEXT/ScriptableBase/Find References/In Scene", true)]
        private static bool ValidateFindReferenceInScene(MenuCommand command) => CanFindReferenceFor(command.context);

        [MenuItem("Assets/Soap/\ud83d\udd0d Find References/In Scene", true)]
        private static bool ValidateFindReferenceInScene() => CanFindReferenceFor(Selection.activeObject);

        [MenuItem("CONTEXT/ScriptableBase/Find References/In Project", true)]
        private static bool ValidateFindReferenceInProject(MenuCommand command) => CanFindReferenceFor(command.context);

        [MenuItem("Assets/Soap/\ud83d\udd0d Find References/In Project", true)]
        private static bool ValidateFindReferenceInProject() => CanFindReferenceFor(Selection.activeObject);

        private static bool CanFindReferenceFor(Object obj)
        {
            var isScriptable = obj is ScriptableBase;
            return isScriptable;
        }

        private static void FindReferenceFor(Object unityObject, FindReferenceType findReferenceType)
        {
            var scriptableBase = unityObject as ScriptableBase;
            if (scriptableBase == null)
                return;
            EditorWindow mouseOverWindow = EditorWindow.mouseOverWindow;
            if (mouseOverWindow == null)
                return;

            ReferencesPopupWindow.ShowWindow(mouseOverWindow.position, scriptableBase, findReferenceType);
        }

        [MenuItem("Assets/Soap/\ud83d\udd16 Set Tag")]
        private static void SetTag()
        {
            var scriptableBases = Selection.GetFiltered<ScriptableBase>(SelectionMode.Assets);
            PopupWindow.Show(new Rect(), new SetTagPopupWindow(scriptableBases));
        }

        [MenuItem("Assets/Soap/\ud83d\udd16 Set Tag", true)]
        private static bool ValidateSetTag()
        {
            return Selection.GetFiltered<ScriptableBase>(SelectionMode.Assets).Length > 0;
        }

        [MenuItem("Assets/Soap/\ud83d\uddbc Set Icon")]
        public static void SetIcon()
        {
            var monoScripts = Selection.GetFiltered<MonoScript>(SelectionMode.Assets);
            SoapInspectorUtils.Icons.SetIcons(monoScripts);
        }

        [MenuItem("Assets/Soap/\ud83d\uddbc Set Icon", true)]
        private static bool ValidateSetIcon()
        {
            var monoScripts = Selection.GetFiltered<MonoScript>(SelectionMode.Assets);
            foreach (var monoScript in monoScripts)
            {
                var scriptClass = monoScript.GetClass();
                if (scriptClass == null)
                    continue;
                if (!scriptClass.IsSubclassOf(typeof(ScriptableBase)))
                {
                    return false;
                }
            }

            return monoScripts.Length > 0;
        }

        [MenuItem("CONTEXT/ScriptableObject/Select Parent", false, 1)]
        private static void PingParent(MenuCommand menuCommand)
        {
            var scriptableBase = menuCommand.context as ScriptableBase;
            var path = AssetDatabase.GetAssetPath(scriptableBase);
            var parent = AssetDatabase.LoadMainAssetAtPath(path);
            Selection.activeObject = parent;
        }

        [MenuItem("CONTEXT/ScriptableObject/Select Parent", true)]
        private static bool IsSubAsset(MenuCommand command)
        {
            var obj = command.context;
            var isScriptable = obj is ScriptableBase;
            var isSubAsset = !AssetDatabase.IsMainAsset(obj);
            return isScriptable && isSubAsset;
        }

        [MenuItem("CONTEXT/Object/\ud83e\uddfc Create Soap ScriptableObjects", false, 0)]
        private static void CreateSoapScriptableObjects(MenuCommand command)
        {
            var obj = command.context;
            var isValid = command.context as MonoBehaviour || command.context as ScriptableObject;
            if (!isValid)
                return;

            var so = new SerializedObject(command.context);
            so.Update();

            // Find all serializable fields of type ScriptableBase (or derived)
            var soapFields = SoapEditorUtils.GetSerializableFields(obj.GetType(),
                typeof(ScriptableBase));
            bool didAnything = false;

            foreach (var soapField in soapFields)
            {
                SerializedProperty property = so.FindProperty(soapField.Name);
                if (property == null)
                    continue;

                // ensure it's an object reference field and is currently null
                if (property.propertyType != SerializedPropertyType.ObjectReference ||
                    property.objectReferenceValue != null)
                    continue;

                var variableName = soapField.Name;
                var prefix = SoapEditorUtils.GetOrCreateSoapSettings().GetPrefix(soapField.FieldType);
                var assetName = $"{prefix}{variableName}";

                if (SoapEditorUtils.CanBeSubAsset(property, soapField))
                    SoapEditorUtils.CreateSubAsset(property, soapField, assetName);
                else
                    SoapEditorUtils.CreateSoapSoAtPath(property, soapField, assetName);

                didAnything = true;
            }

            if (didAnything)
            {
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(obj);
            }
        }

        [MenuItem("CONTEXT/Object/\ud83e\uddfc Create Soap ScriptableObjects", true)]
        private static bool ValidateCreateSoapScriptableObjects(MenuCommand command)
        {
            var isValid = command.context as MonoBehaviour || command.context as ScriptableObject;
            if (!isValid)
                return false;
            var t = command.context.GetType();
            foreach (var field in SoapEditorUtils.GetSerializableFields(t, typeof(ScriptableBase)))
            {
                if (typeof(ScriptableBase).IsAssignableFrom(field.FieldType))
                    return true;
            }
            return false;
        }
    }
}