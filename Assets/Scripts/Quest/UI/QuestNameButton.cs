using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNameButton : MonoBehaviour
{
    public Text questNameText;    //接收任务详情，便于更新按钮图标文本
    public QuestData_SO currentData;
    //public Text questContentText;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }

    void UpdateQuestContent()
    {
        //questContentText.text = currentData.description;//点击按钮后，显示按钮对应的任务详情
        QuestUI.Instance.SetupRequireList(currentData);   //点击按钮后，更新显示右侧任务

        foreach(Transform item in QuestUI.Instance.rewardTransform)  //先清空右侧奖励框内的物体
        {
            Destroy(item.gameObject);
        }

        foreach(var item in currentData.rewards)    //更新右侧奖励框内的物品和数量数据
        {
            QuestUI.Instance.SetupRewardItem(item.itemData, item.amount);
        }
    }

    public void SetupNameButton(QuestData_SO questData)  //给每一个按钮相应的任务数据，并更新任务详情
    {
        currentData = questData;

        if(questData.isComplete)
        {
            questNameText.text = $"<color=green>{questData.questName+"(完成)"}</color>";
        }
        else if(questData.isStarted)
        {
            questNameText.text = $"<color=yellow>{questData.questName}</color>";
        }
        else
        {
            questNameText.text = $"<color=red>{questData.questName}</color>";
        }
    }
}
