using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public static class SoapFileUtils
    {
        /// <summary> Given an absolute path, return a path rooted at the Assets folder. </summary>
        /// <example> /Folder/UnityProject/Assets/resources/music -> Assets/resources/music </example>
        public static string GetRelativePath(string path)
        {
            if (path.StartsWith(Application.dataPath))
                return "Assets" + path.Substring(Application.dataPath.Length);
            else
                throw new ArgumentException("Full path does not contain the current project's Assets folder");
        }

        /// <summary>
        /// Get all available Resources directory paths within the current project.
        /// </summary>
        public static string[] GetResourcesDirectories()
        {
            var result = new List<string>();
            var stack = new Stack<string>();
            stack.Push(Application.dataPath);

            while (stack.Count > 0)
            {
                var currentDir = stack.Pop();
                foreach (var dir in Directory.GetDirectories(currentDir))
                {
                    if (Path.GetFileName(dir).Equals("Resources"))
                        result.Add(dir);
                    stack.Push(dir);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Returns the current selected folder in the Project Window.
        /// Returns "Asset/SoapGenerated" if the project window is not selected.
        /// </summary>
        public static string GetSelectedFolderPathInProjectWindow2()
        {
            var methodInfo = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath",
                BindingFlags.Static | BindingFlags.NonPublic);
            var args = new object[] { null };
            var found = (bool)methodInfo.Invoke(null, args);
            return found ? (string)args[0] : "Assets/SoapGenerated";
        }
        
        public static string GetSelectedFolderPathInProjectWindow()
        {
            // Try the two-column layout method first
            var methodInfo = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath",
                BindingFlags.Static | BindingFlags.NonPublic);
            var args = new object[] { null };
            var found = (bool)methodInfo.Invoke(null, args);
    
            if (found)
                return (string)args[0];
    
            // Fallback: check the current selection (works for one-column layout)
            var selectedObject = Selection.activeObject;
            if (selectedObject != null)
            {
                var path = AssetDatabase.GetAssetPath(selectedObject);
                if (!string.IsNullOrEmpty(path))
                {
                    // If it's a folder, return it directly
                    if (AssetDatabase.IsValidFolder(path))
                        return path;
            
                    // If it's a file, return the containing folder
                    return Path.GetDirectoryName(path).Replace("\\", "/");
                }
            }
    
            return "Assets/SoapGenerated";
        }
    }
}