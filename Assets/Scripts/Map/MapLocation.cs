using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Toolbars;

[System.Serializable] //İNSPECTORDE HER EŞYA İÇİN AYAR KUTULARI EKLEMEK İÇİN
public class LootDrop
{
    public ItemData item;

    [Header("düşme ihtimali")]
    [Range(0f,100f)]
    public float dropChance;

    [Header("kaç adet çıkabilir")]
    public int minAmunt=1;
    public int maxAmunt=3;
}

public class MapLocation : MonoBehaviour
{
    [Header("Bölge ayarları")]
    public int minutesToPass=60;
    [Header("çıkabilicek eşyalar")]
    public List<LootDrop> possibleLoot;

    public void ExploreArea()
    {

        //zaman geçirme
        if (GameManager.instance != null)
        {
            GameManager.instance.PassTime(minutesToPass);
        }

        bool foundAnything = false;
        Debug.Log("--Araştırılıyor--");

        //her eşya için olasılık hesapla
        foreach(LootDrop drop in possibleLoot)
        {
            float randomDice= Random.Range(0f,100f);
            if(randomDice<= drop.dropChance)
            {
                int amountToGive = Random.Range(drop.minAmunt, drop.maxAmunt+1);

                Debug.Log(amountToGive+" adet "+drop.item.itemName+ " Şans %" + drop.dropChance);
                foundAnything=true;

                //envantere ekleme kısmı 
                
            }
        }

        if (!foundAnything)
        {
            Debug.Log("bi bok bulmadın");
        }
    }
}
