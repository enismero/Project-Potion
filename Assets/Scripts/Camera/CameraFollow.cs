using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("takip ayarları")]
    public Transform target;
    public Vector3 offset;

    [Range(1f, 20f)]
    public float followSpeed = 10f;

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition=target.position+offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.fixedDeltaTime);
        }
    }
}
