using UnityEngine;

public class VehicleSaver : MonoBehaviour
{
    // Statik (Kalıcı) Hafıza
    private static Vector3 savedPos;
    private static Quaternion savedRot;
    private static bool hasSaved = false;

    void Awake()
    {
        // Daha önce kaydedilmiş bir konum varsa aracı oraya ışınla
        if (hasSaved)
        {
            transform.position = savedPos;
            transform.rotation = savedRot;

            Rigidbody rb=GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.position=savedPos;
                rb.rotation=savedRot;
            }
        }
    }

    void OnDestroy() // Başka sahneye geçerken aracın son konumunu otomatik kaydet
    {
        savedPos = transform.position;
        savedRot = transform.rotation;
        hasSaved = true;
    }
}
