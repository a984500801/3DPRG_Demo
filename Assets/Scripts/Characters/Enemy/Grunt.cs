using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10;

    ////Animation Event，击晕角色函数
    public void KickOff()
    {
        //攻击目标不为空
        if (attackTarget != null)
        {
            //看的方向
            transform.LookAt(attackTarget.transform);

            //记录方向
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();

            //更改目标agent，停止移动
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            //赋予一个击飞的力
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //播放眩晕动画
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
