using UnityEngine;
using UnityEngine.Events;

namespace Obvious.Soap
{
    [AddComponentMenu("Soap/EventListeners/EventListenerComponent")]
    public class EventListenerComponent : EventListenerGeneric<Component>
    {
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<Component>[] EventResponses => _eventResponses;

        [System.Serializable]
        public class EventResponse : EventResponse<Component>
        {
            [SerializeField] private ScriptableEventComponent _scriptableEvent = null;
            public override ScriptableEvent<Component> ScriptableEvent => _scriptableEvent;

            [SerializeField] private ComponentUnityEvent _response = null;
            public override UnityEvent<Component> Response => _response;
        }

        [System.Serializable]
        public class ComponentUnityEvent : UnityEvent<Component>
        {
        }
    }
}