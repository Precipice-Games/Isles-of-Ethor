using TMPro;
using UnityEngine;

namespace Obvious.Soap
{
    /// <summary>
    /// Binds a string variable to an input field
    /// </summary>
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/2_bindings/bindinputfield")]
    [AddComponentMenu("Soap/Bindings/BindInputField")]
    [RequireComponent(typeof(TMP_InputField))]
    public class BindInputField : CacheComponent<TMP_InputField>
    {
        [SerializeField] private StringVariable _stringVariable = null;

        protected override void Awake()
        {
            base.Awake();
            _component.onValueChanged.AddListener(SetBoundVariable);
        }

        private void Start()
        {
            //Do it in start, as the variable has to be loaded first.
            _component.text = _stringVariable; 
        }

        private void OnDestroy() => _component.onValueChanged.RemoveListener(SetBoundVariable);

        private void SetBoundVariable(string value) => _stringVariable.Value = value;
        
        
    }
}