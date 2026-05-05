using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Obvious.Soap.Attributes;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Obvious.Soap.Editor
{
    public static class SoapEditorUtils
    {
        #region EditorPrefs

        private const string CustomCreationPathKey = "Soap_CreateFolderPath_";
        private const string WizardRootFolderPathKey = "Soap_Wizard_RootFolderPath_";
        internal const string WizardFavoritesKey = "Soap_Wizard_Favorites_";
        private const string WizardTagsKey = "Soap_Wizard_Tags_";
        private const string TypeCreatorDestinationFolderIndexKey = "Soap_TypeCreator_DestinationFolderIndex_";
        private const string TypeCreatorDestinationFolderPathKey = "Soap_TypeCreator_DestinationFolderPath_";
        private const string TypeCreatorNamespaceKey = "Soap_TypeCreator_Namespace_";
        private const string WindowLastCategoryKey = "Soap_Window_LastCategory_";
        private const string WindowHasShownWindowKey = "Soap_Window_HasShownWindow_";
        private const string SingletonInstanceInfoKey = "Soap_SingletonCreator_InstanceInfo_";

        private static int ApplicationHash => Application.dataPath.GetHashCode();

        internal static string CustomCreationPath
        {
            get => EditorPrefs.GetString(CustomCreationPathKey + ApplicationHash, "Assets");
            set => EditorPrefs.SetString(CustomCreationPathKey + ApplicationHash, value);
        }

        internal static string WizardRootFolderPath
        {
            get => EditorPrefs.GetString(WizardRootFolderPathKey + ApplicationHash, "Assets");
            set => EditorPrefs.SetString(WizardRootFolderPathKey + ApplicationHash, value);
        }

        internal static string WizardFavorites
        {
            get => EditorPrefs.GetString(WizardFavoritesKey + ApplicationHash, string.Empty);
            set => EditorPrefs.SetString(WizardFavoritesKey + ApplicationHash, value);
        }

        internal static int WizardTags
        {
            get => EditorPrefs.GetInt(WizardTagsKey + ApplicationHash, 1);
            set => EditorPrefs.SetInt(WizardTagsKey + ApplicationHash, value);
        }

        internal static int TypeCreatorDestinationFolderIndex
        {
            get => EditorPrefs.GetInt(TypeCreatorDestinationFolderIndexKey + ApplicationHash, 0);
            set => EditorPrefs.SetInt(TypeCreatorDestinationFolderIndexKey + ApplicationHash, value);
        }

        internal static string TypeCreatorDestinationFolderPath
        {
            get => EditorPrefs.GetString(TypeCreatorDestinationFolderPathKey + ApplicationHash, "Assets");
            set => EditorPrefs.SetString(TypeCreatorDestinationFolderPathKey + ApplicationHash, value);
        }


        internal static string TypeCreatorNamespace
        {
            get => EditorPrefs.GetString(TypeCreatorNamespaceKey + ApplicationHash, string.Empty);
            set => EditorPrefs.SetString(TypeCreatorNamespaceKey + ApplicationHash, value);
        }

        internal static string WindowLastCategory
        {
            get => EditorPrefs.GetString(WindowLastCategoryKey + ApplicationHash, string.Empty);
            set => EditorPrefs.SetString(WindowLastCategoryKey + ApplicationHash, value);
        }

        internal static bool HasShownWindow
        {
            get => EditorPrefs.GetBool(WindowHasShownWindowKey + ApplicationHash, false);
            set => EditorPrefs.SetBool(WindowHasShownWindowKey + ApplicationHash, value);
        }

        internal static string SingletonInstanceInfo
        {
            get => SessionState.GetString(SingletonInstanceInfoKey + ApplicationHash, string.Empty);
            set => SessionState.SetString(SingletonInstanceInfoKey + ApplicationHash, value);
        }

        #endregion

        internal static Color SoapColor =>
            ColorUtility.TryParseHtmlString(SoapColorHtml, out var color) ? color : Color.magenta;

        internal const string SoapColorHtml = "#f75369";

        internal static Color Lighten(this Color color, float amount)
        {
            return new Color(color.r + amount, color.g + amount, color.b + amount, color.a);
        }

        internal static SoapSettings GetOrCreateSoapSettings()
        {
            var settings = Resources.Load<SoapSettings>("SoapSettings");
            if (settings != null)
                return settings;

            var paths = SoapFileUtils.GetResourcesDirectories();
            foreach (var path in paths)
            {
                var relative = SoapFileUtils.GetRelativePath(path);
                if (!relative.Contains("Obvious") || !relative.Contains("Editor"))
                    continue;
                var forwardSlashPath = relative.Replace('\\', '/');
                var finalPath = forwardSlashPath + "/SoapSettings.asset";
                AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(typeof(SoapSettings)), finalPath);
                settings = Resources.Load<SoapSettings>("SoapSettings");
            }

            return settings;
        }

        /// <summary>
        /// Deletes an object after showing a confirmation dialog.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns> true if the deletion is confirmed</returns>
        public static bool DeleteObjectWithConfirmation(Object obj)
        {
            var confirmDelete = EditorUtility.DisplayDialog("Delete " + obj.name + "?",
                "Are you sure you want to delete '" + obj.name + "'?", "Yes", "No");
            if (confirmDelete)
            {
                if (AssetDatabase.IsSubAsset(obj))
                {
                    DeleteSubAsset(obj);
                }
                else
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    AssetDatabase.DeleteAsset(path);
                }

                return true;
            }

            return false;
        }

        internal static void CreateSoapSoAtPath(SerializedProperty property, FieldInfo fieldInfo, string assetName)
        {
            var soapSettings = GetOrCreateSoapSettings();
            var isCustomPath = soapSettings.CreatePathMode == ECreatePathMode.Manual;
            var path = isCustomPath ? CustomCreationPath : SoapFileUtils.GetSelectedFolderPathInProjectWindow();
            var scriptable = CreateScriptableObject(fieldInfo.FieldType, assetName, path);
            var scriptableBase = (ScriptableBase)scriptable;
            var tagIndex = GetTagIndexFromAttribute(fieldInfo);
            scriptableBase.TagIndex = tagIndex;
            property.objectReferenceValue = scriptableBase;
            property.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        internal static void CreateSubAsset(SerializedProperty property, FieldInfo fieldInfo, string assetName)
        {
            var soapSettings = GetOrCreateSoapSettings();
            var mainAsset = property.serializedObject.targetObject;
            var subAsset = ScriptableObject.CreateInstance(fieldInfo.FieldType);
            var prefix = soapSettings.GetPrefix(fieldInfo.FieldType);
            var cleanedName = CleanSubAssetName(assetName);
            subAsset.name = $"{prefix}{cleanedName}";
            AssetDatabase.AddObjectToAsset(subAsset, mainAsset);
            var scriptableBase = (ScriptableBase)subAsset;
            scriptableBase.TagIndex = GetTagIndexFromAttribute(fieldInfo);
            property.objectReferenceValue = subAsset;
            property.serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Creates a copy of an object. Also adds a number to the copy name.
        /// </summary>
        /// <param name="obj"></param>
        public static void CreateCopy(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var copyFilePath = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CopyAsset(path, copyFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var newAsset = AssetDatabase.LoadMainAssetAtPath(copyFilePath);
            EditorGUIUtility.PingObject(newAsset);
        }

        /// <summary>
        /// Core creator: creates a ScriptableObject asset of the given type at path/name.
        /// If name is null/empty, uses type.Name. Returns the created instance.
        /// </summary>
        public static ScriptableObject CreateScriptableObject(Type type, string name, string path, bool ping = true)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!typeof(ScriptableObject).IsAssignableFrom(type))
                throw new ArgumentException($"Type {type} is not a ScriptableObject.", nameof(type));

            var normalizedPath = EnsureAssetFolder(path);

            var baseName = string.IsNullOrWhiteSpace(name) ? type.Name : name.Trim();
            var assetPath = $"{normalizedPath}/{baseName}.asset";
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            var instance = ScriptableObject.CreateInstance(type);
            instance.name = name == "" ? type.ToString().Replace("Obvious.Soap.", "") : baseName;
            AssetDatabase.CreateAsset(instance, uniquePath);
            if (ping)
                EditorGUIUtility.PingObject(instance);

            return instance;
        }

        /// <summary>
        /// Ensures an "Assets/..." folder path exists, creating subfolders as needed using AssetDatabase.
        /// Accepts either "Assets/Sub/Dir" or "Sub/Dir" (the latter will be rooted under Assets).
        /// Returns a normalized "Assets/..." path.
        /// </summary>
        private static string EnsureAssetFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path must not be null/empty.", nameof(path));

            // Normalize
            path = path.Replace('\\', '/').Trim('/');
            if (!path.StartsWith("Assets", StringComparison.Ordinal))
                path = "Assets/" + path;

            var parts = path.Split('/');
            var current = "Assets";

            // Walk and create
            for (int i = 1; i < parts.Length; i++)
            {
                var next = $"{current}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    var folderName = parts[i];
                    AssetDatabase.CreateFolder(current, folderName);
                }

                current = next;
            }

            return current;
        }

        /// <summary> Renames an asset </summary>
        /// <param name="obj"></param
        /// <param name="newName"></param>
        public static void RenameAsset(Object obj, string newName)
        {
            if (AssetDatabase.IsSubAsset(obj))
            {
                obj.name = newName;
                EditorUtility.SetDirty(obj);
            }
            else
            {
                var path = AssetDatabase.GetAssetPath(obj);
                AssetDatabase.RenameAsset(path, newName);
            }

            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(obj);
        }

        internal static bool CreateClassFromTemplate(string template, string nameSpace, string type, string path,
            out TextAsset newFile, bool isIntrinsic = false, bool isSoapClass = false)
        {
            var folderName = "Templates/";
            folderName +=
                isIntrinsic
                    ? "1_"
                    : string.Empty; //select another template for intrinsic types as they cannot work with nameof(T)
            folderName += template;
            var capitalizedName = type.CapitalizeFirstLetter();

            var fileName = capitalizedName + ".cs";
            if (isSoapClass)
            {
                //variables follow a different naming rules
                fileName = template.Contains("Variable")
                    ? $"{capitalizedName}Variable.cs"
                    : template.Replace("Template", capitalizedName);
            }

            newFile = CreateNewClass(folderName, nameSpace, type, fileName, path);
            return newFile != null;
        }

        internal static bool CreateDictionaryFromTemplate(string template, string nameSpace, string type,
            string key, string value, string path, out TextAsset newFile)
        {
            var folderName = "Templates/";
            folderName += template;
            var capitalizedName = type.CapitalizeFirstLetter();
            var fileName = capitalizedName + ".cs";
            newFile = CreateNewDictionaryClass(folderName, nameSpace, type, key, value, fileName, path);
            return newFile != null;
        }

        private static TextAsset CreateNewClass(string templateName, string nameSpace, string type, string fileName,
            string path)
        {
            if (!IsTypeNameValid(type) || !IsNamespaceValid(nameSpace))
                return null;

            var templateCode = GetTemplateContent(templateName);
            templateCode = templateCode.Replace("$TYPE$", type);
            templateCode = templateCode.Replace("$TYPENAME$", type.CapitalizeFirstLetter());

            //wrap namespace if needed
            if (!string.IsNullOrEmpty(nameSpace))
            {
                templateCode = SplitAndWrapInNamespace(templateCode, nameSpace);
            }

            try
            {
                var newFile = CreateTextFile(templateCode, fileName, path);
                return newFile;
            }
            catch (IOException e)
            {
                EditorUtility.DisplayDialog("Could not create class", e.Message, "OK");
                return null;
            }
        }

        private static TextAsset CreateNewDictionaryClass(string templateName, string nameSpace, string type,
            string key, string value, string fileName, string path)
        {
            if (!IsTypeNameValid(type) || !IsNamespaceValid(nameSpace))
                return null;

            var templateCode = GetTemplateContent(templateName);
            templateCode = templateCode.Replace("$TYPE$", type);
            templateCode = templateCode.Replace("$KEY$", key);
            templateCode = templateCode.Replace("$VALUE$", value);

            //wrap namespace if needed
            if (!string.IsNullOrEmpty(nameSpace))
            {
                templateCode = SplitAndWrapInNamespace(templateCode, nameSpace);
            }

            try
            {
                var newFile = CreateTextFile(templateCode, fileName, path);
                return newFile;
            }
            catch (IOException e)
            {
                EditorUtility.DisplayDialog("Could not create class", e.Message, "OK");
                return null;
            }
        }

        private static TextAsset CreateNewSingletonClass(string templateName, string nameSpace, string type
            , string fileName, string path, string resourcePath)
        {
            if (!IsTypeNameValid(type) || !IsNamespaceValid(nameSpace))
                return null;

            var templateCode = GetTemplateContent(templateName);
            templateCode = templateCode.Replace("$TYPENAME$", type.CapitalizeFirstLetter());
            templateCode = templateCode.Replace("$RESOURCE$", resourcePath);

            //wrap namespace if needed
            if (!string.IsNullOrEmpty(nameSpace))
            {
                templateCode = SplitAndWrapInNamespace(templateCode, nameSpace);
            }

            try
            {
                var newFile = CreateTextFile(templateCode, fileName, path);
                return newFile;
            }
            catch (IOException e)
            {
                EditorUtility.DisplayDialog("Could not create class", e.Message, "OK");
                return null;
            }
        }

        private static string GetTemplateContent(string templateName)
        {
            var template = Resources.Load<TextAsset>(templateName);
            if (template is null)
            {
                Debug.LogError($"Failed to find {templateName} in a Resources folder");
                return null;
            }

            return template.text;
        }

        private static string SplitAndWrapInNamespace(string templateCode, string nameSpace)
        {
            var templateParts = SplitStringAfter(templateCode, "using Obvious.Soap;");
            var partToWrap = string.IsNullOrEmpty(templateParts.Item2)
                ? templateParts.Item1
                : templateParts.Item2;

            templateCode = partToWrap.WrapInNamespace(nameSpace);

            //if there was two parts, add the first part back at the top + spaces
            if (!string.IsNullOrEmpty(templateParts.Item2))
            {
                var partToInsert = templateParts.Item1 + Environment.NewLine + Environment.NewLine;
                templateCode = templateCode.Insert(0, partToInsert);
            }

            return templateCode;
        }

        private static string WrapInNamespace(this string input, string nameSpace)
        {
            if (string.IsNullOrEmpty(nameSpace))
                return input;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"namespace {nameSpace}");
            stringBuilder.AppendLine("{");

            // Split the input into lines for indentation
            var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
                stringBuilder.AppendLine("    " + line);

            stringBuilder.AppendLine("}");
            return stringBuilder.ToString();
        }

        private static (string, string) SplitStringAfter(string input, string splitAfter)
        {
            var index = input.IndexOf(splitAfter);
            if (index != -1)
            {
                // Include the length of splitAfter to get the rest of the string after it
                var splitPoint = index + splitAfter.Length;
                var before = input.Substring(0, splitPoint);
                var after = input.Substring(splitPoint);
                return (before, after);
            }

            return (input, string.Empty);
        }

        /// <summary> Creates a new text asset at a certain location. </summary>
        /// <param name="content"></param>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <exception cref="IOException"></exception>
        private static TextAsset CreateTextFile(string content, string fileName, string path)
        {
            var normalizedPath = EnsureAssetFolder(path);
            var assetPath = $"{normalizedPath}/{fileName}";

            // if (!Directory.Exists(folderPath))
            //     Directory.CreateDirectory(folderPath);

            if (File.Exists(assetPath))
                throw new IOException($"A file with the name {assetPath} already exists.");

            File.WriteAllText(assetPath, content);
            AssetDatabase.Refresh();
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            return textAsset;
        }

        /// <summary> Returns all ScriptableObjects at a certain path. </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        public static List<T> FindAll<T>(string path = "") where T : ScriptableObject
        {
            var scriptableObjects = new List<T>();
            var searchFilter = $"t:{typeof(T).Name}";
            var soNames = path == ""
                ? AssetDatabase.FindAssets(searchFilter)
                : AssetDatabase.FindAssets(searchFilter, new[] { path });

            //we need this to make sure we load the sub assets only once.
            var mainAssetPath = new HashSet<string>();

            foreach (var soName in soNames)
            {
                var soPath = AssetDatabase.GUIDToAssetPath(soName); //sub assets all return the same path
                var asset = AssetDatabase.LoadAssetAtPath<T>(soPath);
                if (mainAssetPath.Contains(soPath))
                    continue;

                var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(soPath);
                //if there are no subAssets, the asset is the main asset, and is of T
                if (subAssets.Length == 0)
                {
                    scriptableObjects.Add(asset);
                    continue;
                }

                foreach (var subAsset in subAssets)
                {
                    //still need to check if the sub asset is of the correct type
                    var subAssetCasted = subAsset as T;
                    if (subAssetCasted != null)
                        scriptableObjects.Add(subAssetCasted);
                }

                mainAssetPath.Add(soPath);
            }

            return scriptableObjects;
        }


        internal static List<T> FindAll<T>(string path, out Dictionary<ScriptableBase, Object> subAssetLookUp)
            where T : ScriptableBase
        {
            var scriptableObjects = new List<T>();
            var searchFilter = $"t:{typeof(T).Name}";
            var soNames = path == ""
                ? AssetDatabase.FindAssets(searchFilter)
                : AssetDatabase.FindAssets(searchFilter, new[] { path });

            //we need this to make sure we load the sub assets only once.
            var mainAssetPath = new HashSet<string>();
            //returns the subAsset as key and the parent asset as value
            subAssetLookUp = new Dictionary<ScriptableBase, Object>();

            foreach (var soName in soNames)
            {
                var soPath = AssetDatabase.GUIDToAssetPath(soName); //sub assets all return the same path
                var asset = AssetDatabase.LoadAssetAtPath<T>(soPath);
                if (mainAssetPath.Contains(soPath))
                    continue;

                var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(soPath);
                //if there are no subAssets, the asset is the main asset, and is of T
                if (subAssets.Length == 0)
                {
                    scriptableObjects.Add(asset);
                    continue;
                }

                foreach (var subAsset in subAssets)
                {
                    scriptableObjects.Add(subAsset as T);
                    var parent = AssetDatabase.LoadMainAssetAtPath(soPath);
                    subAssetLookUp.Add(subAsset as ScriptableBase, parent);
                }

                mainAssetPath.Add(soPath);
            }

            return scriptableObjects;
        }

        internal static Dictionary<ScriptableBase, Object> GetSubAssets<T>(List<T> scriptableObjects)
            where T : ScriptableBase
        {
            var mainAssetPath = new HashSet<string>();
            //returns the subAsset as key and the parent asset as value
            var subAssetLookUp = new Dictionary<ScriptableBase, Object>();
            foreach (var scriptableObject in scriptableObjects)
            {
                //sub assets all return the same path
                var soPath = AssetDatabase.GetAssetPath(scriptableObject);
                if (mainAssetPath.Contains(soPath))
                    continue;

                var parent = AssetDatabase.LoadMainAssetAtPath(soPath);
                var subAssets = GetAllSubAssets(parent);

                if (subAssets.Count == 0)
                    continue;

                foreach (var subAsset in subAssets)
                    subAssetLookUp.Add(subAsset as ScriptableBase, parent);

                mainAssetPath.Add(soPath);
            }

            return subAssetLookUp;
        }


        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }

        internal static List<(GameObject, Type, string, string)> FindReferencesInScene(ScriptableBase scriptableBase)
        {
            var sceneReferences = new List<(GameObject, Type, string, string)>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (!s.isLoaded)
                    continue;

                var rootObjects = s.GetRootGameObjects();
                foreach (var rootObject in rootObjects)
                {
                    FindReferencesInGameObject(rootObject, scriptableBase, ref sceneReferences);
                }
            }

            return sceneReferences;
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = obj.name + "/" + path;
            }

            return path;
        }

        private static void FindReferencesInGameObject(GameObject gameObject, ScriptableBase scriptableBase,
            ref List<(GameObject, Type, string, string)> sceneReferences)
        {
            var components = gameObject.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component == null) //a missing scripts
                    continue;

                var serializedObject = new SerializedObject(component);
                var serializedProperty = serializedObject.GetIterator();
                while (serializedProperty.NextVisible(true))
                {
                    if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference &&
                        serializedProperty.objectReferenceValue is ScriptableBase target)
                    {
                        if (target != scriptableBase)
                            continue;
                        var entry = (gameObject, component.GetType(), serializedProperty.name,
                            GetGameObjectPath(gameObject));
                        sceneReferences.Add(entry);
                    }
                }
            }

            foreach (Transform child in gameObject.transform)
                FindReferencesInGameObject(child.gameObject, scriptableBase, ref sceneReferences);
        }

        internal static Dictionary<string, int> FindReferencesInProject(ScriptableBase scriptableBase)
        {
            var assetPaths = AssetDatabase.FindAssets("t:Prefab t:ScriptableObject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.StartsWith("Assets/") && !path.EndsWith(".unity"));
            Dictionary<string, int> references = new Dictionary<string, int>();
            var pathHash = new HashSet<string>();
            var maxIterations = 100000;

            foreach (var assetPath in assetPaths)
            {
                if (pathHash.Contains(assetPath)) //needed to filter sub assets as they all share the same path
                    continue;

                var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (var asset in assets)
                {
                    if (asset == null || AssetDatabase.IsSubAsset(asset))
                        continue;

                    var serializedObject = new SerializedObject(asset);
                    var property = serializedObject.GetIterator();
                    var referenceCount = 0;
                    var iterations = 0;

                    while (property.NextVisible(true))
                    {
                        iterations++;
                        //Safety for circular references
                        if (iterations > maxIterations)
                            break;

                        if (property.propertyType == SerializedPropertyType.ObjectReference &&
                            property.objectReferenceValue == scriptableBase)
                        {
                            referenceCount++;
                        }
                    }

                    if (referenceCount > 0)
                    {
                        if (references.ContainsKey(assetPath))
                            references[assetPath] += referenceCount;
                        else
                            references.Add(assetPath, referenceCount);
                    }
                }

                pathHash.Add(assetPath);
            }

            return references;
        }

        internal static bool CanBeSubAsset(SerializedProperty property, FieldInfo fieldInfo)
        {
            if (fieldInfo == null) //for Odin dictionary serialization
                return false;

            //main Asset has to be a SO
            var mainAsset = property.serializedObject.targetObject;
            if (mainAsset is ScriptableObject)
            {
                var isScriptableBase = fieldInfo.FieldType.IsSubclassOf(typeof(ScriptableBase));
                if (!isScriptableBase)
                    return false;

                var hasSubAsset = HasAttribute<SubAssetAttribute>(fieldInfo);
                return hasSubAsset;
            }

            return false;
        }

        private static bool HasAttribute<T>(FieldInfo fieldInfo) where T : Attribute
        {
            if (fieldInfo != null)
            {
                T attribute = (T)Attribute.GetCustomAttribute(fieldInfo, typeof(T));
                return attribute != null;
            }

            return false;
        }

        internal static List<Object> GetAllSubAssets(Object mainAsset)
        {
            var mainAssetPath = AssetDatabase.GetAssetPath(mainAsset);
            var allAssets = AssetDatabase.LoadAllAssetsAtPath(mainAssetPath);
            var subAssets = new List<Object>();
            foreach (var asset in allAssets)
            {
                if (asset == mainAsset || asset == null)
                    continue;

                subAssets.Add(asset);
            }

            return subAssets;
        }

        internal static void DeleteSubAsset(Object subAsset)
        {
            Object.DestroyImmediate(subAsset, true);
            AssetDatabase.SaveAssets();
        }

        internal static void AssignTag(ScriptableBase scriptableBase, int tagIndex)
        {
            var serializedObject = new SerializedObject(scriptableBase);
            serializedObject.FindProperty("TagIndex").intValue = tagIndex;
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Checks if a type name is valid.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static bool IsTypeNameValid(string typeName)
        {
            var valid = System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(typeName);
            return valid;
        }

        /// <summary>
        /// Checks if a namespace name is valid. An empty namespace is valid.
        /// </summary>
        /// <param name="namespaceName">The namespace name to validate.</param>
        /// <returns>True if the namespace name is valid; otherwise, false.</returns>
        internal static bool IsNamespaceValid(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName))
                return true;
            var parts = namespaceName.Split('.');
            foreach (var part in parts)
            {
                if (!System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(part))
                    return false;
            }

            if (namespaceName.StartsWith(".") || namespaceName.EndsWith(".") || namespaceName.Contains(".."))
                return false;

            return true;
        }

        internal static bool IsResourcePathValid(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath))
                return false;

            // Check for invalid characters
            char[] invalidChars = Path.GetInvalidPathChars();
            if (resourcePath.IndexOfAny(invalidChars) >= 0)
                return false;

            // Check for consecutive slashes
            if (resourcePath.Contains("//"))
                return false;

            // If any path segment is exactly "Resources" (case\-insensitive), it's valid
            var trimmed = resourcePath.Trim();
            trimmed = trimmed.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var segments = trimmed.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries);
            if (!segments.Any(s => string.Equals(s, "Resources", StringComparison.OrdinalIgnoreCase)))
                return false;

            return true;
        }

        internal static string GenerateGuid(Object obj)
        {
            var guid = string.Empty;
            //SubAssets
            if (!AssetDatabase.IsMainAsset(obj)
                && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out long file))
            {
                guid = GenerateGuidForSubAsset(obj, file);
            }
            else
                guid = GenerateGuidForMainAsset(obj);

            return guid;
        }

        private static string GenerateGuidForMainAsset(Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);
            return guid;
        }

        private static string GenerateGuidForSubAsset(Object obj, long fileId)
        {
            var guid = GenerateGuidForMainAsset(obj);
            guid += fileId.ToString().Substring(0, 5);
            return guid;
        }

        internal static string CleanSubAssetName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            Match match = Regex.Match(input, "<([^>]+)>");
            return match.Success ? match.Groups[1].Value : input;
        }

        internal static IEnumerable<FieldInfo> GetSerializableFields(Type type, Type wanted)
        {
            // Walk the inheritance chain (Unity serializes base fields too)
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.DeclaredOnly;


            for (var t = type; t != null && t != typeof(MonoBehaviour); t = t.BaseType)
            {
                var fields = t.GetFields(flags);
                foreach (var f in fields)
                {
                    if (f.IsStatic) continue;
                    if (!wanted.IsAssignableFrom(f.FieldType)) continue;
                    if (f.IsDefined(typeof(NonSerializedAttribute), inherit: false)) continue;

                    bool isSerialized = f.IsPublic || f.IsDefined(typeof(SerializeField), inherit: false);
                    if (!isSerialized) continue;

                    yield return f;
                }
            }
        }

        internal static int GetTagIndexFromAttribute(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                return 0;

            var soapSettings = GetOrCreateSoapSettings();

            var tagAttribute = (AutoTag)Attribute.GetCustomAttribute(fieldInfo,
                typeof(AutoTag));

            if (tagAttribute == null)
                return 0;

            if (string.IsNullOrEmpty(tagAttribute.Tag))
            {
                return soapSettings.GetTagIndex(tagAttribute.TagIndex);
            }

            return soapSettings.GetTagIndex(tagAttribute.Tag);
        }

        /// <summary>
        /// Clear editor Prefs for Soap.
        /// </summary>
        //[MenuItem("Tools/Obvious Game/Soap/Clear Editor Prefs")]
        internal static void ClearEditorPrefs()
        {
            var applicationHash = Application.dataPath.GetHashCode();
            EditorPrefs.DeleteKey(WizardRootFolderPathKey + applicationHash);
            EditorPrefs.DeleteKey(WizardFavoritesKey + applicationHash);
            EditorPrefs.DeleteKey(WizardTagsKey + applicationHash);
            EditorPrefs.DeleteKey(TypeCreatorDestinationFolderIndexKey + applicationHash);
            EditorPrefs.DeleteKey(TypeCreatorDestinationFolderPathKey + applicationHash);
            EditorPrefs.DeleteKey(WindowLastCategoryKey + applicationHash);
            EditorPrefs.DeleteKey(WindowHasShownWindowKey + applicationHash);
            EditorPrefs.DeleteKey(CustomCreationPathKey + applicationHash);
            Debug.Log("Editor prefs deleted");
        }
    }
}