using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]

public class EnemyController : MonoBehaviour,IEndGameObserver
{
    //����״̬
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;

    //�������Ե����
    protected CharacterStats characterStats;

    [Header("Basic Settings")]
    //���˿��ӷ�Χ
    public float sightRadius;
    //վ׮ģʽ�л�
    public bool isGuard;
    //��¼ԭ���ٶ�
    private float speed;
    //����Ŀ��
    protected GameObject attackTarget;
    //Ѳ��ͣ�¿���ʱ��
    public float lookAtTime;
    //�鿴��ʱ��ʱ��
    private float remainLookAtTime;
    //������ȴʱ��
    private float lastAttackTime;
    //��¼��ת�Ƕ�
    private Quaternion guardRotation;


    [Header("Patrol State")]
    //Ѳ�߷�Χ
    public float patrolRange;
    //·�������
    private Vector3 wayPoint;
    //���˳�ʼ����λ��
    private Vector3 guardPos;

    //bool��϶���ת��
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    //��ɫ����
    bool playerDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        //��¼ԭ��agent���ٶ�
        speed = agent.speed;
        //��¼�ʼ������
        guardPos = transform.position;
        //��¼ԭʼ�ĽǶ�
        guardRotation = transform.rotation;
        //�鿴ʱ�丳ֵ
        remainLookAtTime = lookAtTime;
    }

    void Start()
    {
        //����״̬
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else//Ѳ��״̬
        {
            enemyStates = EnemyStates.PATROL;
            //��ʼһ�����ƶ��ĵ�
            GetNewWayPoint();
        }
        //FIXME:�����л�����
        GameManager.Instance.AddObserver(this);
    }

    //�л�����ʱ����
    //void OnEnable()
    //{
           //����۲�
    //    GameManager.Instance.AddObserver(this);
    //}

    void OnDisable()
    {
        //GameManagerû������ֱ�ӷ���
        if (!GameManager.IsInitialized) return;
        //���б��Ƴ�
        GameManager.Instance.RemoveObserver(this);

        if (GetComponent<LootSpawner>() && isDead)      //������������Ʒ
            GetComponent<LootSpawner>().SpawnLoot();

        if (QuestManager.IsInitialized && isDead)
        {
            QuestManager.Instance.UpdateQuestProgress(this.name, 1);
        }
    } 

    void Update()
    {
        //Ѫ��ֵΪ0����Ϊtrue
        if (characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }
        //��ɫû������ִ���л�״̬���л�����
        if(!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
        
    }

    //�л�״̬����
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
        //�ж������л�����״̬
        if (isDead)
            enemyStates = EnemyStates.DEAD;
        //�������player �л���CHASE
        else if (FoundPlayer())
            enemyStates = EnemyStates.CHASE;

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                //�޸�׷��״̬
                isChase = false;

                //λ�ò��ڳ�ʼ��λ��
                if (transform.position != guardPos)
                {
                    //��ʼ�߶�
                    isWalk = true;
                    agent.isStopped = false;
                    //Ŀ������Ϊ��ʼλ��
                    agent.destination = guardPos;
                }

                //�жϵ�ǰλ�úͳ�ʼλ�õľ����Ƿ�С��ֹͣ����
                if (Vector3.Distance(guardPos, transform.position) <= agent.stoppingDistance)
                {
                    //ֹͣ�߶�
                    isWalk = false;
                    //����ǰ�Ƕ���תΪ��ʼ�Ƕ�
                    transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                }
                break;
            case EnemyStates.PATROL:
                //׷��״̬false
                isChase = false;
                //Ѳ���ٶȱ������ٶ�С
                agent.speed = speed * 0.5f;

                //�ж��Ƿ������Ѳ�ߵ�
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    //�ƶ�״̬false
                    isWalk = false;
                    //�ж�ʱ���Ƿ�Ϊ0
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();//��ȡ�µ�Ѳ�ߵ�
                }
                else
                {
                    //û��Ѳ�ߵ㣬�����ƶ�
                    isWalk = true;
                    //Ŀ�����wayPoint
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                //׷player״̬����
                isWalk = false;
                isChase = true;
                
                //׷�����ٶ�
                agent.speed = speed;
                
                //û�з��ֵ���
                if (!FoundPlayer())
                {
                    //���ѻص���һ��״̬
                    //���涯��false
                    isFollow = false;
                    //ͣ�¹۲�ʱ�䲻Ϊ0
                    if (remainLookAtTime > 0)
                    {
                        //����ǰλ�������agent
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)//ԭ��״̬������
                        enemyStates = EnemyStates.GUARD;
                    else//����Ѳ��״̬
                        enemyStates = EnemyStates.PATROL;
                }
                else
                {
                    //���涯��true
                    isFollow = true;
                    agent.isStopped = false;
                    //���ع���Ŀ������
                    agent.destination = attackTarget.transform.position;
                }

                //�ڹ�����Χ���򹥻�
                if(TargetInAttackRange() || TargetInSkillRange())
                {
                    //ͣ���ƶ�
                    isFollow = false;
                    agent.isStopped = true;

                    //�жϹ���ʱ��
                    if (lastAttackTime < 0)
                    {
                        //��ԭ������ȴʱ��
                        lastAttackTime = characterStats.attackData.coolDown;
                        //�����жϣ��������ֵС���������õı����ʷ���true
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //ִ�й���
                        Attack();
                    }
                }
                break;

            case EnemyStates.DEAD:
                //�ر���ײ��
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    //������Χ�Ƿ���player
    bool FoundPlayer()
    {
        //����һ��collider����
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            //�ҵ�Player
            if (target.CompareTag("Player"))
            {
                //��ȡ����Ŀ��
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    //��������
    void Attack()
    {
        //���򹥻�Ŀ��
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //����������
            anim.SetTrigger("Attack");
        }

        if (TargetInSkillRange())
        {
            //���ܹ�������
            anim.SetTrigger("Skill");
        }

    }

    //�жϽ����빥��
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            //�������͹���Ŀ��ľ���С�ڹ�������
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    //�жϼ��ܹ�����Χ
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            //�������͹���Ŀ��ľ���С�ڼ��ܹ�������
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }



    //Ѳ�ߵ�
    void GetNewWayPoint()
    {
        //��ԭֹͣ�鿴��ʱ��
        remainLookAtTime = lookAtTime;
        //������������
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        //���Vector3����
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        //NavMeshHit����
        NavMeshHit hit;
        //ָ����Χ�ҵ�������������ĵ�
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
        
    }

    //��Scene�л�������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        //����Χ
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    

    //Animation Event �����¼�
    void Hit()
    {
        //�жϹ���Ŀ�겻Ϊ�ղ����ж��Ƿ�����ǰ��
            if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
            {
            //��ù�����Stats
                var targetStats = attackTarget.GetComponent<CharacterStats>();
            //����TakeDamage
                targetStats.TakeDamage(characterStats, targetStats);
            }
    }

    //ʵ�ֹ۲��߽ӿ�
    public void EndNotify()
    {
        //���������Ż�ʤ����
        anim.SetBool("Win", true);
        //��ɫ����
        playerDead = true;
        //ֹͣ�����ƶ�
        isChase = false;
        isWalk = false;
        //ֹͣagent
        attackTarget = null;
    }
}
