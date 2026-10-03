using UnityEngine;

public class ShowButtonsOnTrigger : MonoBehaviour
{
     [Header("Button")]
    public GameObject ButtonObject; 

    private void Start()
    {
        if (ButtonObject != null)
        {
            ButtonObject.SetActive(false);
        }
    }

    //collidera girdiğinde
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ButtonObject.SetActive(true);
        }
    }

    // colliderdan çık
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ButtonObject.SetActive(false);
        }
    }
}
