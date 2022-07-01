using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    //更新血量的事件
    public event Action<int, int> UpdateHealthBarOnAttack;
    //模板数据
    public CharacterData_SO templateData; 
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    private AttackData_SO baseAttackData;
    private RuntimeAnimatorController baseAnimator;

    //角色装备生成武器的位置
    [Header("Equipment")]
    public Transform weaponSlot;
    public Transform shieldSlot;

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        //模板数据不为空，生成一份给characterData
        if (templateData != null)
            characterData = Instantiate(templateData);

        baseAttackData = Instantiate(attackData);//生成原始的攻击数据
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }


    #region Read from Data_SO  读取数据
    public int MaxHealth
    {
        get
        {
            //数据不是空返回数据中的生命值
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

    #region Character Combat 攻击数值计算

    /// <summary>
    /// 攻击者攻击防御者伤害计算
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defener"></param>
    public void TakeDamage(CharacterStats attacker,CharacterStats defener)
    {
        //产生伤害值，数学判断防止负数值
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence,0);
        //计算生命值
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        //暴击播放受伤动画
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        //更新UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //更新人物经验，敌人死亡更新攻击者经验值
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    
    public void TakeDamage(int damage,CharacterStats defener)
    {
        //计算攻击值
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        //更新血量值
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        //更新UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);

    }

    //攻击者的伤害
    private int CurrentDamage()
    {
        //获取随机攻击值
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)//暴击时产生暴击伤害
        {
            coreDamage *= attackData.criticalMultiplier;//乘以暴击百分比
            Debug.Log("暴击：" + coreDamage);
        }
        //返回攻击数值
        return (int)coreDamage;
    }

    #endregion

    #region Equip Weapon
    //切换武器
    public void ChangeWeapon(ItemData_SO weapon){
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    //装备武器
    public void EquipWeapon(ItemData_SO weapon)
    {
        //武器不为空
        if (weapon.weaponPrefab != null)
        {
            //在角色下生成武器
            Instantiate(weapon.weaponPrefab, weaponSlot);
        }

        //更新属性
        attackData.ApplyWeaponData(weapon.weaponData);
        //切换动画
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;
        //InventoryManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
    }

    //卸下武器
    public void UnEquipWeapon()
    {
        if (weaponSlot.transform.childCount != 0)//存在子物体，有装备
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)//销毁所有子物体
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }
        attackData.ApplyWeaponData(baseAttackData);//还原成初始攻击数据
        //TODO:切换动画
        if (shieldSlot.transform.childCount == 0)//判断没有装备盾牌
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

        //TODO:更新属性
        attackData.ApplyShieldData(shield.shieldData);
        //TODO:切换动画
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
        //TODO:切换动画
        if(weaponSlot.transform.childCount == 0)//判断没有装备剑
        {
            GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
        }
    }

    #endregion


    #region Apply Data Change，实现道具的功能
    public void ApplyHealth(int amout)//实现血量变化
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
