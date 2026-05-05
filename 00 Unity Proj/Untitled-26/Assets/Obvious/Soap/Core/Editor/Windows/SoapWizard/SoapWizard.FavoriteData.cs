using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public partial class SoapWizardWindow
    {
        [Serializable]
        private class FavoriteData
        {
            public List<string> FavoriteGUIDs = new List<string>();
            private HashSet<string> _favoriteGUIDsSet = new HashSet<string>();

            public FavoriteData()
            {
                _favoriteGUIDsSet = new HashSet<string>(FavoriteGUIDs);
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this, false);
            }

            public void AddFavorite(ScriptableBase asset)
            {
                var guid = SoapEditorUtils.GenerateGuid(asset);

                if (!string.IsNullOrEmpty(guid) && _favoriteGUIDsSet.Add(guid))
                {
                    FavoriteGUIDs.Add(guid);
                    Save();
                }
            }

            public void RemoveFavorite(ScriptableBase asset)
            {
                if (asset == null) return;
                var guid = SoapEditorUtils.GenerateGuid(asset);

                if (!string.IsNullOrEmpty(guid) && _favoriteGUIDsSet.Remove(guid))
                {
                    FavoriteGUIDs.Remove(guid);
                    Save();
                }
            }

            public bool IsFavorite(ScriptableBase asset)
            {
                if (asset == null)
                    return false;
                var guid = SoapEditorUtils.GenerateGuid(asset);
                return !string.IsNullOrEmpty(guid) && _favoriteGUIDsSet.Contains(guid);
            }

            public List<ScriptableBase> GetFavorites()
            {
                List<ScriptableBase> assets = new List<ScriptableBase>();
                foreach (var guid in FavoriteGUIDs)
                {
                    //ignore sub assets
                    if (guid.Length > 32)
                        continue;

                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<ScriptableBase>(path);
                    if (asset != null)
                        assets.Add(asset);
                }

                return assets;
            }

            public void Save()
            {
                SoapEditorUtils.WizardFavorites = ToString();
                //Debug.Log("Saved Favorites:" + ToString());
            }

            public static FavoriteData Load()
            {
                var key = SoapEditorUtils.WizardFavoritesKey + Application.dataPath.GetHashCode();
                if (EditorPrefs.HasKey(key))
                {
                    string json = SoapEditorUtils.WizardFavorites;
                    FavoriteData data = JsonUtility.FromJson<FavoriteData>(json) ?? new FavoriteData();
                    data._favoriteGUIDsSet = new HashSet<string>(data.FavoriteGUIDs);
                    return data;
                }

                return new FavoriteData();
            }
        }
    }
}