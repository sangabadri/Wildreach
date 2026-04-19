using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; set; }


    [SerializeField] private GameObject craftingScreenUI;
    [SerializeField] private GameObject toolsScreenUI;

    public List<GameObject> craftingList = new List<GameObject>();

    // Category Buttons
    private Button toolsBTN;


    public bool isOpen;
    public bool isToolsOpen;

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

        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });
    }
    private void Start()
    {
        isOpen = false;
        isToolsOpen = false;

        FillSlotList();
        MapButtons();
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.C) && !isOpen)
        {

            craftingScreenUI.SetActive(true);
            isOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
        }
        else if(Input.GetKeyDown(KeyCode.C) && isOpen && isToolsOpen)
        {
            toolsScreenUI.SetActive(false);
            craftingScreenUI.SetActive(true);
            isToolsOpen = false;
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            isOpen = false;
            if(!InventorySystem.Instance.isOpen)
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
        foreach (Transform child in toolsScreenUI.transform)
        {
            if (child.CompareTag("CraftingSlot"))
            {
                craftingList.Add(child.gameObject);
            }
        }
    }

    private void MapButtons()
    {
        foreach(GameObject slot in craftingList)
        {
            Button button = slot.transform.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(delegate { CraftAnyItem(button.transform.parent.gameObject.GetComponent<Blueprint>()); });
        }
    }

    private void CraftAnyItem(Blueprint craftingItemBlueprint)
    {
        for(int i=0;i<craftingItemBlueprint.numberOfRequirements;i++)
        {
            InventorySystem.Instance.RemoveFromInventory(craftingItemBlueprint.req[i], craftingItemBlueprint.reqAmount[i], null);
        }

        InventorySystem.Instance.AddToInventory(craftingItemBlueprint.itemName);

    }

    public void RefreshNeededItems()
    {
        int stone_count = 0;
        int stick_count = 0;
        foreach(GameObject slot in InventorySystem.Instance.slotList)
        {
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            InventoryItem item = slot.transform.GetChild(0).GetComponent<InventoryItem>();
            if (item != null && item.itemName == "Stone") { 
                stone_count += itemSlot.GetItemCount();
            }
            if (item != null && slot.transform.GetChild(0).GetComponent<InventoryItem>().itemName == "Stick")
            {
                stick_count += itemSlot.GetItemCount();
            }
        }

        foreach (GameObject craftingSlot in craftingList)
        {
            Blueprint blueprint = craftingSlot.transform.GetComponent<Blueprint>();
            List<int> count = new List<int>(new int[blueprint.numberOfRequirements]);
            
            for(int i = 0; i < blueprint.numberOfRequirements; i++) {
                count[i] = 0;
            }

            foreach (GameObject slot in InventorySystem.Instance.slotList)
            {
                ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
                InventoryItem item = slot.transform.GetChild(0).GetComponent<InventoryItem>();
                for(int i = 0; i < blueprint.numberOfRequirements; i++)
                {
                    if(item != null && item.itemName == blueprint.req[i])
                    {
                        count[i] += itemSlot.GetItemCount();
                    }
                }
            }

            bool check = true;

            for (int i = 0; i < blueprint.numberOfRequirements; i++)
            {
                if(count[i] < blueprint.reqAmount[i])
                {
                    check = false;
                }
                blueprint.reqTexts[i].text = $"{blueprint.reqAmount[i]} {blueprint.req[i]} [{count[i]}]";
            }

            if (check)
            {
                craftingSlot.transform.Find("Button").gameObject.SetActive(true);
            }
            else
            {
                craftingSlot.transform.Find("Button").gameObject.SetActive(false);
            }
        }

    }

    private void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
        isToolsOpen = true;
    }

}
