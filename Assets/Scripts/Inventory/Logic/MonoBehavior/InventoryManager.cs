using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    //��ק�����࣬��¼ԭʼ�������ԭʼ����
    public class DragData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }

    //�������ݿ⣬���ڱ�������
    [Header("Inventory Data")]
    public InventoryData_SO inventoryTemplate;
    public InventoryData_SO inventoryData;
    public InventoryData_SO actionTemplate;
    public InventoryData_SO actionData;
    public InventoryData_SO equipmentTemplate;
    public InventoryData_SO equipmentData;

    //����UI���
    [Header("ContainerS")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    //��ק��ʱ���
    [Header("Drag Canvas")]
    public Canvas dragCanvas;
    public DragData currentDrag;

    //�����������
    [Header("UI Panel")]
    public GameObject bagPanel;
    public GameObject statsPanel;

    //�Ƿ��
    bool isOpen = false;

    //��������ı�
    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;
    public Text defenceText;

    //��ʾ��Ϣ���
    [Header("Tooltip")]
    public ItemTooltip tooltip;

    protected override void Awake()
    {
        base.Awake();
        //���ݲ�Ϊ�����ɳ�ʼ����
        if (inventoryTemplate != null)
            inventoryData = Instantiate(inventoryTemplate);
        if (actionTemplate != null)
            actionData = Instantiate(actionTemplate);
        if (equipmentTemplate != null)
            equipmentData = Instantiate(equipmentTemplate);
    }



    void Start()
    {
        //��ʼʱ��������
        LoadData();

        //ˢ�����еı���
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))//����������忪��
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            statsPanel.SetActive(isOpen);
        }

        //���ø����ı������������������ݵĶ�Ӧ����
        UpdateStatsText(GameManager.Instance.playerStats.MaxHealth, GameManager.Instance.playerStats.attackData.minDamage, GameManager.Instance.playerStats.attackData.maxDamage, (int)GameManager.Instance.playerStats.attackData.baseDefence);
    }

    //��������
    public void SaveData()
    {
        //�������ݺ��������ֽ��б���
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    //��������
    public void LoadData()
    {
        //�������ݺ��������ּ���
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }

    //�������������ı�
    public void UpdateStatsText(int health,int min,int max,int defence)
    {
        healthText.text = health.ToString();
        attackText.text = min + " �� " + max;
        defenceText.text = defence.ToString();
    }

    #region �����ק��Ʒ�Ƿ���ÿһ�� Slot ��Χ��
    public bool CheckInInventoryUI(Vector3 position)
    {
        //ѭ��ÿ��������
        for (int i = 0; i < inventoryUI.slotHolders.Length; i++)
        {
            RectTransform t = inventoryUI.slotHolders[i].transform as RectTransform;//��ȡ�������transform

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))//�ж����λ���Ƿ�����ڸ��ӷ�Χ��
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform t = actionUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckInEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            RectTransform t = equipmentUI.slotHolders[i].transform as RectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region ���������Ʒ
    public void CheckQuestItemInBag(string questItemName)
    {
        foreach (var item in inventoryData.items)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == questItemName)
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
            }
        }

        foreach (var item in actionData.items)
        {
            if (item.itemData != null)
            {
                if (item.itemData.itemName == questItemName)
                    QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);
            }
        }
    }
    #endregion


    //��ⱳ���Ϳ������Ʒ���Ƿ���Ҫ�ύ������
    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.items.Find(i => i.itemData == questItem);
    }

    public InventoryItem QuestItemInAction(ItemData_SO questItem)
    {
        return actionData.items.Find(i => i.itemData == questItem);
    }
}
