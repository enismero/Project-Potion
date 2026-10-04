using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using JetBrains.Annotations;

//slot kategorileri
public enum SlotCategory{Storage,Dryer,BottleStand,Pouch}

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Slot Type")]
    public SlotCategory slotCategory=SlotCategory.Storage;

    [Header("Slot data")]
    public ItemData currentItem;
    public int amount;

    [Header("Interface references")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    public static InventorySlot draggedSlot;
    private Transform originalParent;

    void Start()
{
    UpdateSlot(currentItem, amount);
}

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


    //koyulan eşya slot kategoriisinde mi testi
    public bool CanAcceptItem(ItemData item)
    {
        if(item==null) return true;

        switch (slotCategory)
        {
            case SlotCategory.Storage: //raf (şişe dışı herşey)
                return item.itemType!=ItemType.Bottle;
            case SlotCategory.Dryer: //kurutucu( taze bitki)
                return item.itemType== ItemType.Plant && item.plantState==PlantState.Fresh;
            case SlotCategory.BottleStand: //şişe standı (sadece şişe)
                return item.itemType==ItemType.Bottle;
            case SlotCategory.Pouch:
                return true;
            default:
                return false;
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;
        draggedSlot = this;
        originalParent = iconImage.transform.parent;

        // İkonun, diğer slotların altında kalıp görünmez olmasını engellemek için gecici olarak Canvas'a alıp en öne getiriyoruz
        Canvas canvas = GetComponentInParent<Canvas>();
        iconImage.transform.SetParent(canvas.transform);
        iconImage.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedSlot == this)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)iconImage.transform.parent, 
                eventData.position, 
                eventData.pressEventCamera, 
                out Vector3 globalMousePos))
                
            {
                iconImage.transform.position = globalMousePos;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedSlot == this)
        {
            // İkonu tekrar kendi slotunun içine al ve tam merkeze oturt
            iconImage.transform.SetParent(originalParent);
            iconImage.transform.localPosition = Vector3.zero; 
            draggedSlot = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Gelen bir eşya yoksa veya eşyayı kendi üstüne bıraktıysak hiçbir şey yapma
        if (draggedSlot == null || draggedSlot == this) return;

        //hedef slot eşyaya uygun mu
        if(!this.CanAcceptItem(draggedSlot.currentItem)) return;
        //takaslancaksa(swap) elindeki eski slot da eşyayı kabul ediyo mu (İKSİR STANDINDAN İKSİRLE KESEDEKİ BİTKİ DEĞİŞMESİN)
        if(this.currentItem!=null&& !draggedSlot.CanAcceptItem(this.currentItem)) return;

        //Kurallar geçildiyse taşıma stacke devam et

        // DURUM A: ÜST ÜSTE EKLEME (STACK)
        // Eğer gelen eşya ile bu slotun içindeki eşya aynı türdeyse
        if (this.currentItem == draggedSlot.currentItem)
        {
            int totalAmount = this.amount + draggedSlot.amount;
            int maxStack = this.currentItem.maxStack;

            if (totalAmount <= maxStack)
            {
                // Toplam miktar sınırı aşmıyorsa hepsini buraya al, eski slotu tamamen temizle
                this.UpdateSlot(this.currentItem, totalAmount);
                draggedSlot.ClearSlot();
            }
            else
            {
                // Miktar sınırı aşıyorsa, sığabildiği kadarını al, kalanı eski slotta bırak
                int leftover = totalAmount - maxStack;
                this.UpdateSlot(this.currentItem, maxStack);
                draggedSlot.UpdateSlot(draggedSlot.currentItem, leftover);
            }
        }
        // DURUM B: YER DEĞİŞTİRME (SWAP) VE BOŞ SLOTA KOYMA
        // Eşyalar farklıysa veya bu slot boşsa, ikisinin verilerini birbiriyle takas et
        else
        {
            ItemData tempItem = this.currentItem;
            int tempAmount = this.amount;

            // Kendi verimizi, gelen eşyanın verisiyle güncelliyoruz
            this.UpdateSlot(draggedSlot.currentItem, draggedSlot.amount);
            
            // Gelen eşyanın eski slotuna da, kendi verimizi (veya boşsak boşluğu) gönderiyoruz
            draggedSlot.UpdateSlot(tempItem, tempAmount);
        }
    }


}
