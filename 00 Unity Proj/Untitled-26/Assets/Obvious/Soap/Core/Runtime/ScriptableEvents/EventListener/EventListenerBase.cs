using System.Threading;
using UnityEngine;

namespace Obvious.Soap
{
    /// <summary>
    /// Base class for all event listeners
    /// </summary>
    [HelpURL("https://obvious-game.gitbook.io/soap/soap-core-assets/event-listener")]
    public abstract class EventListenerBase : MonoBehaviour
    {
        protected enum Binding
        {
            UNTIL_DESTROY,
            UNTIL_DISABLE
        }

        [SerializeField] protected Binding _binding = Binding.UNTIL_DESTROY;
        [SerializeField] protected bool _disableAfterSubscribing = false;
        protected readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        protected abstract void ToggleRegistration(bool toggle);

        /// <summary>
        /// Returns true if the event listener contains a call to the method with the given name
        /// </summary>
        public abstract bool ContainsCallToMethod(string methodName);

        protected virtual void Awake()
        {
            if (_binding == Binding.UNTIL_DESTROY)
                ToggleRegistration(true);

            gameObject.SetActive(!_disableAfterSubscribing);
        }

        protected virtual void OnEnable()
        {
            if (_binding == Binding.UNTIL_DISABLE)
                ToggleRegistration(true);
        }

        protected virtual void OnDisable()
        {
            if (_binding == Binding.UNTIL_DISABLE)
            {
                ToggleRegistration(false);
                _cancellationTokenSource.Cancel();
            }
        }

        protected virtual void OnDestroy()
        {
            if (_binding == Binding.UNTIL_DESTROY)
            {
                ToggleRegistration(false);
                _cancellationTokenSource.Cancel();
            }
        }
    }
}