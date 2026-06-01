using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;    //一个是现在血量，一个是满血量。事件系统，Action是一个委托，在TakeDamage方法中调用这个事件来更新血条UI
    public CharacterData_SO templateData;   //角色属性模板数据，方便在编辑器中设置不同角色和敌人的属性
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    private AttackData_SO baseAttackData;    //基础攻击数据（手刀）
    private RuntimeAnimatorController baseAnimator;    //基础动画控制器（手刀）

    [Header("Weapon")]
    public Transform weaponSlot;

    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        if(templateData != null)
        {
            characterData=Instantiate(templateData);   //实例化模板数据，确保每个角色都有独立的属性数据，避免修改一个角色的属性会影响其他角色
        }
        baseAttackData=Instantiate(attackData);   //实例化攻击数据，确保每个角色都有独立的攻击数据
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }

    #region Read from Data_SO
    public int MaxHealth
    { //get和set说明可读写属性，外部可以访问和修改
        get   
        {
            if(characterData != null)
                return characterData.maxHealth;
            return 0;
        }

        set
        {
            characterData.maxHealth = value;
        }
        
     }         
    public int CurrentHealth
    { //get和set说明可读写属性，外部可以访问和修改
        get   
        {
            if(characterData != null)
                return characterData.currentHealth;
            return 0;
        }

        set
        {
            characterData.currentHealth = value;
        }
        
     }         
    public int BaseDefence
    { //get和set说明可读写属性，外部可以访问和修改
        get   
        {
            if(characterData != null)
                return characterData.baseDefence;
            return 0;
        }

        set
        {
            characterData.baseDefence = value;
        }
        
     }         
    public int CurrentDefence
    { //get和set说明可读写属性，外部可以访问和修改
        get   
        {
            if(characterData != null)
                return characterData.currentDefence;
            return 0;
        }

        set
        {
            characterData.currentDefence = value;
        }
        
     }
    #endregion

    #region  Character Combat
    public void TakeDamage(CharacterStats attacker,CharacterStats defender)
    {
        int damage = Mathf.Max((attacker.CurrentDamage() - defender.CurrentDefence),0);
        CurrentHealth=Mathf.Max(CurrentHealth - damage,0);

        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");     //想在暴击时才播放受击动画，统一用Trigger“Hit"不用分别在玩家和敌人的代码里写了
        }
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);   //调用事件来更新血条UI，传入当前血量和满血量

        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);   //击败敌人后，攻击者获得经验值，调用角色数据中的方法来更新经验值
        }
    }

    public void TakeDamage(int damage,CharacterStats defender)    //重载TakeDamage方法，允许直接传入伤害值，（用于石头攻击）适用于一些特殊技能或效果直接造成固定伤害的情况
    {
        int currentDamage = Mathf.Max(damage - defender.CurrentDefence, 0);
        CurrentHealth=Mathf.Max(CurrentHealth - currentDamage, 0);
        // if (isCritical)
        //{
        //    defender.GetComponent<Animator>().SetTrigger("Hit");     //想在暴击时才播放受击动画，统一用Trigger“Hit"不用分别在玩家和敌人的代码里写了
        //}
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);   //石头攻击也能获得经验值，调用角色数据中的方法来更新经验值
    }

    private int CurrentDamage()
    {
        float coreDamage=UnityEngine.Random.Range(attackData.minDamage,attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击"+coreDamage);
        }
        return (int)coreDamage;
    }
    #endregion

    #region Equip Weapon
    public void ChangeWeapon(ItemData_SO weapon)   //换武器的方法，先卸下当前武器，再装备新武器
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    public void EquipWeapon(ItemData_SO weapon)
    {
        if(weapon.weaponPrefab!=null)
        {
            Instantiate(weapon.weaponPrefab, weaponSlot);
            //这里可以添加一些逻辑来处理换武器，比如销毁之前的武器，或者把新武器放在特定的位置等等
        }
        attackData.ApplyWeaponData(weapon.weaponData);
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;   //换武器时也要换动画控制器，确保攻击动画和武器匹配
        InventoryManager.Instance.UpdateStatsText(MaxHealth,attackData.minDamage,attackData.maxDamage);
    }

    public void UnEquipWeapon()
    {
        if (weaponSlot.transform.childCount != 0)    //说明武器槽里有武器
        {
            for(int i=0;i<weaponSlot.transform.childCount;i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);   //销毁武器槽里的武器
            }
        }
        attackData.ApplyWeaponData(baseAttackData);
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
        InventoryManager.Instance.UpdateStatsText(MaxHealth, attackData.minDamage, attackData.maxDamage);
    }
    #endregion

    #region Apply Data Change
    public void ApplyHealth(int amount)    //吃蘑菇后可以回血
    {
        if(CurrentHealth+amount<=MaxHealth)
        {
            CurrentHealth += amount;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }
    }
    #endregion
}
