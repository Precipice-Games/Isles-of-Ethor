using System;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/9_scriptabledictionaries")]
    [SelectionBase]
    [RequireComponent(typeof(Element))]
    public class AddRemoveElementToDictionary : MonoBehaviour
    {
        [SerializeField] private ScriptableDictionaryElementInt _scriptableDictionary = null;
        private Element _element;

        private void Start()
        {
            _element = GetComponent<Element>();

            //Try to add the first element of this type
            if (!_scriptableDictionary.TryAdd(_element.ElementType, 1))
            {
                //If its already in, just increment the count
                _scriptableDictionary[_element.ElementType]++;
            }
        }

        private void OnDestroy()
        {
            //Decrement the count of the element
            _scriptableDictionary[_element.ElementType]--;

            //If the count is 0, remove the element from the dictionary
            if (_scriptableDictionary[_element.ElementType] == 0)
            {
                _scriptableDictionary.Remove(_element.ElementType);
            }
        }
    }
}