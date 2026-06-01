using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestRequirement : MonoBehaviour
{
    private Text requireName;   //任务需求
    private Text progressNumber;   //任务需求数量及进度

    private void Awake()
    {
        requireName = GetComponent<Text>();
        progressNumber =transform.GetChild(0).GetComponent<Text>();
    }

    //用于更新自身的任务需求和数量进度
    public void SetupRequirement(string name,int amount,int currentAmount)
    {
        requireName.text = name;
        progressNumber.text = currentAmount.ToString()+"/"+amount.ToString();
    }
}
