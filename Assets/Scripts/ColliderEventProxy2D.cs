using UnityEngine;

namespace Assets.Scripts
{
    public class ColliderEventProxy2D : MonoBehaviour
    {
        public UnityEngine.Events.UnityEvent onEnterEvents;
        public UnityEngine.Events.UnityEvent onStayEvents;
        public UnityEngine.Events.UnityEvent onExitEvents;

        private void OnTriggerEnter2D(Collider2D other)
        {
            onEnterEvents.Invoke();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            onStayEvents.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            onExitEvents.Invoke();
        }
    }
}