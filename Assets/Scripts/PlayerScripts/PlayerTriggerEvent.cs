using UnityEngine;
using UnityEngine.Events;

namespace PlayerScripts
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerTriggerEvent : MonoBehaviour
    {
        public UnityEvent onEnter;
        public UnityEvent onExit;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                onEnter.Invoke();
            }
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                onExit.Invoke();
            }
        }
    }
}
