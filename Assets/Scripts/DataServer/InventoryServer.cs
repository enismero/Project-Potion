using UnityEngine;

public class InventoryServer : MonoBehaviour
{
    private InventorySlot[] allSlots;
    private static ItemData[] savedItems;
    private static int[] savedAmounts;
    private static bool hasSaved=false;

    void Awake()
    {
        allSlots= GetComponentsInChildren<InventorySlot>(true);

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

    void OnDisable()
    {
        if(allSlots==null || allSlots.Length==0) return;

        savedItems=new ItemData[allSlots.Length];
        savedAmounts=new int[allSlots.Length];

        for(int i = 0; i < allSlots.Length; i++)
        {
            savedItems[i]=allSlots[i].currentItem;
            savedAmounts[i]=allSlots[i].amount;
        }
        hasSaved=true;
    }
}
