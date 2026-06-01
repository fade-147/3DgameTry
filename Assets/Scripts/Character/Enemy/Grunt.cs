using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;  

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10;
    public void KickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);   //朝向攻击目标
            Vector3 direction=attackTarget.transform.position - transform.position;   //计算攻击目标的方向
            direction.Normalize();     //将方向向量归一化，使其长度为1

            attackTarget.GetComponent<NavMeshAgent>().isStopped=true;   //停止目标的导航代理，使其无法移动
            attackTarget.GetComponent<NavMeshAgent>().velocity=direction * kickForce;   //给目标一个力，使其被踢飞
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");  //触发目标的眩晕动画   
        }
    }
}
