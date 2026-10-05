using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using JetBrains.Annotations;
using UnityEditor.MPE;

//slot kategorileri
public enum SlotCategory{Storage,Dryer,BottleStand,Pouch,Mortar}

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

    public Image progressBar; //dryer progress bar

    [Header("Dryer Settings")]
    public float timeToProcess = 5f;
    private float processTimer = 0f;

    public static InventorySlot draggedSlot;
    private Transform originalParent;

    void Start()
    {
        UpdateSlot(currentItem, amount);
    }

    void Update()
    {   //Dryer
        if(slotCategory==SlotCategory.Dryer && currentItem != null)
        {
           if(currentItem.itemType==ItemType.Plant && currentItem.plantState==PlantState.Fresh && currentItem.diredVersion != null)
            {
                processTimer+=Time.deltaTime;

                if (progressBar != null)
                {
                    progressBar.fillAmount=processTimer/timeToProcess;
                    progressBar.enabled=true;
                }

                if (processTimer >= timeToProcess)
                {
                    ItemData newDriedItem = currentItem.diredVersion;
                    int currentAmount=amount;

                    UpdateSlot(newDriedItem,currentAmount);
                }
            } 
        }
    }

    public void UpdateSlot(ItemData newItem, int newAmount)
    {
        processTimer=0f;
        //eşya değiştiğinde barı sıfırla
        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
            progressBar.enabled = false;
        }
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
        processTimer=0f;
        
        // Eşya geri alındığında barı sıfırla ve gizle
        if (progressBar != null)
        {
            progressBar.fillAmount = 0f;
            progressBar.enabled = false;
        }

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
            case SlotCategory.Mortar: //sadece bitki kuru ve nor
                return item.itemType==ItemType.Plant&&item.plantForm == PlantForm.Normal;
            default:
                return false;
        }
    }

    public int GetMaxCapacity(ItemData item)
    {
        if(slotCategory== SlotCategory.Dryer) return 1;
        if (slotCategory== SlotCategory.BottleStand) return 1;
        if(slotCategory==SlotCategory.Mortar) return 1;

        if(item!=null) return item.maxStack;
        return 64;
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

        //tür güvenlik
        //hedef slot eşyaya uygun mu
        if(!this.CanAcceptItem(draggedSlot.currentItem)) return;

        //slot sınırı öğren 
        int thisMaxCapacity=this.GetMaxCapacity(draggedSlot.currentItem);
        int draggedMaxCapacity= draggedSlot.GetMaxCapacity(this.currentItem);

        //Kurallar geçildiyse taşıma stacke devam et

        // DURUM A: ÜST ÜSTE EKLEME (STACK)
        // Eğer gelen eşya ile bu slotun içindeki eşya aynı türdeyse
        if (this.currentItem == draggedSlot.currentItem)
        {
            int totalAmount = this.amount + draggedSlot.amount;
            

            if (totalAmount <= thisMaxCapacity)
            {
                // Toplam miktar sınırı aşmıyorsa hepsini buraya al, eski slotu tamamen temizle
                this.UpdateSlot(this.currentItem, totalAmount);
                draggedSlot.ClearSlot();
            }
            else
            {
                // Miktar sınırı aşıyorsa, sığabildiği kadarını al, kalanı eski slotta bırak
                int leftover = totalAmount - thisMaxCapacity;
                if (leftover == draggedSlot.amount) return; // Hedef zaten tam doluysa işlem yapma
                
                this.UpdateSlot(this.currentItem, thisMaxCapacity);
                draggedSlot.UpdateSlot(draggedSlot.currentItem, leftover);
            }
        }
        // DURUM B: YER DEĞİŞTİRME (SWAP) VE BOŞ SLOTA KOYMA
        // Eşyalar farklıysa veya bu slot boşsa, ikisinin verilerini birbiriyle takas et
        else
        {
            if(this.currentItem!=null && !draggedSlot.CanAcceptItem(this.currentItem)) return;
            
            if(this.currentItem==null && draggedSlot.amount > thisMaxCapacity)
            {
                this.UpdateSlot(draggedSlot.currentItem, thisMaxCapacity); //sadece 1 al
                draggedSlot.UpdateSlot(draggedSlot.currentItem,draggedSlot.amount-thisMaxCapacity); //kalanı yerine gönder
            }
            else if(this.currentItem!=null&&(draggedSlot.amount>thisMaxCapacity || this.amount > draggedMaxCapacity))
            {
                return;
            }
            else
            {
                ItemData tempItem = this.currentItem;
                int tempAmount = this.amount;

                this.UpdateSlot(draggedSlot.currentItem,draggedSlot.amount);
                // Gelen eşyanın eski slotuna da, kendi verimizi (veya boşsak boşluğu) gönderiyoruz
                draggedSlot.UpdateSlot(tempItem, tempAmount);
            }
              
        }
    }
}
