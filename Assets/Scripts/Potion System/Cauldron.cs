using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class Cauldron : MonoBehaviour
{
    [Header("Recipes DataBase")]
    [Tooltip("All Recipes Data")]
    public List<RecipeData> allRecipes;

    [Header("kazanın şuanki hafızası")]
    public LiquidBase currentBase = LiquidBase.None; //hangi sıvı bulunuyo
    public List<RecipeStep> actionHistory = new List<RecipeStep>(); //step step ne yapılmış

    [Header("Damıtma Distill Slot")]
    public InventorySlot bottleSlot;
    //boş şişe olucak slot


    //butonla çağırabilmek için (addBase içinde parametreyi anlamadığı için böyle yaptık)
    public void TestAddWater() { AddBase(LiquidBase.Water); }
    public void TestAddAlcohol() { AddBase(LiquidBase.Alcohol); }
    public void TestAddOil() { AddBase(LiquidBase.Oil); }
    public void TestAddWine() { AddBase(LiquidBase.Wine); }

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
    public void AcceptItem(InventorySlot droppedSlot)
    {
        Debug.Log("kazanın üzerine bırakıldı");

        //kazanda sıvı var mı
        if (currentBase == LiquidBase.None)
        {
            Debug.LogWarning("Sıvı koymalısın");
            return;
        }

        ItemData droppedItem = droppedSlot.currentItem;

        //kazandaki son hamleyi kontrol et
        if (actionHistory.Count > 0)
        {
            RecipeStep lastStep =actionHistory[actionHistory.Count-1];
            //eğer item atıldıysa ve aynıysa
            if (lastStep.actionType == ActionType.AddItem && lastStep.requiredItem == droppedItem)
            {
                lastStep.amount++;
                Debug.Log("1 tane daha "+ droppedItem.itemName +"atıldı :"+lastStep.amount);
            }
            else
            {
                // araya yeni işlem giriyosa yeni step yap
                AddNewItemStep(droppedItem);
            }
        }
        else
        {
            //eğer atılan ilk eşyaysa
            AddNewItemStep(droppedItem);
        }

        //elden 1 tane al
        droppedSlot.amount--;

        if (droppedSlot.amount <= 0)
        {
            droppedSlot.ClearSlot();//hiç yoksa full boşalt
        }
        else
        {
            //dah kaldıysa güncelle
            droppedSlot.UpdateSlot(droppedSlot.currentItem,droppedSlot.amount);
        }
    }

    private void AddNewItemStep(ItemData item)
    {
        RecipeStep step = new RecipeStep();
        step.actionType = ActionType.AddItem;
        step.requiredItem = item;
        step.amount = 1; // Sadece 1 tane ekle

        actionHistory.Add(step);
        Debug.Log("1 tane " + item.itemName + " atıldı");
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
            Debug.Log("yapılan iksir"+matchedRecipe.recipeName);
            emptyBottleSlot.UpdateSlot(matchedRecipe.resultItem,1);
        }
        else
        {
            Debug.Log("iksir yapılamadı");
        }
        ResetCauldron();
    }

    //butonla damıtma fonksiyonu
    public void TryDistill()
    {
        if(bottleSlot==null || bottleSlot.currentItem == null)
        {
            Debug.LogWarning("damıtma slotu boş şişe yok");
            return;
        }
        RecipeData matchedRecipe=null;

        //yaptığımız hamleleri karşılaştır
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
            Debug.Log(matchedRecipe.resultItem.itemName  + " üretildi");
            bottleSlot.UpdateSlot(matchedRecipe.resultItem,1);
        }
        else
        {
            Debug.LogWarning("başarısız");
            bottleSlot.ClearSlot();
        }
        ResetCauldron();
    }

    //yapılanla tarifleri karşılaştırıcak kısım
    private bool CheckRecipeMatch(RecipeData recipe)
    {
        //step sayısı uuymuyosa baştan sil
        if(recipe.steps.Count!=actionHistory.Count) return false;

        for(int i=0; i< recipe.steps.Count; i++)
        {
            RecipeStep required = recipe.steps[i];
            RecipeStep playerAction = actionHistory[i];

            if(required.actionType != playerAction.actionType) return false;

            if(required.actionType == ActionType.AddBase && required.requiredBase != playerAction.requiredBase) return false;

            if (required.actionType == ActionType.AddItem)
            {
                if(required.requiredItem != playerAction.requiredItem || required.amount!=playerAction.amount) return false;
            }

            if(required.actionType==ActionType.Boil && required.boilCount!=playerAction.boilCount) return false;
        }
        return true;
    }

    public void ResetCauldron()
    {
        currentBase=LiquidBase.None;
        actionHistory.Clear();
        Debug.Log("kazan temizlendi");
    }
}
