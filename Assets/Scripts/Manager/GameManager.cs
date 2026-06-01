using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;   //玩家属性
    private CinemachineFreeLook followCamera;   //虚拟相机组件
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();   //结束游戏观察者列表,当游戏结束时会通知这些观察者执行相应的操作

    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);   //场景切换时不销毁这个对象
    }
    public void RigisterPlyaer(CharacterStats player)
    {
        playerStats = player;   //注册玩家属性

        followCamera = FindObjectOfType<CinemachineFreeLook>();   //找到场景中的虚拟相机组件

        if(followCamera != null)
        {     //让摄像头在切换场景后继续跟随玩家
            followCamera.Follow = player.transform.GetChild(2);   //设置虚拟相机跟随玩家
            followCamera.LookAt = player.transform.GetChild(2);   //设置虚拟相机注视玩家
        }
    }
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);        //添加结束游戏观察者
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);     //移除结束游戏观察者
    }

    public void NotitfyObservers()   //通知所有结束游戏观察者
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();     //调用每个观察者的结束通知方法
        }
    }
    public Transform GetEntrance()
    {
        foreach(var item in FindObjectsOfType<TransitionDestination>())   //在当前场景中找到所有的传送目的地
        {
            if(item.destinationType == TransitionDestination.DestinationType.ENTER)    //找到目的地类型为入口的目的地
            {
                return item.transform;   //返回入口的Transform组件
            }
        }
        return null;    //如果没有找到入口，则返回null
    }
}
