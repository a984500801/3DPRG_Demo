using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 25;
    //ʯͷprefab
    public GameObject rockPrefab;
    //����ʯͷ��λ��
    public Transform handPos;

    //Animation Event�����ɽ�ɫ����
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            //��ȡ����Ŀ�����
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            //��ȡ���򣬹���Ŀ����������������л�
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //direction.Normalize();
            //�ر�agent
            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            //����
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            //����
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");

            //������ֵ����
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    //Animation Event �ӳ�ʯͷ
    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            //����ʯͷ
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            //�ѹ���Ŀ�긳��ʯͷĿ��
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }


}
