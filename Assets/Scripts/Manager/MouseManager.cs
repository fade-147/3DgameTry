using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;

//[System.Serializable]
//public class EventVector3:UnityEvent<Vector3> { }
public class MouseManager : Singleton<MouseManager>
{

    public Texture2D point, doorway, attack, target, arrow;   //定义不同的鼠标指针纹理
    RaycastHit hitInfo;
    public event Action<Vector3> onMouseClicked; //这是unity自带的事件系统，Action是一个委托，Vector3是参数类型，onMouseClicked是事件名称
    public event Action<GameObject> onEnemyClicked;

    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        SetCursorTexture ();
        if(InteractWithUI())   //如果鼠标指向UI物体，就不执行鼠标控制
        {
            return;
        }
        MouseControl();
    }

    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //用鼠标的点来返回这个点的射线
        if (Physics.Raycast(ray, out hitInfo))
        {
            switch (hitInfo.collider.gameObject.tag) {      //有碰撞体才能被检测到，我们给树木特定的图层了，所以射线会直接穿过树木
                case "Ground":         //也可以把树木的碰撞体删掉         。其实也可以把树木的（图层改成Ground，这样射线就会被树木检测到，鼠标指针也会变成point纹理）
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);   //设置鼠标指针为point纹理，热点为(16,16)，自动模式
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }

        }
    }
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0)&&hitInfo.collider!=null)    //点击鼠标左键+判空
        {
            if(hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                onMouseClicked?.Invoke(hitInfo.point);      //Invoke代表启动了这个事件，给事件传hitInfo.point是一个Vector3类型的参数
            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                onEnemyClicked?.Invoke(hitInfo.collider.gameObject);      
            }
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                onEnemyClicked?.Invoke(hitInfo.collider.gameObject);      
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                onMouseClicked?.Invoke(hitInfo.point);      
            }
            if (hitInfo.collider.gameObject.CompareTag("Item"))
            {
                onMouseClicked?.Invoke(hitInfo.point);      
            }
        }
    }
    bool InteractWithUI()
    {
        if(EventSystem.current!=null&&EventSystem.current.IsPointerOverGameObject())   //如果鼠标指向UI物体
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
