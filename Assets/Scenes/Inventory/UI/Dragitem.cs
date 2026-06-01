using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemUI))]
public class Dragitem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    ItemUI currentItemUI;      //当前被拖动的物品UI组件
    SlotHolder currentHolder;     //用于交换物品格子
    SlotHolder targetHolder;

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentHolder=GetComponentInParent<SlotHolder>();    //格子就是该物品的父级
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();
        InventoryManager.Instance.currentDrag.originalHolder = GetComponentInParent<SlotHolder>();
        InventoryManager.Instance.currentDrag.originalParent = transform.parent as RectTransform;
        //记录原始信息// 记录原始的格子Holder//记录物品UI原来的父物体（RectTransform类型）

        transform.SetParent(InventoryManager.Instance.dragCanvas.transform,true);   //临时将物品UI移动到Drag Canvas下，确保它在最上层显示
    }

    public void OnDrag(PointerEventData eventData)
    {
        //跟随鼠标位置移动
        transform.position = eventData.position; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //放下物品，交换数据
        //是否指向UI物体
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if(InventoryManager.Instance.CheckInInventoryUI(eventData.position)|| InventoryManager.Instance.CheckInActionUI(eventData.position)||
               InventoryManager.Instance.CheckInEquipmentUI(eventData.position))
            {
                //因为鼠标会把本格子里的Slot拖走，所以看看鼠标指向的物体是否有SlotHolder组件，如果有就说明找到了目标(空的)（其他）格子，没有就说明还是本格子
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                {
                    //找到目标（空的）（其他）格子
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                }
                else
                {
                    //如果检测不到SlotHolder,可能是本格子上已经有物品了，只能检测到该物品的Image，所以可使用其父级的组件
                    targetHolder=eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>(); 
                }

                //拖拽的物品格子和目标格子不是同一个格子才进行交换（如果是同一个格子就不需要交换了）
                if (targetHolder != InventoryManager.Instance.currentDrag.originalHolder)
                switch (targetHolder.slotType)   //根据目标格子类型判断是否可以放置，并进行交换
                {
                    case SlotType.BAG:
                            SwapItem();
                        break;
                    case SlotType.WEAPON:
                        if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)   //只有可使用的才能放在快捷栏
                            SwapItem();
                        break;
                    case SlotType.ARMOR:
                        if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Armor)   //只有可使用的才能放在快捷栏
                            SwapItem();
                        break;
                    case SlotType.ACTION:
                        if (currentItemUI.Bag.items[currentItemUI.Index].itemData.itemType==ItemType.Useable)   //只有可使用的才能放在快捷栏
                            SwapItem();
                        
                        break;
                }

                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }

        //无论如何都要把物品格子本身放回原位（即使放置失败或放到空白处）
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);

        // 重置RectTransform的偏移，让物品UI完全贴合原来的格子（视觉上归位）
        RectTransform t =transform as RectTransform;
        t.offsetMax = Vector2.zero;
        t.offsetMin = Vector2.zero;
    }
    public void SwapItem()
    {
        var targetItem = targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index];   //拿到目标格子里物品在其背包里面相应序号的物品
        var tempItem=currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index];   //当前拖拽的物品对应背包里实际的物品

        bool isSameItem=tempItem.itemData==targetItem.itemData;   //判断是否是同种物品
        if (isSameItem && targetItem.itemData.Stackable)
        {
            targetItem.amount += tempItem.amount;   //如果是同种物品且可堆叠，就把数量加在一起
            tempItem.itemData = null;   //原格子物品数据清空
            tempItem.amount = 0;
        }
        else   //不是相同的物品或者不可堆叠，就直接交换
        {
            targetHolder.itemUI.Bag.items[targetHolder.itemUI.Index] = tempItem;
            currentHolder.itemUI.Bag.items[currentHolder.itemUI.Index] = targetItem;
        }
    }
}
