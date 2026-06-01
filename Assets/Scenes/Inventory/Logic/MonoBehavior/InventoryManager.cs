using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData     //用于在拖动物品时记录原始信息，方便在放下物品时进行交换
    {
        public SlotHolder originalHolder;
        public RectTransform originalParent;   //使用RectTransform是因为UI元素通常使用RectTransform组件，能够判断鼠标在哪个格子的范围上
    }


    //该代码是挂载到背包canvas，负责背包数据的管理
    [Header("Inventory Data")]
    public InventoryData_SO inventoryTemplate;
    public InventoryData_SO actionTemplate;
    public InventoryData_SO equipmentTemplate;

    public InventoryData_SO inventoryData;
    public InventoryData_SO actionData;
    public InventoryData_SO equipmentData;

    [Header("ContainerS")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Drag Cnavas")]
    public Canvas dragCanvas;
    public DragData currentDrag;      //当开始拖拽时，记录当前正在被拖动的物品信息

    [Header("UI panel")]
    bool isOpen = false;
    public GameObject bagPanel;
    public GameObject statsPanel;

    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;

    [Header("Tooltip")]
    public ItemTooltip tooltip;

    protected override void Awake()
    {
        base.Awake();
        if(inventoryTemplate != null )
        inventoryData = Instantiate(inventoryTemplate);   //实例化背包数据，确保每个角色有独立的背包数据
        if(actionTemplate!=null)
        actionData = Instantiate(actionTemplate);
        if(equipmentTemplate!=null)
        equipmentData = Instantiate(equipmentTemplate);
    }
    private void Start()    
    {
        LoadData();   //加载数据，确保在游戏开始时背包数据被正确加载
        inventoryUI.RefreshUI();   //在游戏开始时刷新背包UI显示
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            statsPanel.SetActive(isOpen);
        }
    }

    public void SaveData()    //保存
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);   //保存背包数据，第二个参数是保存文件的名字，可以根据需要修改
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    public void LoadData()   //加载
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);   //加载背包数据，第二个参数是保存文件的名字，可以根据需要修改
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }

    public void UpdateStatsText(int health,int min,int max)   //更新角色属性UI
    {
        healthText.text =health.ToString();
        attackText.text =min+"-"+max;
    }

    #region 检查拖拽的物体是否在每一个Slot范围内
    public bool CheckInInventoryUI(Vector3 position)
    {
        for(int i = 0;i<inventoryUI.slotHolders.Length;i++)    //遍历每一个格子
        {
            RectTransform t=inventoryUI.slotHolders[i].transform as RectTransform;    //拿到每一个格子的范围

            if(RectTransformUtility.RectangleContainsScreenPoint(t,position))   //判断鼠标位置是否在格子范围内
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckInActionUI(Vector3 position)
    {
        for(int i = 0;i<actionUI.slotHolders.Length;i++)    //遍历每一个格子
        {
            RectTransform t=actionUI.slotHolders[i].transform as RectTransform;    //拿到每一个格子的范围
            if(RectTransformUtility.RectangleContainsScreenPoint(t,position))   //判断鼠标位置是否在格子范围内
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckInEquipmentUI(Vector3 position)
    {
        for(int i = 0;i<equipmentUI.slotHolders.Length;i++)    //遍历每一个格子
        {
            RectTransform t=equipmentUI.slotHolders[i].transform as RectTransform;    //拿到每一个格子的范围
            if(RectTransformUtility.RectangleContainsScreenPoint(t,position))   //判断鼠标位置是否在格子范围内
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
        foreach(var item in inventoryData.items)
        {
            if(item.itemData != null && item.itemData.itemName == questItemName)  //如果背包里有这个物品了
            {
                QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);  //更新任务进度，把这个物品的名字和数量传给任务系统，更新任务进度
            }
        }
        foreach(var item in actionData.items)
        {
            if(item.itemData != null && item.itemData.itemName == questItemName)  //如果背包里有这个物品了
            {
                QuestManager.Instance.UpdateQuestProgress(item.itemData.itemName, item.amount);  //更新任务进度，把这个物品的名字和数量传给任务系统，更新任务进度
            }
        }
    }
    #endregion

    //检查背包和快捷栏的物品是否满足任务需求，更新任务进度
    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.items .Find(i=>i.itemData ==questItem);
    }
    public InventoryItem QuestItemInAction(ItemData_SO questItem)
    {
        return actionData.items .Find(i=>i.itemData ==questItem);
    }
}
