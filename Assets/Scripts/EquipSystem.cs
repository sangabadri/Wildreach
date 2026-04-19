using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    
    public static EquipSystem Instance { get; set; }

    public GameObject weaponHolder;

    public GameObject equipScreenUI;
    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList;

    private int equippedSlot = -1;
    public GameObject equippedItem = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void  Start()
    {
        FillSlotList();
        FillItemList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EquipItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EquipItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            EquipItem(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            EquipItem(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            EquipItem(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            EquipItem(6);
        }
    }

    private void EquipItem(int index)
    {
        int prevEquippedSlot = equippedSlot;
        UnequipItem();
        if (itemList[index] != null)
        {
            if(prevEquippedSlot == index)
            {
                return;
            }
            equippedSlot = index;
            QuickSlot quickSlot = slotList[index].GetComponent<QuickSlot>();
            quickSlot.SetIsEquiped(true);
            quickSlot.SetQuickSlotIndexColor(Color.white);
            InventoryItem item = quickSlot.transform.GetChild(0).GetComponent<InventoryItem>();
            item.SetIsEquipped(true);
            equippedItem = Instantiate(Resources.Load<GameObject>("Models/" + item.itemName), new Vector3(1.7f, 1f, 2f), Quaternion.Euler(10f, -10f, -20f));
            equippedItem.transform.SetParent(weaponHolder.transform, false);
        }
    }

    private void UnequipItem()
    {
        if (equippedSlot != -1)
        {
            QuickSlot quickSlot = slotList[equippedSlot].GetComponent<QuickSlot>();
            quickSlot.SetIsEquiped(false);
            quickSlot.SetQuickSlotIndexColor(Color.gray);
            InventoryItem item = quickSlot.transform.GetChild(0).GetComponent<InventoryItem>();
            item.SetIsEquipped(false);
            equippedSlot = -1;
            Destroy(equippedItem);
            equippedItem = null;
        }
    }

    private void FillSlotList()
    {
        foreach (Transform child in equipScreenUI.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                slotList.Add(child.gameObject);
            }
        }
    }

    private void FillItemList()
    {
        itemList = new List<string>(new string[slotList.Count]);
        int i = 0;
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 2)
            {
                MapItemList(slot, slot.transform.GetChild(0).GetComponent<InventoryItem>().itemName);
            }
            else itemList[i] = null;
            i++;
        }
    }

    public void UnMapItemList(GameObject slot)
    {
        QuickSlot quickSlot = slot.GetComponent<QuickSlot>();
        quickSlot.SetIsFull(false);
        quickSlot.SetIsEquiped(false);
        quickSlot.SetBackgroundActive(true);
        InventoryItem item = quickSlot.transform.GetChild(0).GetComponent<InventoryItem>();
        item.SetIsInQuickSlot(false);
        int index = slotList.IndexOf(slot);
        itemList[index] = null;
    }

    public void MapItemList(GameObject slot, string itemName)
    {
        QuickSlot quickSlot = slot.GetComponent<QuickSlot>();
        quickSlot.SetIsFull(true);
        quickSlot.SetIsEquiped(false);
        quickSlot.SetBackgroundActive(false);
        InventoryItem item = quickSlot.transform.GetChild(0).GetComponent<InventoryItem>();
        item.SetIsInQuickSlot(true);
        int index = slotList.IndexOf(slot);
        itemList[index] = itemName;
    }

}
