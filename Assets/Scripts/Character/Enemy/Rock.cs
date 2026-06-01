using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    private Rigidbody rb;
    public enum RockStates {HitPlayer,HitEnemy,HitNothing }
    public RockStates rockStates;

    [Header("Basic Settings")]
    public float force;
    public int damage;
    public GameObject target;
    private Vector3 direction;
    public GameObject breakEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;     //刚生成是速度是0，防止被FixedUpdate中的逻辑误判

        rockStates=RockStates.HitPlayer;   //默认状态是可以击中玩家的
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        if(rb.velocity.sqrMagnitude<1f)   //当岩石的速度很小的时候，说明它已经停下来或者快要停下来
        {
            rockStates = RockStates.HitNothing;
        }
    }
    public void FlyToTarget()
    {
        if(target==null)
        {
            Destroy(gameObject);   //如果目标不存在，销毁岩石
            return;
        }
        direction = (target.transform.position - transform.position+Vector3.up).normalized;   //计算飞行方向，单位化向量
        rb.AddForce(direction * force, ForceMode.Impulse);   //给岩石一个瞬时的力，使其飞向目标
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if(other.gameObject.CompareTag("Player"))
                {
                    //对玩家造成伤害  .给玩家击飞+眩晕
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;   //给玩家一个力，使其被击飞

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");     //使玩家进入眩晕状态
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage,other.gameObject.GetComponent<CharacterStats>());   //对玩家造成伤害

                    rockStates = RockStates.HitNothing;   //改变状态，防止重复触发
                }
                break;

            case RockStates.HitEnemy:
                if(other.gameObject.GetComponent<Golem>())    //如果碰撞到的是敌人
                {
                    //对敌人造成伤害
                    var otherStats= other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);   //对敌人造成伤害
                    Instantiate(breakEffect,transform.position,Quaternion.identity);
                    Destroy(gameObject);   //销毁岩石
                }
                break;
        }
    }
}
