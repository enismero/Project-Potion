using UnityEngine;
using System.Collections.Generic;

public class InventorySaver : MonoBehaviour
{
    public static InventorySaver instance;

    [Header("İçinde Slot Olan Paneller/Canvaslar")]
    public GameObject[] slotContainers;

    private InventorySlot[] allSlots;
    private static ItemData[] savedItems;
    private static int[] savedAmounts;
    private static bool hasSaved=false;

    void Awake()
    {
        instance=this;

        // Verdiğimiz tüm container'lardaki (Canvas'lardaki) slotları tek bir listede sırayla topluyoruz
        List<InventorySlot> tempList = new List<InventorySlot>();
        
        if (slotContainers != null)
        {
            foreach (GameObject container in slotContainers)
            {
                if (container != null)
                {
                    tempList.AddRange(container.GetComponentsInChildren<InventorySlot>(true));
                }
            }
        }

        allSlots= tempList.ToArray();

        if (hasSaved && savedItems != null)
        {
            for(int i = 0; i < allSlots.Length; i++)
            {
                if (i < savedItems.Length)
                {
                    allSlots[i].UpdateSlot(savedItems[i],savedAmounts[i]);
                }
            }
        }

    }

    public void Kaydet()
    {
        if(allSlots==null || allSlots.Length==0) return;

        savedItems=new ItemData[allSlots.Length];
        savedAmounts=new int[allSlots.Length];

        for(int i = 0; i < allSlots.Length; i++)
        {
            if (allSlots[i] != null) 
            {
                savedItems[i] = allSlots[i].currentItem;
                savedAmounts[i] = allSlots[i].amount;
            }
        }
        hasSaved=true;
    }

    //haritada eşya bulunca çağır
    public static void AddItemToSaveData(ItemData newItem, int addAmount)
    {
        if(!hasSaved || savedItems == null)
        {
            Debug.LogWarning("kayıt yok");
            return;
        }
        //envanterde zaten var
        for (int i=0; i < savedItems.Length; i++)
        {
            if(savedItems[i] !=null && savedItems[i] == newItem)
            {
                savedAmounts[i]+=addAmount;
                Debug.Log(newItem.itemName+"vardı "+ addAmount+" eklendi");
                return;
            }
        }
        //aynısından yok boşluğa koy
        for(int i = 0; i < savedItems.Length; i++)
        {
            if (savedItems[i] == null)
            {
                savedItems[i]=newItem;
                savedAmounts[i]=addAmount;
                Debug.Log(newItem.itemName +" "+ addAmount + " boşluğa koyuldu");
                return;
            }
        }
        //hiç boş yer yok
        Debug.LogWarning("envanter dolu");
    }
}
