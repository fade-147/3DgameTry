using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();

    public Dictionary<string, DialoguePiece> dialogueIndex = new Dictionary<string, DialoguePiece>();  //用于快速查找对话片段的索引，键是对话片段的ID，值是对应的对话片段对象

#if UNITY_EDITOR
    void OnValidate()  //在编辑器中修改数据时自动调用，更新对话索引
    {
        dialogueIndex.Clear();  //清空原有的对话索引
        foreach (DialoguePiece piece in dialoguePieces)  //遍历所有的对话片段，将它们的ID和内容存储在对话索引中
        {
            if (!dialogueIndex.ContainsKey(piece.ID))  //如果对话索引中还没有这个ID，则添加新的键值对
            {
                dialogueIndex.Add(piece.ID, piece);
            }
            else  //如果对话索引中已经有这个ID了，说明有重复的ID，需要给出警告提示
            {
                Debug.LogWarning("Duplicate Dialogue Piece ID: " + piece.ID + " in Dialogue Data: " + name);
            }
        }
    }

#endif

    public QuestData_SO GetQuest()
    {  //循环每一个对话，看看有没有包含任务，如果有，就返回这个任务，注意如果有多个对话包含任务，那么只会返回最后一个包含任务的对话的任务
        QuestData_SO currentquest=null;
        foreach(var piect in dialoguePieces){
            if(piect.quest != null){
                currentquest = piect.quest;
            }
        }
                return currentquest;
    }
}
