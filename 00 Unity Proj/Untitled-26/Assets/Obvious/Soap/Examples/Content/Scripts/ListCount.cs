using TMPro;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/3_scriptablelists")]
    [RequireComponent(typeof(TMP_Text))]
    public class ListCount : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text = null;
        [SerializeField] private ScriptableListPlayer scriptableListPlayer = null;

        void Awake()
        {
            scriptableListPlayer.Modified += UpdateText;
        }

        private void OnDestroy()
        {
            scriptableListPlayer.Modified -= UpdateText;
        }

        private void UpdateText()
        {
            _text.text = $"Count : {scriptableListPlayer.Count}";
        }
    }
}