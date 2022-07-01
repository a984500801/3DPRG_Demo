using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    //����Ѫ�����¼�
    public event Action<int, int> UpdateHealthBarOnAttack;
    //ģ������
    public CharacterData_SO templateData; 
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    private AttackData_SO baseAttackData;
    private RuntimeAnimatorController baseAnimator;

    //��ɫװ������������λ��
    [Header("Equipment")]
    public Transform weaponSlot;
    public Transform shieldSlot;

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        //ģ�����ݲ�Ϊ�գ�����һ�ݸ�characterData
        if (templateData != null)
            characterData = Instantiate(templateData);

        baseAttackData = Instantiate(attackData);//����ԭʼ�Ĺ�������
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }


    #region Read from Data_SO  ��ȡ����
    public int MaxHealth
    {
        get
        {
            //���ݲ��ǿշ��������е�����ֵ
            if (characterData != null)
                return characterData.maxHealth;
            else
                return 0;
        }
        set
        {
            characterData.maxHealth = value;
        }
    }

    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else
                return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }
    }

    public int BaseDefence
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;
            else
                return 0;
        }
        set
        {
            characterData.baseDefence = value;
        }
    }

    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
                return characterData.currentDefence;
            else
                return 0;
        }
        set
        {
            characterData.currentDefence = value;
        }
    }
    #endregion

    #region Character Combat ������ֵ����

    /// <summary>
    /// �����߹����������˺�����
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defener"></param>
    public void TakeDamage(CharacterStats attacker,CharacterStats defener)
    {
        //�����˺�ֵ����ѧ�жϷ�ֹ����ֵ
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);
        //��������ֵ
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        //�����������˶���
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        //����UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //�������ﾭ�飬�����������¹����߾���ֵ
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    
    public void TakeDamage(int damage,CharacterStats defener)
    {
        //���㹥��ֵ
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        //����Ѫ��ֵ
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        //����UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);

    }

    //�����ߵ��˺�
    private int CurrentDamage()
    {
        //��ȡ�������ֵ
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)//����ʱ���������˺�
        {
            coreDamage *= attackData.criticalMultiplier;//���Ա����ٷֱ�
            Debug.Log("������" + coreDamage);
        }
        //���ع�����ֵ
        return (int)coreDamage;
    }

    #endregion

    #region Equip Weapon
    //�л�����
    public void ChangeWeapon(ItemData_SO weapon){
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    //װ������
    public void EquipWeapon(ItemData_SO weapon)
    {
        //������Ϊ��
        if (weapon.weaponPrefab != null)
        {
            //�ڽ�ɫ����������
            Instantiate(weapon.weaponPrefab, weaponSlot);
        }

        //��������
        attackData.ApplyWeaponData(weapon.weaponData);
        //�л�����
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;
        //InventoryManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
    }

    //ж������
    public void UnEquipWeapon()
    {
        if (weaponSlot.transform.childCount != 0)//���������壬��װ��
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)//��������������
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }
        attackData.ApplyWeaponData(baseAttackData);//��ԭ�ɳ�ʼ��������
        //TODO:�л�����
        if (shieldSlot.transform.childCount == 0)//�ж�û��װ������
        {
            GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
        }     
    }

    public void ChangeShield(ItemData_SO shield)
    {
        UnEquipShield();
        EquipShield(shield);
    }

    public void EquipShield(ItemData_SO shield)
    {
        if (shield.shieldPrefab != null)
        {
            Instantiate(shield.shieldPrefab, shieldSlot) ;
        }

        //TODO:��������
        attackData.ApplyShieldData(shield.shieldData);
        //TODO:�л�����
        GetComponent<Animator>().runtimeAnimatorController = shield.weaponAnimator;
        //InventoryManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
    }

    public void UnEquipShield()
    {
        if (shieldSlot.transform.childCount != 0)
        {
            for (int i = 0; i < shieldSlot.transform.childCount; i++)
            {
                Destroy(shieldSlot.transform.GetChild(i).gameObject);
            }
        }
        attackData.ApplyShieldData(baseAttackData);
        //TODO:�л�����
        if(weaponSlot.transform.childCount == 0)//�ж�û��װ����
        {
            GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
        }
    }

    #endregion


    #region Apply Data Change��ʵ�ֵ��ߵĹ���
    public void ApplyHealth(int amout)//ʵ��Ѫ���仯
    {
        if (CurrentHealth + amout <= MaxHealth)
        {
            CurrentHealth += amout;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }
    }
    #endregion
}
