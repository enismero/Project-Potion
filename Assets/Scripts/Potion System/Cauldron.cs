using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Cauldron : MonoBehaviour
{
    [Header("Recipes DataBase")]
    [Tooltip("All Recipes Data")]
    public List<RecipeData> allRecipes;

    [Header("kazanın şuanki hafızası")]
    public LiquidBase currentBase = LiquidBase.None; //hangi sıvı bulunuyo
    public List<RecipeStep> actionHistory = new List<RecipeStep>(); //step step ne yapılmış

    //ADD BASE LIQUID in cauldron
    //ilerde sıvı ekleyle gelicek
    public void AddBase(LiquidBase newBase)
    {
        if (currentBase != LiquidBase.None)
        {
            Debug.LogWarning("kazanda zaten sıvı var");
            return;
        }

        currentBase=newBase;
        //eklenen sıvıyı kaydet
        RecipeStep step=new RecipeStep();
        step.actionType= ActionType.AddBase;
        step.requiredBase=newBase;

        actionHistory.Add(step);
        Debug.Log(newBase.ToString() + " eklendi.");
    }

    //ADD ITEM
    public void OnDrop(PointerEventData eventData)
    {
        if(InventorySlot.draggedSlot==null) return;
        //kazanda sıvı var mı
        if (currentBase == LiquidBase.None)
        {
            Debug.LogWarning("Sıvı koymalısın");
            return;
        }

        ItemData droppedItem = InventorySlot.draggedSlot.currentItem;
        int droppedAmount=InventorySlot.draggedSlot.amount;

        //ekleneni geçmişe kaydet
        RecipeStep step = new RecipeStep();
        step.actionType=ActionType.AddItem;
        step.requiredItem=droppedItem;
        step.amount=droppedAmount;

        actionHistory.Add(step);
        Debug.Log(droppedAmount + " " + droppedItem.itemName + " atıldı.");

        //atılanı sil
        InventorySlot.draggedSlot.ClearSlot();
        
    }

    //BOİLİNG
    //ilerde kaynatmayı çeğırıcaz
    public void Boil()
    {
        if(currentBase==LiquidBase.None) return;
        //önceki hamle kaynatmaksa yeni hamle oluşturma sayıyı arttır
        if(actionHistory.Count>0 && actionHistory[actionHistory.Count - 1].actionType == ActionType.Boil)
        {
            actionHistory[actionHistory.Count-1].boilCount++;
            Debug.Log("Art arda kaynatıldı toplam"+actionHistory[actionHistory.Count-1].boilCount);
        }
        else
        {
            RecipeStep step=new RecipeStep();
            step.actionType=ActionType.Boil;
            step.boilCount=1;
            actionHistory.Add(step);
            Debug.Log("1 defa kaynatıldı");

        }
    }

    //DISTILL (DAMITMA)
    public void Distill(InventorySlot emptyBottleSlot)
    {
        RecipeData matchedRecipe=null;
        //yapılan stepleri database deki tarif stepleriyle karşılaştır
        foreach(RecipeData recipe in allRecipes)
        {
            if (CheckRecipeMatch(recipe))
            {
                matchedRecipe=recipe;
                break; //tarif bulundu
            }
        }

        if (matchedRecipe != null)
        {
            Debug.Log("yapılan iksir"+matchedRecipe.recepieName);
        }
    }
}
