using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialoguePiece 
{
    //说话人的图片的语句，选项，以及本句话的ID，选择选项后会跳转到对应ID的下一句话
    public string ID;
    public Sprite image;

    [TextArea]
    public string text;

    public QuestData_SO quest;  //该对话接的任务

    public List<DialogueOption> options = new List<DialogueOption>();
}
