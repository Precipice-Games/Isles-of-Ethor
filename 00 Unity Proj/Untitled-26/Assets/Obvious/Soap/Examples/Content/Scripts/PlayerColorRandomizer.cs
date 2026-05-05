using UnityEngine;

namespace Obvious.Soap.Example
{
    public class PlayerColorRandomizer : MonoBehaviour
    {
        [SerializeField] private ColorVariable _colorVariable;

        private void Start()
        {
            if (SoapGameParams.Instance.RandomPlayerColorMode)
            {
                _colorVariable.SetRandom();
            }
        }
    }
}