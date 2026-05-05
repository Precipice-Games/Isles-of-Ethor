using System;
using System.Collections.Generic;
using UnityEngine;

namespace Obvious.Soap.Example
{
    // This class is a simple example of a Save Data class.
    // This should be in its own file, but for readability, it is included here.
    [Serializable]
    public class SaveData
    {
        public int Version = 1;
        public int Level = 0;
        public List<Item> Items = new List<Item>();
    }

    [Serializable]
    public class Item
    {
        public string Id;
        public string Name;

        public Item(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }
    }

    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/5_scriptablesaves/save-data")]
    [CreateAssetMenu(fileName = "scriptableSaveExample.asset", menuName = "Soap/Examples/ScriptableSaves/ScriptableSaveExample")]
    public class ScriptableSaveExample : ScriptableSave<SaveData>
    {
        //Useful getters
        public int Version => _data.Version;
        public int Level => _data.Level;
        public IReadOnlyList<Item> Items => _data.Items.AsReadOnly();

        #region Useful Methods

        public void AddRandomItem() => AddItem(new Item("RandomItem_" + Items.Count));

        public void AddItem(Item item)
        {
            _data.Items.Add(item);
            Save();
        }

        public Item GetItemById(string id)
        {
            return _data.Items.Find(item => item.Id == id);
        }

        public void ClearItems()
        {
            _data.Items.Clear();
            Save();
        }
        
        public void IncrementLevel(int value)
        {
            _data.Level += value;
            Save();
        }
        
        public void SetLevel(int value)
        {
            _data.Level = value;
            Save();
        }
        

        #endregion
        
        protected override void Upgrade(SaveData oldData)
        {
            if (_debugLogEnabled)
                Debug.Log("Upgrading data from version " + oldData.Version + " to " + _data.Version);
            // Implement additional upgrade logic here
            oldData.Version = _data.Version;
        }

        protected override bool RequiresUpgrade(SaveData saveData)
        {
            return saveData.Version < _data.Version;
        }
    }
}