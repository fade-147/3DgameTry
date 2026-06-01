using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType {BAG,WEAPON,ARMOR,ACTION }    //用于区分这个格子是什么类型的
public class SlotHolder : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler, IPointerExitHandler  //第一个接口可以记录鼠标的点击以及点击次数。用于检测鼠标点击事件，双击使用物品。。后两个接口可以记录鼠标进入（悬停在格子上）和离开格子的事件。用于显示和隐藏物品信息的tooltip
{
    public SlotType slotType;
    public ItemUI itemUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount % 2 == 0)   //双击时使用物品
        {
            UseItem();
        }
    }

    public void UseItem()
    {
        if(itemUI.GetItem()!=null)
        if(itemUI.Bag.items[itemUI.Index].itemData.itemType==ItemType.Useable && itemUI.Bag.items[itemUI.Index].amount>0)  //如果这个格子里有物品,并且是Useable类型的物品，就使用它
        {
            GameManager.Instance.playerStats.ApplyHealth(itemUI.Bag.items[itemUI.Index].itemData.usableItemData.healthPoint);  //使用物品，增加玩家血量，把这个物品的可用物品数据中的血量值传给玩家属性系统，更新玩家属性
            itemUI.Bag.items[itemUI.Index].amount--;  //使用后数量-1
                QuestManager.Instance.UpdateQuestProgress(itemUI.GetItem().itemName, -1);  //使用物品后，更新任务进度，把这个物品的名字和数量-1传给任务系统，更新任务进度
            }
        UpdateItem();  //使用后更新UI显示
    }

    public void UpdateItem()
    {
        switch (slotType)
        {
            case SlotType.BAG:
                itemUI.Bag=InventoryManager.Instance.inventoryData;  //如果格子类型是背包格子，就把背包数据传给它
                break;
            case SlotType.WEAPON:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                //装备武器，切换武器
                if(itemUI.Bag.items[itemUI.Index].itemData != null)  //如果这个格子里有武器
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.Bag.items[itemUI.Index].itemData);  //切换武器,把这个武器的数据传给玩家属性系统，更新玩家属性
                }
                else
                {
                    GameManager.Instance.playerStats.UnEquipWeapon();
                }
                    break;
            case SlotType.ARMOR:
                itemUI.Bag = InventoryManager.Instance.equipmentData;
                break;
            case SlotType.ACTION:
                itemUI.Bag = InventoryManager.Instance.actionData;
                break;
        }

        var item = itemUI.Bag.items[itemUI.Index];  //拿到背包，在那个背包数据库（列表）中，根据格子类型和格子索引获取物品数据
        itemUI.SetupItemUI(item.itemData,item.amount);  //把物品数据传给UI，更新UI显示
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemUI.GetItem()!=null)  //如果这个格子里有物品，就显示tooltip
        {
            InventoryManager.Instance.tooltip.SetupTooltip(itemUI.GetItem());  //把这个物品的数据传给tooltip，更新tooltip显示
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);  //显示tooltip
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);   //鼠标离开格子时，隐藏tooltip
    }

    void OnDisable()
    {
        //当背包被关闭时，也要把tooltip隐藏，避免关闭背包后鼠标还在格子上，tooltip还在显示的情况
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);   //鼠标离开格子时，隐藏tooltip
    }
}
