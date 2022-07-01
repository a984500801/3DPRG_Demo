using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey;               //按键
    private SlotHolder currentSlotHolder;   //操作栏的背包格

    void Awake()
    {
        currentSlotHolder = GetComponent<SlotHolder>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(actionKey) && currentSlotHolder.itemUI.GetItem())//按下按键并且有物品
            currentSlotHolder.UseItem();    //使用物品
    }
}
