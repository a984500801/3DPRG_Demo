using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    //ö��ֵ��ʯͷ״̬�л�
    public enum RockStates { HitPlayer,HitEnemy,HitNothing }
    //����
    private Rigidbody rb;
    //ʯͷ״̬
    public RockStates rockStates;

    [Header("Basic Settings")]
    public float force;//����
    public int damage;//����ֵ
    public GameObject target;//����Ŀ��
    private Vector3 direction;//����
    public GameObject breakEffect;//����Ч��

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //��ʼ�ٶȶ�Ϊ1
        rb.velocity = Vector3.one;

        //��ʼ״̬Ϊ�������
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }

    //FixedUpdate���������ж�
    void FixedUpdate()
    {
        //ʯͷ����С��1��ʯͷ�л�Ϊ�޵�״̬
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    //����Ŀ��
    public void FlyToTarget()
    {
        //ʯͷĿ���գ��ҵ���ɫ������ΪĿ��
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        //��ȡ����
        direction = (target.transform.position - transform.position).normalized;
        //���������
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    //ʯͷ������ײ�߼�
    private void OnCollisionEnter(Collision collision)
    {
        switch (rockStates)
        {
            //�򵽽�ɫ
            case RockStates.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))
                {
                    //ֹͣ�ƶ�
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    //���������һ����
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    //���ζ���
                    collision.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    //���㹥����ֵ
                    collision.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, collision.gameObject.GetComponent<CharacterStats>());
                    //��������л�״̬
                    rockStates = RockStates.HitNothing;
                    //����ʯͷ����Ч��
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
                //��������״̬
            case RockStates.HitEnemy:
                if (collision.gameObject.GetComponent<Golem>())
                {
                    //��õ���Stats
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    //���㹥����ֵ
                    otherStats.TakeDamage(damage, otherStats);
                    //����ʯͷ����Ч��
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing:

                break;
        }
    }
}
