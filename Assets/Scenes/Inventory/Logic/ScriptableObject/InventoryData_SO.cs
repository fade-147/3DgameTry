using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Inventory",menuName ="Inventory/Inventory Data")]
public class InventoryData_SO : ScriptableObject
{
    //该代码是背包数据的脚本ableObject，负责存储背包内物品的数据
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(ItemData_SO newItemData, int amount)
    {
        bool found =false;

        if (newItemData.Stackable)   //如果物品能够堆叠 且背包内已有相同物品就堆叠
        {
            foreach(var item in items)
            {
                if(item.itemData== newItemData)
                {
                    item.amount += amount;
                    found = true;
                    break;
                }
            }
        }
        //物品不能堆叠或者背包内没有该物体,找到空格并放进去
        for(int i=0;i<items.Count; i++)
        {
            if(items[i].itemData== null && !found)
            {
                items[i].itemData = newItemData;
                items[i].amount = amount;
                break;
            }
        }
    }
}

[System.Serializable]
public class InventoryItem
{
    public ItemData_SO itemData;
    public int amount;
}
