using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerUI : MonoBehaviour
{
    public SlotHolder[] slotHolders;  //拿到容器UI中所有的格子

    public void RefreshUI()
    {
        for (int i = 0;i < slotHolders.Length; i++)   //遍历所有格子，更新每个格子的UI显示
        {
            slotHolders[i].itemUI.Index = i;   //根据SlotHolder[]中每个格子的序号告诉格子本身
            slotHolders[i].UpdateItem();      //让格子根据自己的序号同步背包数据库中对应序号的物品数据，并更新UI显示
        }
    }
}
