using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10;
    public GameObject rockPrefab;
    public Transform handPos;

    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            Vector3 direction =(attackTarget.transform.position - transform.position).normalized;
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;   //停止目标的导航代理，使其无法移动
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;   //给目标一个力，使其被踢飞
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");  //触发目标的眩晕动画   
            targetStats.TakeDamage(characterStats, targetStats);   //调用目标的TakeDamage方法，传入攻击者和被攻击者的CharacterStats
        }
    }

    //Animation Event
    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            GameObject rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);   //在手的位置生成一个岩石
            rock.GetComponent<Rock>().target = attackTarget;   //设置岩石的目标为攻击目标
        }
    }
}
