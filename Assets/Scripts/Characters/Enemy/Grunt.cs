using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10;

    ////Animation Event�����ν�ɫ����
    public void KickOff()
    {
        //����Ŀ�겻Ϊ��
        if (attackTarget != null)
        {
            //���ķ���
            transform.LookAt(attackTarget.transform);

            //��¼����
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();

            //����Ŀ��agent��ֹͣ�ƶ�
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            //����һ�����ɵ���
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //����ѣ�ζ���
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
