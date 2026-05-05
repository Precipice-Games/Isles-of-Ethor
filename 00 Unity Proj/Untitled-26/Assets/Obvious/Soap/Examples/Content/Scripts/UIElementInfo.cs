using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/9_scriptabledictionaries")]
    public class UIElementInfo : MonoBehaviour
    {
        [SerializeField] private ScriptableEnumElement _scriptableEnumElement = null;
        [SerializeField] private ScriptableDictionaryElementInt _scriptableDictionary = null;
        [SerializeField] private TextMeshProUGUI _text = null;
        [SerializeField] private Image _image = null;

        private void Awake()
        {
            _scriptableDictionary.OnItemAdded += OnItemAdded;
            _scriptableDictionary.OnItemRemoved += OnItemRemoved;
            _scriptableDictionary.Modified += OnModified;
            _text.transform.parent.gameObject.SetActive(false);
            _image.sprite = _scriptableEnumElement.Icon;
        }

        private void OnDestroy()
        {
            _scriptableDictionary.OnItemAdded -= OnItemAdded;
            _scriptableDictionary.OnItemRemoved -= OnItemRemoved;
            _scriptableDictionary.Modified -= OnModified;
        }
        
        private void OnItemAdded(ScriptableEnumElement element, int value)
        {
            if (element != _scriptableEnumElement)
                return;
            _text.transform.parent.gameObject.SetActive(true);
        }

        private void OnItemRemoved(ScriptableEnumElement element, int value)
        {
            if (element != _scriptableEnumElement)
                return;
            _text.transform.parent.gameObject.SetActive(false);
        }

        private void OnModified()
        {
            if (_scriptableDictionary.TryGetValue(_scriptableEnumElement, out var count))
                _text.text = $"Count: {count}";
        }
    }
}