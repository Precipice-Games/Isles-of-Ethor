using UnityEngine;
using UnityEditor;

namespace Obvious.Soap.Example.Editor
{
    [CustomEditor(typeof(ScriptableSaveExample))]
    public class ScriptableSaveExampleEditor : UnityEditor.Editor
    {
        private string newItemName = "NewItem";
        private int levelValue = 0;

        private float Spacing => EditorGUIUtility.singleLineHeight * 0.5f;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ScriptableSaveExample scriptableSave = (ScriptableSaveExample)target;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Useful Methods", EditorStyles.boldLabel);
            GUILayout.Space(Spacing);

            EditorGUILayout.BeginHorizontal();
            levelValue = EditorGUILayout.IntField("Level", levelValue);
            if (GUILayout.Button("Set Level"))
            {
                scriptableSave.SetLevel(levelValue);
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(Spacing);

            EditorGUILayout.BeginHorizontal();
            newItemName = EditorGUILayout.TextField("Item Name", newItemName);
            if (GUILayout.Button("Add Item"))
            {
                scriptableSave.AddItem(new Item(newItemName));
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Clear Items"))
            {
                scriptableSave.ClearItems();
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save", GUILayout.Height(30f)))
            {
                scriptableSave.Save();
            }

            if (GUILayout.Button("Load", GUILayout.Height(30f)))
            {
                scriptableSave.Load();
            }

            if (GUILayout.Button("Print", GUILayout.Height(30f)))
            {
                scriptableSave.PrintToConsole();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete", GUILayout.Height(30f)))
            {
                scriptableSave.Delete();
            }

            if (GUILayout.Button("Open Save Location", GUILayout.Height(30f)))
            {
                scriptableSave.OpenSaveLocation();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}