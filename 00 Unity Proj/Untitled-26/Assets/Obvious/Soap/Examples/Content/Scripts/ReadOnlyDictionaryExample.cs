using UnityEngine;

namespace Obvious.Soap.Example
{
    public class ReadOnlyDictionaryExample : MonoBehaviour
    {
        [SerializeField] private ScriptableDictionaryElementIntReadOnly _dictionaryReadOnly;

        private void Awake()
        {
            // Subscribing to events as usual is possible for read-only.
            _dictionaryReadOnly.Modified += OnModified;
            _dictionaryReadOnly.OnItemAdded += OnItemAdded;
            _dictionaryReadOnly.OnItemRemoved += OnItemRemoved;

            /* Adding items to the read-only dictionary is not possible!
            This will not compile, as expected:
            if (!_dictionaryReadOnly.TryAdd(_enumElement, 1))
            {
                _dictionaryReadOnly[_enumElement]++;
            }*/
        }

        private void OnItemRemoved(ScriptableEnumElement key, int value)
        {
            Debug.Log("Item Removed: " + key.name + " with value: " + value);
        }

        private void OnItemAdded(ScriptableEnumElement key, int value)
        {
            Debug.Log("Item Added: " + key.name + " with value: " + value);
        }

        private void OnModified()
        {
            foreach (var kvp in _dictionaryReadOnly)
            {
                Debug.Log("Dictionary Modified: " + kvp.Key.name + " => " + kvp.Value);
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events to prevent memory leaks.
            _dictionaryReadOnly.Modified -= OnModified;
            _dictionaryReadOnly.OnItemAdded -= OnItemAdded;
            _dictionaryReadOnly.OnItemRemoved -= OnItemRemoved;
        }
    }
}