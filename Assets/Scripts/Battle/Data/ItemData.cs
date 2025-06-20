using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]

public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public string itemDesc;
    public ItemCategory itemCategory;
    public int value;
    public int price;

}
