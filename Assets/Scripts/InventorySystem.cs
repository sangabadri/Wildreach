using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventorySystem : MonoBehaviour
{

    public static InventorySystem Instance { get; set; }

    public Canvas canvas;

    public GameObject inventoryScreenUI;
    public bool isOpen;
    public List<GameObject> slotList = new List<GameObject>();
    public List<string> itemList;

    private int itemCount=0;

    public GameObject pickupAlert;
    public Text pickupText;
    public Image pickupImage;
    private Coroutine coroutine = null;

    public GameObject itemInfoPanel;

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


    private void Start()
    {
        isOpen = false;
        FillSlotList();
        FillItemList();
        CraftingSystem.Instance.RefreshNeededItems();

        Cursor.visible = false;
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {

            inventoryScreenUI.SetActive(true);
            isOpen = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);
            isOpen = false;
            if(!CraftingSystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SelectionManager.Instance.EnableSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
            }
        }
    }

    private void FillSlotList()
    {
        foreach(Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("ItemSlot"))
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
            if (slot.transform.childCount > 1)
            {
                MapItemList(slot, slot.transform.GetChild(0).GetComponent<InventoryItem>().itemName);
            }
            else itemList[i] = null;
            i++;
        }
    }

    public bool IsFull(string itemName)
    {
        if (itemCount < slotList.Count)
        {
            return false;
        }
        foreach(GameObject slot in slotList)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            if (slot.transform.GetChild(0).GetComponent<InventoryItem>().itemName == itemName && itemSlot.GetItemCount() < itemSlot.GetItemsPerSlot())
            {
                return false;
            }
        }

        return true;
    }

    public void AddToInventory(string itemName)
    {
        GameObject emptySlot = GetEmptySlot(itemName);
        if (emptySlot.transform.childCount > 1)
        {
            ItemSlot itemSlot = emptySlot.GetComponent<ItemSlot>();
            itemSlot.SetItemCount(itemSlot.GetItemCount() + 1);

            TriggerPickupAlert(true, itemName, emptySlot.transform.GetChild(0).GetComponent<Image>().sprite);
        }
        else
        {
            GameObject item = Instantiate(Resources.Load<GameObject>(itemName), emptySlot.transform.position, emptySlot.transform.rotation);
            TriggerPickupAlert(true, itemName, item.GetComponent<Image>().sprite);
        
            item.transform.SetParent(emptySlot.transform);
            item.transform.SetSiblingIndex(0);
            item.transform.localPosition = new Vector2(0, 0);
            MapItemList(emptySlot, itemName);
        }

        CraftingSystem.Instance.RefreshNeededItems();
    }

    public void RemoveFromInventory(string itemName, int amountToRemove, GameObject slot)
    {
        if (slot)
        {
            TriggerPickupAlert(false, itemName, slot.transform.GetChild(0).GetComponent<Image>().sprite);
            UnMapItemList(slot);
            DestroyImmediate(slot.transform.GetChild(0).gameObject);
            CraftingSystem.Instance.RefreshNeededItems();
            return;
        }
        Image itemSprite = null;
        for (int i=0;i<itemList.Count;i++)
        {
            if(itemList[i] == itemName)
            {
                if (!itemSprite)
                {
                    itemSprite = slotList[i].transform.GetChild(0).GetComponent<Image>();
                }
                ItemSlot itemSlot = slotList[i].GetComponent<ItemSlot>();
                if(itemSlot.GetItemCount() > amountToRemove)
                {
                    itemSlot.SetItemCount(itemSlot.GetItemCount() - amountToRemove);
                    break;
                }
                else
                {
                    DestroyImmediate(slotList[i].transform.GetChild(0).gameObject);
                    amountToRemove -= itemSlot.GetItemCount();
                    UnMapItemList(slotList[i]);
                }
                if (amountToRemove <= 0) break;
            }
        }
        TriggerPickupAlert(false, itemName, itemSprite.sprite);
        CraftingSystem.Instance.RefreshNeededItems();
    }

    public void TriggerPickupAlert(bool check, string itemName, Sprite itemSprite)
    {
        if(itemSprite == null && itemName==null)
        {
            pickupAlert.SetActive(true);
            pickupText.text = "Inventory is full!";
            pickupImage.gameObject.SetActive(false);
            pickupText.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            return;
        }

        if (check)
        {
            pickupAlert.SetActive(true);
            pickupText.text = itemName + " Added";
            pickupImage.sprite = itemSprite;
        }
        else
        {
            pickupAlert.SetActive(true);
            pickupText.text = itemName + " Removed";
            pickupImage.sprite = itemSprite;
        }
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(HidePickupAlert());
    }

    IEnumerator HidePickupAlert()
    {
        yield return new WaitForSeconds(2f);
        pickupAlert.SetActive(false);
    }

    public int UnMapItemList(GameObject slot)
    {
        ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
        int count = itemSlot.GetItemCount();
        itemSlot.SetItemCount(0);
        itemSlot.SetItemsPerSlot(0);
        itemSlot.itemCountText.gameObject.SetActive(false);
        int index = slotList.IndexOf(slot);
        itemList[index] = null;
        itemCount--;

        return count;
    }

    public void MapItemList(GameObject slot, string itemName)
    {
        ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
        itemSlot.SetItemCount(1);
        itemSlot.SetItemsPerSlot(slot.transform.GetChild(0).GetComponent<InventoryItem>().itemsPerSlot);
        itemSlot.itemCountText.gameObject.SetActive(true);
        int index = slotList.IndexOf(slot);
        itemList[index] = itemName;
        itemCount++;
    }

    private GameObject GetEmptySlot(string itemName)
    {
        foreach(GameObject slot in slotList)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            if (slot.transform.childCount > 1 && slot.transform.GetChild(0).GetComponent<InventoryItem>().itemName == itemName && itemSlot.GetItemCount() < itemSlot.GetItemsPerSlot())
            {
                return slot;
            }
        }

        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 1)
            {
                return slot;
            }
        }
        return null;
    }

}