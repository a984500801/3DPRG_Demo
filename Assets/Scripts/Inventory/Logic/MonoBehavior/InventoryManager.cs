using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    //拖拽数据类，记录原始背包格和原始父级
    public class DragData
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;
    }

    //背包数据库，用于保存数据
    [Header("Inventory Data")]
    public InventoryData_SO inventoryTemplate;
    public InventoryData_SO inventoryData;
    public InventoryData_SO actionTemplate;
    public InventoryData_SO actionData;
    public InventoryData_SO equipmentTemplate;
    public InventoryData_SO equipmentData;

    //背包UI组件
    [Header("ContainerS")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    //拖拽临时面板
    [Header("Drag Canvas")]
    public Canvas dragCanvas;
    public DragData currentDrag;

    //背包面板的组件
    [Header("UI Panel")]
    public GameObject bagPanel;
    public GameObject statsPanel;

    //是否打开
    bool isOpen = false;

    //人物面板文本
    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;
    public Text defenceText;

    //提示信息面板
    [Header("Tooltip")]
    public ItemTooltip tooltip;

    protected override void Awake()
    {
        base.Awake();
        //数据不为空生成初始数据
        if (inventoryTemplate != null)
            inventoryData = Instantiate(inventoryTemplate);
        if (actionTemplate != null)
            actionData = Instantiate(actionTemplate);
        if (equipmentTemplate != null)
            equipmentData = Instantiate(equipmentTemplate);
    }



    void Start()
    {
        //开始时加载数据
        LoadData();

        //刷新所有的背包
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))//按键控制面板开关
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            statsPanel.SetActive(isOpen);
        }

        //调用更新文本方法，传入人物数据的对应属性
        UpdateStatsText(GameManager.Instance.playerStats.MaxHealth, GameManager.Instance.playerStats.attackData.minDamage, GameManager.Instance.playerStats.attackData.maxDamage, (int)GameManager.Instance.playerStats.attackData.baseDefence);
    }

    //保存数据
    public void SaveData()
    {
        //传入数据和数据名字进行保存
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    //加载数据
    public void LoadData()
    {
        //传入数据和数据名字加载
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }

    //更新人物面板的文本
    public void UpdateStatsText(int health,int min,int max,int defence)
    {
        healthText.text = health.ToString();
        attackText.text = min + " — " + max;
        defenceText.text = defence.ToString();
    }

    #region 检查拖拽物品是否在每一个 Slot 范围内
    public bool CheckInInventoryUI(Vector3 position)
    {
        //循环每个背包格
        for (int i = 0; i < inventoryUI.slotHolders.Length; i++)
        {
            RectTransform t = inventoryUI.slotHolders[i].transform as RectTransform;//获取背包格的transform

            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))//判断鼠标位置是否包含在格子范围内
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

    #region 检测任务物品
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


    //检测背包和快捷栏物品，是否需要提交给任务
    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.items.Find(i => i.itemData == questItem);
    }

    public InventoryItem QuestItemInAction(ItemData_SO questItem)
    {
        return actionData.items.Find(i => i.itemData == questItem);
    }
}
