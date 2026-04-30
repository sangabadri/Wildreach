using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class ItemSlot : MonoBehaviour, IDropHandler
{

    private int itemCount = 0;
    public Text itemCountText;
    private int itemsPerSlot = 0;

    public GameObject Item()
    {

        if (transform.childCount > 1)
        {
            return transform.GetChild(0).gameObject;
        }

        return null;
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");

        GameObject draggedItem = DragDrop.itemBeingDragged.gameObject;
        GameObject draggedItemParent = DragDrop.itemBeingDraggedParent.gameObject;
        draggedItem.transform.SetParent(draggedItemParent.transform);
        draggedItem.transform.SetSiblingIndex(0);
        if (!Item() && draggedItemParent.CompareTag("QuickSlot"))
        {
            EquipSystem.Instance.UnMapItemList(draggedItemParent);
            draggedItem.GetComponent<InventoryItem>().SetIsInQuickSlot(false);
            draggedItem.transform.SetParent(transform);
            draggedItem.transform.SetSiblingIndex(0);
            InventorySystem.Instance.MapItemList(gameObject, draggedItem.GetComponent<InventoryItem>().itemName);
        }
        else if (!Item() && draggedItemParent.CompareTag("ItemSlot"))
        {
            int count = InventorySystem.Instance.UnMapItemList(draggedItemParent);
            draggedItem.transform.SetParent(transform);
            draggedItem.transform.SetSiblingIndex(0);
            InventorySystem.Instance.MapItemList(gameObject, draggedItem.GetComponent<InventoryItem>().itemName);
            SetItemCount(count);
        }
        else if(Item() && draggedItemParent.CompareTag("ItemSlot") && Item().GetComponent<InventoryItem>().itemName == draggedItem.GetComponent<InventoryItem>().itemName)
        {
            if(itemCount + draggedItemParent.GetComponent<ItemSlot>().GetItemCount() <= itemsPerSlot)
            {
                int count = InventorySystem.Instance.UnMapItemList(draggedItemParent);
                Destroy(draggedItem);
                SetItemCount(itemCount + count);
            }
            else
            {
                int gap = itemsPerSlot - itemCount;
                SetItemCount(itemsPerSlot);
                draggedItemParent.GetComponent<ItemSlot>().SetItemCount(draggedItemParent.GetComponent<ItemSlot>().GetItemCount() - gap);

            }
        }

    }

    public void SetItemCount(int count) {
        itemCount = count;
        itemCountText.text = $"{itemCount}";
    }

    public int GetItemCount() { 
        return itemCount; 
    }

    public void SetItemsPerSlot(int count) {
        itemsPerSlot = count;
    }

    public int GetItemsPerSlot() {
        return itemsPerSlot;
    }


}