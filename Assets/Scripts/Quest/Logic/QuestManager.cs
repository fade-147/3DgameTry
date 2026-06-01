using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;   //一般用于数组中的查询和操作，这里用来循环查找当前任务列表中是否已经存在某个任务

public class QuestManager : Singleton<QuestManager>
{
    [System.Serializable]
    //用于记录当前接受的任务
    public class QuestTask
    {
        public QuestData_SO questData;  //任务数据
        public bool IsStarted { get { return questData.isStarted; } set { questData.isStarted = value; } }  //任务是否已开始
        public bool IsComplete { get { return questData.isComplete; } set { questData.isComplete = value; } }
        public bool IsFinished { get { return questData.isFinished; } set { questData.isFinished = value; } }
    }

    public List<QuestTask> tasks = new List<QuestTask>();  //当前接受的任务列表


    public void UpdateQuestProgress(string requireName,int amount)
    {
        //在敌人死亡的时候和杀敌的时候都需要调用这个方法来更新任务进度，所以需要一个参数来区分是哪个任务的哪个需求被更新了，这里通过需求名称来区分

        foreach (var task in tasks)
        {
            var matchTask = task.questData.requires.Find(r => r.name == requireName);  //根据任务的名字来寻找那个任务
            if (matchTask != null)
            {
                matchTask.currentAmount += amount;   //更新任务进度
            }
            task.questData.CheckQuestProgress();  //检查任务进度是否完成
        }
    }

    public bool HaveQuest(QuestData_SO data)
    {
        if (data != null)
        {
            return tasks.Any(q=> q.questData.questName == data.questName);  //Any可以查找列表中是否有我想要的值，这里通过名字检查当前任务列表中是否已经存在该任务
        }
        else
        {
            return false;
        }
    }

    public QuestTask GetQuest(QuestData_SO data)
    {
        if (data != null)
        {
            return tasks.FirstOrDefault(q => q.questData.questName == data.questName);  //FirstOrDefault可以查找列表中第一个满足条件的元素，如果没有满足条件的元素，则返回默认值（这里是null）
        }
        else
        {
            return null;
        }
    }
}
