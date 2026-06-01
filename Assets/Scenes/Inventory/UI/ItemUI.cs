using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//该代码是背包UI中每个物品格子的UI脚本，负责显示物品图标和数量
public class ItemUI : MonoBehaviour
{
    public Image icon = null;
    public Text amount = null;
    public ItemData_SO currentItemData;
    public InventoryData_SO Bag { get; set; }
    public int Index { get; set; }=-1;    //设置初始值为-1，表示这个格子没有物品，防止乱排序

    public void SetupItemUI(ItemData_SO item,int itemAmount)
    {
        if (itemAmount == 0)    //吃掉蘑菇后，如果数量为0了，就把这个格子清空
        {
            Bag.items[Index].itemData = null;  //如果数量为0了，就把这个格子清空
            icon.gameObject.SetActive(false);
            return;   //不用再执行下面的代码了
        }

        if (itemAmount < 0)
        {
            item = null;
        }

        if(item!= null)
        {
            currentItemData= item;   //获得当前物体的数据，这个主要是用在任务奖励的物品显示时需要的
            icon.sprite = item.itemIcon;
            amount.text = itemAmount.ToString();
            icon.gameObject.SetActive(true);
        }
        else
        {
            icon.gameObject.SetActive(false);
        }
    }

    public ItemData_SO GetItem()
    {
        return Bag.items[Index].itemData;      //可以直接拿到该UI在背包里对应的那个物体
    }
}
