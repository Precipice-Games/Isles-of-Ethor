using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/useful-tips/read-only-assets")]
    public class ReadOnlyListExample : MonoBehaviour
    {
        [SerializeField] private ScriptableListPlayerReadOnly _listReadOnly;

        private void Awake()
        {
            // Subscribe to events as usual is possible for read-only.
            _listReadOnly.Modified += OnModified;
            _listReadOnly.OnItemAdded += OnItemAdded;
            _listReadOnly.OnItemRemoved += OnItemRemoved;

            // Modifying the list is not allowed and would cause a compile-time error, as expected:
            // _listReadOnly.Add(go);
        }

        private void OnItemRemoved(Player obj)
        {
            Debug.Log("Item Removed: " + obj.name);
        }

        private void OnItemAdded(Player obj)
        {
            Debug.Log("Item Added: " + obj.name);
        }

        private void OnModified()
        {
            Debug.Log("List Modified :" + _listReadOnly.Count);
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks.
            _listReadOnly.Modified -= OnModified;
            _listReadOnly.OnItemAdded -= OnItemAdded;
            _listReadOnly.OnItemRemoved -= OnItemRemoved;
        }
    }
}