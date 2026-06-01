using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueOption 
{
    public string text;
    public string targetID;
    public bool takeQuest;   //用于记录玩家是否接受任务
}
