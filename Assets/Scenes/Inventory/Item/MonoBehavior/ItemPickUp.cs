using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//该代码是挂载在物品预制体上的，负责当玩家碰撞到物品时将物品添加到背包并销毁物品预制体
public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //将物品添加到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData,itemData.itemAmount);
            InventoryManager.Instance.inventoryUI.RefreshUI();   //刷新背包UI显示
            //装备武器
            //GameManager.Instance.playerStats.EquipWeapon(itemData);

            //检查是否获得任务对象
            QuestManager.Instance.UpdateQuestProgress(itemData.itemName, itemData.itemAmount);  // 参数是物品名称和数量

            Destroy(gameObject);
        }
    }
}
