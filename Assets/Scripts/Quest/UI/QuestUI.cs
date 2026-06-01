using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements")]
    public GameObject questPanel;
    public ItemTooltip tooltip;   //鼠标悬停描述
    bool isOpen;   //用于记录任务面板的开关状态

    [Header("Quest Name")]   //获得左侧按钮的框，用于在里面生成任务按钮
    public RectTransform questListTransform;
    public QuestNameButton questNameButton;

    [Header("Text Content")]     //右侧的任务详情
    public Text questContentText;

    [Header("Requirement")]   //获得右侧的任务需求框，用于在里面生成任务需求预制体
    public RectTransform requireTransform;
    public QuestRequirement requirement;

    [Header("Reward Panel")]    //获得右侧的任务奖励框，用于在里面生成任务奖励预制体
    public RectTransform rewardTransform;
    public ItemUI rewardUI;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            isOpen = !isOpen;
            questPanel.SetActive(isOpen);

            SetupQuestList();

            if (!isOpen)   //当任务面板关闭时，也关闭鼠标悬停描述
            {
                tooltip.gameObject.SetActive(false);
            }
        }
    }

    public void SetupQuestList()
    {
        questContentText.text = "";    //打开任务面板时清空右侧的任务详情文本
        //每次打开任务面板时，先清空之前的任务列表、任务需求和任务奖励
        foreach (Transform item in questListTransform)
        {
            Destroy(item.gameObject);
        }

        foreach(Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        foreach(Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }

        foreach(var task in QuestManager.Instance.tasks)
        {
            //将任务列表中的每个任务生成一个按钮，在左侧框中生成按钮
            var newTask = Instantiate(questNameButton,questListTransform);

            //再将任务数据传递给按钮，让按钮显示对应的任务名称
            newTask.SetupNameButton(task.questData);
            //newTask.questContentText = questContentText;  
        }
    }

    public void SetupRequireList(QuestData_SO questData)    //点击左侧按钮时调用，用于更新右侧任务详情
    {
        questContentText.text = questData.description;

        //清空任务需求
        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        //根据任务数据中的需求列表，在右侧框中生成对应的任务需求预制体,并给这些预制体相应的任务需求数据
        foreach (var require in questData.requires)
        {
            var q = Instantiate(requirement, requireTransform);
            q.SetupRequirement(require.name, require.requireAmount, require.currentAmount);
        }
    }

    public void SetupRewardItem(ItemData_SO itemData,int amount)   //奖励物品的数据以及数量
    {
        var item = Instantiate(rewardUI, rewardTransform);   //在奖励框内生成奖品物体
        item.SetupItemUI(itemData, amount);    //把物体和数量传递进去
    }
}
