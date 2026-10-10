using UnityEngine;
using System.Collections.Generic;

//sıvı bazlar
public enum LiquidBase {None,Water,Alcohol,Oil,Wine}
//yapılabilicek hamleler
public enum ActionType {AddBase,AddItem,Boil}

//Tarifin Uygulanması
[System.Serializable]
public class RecipeStep
{
    public ActionType actionType;

    [Header("Eğer baz ekleniyosa (AddBase)")]
    public LiquidBase requiredBase;

    [Header("eğer eşya atılıyosa (AddItem)")]
    public ItemData requiredItem;
    public int amount=1;

    [Header("eğer kaynatılıyosa (Boil)")]
    public int boilCount=1;

}

//Tarifin kendisi(Scriptable object)
[CreateAssetMenu(fileName = "New Recipe", menuName = "Alchemy/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Tarif sonucu")]
    public string recepieName;
    public ItemData resultItem;

    [Header("Sırasıyla yapılışı")]
    public List<RecipeStep> steps=new List<RecipeStep>();
} 
