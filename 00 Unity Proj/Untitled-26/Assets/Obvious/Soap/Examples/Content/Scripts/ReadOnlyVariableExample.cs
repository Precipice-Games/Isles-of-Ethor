using UnityEngine;

namespace Obvious.Soap.Example
{
    public class ReadOnlyVariableExample : MonoBehaviour
    {
        [SerializeField] private FloatVariableReadOnly _floatVariableReadOnly;

        private void Start()
        {
            _floatVariableReadOnly.OnValueChanged += OnValueChanged;

            //Adding to this variable is not possible because its read only.
            //_floatVariableReadOnly.Value = 50f; //This line would cause a compile error.
        }

        //Click on the button Damage or Heal in the Scene to make the value change and see this method being called.
        private void OnValueChanged(float newValue)
        {
            Debug.Log($"The new value of the Float Variable is: {newValue}");
        }

        private void OnDestroy()
        {
            //Always unsubscribe from events when destroying objects to avoid memory leaks.
            _floatVariableReadOnly.OnValueChanged -= OnValueChanged;
        }
    }
}