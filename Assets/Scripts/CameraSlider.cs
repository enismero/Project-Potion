using System.Security.Cryptography;
using UnityEngine;

public class CameraSlider : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isMoving=false;

    [Header("Settings")]
    public float smoothTime=0.03f;
    private Vector3 velocity=Vector3.zero;

    void Start()
    {
        targetPosition=transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position=Vector3.SmoothDamp(transform.position,targetPosition, ref velocity,smoothTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position=targetPosition;
                isMoving=false;
            }
        }
    }

    public void GoToRoom(Transform roomTarget)
    {
        targetPosition=new Vector3(roomTarget.position.x, roomTarget.position.y, transform.position.z);
        isMoving=true;
    }
}
