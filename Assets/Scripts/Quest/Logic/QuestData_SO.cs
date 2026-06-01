using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData_SO : ScriptableObject
{
    [System.Serializable]
    public class QuestRequire
    {
        public string name;  //需求名称
        public int requireAmount;   //需求数量
        public int currentAmount;   //当前数量
    }

    public string questName;  //任务名称

    [TextArea]
    public string description;  //任务描述

    //任务的状态
    public bool isStarted;
    public bool isComplete;
    public bool isFinished;

    public List<QuestRequire> requires = new List<QuestRequire>();  //任务需求列表
    public List<InventoryItem> rewards = new List<InventoryItem>();    //奖励列表

    public void CheckQuestProgress()
    {
        //如果现在的数量大于等于目标数量，说明是完成任务了
        var finishRequires=requires.Where(r => r.currentAmount >= r.requireAmount);  //检查任务需求是否完成，这里通过LINQ的Where方法来筛选出满足条件的需求

        //比如需要3个蘑菇，2个木头，如果现在的数量是3个蘑菇，1个木头，那么满足条件的需求数量就是1个（蘑菇），如果现在的数量是4个蘑菇，3个木头，那么满足条件的需求数量也是2个（蘑菇和木头）
        isComplete = finishRequires.Count() == requires.Count;  //如果满足条件的需求数量等于总需求数量，说明任务完成了。
    }

    public void GiveRewards()
    {
        foreach(var reward in rewards)
        {
            if(reward.amount < 0)
            {
                //奖品的数量小于0，说明是扣除物品了，那么就把这个物品从玩家的背包中扣除掉
                int requireAmount = Mathf.Abs(reward.amount);  //获取需要扣除的数量(绝对值)
                if (InventoryManager.Instance.QuestItemInBag(reward.itemData) != null)
                {
                    if (InventoryManager.Instance.QuestItemInBag(reward.itemData).amount <= requireAmount)
                    {
                        //背包中的不够了，剩下的应该在快捷栏中
                        requireAmount -= InventoryManager.Instance.QuestItemInBag(reward.itemData).amount;
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount = 0;

                        if(InventoryManager.Instance.QuestItemInAction(reward.itemData) != null)
                        {
                            InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireAmount;
                        }
                    }
                    else
                    {
                        InventoryManager.Instance.QuestItemInBag(reward.itemData).amount -= requireAmount;
                    }
                }
                else
                {
                    //如果背包中没有这个物品，说明全在快捷栏
                    InventoryManager.Instance.QuestItemInAction(reward.itemData).amount -= requireAmount;  //从快捷栏扣除物品
                }
            }
            else
            {
                //奖品的数量大于0，说明是奖励物品了，那么就把这个物品添加到玩家的背包中
                InventoryManager.Instance.inventoryData.AddItem(reward.itemData, reward.amount);
            }
            InventoryManager.Instance.inventoryUI.RefreshUI();  //刷新背包界面
            InventoryManager.Instance.actionUI.RefreshUI();  //刷新操作界面
        }
    }

    //当前任务中需要收集或者是消灭的目标名字的列表
    public List<string> RequireTargetName()
    {
        List<string> targetNameList = new List<string>();

        foreach(var require in requires)
        {
            targetNameList.Add(require.name);
        }
        return targetNameList;
    }
}
