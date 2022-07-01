using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public KeyCode actionKey;               //����
    private SlotHolder currentSlotHolder;   //�������ı�����

    void Awake()
    {
        currentSlotHolder = GetComponent<SlotHolder>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(actionKey) && currentSlotHolder.itemUI.GetItem())//���°�����������Ʒ
            currentSlotHolder.UseItem();    //ʹ����Ʒ
    }
}
