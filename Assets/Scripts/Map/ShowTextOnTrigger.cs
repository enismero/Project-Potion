using UnityEngine;

public class ShowTextOnTrigger : MonoBehaviour
{
    [Header("Text")]
    public GameObject textObject; 

    private void Start()
    {
        if (textObject != null)
        {
            textObject.SetActive(false);
        }
    }

    //collidera girdiğinde
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textObject.SetActive(true);
        }
    }

    // colliderdan çık
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textObject.SetActive(false);
        }
    }
}