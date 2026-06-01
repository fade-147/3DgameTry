using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode ActionKey;   //可以在Inspector中设置该按钮对应的快捷键,便于挂载同一个代码的不同物体使用不同的按键来触发
    private SlotHolder currentSlotHolder;

    private void Awake()
    {
        currentSlotHolder = GetComponent<SlotHolder>();   //获取当前按钮所在的格子组件
    }

    private void Update()
    {
        if(Input.GetKeyDown(ActionKey)&&currentSlotHolder.itemUI.GetItem())   //当按下对应的快捷键时，并且格子里有物体
        {
            currentSlotHolder.UseItem();   //调用格子组件的UseItem方法来使用物品
        }
    }
}
