using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    [SerializeField] private UnityEvent myTrigger;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player")){
            Debug.Log("Event");
            myTrigger.Invoke();      
        }
    }
}
