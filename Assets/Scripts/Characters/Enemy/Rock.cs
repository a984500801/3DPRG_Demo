using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    //枚举值，石头状态切换
    public enum RockStates { HitPlayer,HitEnemy,HitNothing }
    //刚体
    private Rigidbody rb;
    //石头状态
    public RockStates rockStates;

    [Header("Basic Settings")]
    public float force;//力度
    public int damage;//攻击值
    public GameObject target;//攻击目标
    private Vector3 direction;//方向
    public GameObject breakEffect;//破碎效果

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //初始速度都为1
        rb.velocity = Vector3.one;

        //初始状态为攻击玩家
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    //FixedUpdate进行物理判断
    void FixedUpdate()
    {
        //石头刚体小于1，石头切换为无的状态
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    //飞向目标
    public void FlyToTarget()
    {
        //石头目标变空，找到角色物体作为目标
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        //获取方向
        direction = (target.transform.position - transform.position).normalized;
        //刚体添加力
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    //石头发生碰撞逻辑
    private void OnCollisionEnter(Collision collision)
    {
        switch (rockStates)
        {
            //打到角色
            case RockStates.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))
                {
                    //停止移动
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    //给方向添加一个力
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    //击晕动画
                    collision.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    //计算攻击数值
                    collision.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, collision.gameObject.GetComponent<CharacterStats>());
                    //攻击完成切换状态
                    rockStates = RockStates.HitNothing;
                    //生成石头破碎效果
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
                //攻击敌人状态
            case RockStates.HitEnemy:
                if (collision.gameObject.GetComponent<Golem>())
                {
                    //获得敌人Stats
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    //计算攻击数值
                    otherStats.TakeDamage(damage, otherStats);
                    //生成石头破碎效果
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing:

                break;
        }
    }
}
