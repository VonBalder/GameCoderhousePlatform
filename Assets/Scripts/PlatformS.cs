using UnityEngine;

public class PlatformS : MonoBehaviour
{
    [SerializeField] GameObject player;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        player.transform.parent = transform;
    }
    private void OnTriggerExit(Collider other)
    {
        player.transform.parent = null;
    }
}
