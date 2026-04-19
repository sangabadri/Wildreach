using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrashSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject trashAlertUI;


    public Sprite trashClosed;
    public Sprite trashOpened;

    private Image trash;

    private Button YesBTN, NoBTN;
    private Text question;
    private Slider slider;
    private Text minValue, maxValue;
    private Text text;

    private GameObject draggedItem
    {
        get
        {
            return DragDrop.itemBeingDragged;
        }
    }

    private GameObject draggedItemParent
    {
        get
        {
            return DragDrop.itemBeingDraggedParent;
        }
    }
    private GameObject itemToBeDeleted;
    private GameObject itemToBeDeletedParent;

    private void Awake()
    {
        trash = transform.Find("Trash").GetComponent<Image>();


        YesBTN = trashAlertUI.transform.Find("YesBTN").GetComponent<Button>();
        YesBTN.onClick.AddListener(delegate { DeleteItem(); });

        NoBTN = trashAlertUI.transform.Find("NoBTN").GetComponent<Button>();
        NoBTN.onClick.AddListener(delegate { CancelDeletion(); });

        question = trashAlertUI.transform.Find("Text").GetComponent<Text>();
        minValue = trashAlertUI.transform.Find("Quantity").Find("MinValue").GetComponent<Text>();
        maxValue = trashAlertUI.transform.Find("Quantity").Find("MaxValue").GetComponent<Text>();
        slider = trashAlertUI.transform.Find("Quantity").Find("Slider").GetComponent<Slider>();
        text = trashAlertUI.transform.Find("Quantity").Find("Slider").Find("HandleSlideArea").Find("Handle").Find("Text").GetComponent<Text>();

        slider.onValueChanged.AddListener(UpdateSlider);

        ReserSlider();
        slider.minValue = 1;

    }


    public void OnDrop(PointerEventData eventData)
    {
        if (draggedItem.GetComponent<InventoryItem>().isTrashable == true && draggedItem.GetComponent<InventoryItem>().GetIsInQuickSlot() == false)
        {
            itemToBeDeleted = draggedItem.gameObject;
            itemToBeDeletedParent = draggedItemParent.gameObject;
            ItemSlot slot = itemToBeDeletedParent.GetComponent<ItemSlot>();
            maxValue.text = slot.GetItemCount().ToString();
            slider.maxValue = slot.GetItemCount();
            question.text = "How many of " + draggedItem.GetComponent<InventoryItem>().itemName + " do you want to remove?";
            trashAlertUI.SetActive(true);
        }

    }


    private void CancelDeletion()
    {
        trash.sprite = trashClosed;
        ReserSlider();
        trashAlertUI.SetActive(false);
    }

    private void DeleteItem()
    {
        trash.sprite = trashClosed;
        ItemSlot slot = itemToBeDeletedParent.GetComponent<ItemSlot>();
        if (slider.value == slider.maxValue)
        {
            DestroyImmediate(itemToBeDeleted.gameObject);
            InventorySystem.Instance.UnMapItemList(slot.gameObject);
        }
        else
        {
            slot.SetItemCount(slot.GetItemCount() - (int)slider.value);
        }
        CraftingSystem.Instance.RefreshNeededItems();
        ReserSlider();
        trashAlertUI.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (draggedItem != null && draggedItem.GetComponent<InventoryItem>().isTrashable == true)
        {
            trash.sprite = trashOpened;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (draggedItem != null && draggedItem.GetComponent<InventoryItem>().isTrashable == true)
        {
            trash.sprite = trashClosed;
        }
    }

    void UpdateSlider(float value)
    {
        text.text = ((int)slider.value).ToString();
    }

    void ReserSlider()
    {
        text.text = "1";
        slider.value = 1;
    }

}