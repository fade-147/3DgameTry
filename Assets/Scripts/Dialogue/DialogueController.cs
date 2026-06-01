using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    //挂载到人物身上，用于发起对话
    public DialogueData_SO currentData;   //该人物的对话数据
    bool canTake = false;
    private void OnTriggerEnter(Collider other)
    {
        if(currentData != null && other.CompareTag("Player"))
        {
            canTake = true;   //只有玩家靠近才可以触发动画
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
        }
    }
    private void Update()
    {
        if (canTake && Input.GetMouseButtonDown(1)) //按下鼠标右键
        {
            OpenDialogue();
        }
    }
    void OpenDialogue()
    {
        //打开UI面板
        //开始对话，传输对话内容信息
        DialogueUI.Instance.UpdateDialogueData(currentData);
        DialogueUI.Instance.UpdateMainDialogue(currentData.dialoguePieces[0]);  //显示第一段对话内容
    }
}

