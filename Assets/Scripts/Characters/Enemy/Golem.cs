using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 25;
    //石头prefab
    public GameObject rockPrefab;
    //产生石头的位置
    public Transform handPos;

    //Animation Event，击飞角色函数
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            //获取攻击目标组件
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            //获取方向，攻击目标与自身相减并序列化
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //direction.Normalize();
            //关闭agent
            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            //击飞
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //击晕
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");

            //攻击数值计算
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    //Animation Event 扔出石头
    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            //生成石头
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            //把攻击目标赋给石头目标
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }


}
