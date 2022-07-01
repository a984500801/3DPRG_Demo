using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
        Useable,Weapon,Shield
}

[CreateAssetMenu(fileName="New Item",menuName="Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemIcon;
    public int itemAmount;
    [TextArea]
    public string description = "";
    public bool stackable;

    [Header("Weapon")]
    public GameObject weaponPrefab;
    public GameObject shieldPrefab;
    public AttackData_SO weaponData;
    public AttackData_SO shieldData;
    public AnimatorOverrideController weaponAnimator;

    [Header("Useable Item")]
    public UseableItemData_SO useableData;
}
