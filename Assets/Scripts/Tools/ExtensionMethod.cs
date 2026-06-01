using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod   //静态类，存放一些扩展方法，方便全局调用
{
    private const float dotThreshold = 0.5f;    //需要是常量，不可被更改 
    public static bool IsFacingTarget(this Transform transform, Transform target)   //判断敌人是否在攻击范围内
    {
        var vectorToTarget = (target.position - transform.position).normalized;   //计算敌人和目标之间的向量，并归一化

        float dot =Vector3.Dot(transform.forward, vectorToTarget);   //计算敌人朝向和目标之间的点积
        return dot >= dotThreshold;   //如果点积大于阈值，说明敌人朝向目标，否则说明敌人没有朝向目标(玩家在敌人正前方是1，斜60度是cos60=0.5)
    }
}
