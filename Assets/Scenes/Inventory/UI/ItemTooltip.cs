using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public Text itemNameText;
    public Text itemInfoText;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform=GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        UpdatePosition();    //物品被启动时都先调整一下坐标
    }
    public void SetupTooltip(ItemData_SO item)
    {
        itemNameText.text = item.itemName;
        itemInfoText.text = item.description;
    }
    private void Update()
    {
        UpdatePosition();
    }
    public void UpdatePosition()
    {
        Vector3 mousePos=Input.mousePosition;

        Vector3[] corners=new Vector3[4];
        rectTransform.GetWorldCorners(corners);   //获取tooltip的四个角的世界坐标

        float width = corners[3].x - corners[0].x;   //计算tooltip的宽度
        float height = corners[1].y - corners[0].y;  //计算tooltip的高度

        if (mousePos.y < height)   //鼠标的坐标是以屏幕的左下角为原点的，如果鼠标在屏幕下半部分，就把tooltip放在鼠标上方，避免tooltip被遮挡
        {
            rectTransform.position=mousePos+Vector3.up*height*0.6f;
        }
        else if(Screen.width-mousePos.x>width)        //如果鼠标远离右侧边缘时，就把tooltip放在鼠标右边。
        {
            rectTransform.position=mousePos+Vector3.right*width*0.6f;
        }
        else    //如果鼠标在屏幕右边缘，就把tooltip放在鼠标左边，避免tooltip被遮挡
        {
            rectTransform.position = mousePos + Vector3.left * width * 0.6f;
        }
    }
}
