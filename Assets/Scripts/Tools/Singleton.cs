using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    //泛型单例类
    private static T instance;    //依然是泛型那么（mouse manager等）所有类型都可以作为单例类

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;   //如果实例不存在就将当前对象赋值给实例
        }
        else
        {
            Destroy(gameObject);    //如果实例已经存在就销毁当前对象
        }
    }
    public static bool IsInitialized
    {
        get
        {
            return instance != null;   //判断实例是否已经初始化
        }
    }
    protected virtual void OnDestroy()
    {
        if (instance == this)   //如果当前对象是实例就将实例置空
        {
            instance = null;
        }
    }
}
