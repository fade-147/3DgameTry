using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : Singleton<DialogueUI>
{
    [Header("Basic Elements")]
    public Image icon;
    public Text mainText;
    public Button nextButton;
    public GameObject dialoguePanel;  //用于开启和关闭对话UI

    [Header("Options")]
    public RectTransform optionsPanel;
    public OptionUI optionPrefab;

    [Header("Data")]
    public DialogueData_SO currentData;
    int currentIndex = 0;  //用于记录当前对话的索引

    override protected void Awake()
    {
        base.Awake();
        nextButton.onClick.AddListener(ContinueDialogue);  //为下一句按钮添加点击事件监听器
    }

    void ContinueDialogue()
    {
        if(currentIndex<currentData.dialoguePieces.Count)  //如果还有下一句话
            UpdateMainDialogue(currentData.dialoguePieces[currentIndex]);  //显示下一段对话内容
        else
            dialoguePanel.SetActive(false);  //如果没有下一句话了，就关闭对话UI
    }

    public void UpdateDialogueData(DialogueData_SO data)   //开始新的对话时调用，更新当前对话数据
    {
        currentData = data;
        currentIndex = 0;  //重置当前对话索引
    }

    public void UpdateMainDialogue(DialoguePiece piece)
    {
        dialoguePanel.SetActive(true);
        currentIndex++;

        if (piece.image != null)   //如果当前对话片段有头像图片，则显示头像，否则隐藏头像
        {
            icon.enabled = true;
            icon.sprite = piece.image;
        }
        else
        {
            icon.enabled = false;
        }

        mainText.text = "";  //清空主文本框的内容
        //mainText.text=piece.text;  //将当前对话片段的文本内容显示在主文本框中
        mainText.DOText(piece.text, 1f);  //使用DOTween实现文字逐字显示，持续时间为1秒

        if(currentData.dialoguePieces.Count>0 && piece.options.Count == 0) //如果当前对话后没有选项,并且该对话还有下一句，那么就显示下一句按钮，否则隐藏下一句按钮
        {
            nextButton.interactable = true;
            nextButton.gameObject.SetActive(true);
            nextButton.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            //nextButton.gameObject.SetActive(false);
            //不直接关闭按钮是为了防止自适应排版组件的布局问题，直接隐藏按钮会导致排版组件重新计算布局
            nextButton.transform.GetChild(0).gameObject.SetActive(false);  //如果当前对话后有选项了，就隐藏下一句按钮的文本
            nextButton.interactable = false;  //禁用下一句按钮的交互功能，防止玩家点击它
        }
        CreatOptions(piece);
    }

    void CreatOptions(DialoguePiece piece)
    {
        if(optionsPanel.childCount>0)  //如果选项面板中已经有选项了，就先清空选项面板
        {
            for(int i=0;i<optionsPanel.childCount;i++)
            {
                Destroy(optionsPanel.GetChild(i).gameObject);
            }
        }
        //创建新的选项
        for(int i=0;i<piece.options.Count;i++)
        {
            var option = Instantiate(optionPrefab, optionsPanel);
            option.UpdateOption(piece, piece.options[i]);     //传入每个选项的内容文本
        }
    }
}
