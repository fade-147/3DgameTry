using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { GUARD, PATROL, CHASE, DEAD }   //定义敌人状态枚举，分别为闲置、巡逻、追逐、死亡
[RequireComponent(typeof(NavMeshAgent))]   //确保敌人对象上有NavMeshAgent组件，如果没有会自动添加
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyState enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;      //所有种类的碰撞体都是collider...用于在死亡后关闭碰撞体，防止敌人死后还被攻击

    protected CharacterStats characterStats;
    [Header("Basic Settings")]
    public float sightRadius;   //敌人视野范围
    public bool isGuard;    //区分敌人是否为站桩是敌人
    private float speed;    //记录他原来的速度
    protected GameObject attackTarget;   //敌人攻击目标
    public float lookAtTime; 
    private float remainLookAtTime;   //敌人转向时间，超过这个时间就不转了，防止敌人一直转来转去
    private float lastAttackTime;   //记录上次攻击时间，控制攻击频率
    private Quaternion guardRotation;

    [Header("Patrol State")]
    public float patrolRange;   //巡逻范围
    private Vector3 wayPoint;
    private Vector3 guardPos;
    //bool配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll= GetComponent<Collider>();

        speed =agent.speed;
        guardPos=transform.position;   //记录敌人初始位置，作为站桩式敌人的守卫位置和巡逻式敌人的中心点
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }
    private void Start()
    {
        if (isGuard)   //游戏开始，让站桩式敌人和巡逻式敌人选择状态
        {
            enemyStates=EnemyState.GUARD;
        }
        else
        {
            enemyStates = EnemyState.PATROL;
            GetNewWayPoint();   //初始给一个移动的点，防止乱走
        }
        GameManager.Instance.AddObserver(this);   //注册结束游戏观察者
    }
    //void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);   //注册结束游戏观察者
    //}
    void OnDisable()
    {
        if (!GameManager.IsInitialized) return;    //在敌人对象被禁用时，移除结束游戏观察者，防止敌人死了还在观察玩家状态，导致报错
        GameManager.Instance.RemoveObserver(this);    //移除结束游戏观察者

        if (GetComponent<LootSpawner>() && isDead)    //当敌人死亡时，如果这个敌人有掉落组件，就生成掉落物
        {
            GetComponent<LootSpawner>().SpawnLoot();  
        }
        if (QuestManager.IsInitialized && isDead)
        {
            QuestManager.Instance.UpdateQuestProgress(this.name, 1);   //当敌人死亡时，如果这个敌人是任务目标，就更新任务进度，参数是敌人名字和数量（这里假设每个敌人都只需要杀死一个，如果有需要可以改成不同的数量）
        }
    }

    private void Update()
    {
        if (characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }
        if (!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical); 
        anim.SetBool("Death", isDead);
    }
    void SwitchStates()     //根据敌人状态来执行不同的行为
    {
        if (isDead)
        {
            enemyStates = EnemyState.DEAD;
        }else if(FoundPlayer())    //（只有当未死亡时才寻找玩家，防止打扰DEAD的状态）如果发现玩家，切换到追逐状态（CHASE）
        {
            enemyStates = EnemyState.CHASE;
            //Debug.Log("Found Player! Switching to CHASE state.");
        }

        switch (enemyStates)
        {
            case EnemyState.GUARD:
                isChase = false;    //站桩的时候，速度慢，移动到守卫位置
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped= false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk=false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;
            case EnemyState.PATROL:
                isChase = false;    //巡逻的时候，速度慢，移动到范围内某个随机的点
                agent.speed= speed*0.5f;
                //判断是否到了随机巡逻点，如果到了就重新随机一个点，如果没有就继续朝那个点走
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;  //到巡逻点后，先观察一会
                    }
                    else
                    {
                        GetNewWayPoint();
                    }
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }

                    break;
            case EnemyState.CHASE:
                //追player
                isWalk = false;
                isChase = true;
                
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    //拉脱回到上一个状态
                    isFollow = false;
                    if(remainLookAtTime> 0)
                    {
                        agent.destination = transform.position;   //如果没有找到玩家，停止移动。防止敌人移动到玩家最后的位置时才停止
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard)
                    {
                        enemyStates=EnemyState.GUARD;
                    }
                    else
                    {
                        enemyStates=EnemyState.PATROL;
                    }
                }
                else
                {
                    isFollow=true;
                    agent.isStopped = false;
                    agent.destination=attackTarget.transform.position;
                }

                //在攻击范围内则攻击
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent .isStopped = true;     //停下来攻击
                    if(lastAttackTime<=0)
                    {
                        lastAttackTime=characterStats.attackData.coolDown;
                        characterStats.isCritical = Random.value<characterStats.attackData.criticalChance;    //暴击判断
                        Attack();
                    }
                }
                    break;
            case EnemyState.DEAD:
                coll.enabled = false;   //死亡时，禁用碰撞体，防止敌人死后还被攻击(在播放死亡动画时)
                //agent.enabled = false;   //死亡时，禁用导航组件，停止一切移动
                agent.radius=0;    //死亡时，将导航组件的半径设置为0，防止敌人死后还挡路
                Destroy(gameObject, 2f);   //2秒后销毁敌人对象，给死亡动画留出时间
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);   //面向(转向)目标
        if(TargetInAttackRange())
        {
            anim.SetTrigger("Attack");
        }
        if(TargetInSkillRange())
        {
            anim.SetTrigger("Skill");
        }
    }
    bool FoundPlayer()   //判断是否发现玩家，暂时用距离来判断，后续可以用视野锥体来判断
    {
        var colliders=Physics.OverlapSphere(transform.position, sightRadius);   //获取敌人周围一定范围内的所有碰撞体(一组)
        foreach(var target in colliders)   //遍历这些碰撞体，看看有没有玩家
        {
            if(target.CompareTag("Player"))
            {
                attackTarget=target.gameObject;   //如果找到了玩家，设置攻击目标为这个玩家
                return true;    //如果找到了玩家，返回true
            }
        }
        attackTarget= null;
        return false;   //如果没有找到玩家，返回false
    }
    bool TargetInAttackRange()
    {
        if(attackTarget!=null)
        {
            return Vector3.Distance(attackTarget.transform.position,transform.position)<=characterStats.attackData.attackRange;
        }
        else
        {
            return false;
        }
    }
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        }
        else
        {
            return false;
        }
    }
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX=Random.Range(-patrolRange,patrolRange);
        float randomZ=Random.Range(-patrolRange,patrolRange);

        Vector3 randomPoint=new Vector3(guardPos.x+randomX,transform.position.y,guardPos.z+randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)?hit.position:transform.position;   //在导航网格上找到离随机点最近的点，确保敌人不会走到不可行走的地方

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);   //在编辑器中绘制一个蓝色的线框球，表示敌人的视野范围
    }
    //Animator Event
    void Hit()
    {
        if (attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);   //调用目标的TakeDamage方法，传入攻击者和被攻击者的CharacterStats
        }
    }

    public void EndNotify()
    {
        anim.SetBool("Win", true);    //当游戏结束时，播放胜利动画
        playerDead = true;   //玩家死了，敌人赢了
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
