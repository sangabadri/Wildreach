using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour, IDropHandler
{
    private GameObject background;
    private Text quickSlotIndex;
    private int index;
    private bool isFull = false;
    private bool isEquiped = false;

    public GameObject Item()
    {

        if (transform.childCount > 2)
        {
            return transform.GetChild(0).gameObject;
        }

        return null;
    }

    private void Awake()
    {
        background = transform.Find("Background").gameObject;
        quickSlotIndex = transform.Find("Image/QuickSlotIndex").GetComponent<Text>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");

        GameObject draggedItem = DragDrop.itemBeingDragged.gameObject;
        GameObject draggedItemParent = DragDrop.itemBeingDraggedParent;
        draggedItem.transform.SetParent(draggedItemParent.transform);
        draggedItem.transform.SetSiblingIndex(0);
        if (!Item() && draggedItemParent.CompareTag("QuickSlot"))
        {
            bool equipped = draggedItemParent.GetComponent<QuickSlot>().GetIsEquiped();
            if (equipped)
            {
                EquipSystem.Instance.UnequipItem();
            }
            EquipSystem.Instance.UnMapItemList(draggedItemParent);
            draggedItem.transform.SetParent(transform);
            draggedItem.transform.SetSiblingIndex(0);
            EquipSystem.Instance.MapItemList(gameObject, draggedItem.GetComponent<InventoryItem>().itemName);
            if (equipped)
            {
                EquipSystem.Instance.EquipItem(index);
            }
        }
        else if (!Item() && draggedItemParent.CompareTag("ItemSlot"))
        {
            if (!draggedItem.GetComponent<InventoryItem>().isEquippable) return;
            int count = InventorySystem.Instance.UnMapItemList(draggedItemParent);
            draggedItem.transform.SetParent(transform);
            draggedItem.transform.SetSiblingIndex(0);
            draggedItem.GetComponent<InventoryItem>().SetIsInQuickSlot(true);
            EquipSystem.Instance.MapItemList(gameObject, draggedItem.GetComponent<InventoryItem>().itemName);
        }

    }

    public void SetIsFull(bool value)
    {
        isFull = value;
    }

    public bool GetIsFull()
    {
        return isFull;
    }

    public void SetIsEquiped(bool value)
    {
        isEquiped = value;
    }

    public bool GetIsEquiped()
    {
        return isEquiped;
    }

    public void SetQuickSlotIndexColor(Color color)
    {
        quickSlotIndex.color = color;
    }

    public void SetBackgroundActive(bool active)
    {
        background.SetActive(active);
    }

    public void SetIndex(int index)
    {
        this.index = index;
        quickSlotIndex.text = (index + 1).ToString();
    }

}
