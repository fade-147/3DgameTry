using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Useable,Weapon,Armor
}

//该代码是物品数据的脚本ableObject，负责存储物品的基本数据
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemAmount;   //记录物体堆叠的数量

    [TextArea]
    public string description = "";
    public bool Stackable;   //是否可堆叠

    [Header("Useable Item")]
    public UsableItemData_SO usableItemData;

    [Header("Weapon")]
    public GameObject weaponPrefab;
    public AttackData_SO weaponData;
    public AnimatorOverrideController weaponAnimator;
}
