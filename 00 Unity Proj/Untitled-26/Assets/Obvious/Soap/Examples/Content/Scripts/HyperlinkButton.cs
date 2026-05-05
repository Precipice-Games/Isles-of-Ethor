using UnityEngine;
using UnityEngine.UI;

namespace Obvious.Soap.Example
{
    [RequireComponent(typeof(Button))]
    public class HyperlinkButton : CacheComponent<Button>
    {
        [SerializeField] private string _url = string.Empty;

        protected override void Awake()
        {
            base.Awake();
            _component.onClick.AddListener(() => { Application.OpenURL(_url); });
        }
    }
}