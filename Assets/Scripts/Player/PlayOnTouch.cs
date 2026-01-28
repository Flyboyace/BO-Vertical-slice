using UnityEngine;

public class PlayOnTouch : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit: " + other.name);

        if (gameObject.CompareTag("TriggerAnim") && other.CompareTag("Player"))
        {
            anim.SetTrigger("Play");
        }
    }
}
