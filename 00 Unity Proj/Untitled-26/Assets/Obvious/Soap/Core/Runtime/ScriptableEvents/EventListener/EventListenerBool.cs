using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Obvious.Soap
{
    /// <summary>
    /// A listener for a ScriptableEventBool
    /// </summary>
    [AddComponentMenu("Soap/EventListeners/EventListenerBool")]
    public class EventListenerBool : EventListenerGeneric<bool>
    {
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<bool>[] EventResponses => _eventResponses;

        [System.Serializable]
        public class EventResponse : EventResponse<bool>
        {
            [SerializeField] private ScriptableEventBool _scriptableEvent = null;
            public override ScriptableEvent<bool> ScriptableEvent => _scriptableEvent;

            [SerializeField] private BoolUnityEvent _response = null;
            public override UnityEvent<bool> Response => _response;
        }

        [System.Serializable]
        public class BoolUnityEvent : UnityEvent<bool>
        {
        }
    }
}