using UnityEngine;
using UnityEngine.Events;

public class OnDead : MonoBehaviour
{
    public UnityEvent onDead;

    private void OnDestroy()
    {
        onDead.Invoke();
    }
}
