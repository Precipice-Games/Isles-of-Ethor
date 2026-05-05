using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Obvious.Soap.Editor
{
    internal static class SoapPackageInfo
    {
        private const string PackageName = "com.obvious.soap";

        /// <summary>
        /// Finds the package.json via AssetDatabase and extracts the "version" field.
        /// Returns false if not found or if parsing fails.
        /// </summary>
        public static bool TryGetVersion(out string version)
        {
            version = null;
            string[] guids = AssetDatabase.FindAssets("package t:TextAsset");

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (!assetPath.EndsWith("package.json"))
                    continue;

                string json;
                try
                {
                    json = File.ReadAllText(assetPath, new UTF8Encoding(false));
                }
                catch
                {
                    continue;
                }

                if (!ContainsPackageName(json, PackageName))
                    continue;

                // Extract version
                string v;
                if (TryExtractJsonString(json, "version", out v))
                {
                    version = v;
                    return true;
                }

                return false;
            }

            return false;
        }

        private static bool ContainsPackageName(string json, string expectedName)
        {
            string nameValue;
            return TryExtractJsonString(json, "name", out nameValue) && nameValue == expectedName;
        }

        private static bool TryExtractJsonString(string json, string key, out string value)
        {
            value = null;

            var match = Regex.Match(json, "\"" + Regex.Escape(key) + "\"\\s*:\\s*\"(?<v>[^\"]+)\"");
            if (!match.Success)
                return false;

            value = match.Groups["v"].Value.Trim();
            return !string.IsNullOrEmpty(value);
        }
    }
}