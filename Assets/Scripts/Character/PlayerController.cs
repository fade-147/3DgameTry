using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    private GameObject attackTarget;
    private float lastAttackTime;
    private bool isDead;
    private float stopDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        stopDistance=agent.stoppingDistance;   //记录NavMeshAgent的停止距离，攻击的时候需要让玩家停在攻击范围内
    }
    private void OnEnable()    //当玩家对象启用时，添加事件监听，监听鼠标点击事件和敌人点击事件
    {
        MouseManager.Instance.onMouseClicked += MoveToTarget;   //添加事件监听，当鼠标点击事件发生时，调用MoveToTarget方法
        MouseManager.Instance.onEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlyaer(characterStats);   //注册玩家属性到GameManager，方便其他脚本访问玩家属性
    }
    private void Start()
    {
        //characterStats.MaxHealth
        SaveManager.Instance.LoadPlayerData();   //加载玩家数据
    }
    private void OnDisable()   //当玩家对象禁用时，移除事件监听，防止内存泄漏
    {
        if(!MouseManager.IsInitialized) return;   //如果MouseManager没有初始化，就不需要移除事件监听了，防止报错
        MouseManager.Instance.onMouseClicked -= MoveToTarget;
        MouseManager.Instance.onEnemyClicked -= EventAttack;
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;   //当玩家当前生命值等于0时，isDead为true，播放死亡动画
        if (isDead)
        {
            GameManager.Instance.NotitfyObservers();   //通知所有结束游戏观察者，游戏结束了
        }
        switchAnimation();
    }
    private void switchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);   //根据NavMeshAgent的速度来切换动画，速度越大，动画越快
        anim.SetBool ("Death", isDead);    //根据isDead参数来切换死亡动画
    }
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();   //停止所有协程，确保玩家在移动的时候不会被攻击打断，确保玩家可以打断攻击继续移动
        if (isDead) return;    //如果玩家已经死亡，就不能移动了
        agent.stoppingDistance=stopDistance;   //恢复NavMeshAgent的停止距离，确保玩家可以正常移动，不会停在攻击范围内
        agent.isStopped=false;   //开始移动，确保玩家可以移动(攻击的时候会让玩家停止移动)
        agent.SetDestination(target);
    }
    private void EventAttack(GameObject target)
    {
        if (isDead) return;

        if (target!=null)     //防止目标死亡而丢失报错
        {
            attackTarget = target;
            characterStats .isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;   //根据暴击率随机判断是否暴击
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped=false;   //开始移动,确保玩家可以移动
        agent.stoppingDistance=characterStats.attackData.attackRange;   //设置NavMeshAgent的停止距离为攻击范围，这样玩家就会停在攻击范围内，不会贴着目标
        transform.LookAt(attackTarget.transform);   //面向(转向)目标
        while (Vector3.Distance(attackTarget.transform.position, transform.position) >characterStats.attackData.attackRange)   //当玩家与目标的距离大于攻击范围时，继续移动
        {                                              //不同的武器要有不同的攻击范围，这里暂时写死为1
            agent.SetDestination(attackTarget.transform.position);
            yield return null;   //null代表等待下一帧，继续执行这个协程
        }
        agent.isStopped=true;    //停止移动

        //攻击逻辑
        if(Time.time-lastAttackTime>characterStats.attackData.coolDown)   //攻击间隔
        {
            anim.SetBool("Critical", characterStats.isCritical);    //设置暴击参数，暴击时播放暴击动画)
            anim.SetTrigger("Attack");    //触发攻击动画
            lastAttackTime = Time.time;   //记录上次攻击时间
        }
    }
    //Animator Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))     //如果是可攻击的（石头）。不然就是普通敌人
        {
            if (attackTarget.GetComponent<Rock>()&&attackTarget.GetComponent<Rock>().rockStates==Rock.RockStates.HitNothing)    //在地上的石头才可以反击回去
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;    //玩家可以攻击石头，把石头打回去
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;   //给一个初始的速度，防止其被FixUpdata中的逻辑误判
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);   //调用目标的TakeDamage方法，传入攻击者和被攻击者的CharacterStats
        }
    }
}
