using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;

    //Ŀ�깥������
    private GameObject attackTarget;
    //����ʱ��
    private float lastAttackTime;
    //�ж�����״̬
    private bool isDead;
    //agentͣ�¾���
    private float stopDistance;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        //��¼��ʼͣ�±���
        stopDistance = agent.stoppingDistance;
    }

    //����ʱ��Ӷ���
    void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        //����ע�������������
        GameManager.Instance.RegisterPlayer(characterStats);
    }

    void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }

    //�л�ʱȡ������
    void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
    }

    void Update()
    {
        //�Ƿ�����
        isDead = characterStats.CurrentHealth == 0;

        //��ɫ����ʱ�����й㲥
        if (isDead)
            GameManager.Instance.NotifyObservers();

        SwitchAnimation();
        //��������ȴʱ�����
        lastAttackTime -= Time.deltaTime;
    }

    //�л�����
    private void SwitchAnimation()
    {
        //agent�ٶ�ֵ�����ƶ�����
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        //��������
        anim.SetBool("Death", isDead);
    }

    //�����꣬����Vector3ֵ
    public void MoveToTarget(Vector3 target)
    {
        //ֹͣЭ��
        StopAllCoroutines();
        //��ɫ�Ѿ���������ִ��
        if (isDead) return;

        agent.stoppingDistance = stopDistance;
        //�ָ��ƶ�
        agent.isStopped = false;
        agent.destination = target;
    }


    //�¼����ù���
    private void EventAttack(GameObject target)
    {
        //�Ѿ�������ִ��
        if (isDead) return;

        //����Ŀ���Ƿ�Ϊ��
        if (target != null)
        {
            //��ֵ:����Ŀ��
            attackTarget = target;
            //�жϱ���
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            //ִ��Э��
            StartCoroutine(MoveToAttackTarget());
        }
    }

    //Э��
    IEnumerator MoveToAttackTarget()
    {
        //�ʼ�����ƶ�
        agent.isStopped = false;
        //����ʱ��ֹͣ����Ϊ��������
        agent.stoppingDistance = characterStats.attackData.attackRange;

        //����ת��Ŀ��
        transform.LookAt(attackTarget.transform);

        //�޸Ĺ�����Χ��������������
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            //����̫Զ���򹥻�Ŀ�������ƶ�
            agent.destination = attackTarget.transform.position;
            //��һ֡�ٴ�ִ��ѭ��
            yield return null;
        }

        //ִ�й���ǰ��ͣ����
        agent.isStopped = true;

        //Attack
        //������ȴʱ��
        if (lastAttackTime < 0)
        {
            //ִ�й�������
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //������ȴʱ��
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }


    //Animation Event�����¼�����
    void Hit()
    {
        //����Ŀ���ǿɹ���������
        if (attackTarget.CompareTag("Attackable"))
        {
            //�ж��Ƿ���ʯͷ�Լ�ʯͷ״̬����
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing) 
            {
                //����ʯͷ״̬Ϊ��������
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                //������Ŀ�긳���ٶ�
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                //��������ǰ���ɵ���
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            //��ȡ����Ŀ�����ϵ�Stats
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            //����TakeDamage���빥���ߺͷ�����Stats
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

}
