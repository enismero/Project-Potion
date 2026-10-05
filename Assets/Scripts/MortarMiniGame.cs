using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MortarMiniGame : MonoBehaviour
{
    [UnitHeaderInspectable("referances")]
    public InventorySlot mortarSlot;
    public Image progressBar; //UI

    [Header("settings")]
    public float requiredRotation=4f; //kaç tur dönmesi gerek

    private float totalRotation=0f;
    private float lastAngle = 0f;
    private bool isGrinding = false;

    void Update()
    {
        //havanda eşya var mı uygun mu
        if (mortarSlot.currentItem != null && mortarSlot.currentItem.itemType == ItemType.Plant && mortarSlot.currentItem.plantForm == PlantForm.Normal && mortarSlot.currentItem.groundVersion != null)
        {
            //sağ tıkla (1) ezmeyi başlat
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                isGrinding=true;
                lastAngle=GetMouseAngle();
            }

            //sağ tıka basılı açıyı takip et
            if(Mouse.current.rightButton.isPressed && isGrinding)
            {
                float currentAngle=GetMouseAngle();
                float delta =Mathf.DeltaAngle(lastAngle,currentAngle); //açı farkını bul
                totalRotation+=Mathf.Abs(delta); //mutlak
                lastAngle=currentAngle;

                //arayüzdeki yuvarlak barı doldur
                float targetRotation=requiredRotation*360f;
                if (progressBar != null)
                {
                    progressBar.fillAmount=totalRotation/targetRotation;
                    progressBar.enabled=true;
                }

                //istenen tur sayısına ulaşınca eşyayı dönüştür
                if (totalRotation >= targetRotation)
                {
                    ItemData groundItem= mortarSlot.currentItem.groundVersion;
                    int currentAmount=mortarSlot.amount;

                    mortarSlot.UpdateSlot(groundItem,currentAmount);
                    ResetGrind(); //bar ve dönüşü sıfırla
                }
            }
            //sağ tık bırakılırsa ezme dur
            else if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                isGrinding=false;
            }
        }
        else
        {
            ResetGrind(); //havanda doğru item yoksa barı kapa
        }
    }


    private float GetMouseAngle()
    {
        Vector2 screenPos=Camera.main.WorldToScreenPoint(transform.position); //havanın pozisyonu
        Vector2 mousePos = Mouse.current.position.ReadValue(); 
        Vector2 dir = mousePos - screenPos; //mousedan havana vektör at
        return Mathf.Atan2(dir.y,dir.x)*Mathf.Rad2Deg; //vektörü dereceye çevir
    }

    private void ResetGrind()
    {
        isGrinding=false;
        totalRotation=0f;

        if (progressBar != null)
        {
            progressBar.fillAmount=0f;
            progressBar.enabled=false;
        }
    }
}
