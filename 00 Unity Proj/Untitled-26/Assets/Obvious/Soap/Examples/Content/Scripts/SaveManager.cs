using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/5_scriptablesaves/save-manager")]
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private ScriptableSaveExample _scriptableSaveExample;
        [SerializeField] private IntVariable _levelVariable;

        private void Awake()
        {
            _scriptableSaveExample.OnLoaded += SetLevelValue;
            _scriptableSaveExample.OnSaved += SetLevelValue;
        }

        //Load on start as other classes might register to OnLoaded in their Awake
        private void Start()
        {
            if (_scriptableSaveExample.LoadMode == ScriptableSaveBase.ELoadMode.Manual)
                _scriptableSaveExample.Load();
            else
                SetLevelValue();
        }

        private void OnDestroy()
        {
            _scriptableSaveExample.OnLoaded -= SetLevelValue;
            _scriptableSaveExample.OnSaved -= SetLevelValue;
        }

        private void SetLevelValue()
        {
            _levelVariable.Value = _scriptableSaveExample.Level;
        }
    }
}