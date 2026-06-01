using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionUI : MonoBehaviour
{
    public Text optionText;
    private Button thisButton;
    private DialoguePiece currentPiece;

    private bool takeQuest;  //用于标记当前选项是否是一个接受任务的选项 
    private string nextPieceID;  //用于存储选项对应的下一段对话的ID

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OnOptionClicked);  //为选项按钮添加点击事件监听器
    }

    public void UpdateOption(DialoguePiece piece,DialogueOption option)
    {
        currentPiece = piece;
        optionText.text = option.text;  //将选项文本设置为当前对话片段的文本内容
        nextPieceID = option.targetID;  //将选项对应的下一段对话的ID存储在nextPieceID变量中，以便在点击选项时使用
        takeQuest=option.takeQuest;  //将选项是否是一个接受任务的选项的标记存储在takeQuest变量中，以便在点击选项时使用
    }

    public void OnOptionClicked()
    {
        if(currentPiece.quest!=null)  //如果当前对话片段关联了任务
        {
            var newTask = new QuestManager.QuestTask
            {
                questData = Instantiate(currentPiece.quest)       //把任务数据转换成任务列表里的元素的形式，方便把他添加到任务列表
            };


            if (takeQuest)
            {
                //添加任务到任务列表
                if (QuestManager.Instance.HaveQuest(currentPiece.quest))
                {
                    //如果任务列表已经有该任务了，说明是完成任务了,则给予奖励物品
                    if(QuestManager.Instance.GetQuest(currentPiece.quest).IsComplete)
                    {
                        newTask.questData.GiveRewards();
                        QuestManager.Instance.GetQuest(newTask.questData).IsFinished = true;
                    }
                }
                else
                {
                    QuestManager.Instance.tasks.Add(newTask);   //列表中没有该任务，则添加（接收）任务
                    QuestManager.Instance.GetQuest(newTask.questData).IsStarted= true;

                    foreach(var requireItem in newTask.questData.RequireTargetName())
                    {
                        InventoryManager.Instance.CheckQuestItemInBag(requireItem);
                    }
                }
            }
        }


        if (nextPieceID == "")    //如果选项对应的下一段对话ID为空字符串，说明这是一个结束选项，直接关闭对话UI
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            return;
        }
        else    //否则，继续显示下一段对话
        {
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentData.dialogueIndex[nextPieceID]);  //通过选项对应的下一段对话ID获取下一段对话内容，并更新主对话框显示
        }
    }
}
