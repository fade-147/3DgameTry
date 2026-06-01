using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class QuestGiver : MonoBehaviour
{
    DialogueController controller;
    QuestData_SO currentQuest;

    //该部分用于玩家做任务的不同阶段可以触发不同的对话
    public DialogueData_SO startDialogue;    //第一次对话，接受任务时的对话
    public DialogueData_SO progressDialogue;  //第二次，任务进行中
    public DialogueData_SO completeDialogue;  //第三次，任务完成后，接收奖励
    public DialogueData_SO finishDialogue;  //第四次，接收奖励后的对话

    #region 获得任务状态
    public bool IsStarted
    {   //每次调用这个函数，都会去检测这个任务是否已经开始了，如果开始了就切换到任务进行中的对话
        get
        {
            if(QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetQuest(currentQuest).IsStarted;
            }
            return false;
        }
    }
    public bool IsComplete
    {   //每次调用这个函数，都会去检测这个任务是否已经完成了，如果完成了就切换到任务完成后的对话
        get
        {
            if(QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetQuest(currentQuest).IsComplete;
            }
            return false;
        }
    }
    public bool IsFinished
    {  
        get
        {
            if(QuestManager.Instance.HaveQuest(currentQuest))
            {
                return QuestManager.Instance.GetQuest(currentQuest).IsFinished;
            }
            return false;
        }
    }

    #endregion
    private void Awake()
    {
        controller = GetComponent<DialogueController>();
    }

    private void Start()
    {
        controller.currentData = startDialogue;  //初始对话设置为接受任务时的对话
        currentQuest=controller.currentData.GetQuest();  //从对话数据中获取当前任务数据
    }
    private void Update()
    {
        if (IsStarted)
        {
            if (IsComplete)
            {
                controller.currentData = completeDialogue;
            }
            else
            {
                controller.currentData = progressDialogue;
            }
        }

        if (IsFinished)
        {
            controller.currentData = finishDialogue;
        }
    }
}
