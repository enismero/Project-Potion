using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class InventorySlot : MonoBehaviour
{
    [Header("Slot data")]
    public ItemData currentItem;
    public int amount;

    [Header("Interface references")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    public void UpdateSlot(ItemData newItem, int newAmount)
    {
        currentItem=newItem;
        amount=newAmount;

        if(currentItem!=null && amount > 0)
        {
            iconImage.sprite=currentItem.itemIcon;
            iconImage.enabled=true;

            amountText.text=amount>1 ? "x" + amount.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem=null;
        amount=0;
        iconImage.enabled=false;
        amountText.text="";
    }
}
