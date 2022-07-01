using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]

public class EnemyController : MonoBehaviour,IEndGameObserver
{
    //敌人状态
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;

    //数据属性的组件
    protected CharacterStats characterStats;

    [Header("Basic Settings")]
    //敌人可视范围
    public float sightRadius;
    //站桩模式切换
    public bool isGuard;
    //记录原来速度
    private float speed;
    //攻击目标
    protected GameObject attackTarget;
    //巡逻停下看的时间
    public float lookAtTime;
    //查看计时器时间
    private float remainLookAtTime;
    //攻击冷却时间
    private float lastAttackTime;
    //记录旋转角度
    private Quaternion guardRotation;


    [Header("Patrol State")]
    //巡逻范围
    public float patrolRange;
    //路线坐标点
    private Vector3 wayPoint;
    //敌人初始坐标位置
    private Vector3 guardPos;

    //bool配合动画转换
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    //角色死亡
    bool playerDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        //记录原来agent的速度
        speed = agent.speed;
        //记录最开始的坐标
        guardPos = transform.position;
        //记录原始的角度
        guardRotation = transform.rotation;
        //查看时间赋值
        remainLookAtTime = lookAtTime;
    }

    void Start()
    {
        //守卫状态
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else//巡逻状态
        {
            enemyStates = EnemyStates.PATROL;
            //初始一个可移动的点
            GetNewWayPoint();
        }
        //FIXME:场景切换更改
        GameManager.Instance.AddObserver(this);
    }

    //切换场景时启用
    //void OnEnable()
    //{
           //加入观察
    //    GameManager.Instance.AddObserver(this);
    //}

    void OnDisable()
    {
        //GameManager没有生成直接返回
        if (!GameManager.IsInitialized) return;
        //从列表移除
        GameManager.Instance.RemoveObserver(this);

        if (GetComponent<LootSpawner>() && isDead)      //死亡并掉落物品
            GetComponent<LootSpawner>().SpawnLoot();

        if (QuestManager.IsInitialized && isDead)
        {
            QuestManager.Instance.UpdateQuestProgress(this.name, 1);
        }
    } 

    void Update()
    {
        //血量值为0死亡为true
        if (characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }
        //角色没有死亡执行切换状态和切换动画
        if(!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
        
    }

    //切换状态动画
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    void SwitchStates()
    {
        //判断死亡切换死亡状态
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        //如果发现player 切换到CHASE
        else if (FoundPlayer())
            enemyStates = EnemyStates.CHASE;

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                //修改追击状态
                isChase = false;

                //位置不在初始的位置
                if (transform.position != guardPos)
                {
                    //开始走动
                    isWalk = true;
                    agent.isStopped = false;
                    //目标设置为初始位置
                    agent.destination = guardPos;
                }

                //判断当前位置和初始位置的距离是否小于停止距离
                if (Vector3.Distance(guardPos, transform.position) <= agent.stoppingDistance)
                {
                    //停止走动
                    isWalk = false;
                    //将当前角度旋转为初始角度
                    transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                }
                break;
            case EnemyStates.PATROL:
                //追击状态false
                isChase = false;
                //巡逻速度比正常速度小
                agent.speed = speed * 0.5f;

                //判断是否到了随机巡逻点
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    //移动状态false
                    isWalk = false;
                    //判断时间是否为0
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();//获取新的巡逻点
                }
                else
                {
                    //没到巡逻点，继续移动
                    isWalk = true;
                    //目标点是wayPoint
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                //追player状态动画
                isWalk = false;
                isChase = true;
                
                //追击的速度
                agent.speed = speed;
                
                //没有发现敌人
                if (!FoundPlayer())
                {
                    //拉脱回到上一个状态
                    //跟随动画false
                    isFollow = false;
                    //停下观察时间不为0
                    if (remainLookAtTime > 0)
                    {
                        //将当前位置坐标给agent
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)//原有状态是守卫
                        enemyStates = EnemyStates.GUARD;
                    else//否则巡逻状态
                        enemyStates = EnemyStates.PATROL;
                }
                else
                {
                    //跟随动画true
                    isFollow = true;
                    agent.isStopped = false;
                    //传回攻击目标坐标
                    agent.destination = attackTarget.transform.position;
                }

                //在攻击范围内则攻击
                if(TargetInAttackRange() || TargetInSkillRange())
                {
                    //停下移动
                    isFollow = false;
                    agent.isStopped = true;

                    //判断攻击时间
                    if (lastAttackTime < 0)
                    {
                        //还原攻击冷却时间
                        lastAttackTime = characterStats.attackData.coolDown;
                        //暴击判断，随机的数值小于数据设置的暴击率返回true
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }
                }
                break;

            case EnemyStates.DEAD:
                //关闭碰撞体
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    //查找周围是否有player
    bool FoundPlayer()
    {
        //返回一个collider数组
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            //找到Player
            if (target.CompareTag("Player"))
            {
                //获取攻击目标
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    //攻击方法
    void Attack()
    {
        //看向攻击目标
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }

        if (TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");
        }

    }

    //判断近距离攻击
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            //自身距离和攻击目标的距离小于攻击距离
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    //判断技能攻击范围
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            //自身距离和攻击目标的距离小于技能攻击距离
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }



    //巡逻点
    void GetNewWayPoint()
    {
        //还原停止查看的时间
        remainLookAtTime = lookAtTime;
        //随机生成坐标点
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        //组成Vector3坐标
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        //NavMeshHit变量
        NavMeshHit hit;
        //指定范围找到导航网格最近的点
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
        
    }

    //在Scene中画出区域
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        //画范围
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    

    //Animation Event 攻击事件
    void Hit()
    {
        //判断攻击目标不为空并且判断是否在正前方
            if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
            {
            //获得攻击者Stats
                var targetStats = attackTarget.GetComponent<CharacterStats>();
            //调用TakeDamage
                targetStats.TakeDamage(characterStats, targetStats);
            }
    }

    //实现观察者接口
    public void EndNotify()
    {
        //结束，播放获胜动画
        anim.SetBool("Win", true);
        //角色死亡
        playerDead = true;
        //停止所有移动
        isChase = false;
        isWalk = false;
        //停止agent
        attackTarget = null;
    }
}
