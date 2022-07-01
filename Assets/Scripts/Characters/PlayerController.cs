using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;

    //目标攻击物体
    private GameObject attackTarget;
    //攻击时间
    private float lastAttackTime;
    //判断死亡状态
    private bool isDead;
    //agent停下距离
    private float stopDistance;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        //记录初始停下变量
        stopDistance = agent.stoppingDistance;
    }

    //生成时添加订阅
    void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        //单例注册人物数据组件
        GameManager.Instance.RegisterPlayer(characterStats);
    }

    void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }

    //切换时取消订阅
    void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
    }

    void Update()
    {
        //是否死亡
        isDead = characterStats.CurrentHealth == 0;

        //角色死亡时，进行广播
        if (isDead)
            GameManager.Instance.NotifyObservers();

        SwitchAnimation();
        //攻击的冷却时间减少
        lastAttackTime -= Time.deltaTime;
    }

    //切换动画
    private void SwitchAnimation()
    {
        //agent速度值播放移动动画
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        //死亡动画
        anim.SetBool("Death", isDead);
    }

    //点击鼠标，接收Vector3值
    public void MoveToTarget(Vector3 target)
    {
        //停止协程
        StopAllCoroutines();
        //角色已经死亡不再执行
        if (isDead) return;

        agent.stoppingDistance = stopDistance;
        //恢复移动
        agent.isStopped = false;
        agent.destination = target;
    }


    //事件调用攻击
    private void EventAttack(GameObject target)
    {
        //已经死亡不执行
        if (isDead) return;

        //攻击目标是否为空
        if (target != null)
        {
            //赋值:攻击目标
            attackTarget = target;
            //判断暴击
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            //执行协程
            StartCoroutine(MoveToAttackTarget());
        }
    }

    //协程
    IEnumerator MoveToAttackTarget()
    {
        //最开始可以移动
        agent.isStopped = false;
        //攻击时，停止距离为攻击距离
        agent.stoppingDistance = characterStats.attackData.attackRange;

        //人物转向目标
        transform.LookAt(attackTarget.transform);

        //修改攻击范围参数，攻击距离
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            //距离太远，向攻击目标坐标移动
            agent.destination = attackTarget.transform.position;
            //下一帧再次执行循环
            yield return null;
        }

        //执行攻击前，停下来
        agent.isStopped = true;

        //Attack
        //攻击冷却时间
        if (lastAttackTime < 0)
        {
            //执行攻击动画
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }


    //Animation Event动画事件调用
    void Hit()
    {
        //攻击目标是可攻击的物体
        if (attackTarget.CompareTag("Attackable"))
        {
            //判断是否是石头以及石头状态是无
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing) 
            {
                //更改石头状态为攻击敌人
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                //给攻击目标赋予速度
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                //给刚体向前击飞的力
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            //获取攻击目标身上的Stats
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            //调用TakeDamage传入攻击者和防御者Stats
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

}
