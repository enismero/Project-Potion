using UnityEngine;

//ana ayrım
public enum ItemType {Default,Plant,Bottle}
//şişe
public enum BottleType{None,Empty,Potion}
//bitki
public enum PlantState{None,Fresh,Dried} //taze kuru
public enum PlantForm{None,Normal,Ground} //normal öğürülmüş

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("temel bilgi")]
    public string itemName;
    public Sprite itemIcon;
    public int maxStack=64;

    [Header("eşya sınıfı")]
    public ItemType itemType=ItemType.Default;

    [Header("şişe ise")]
    public BottleType bottleType = BottleType.None;

    [Header("Bitki İse")]
    public PlantState plantState = PlantState.None;
    public PlantForm plantForm = PlantForm.None;

}
