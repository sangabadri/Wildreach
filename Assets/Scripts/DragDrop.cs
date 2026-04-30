using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public static GameObject itemBeingDragged;
    public static GameObject itemBeingDraggedParent;
    private Transform startParent;



    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

    }

    private void Start()
    {
        canvas = InventorySystem.Instance.canvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .6f;
        //So the ray cast will ignore the item itself.
        canvasGroup.blocksRaycasts = false;
        startParent = transform.parent;
        if (startParent.CompareTag("ItemSlot"))
        {
            startParent.gameObject.GetComponent<ItemSlot>().itemCountText.gameObject.SetActive(false);
        }
        else if(startParent.CompareTag("QuickSlot"))
        {
            startParent.gameObject.GetComponent<QuickSlot>().SetBackgroundActive(true);
        }
        transform.SetParent(canvas.transform);
        itemBeingDragged = gameObject;
        itemBeingDraggedParent = startParent.gameObject;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //So the item will move with our mouse (at same speed)  and so it will be consistant if the canvas has a different scale (other then 1);
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

    }



    public void OnEndDrag(PointerEventData eventData)
    {

        if (transform.parent == canvas.transform || transform.parent == itemBeingDraggedParent.transform)
        {
            transform.SetParent(startParent);
            transform.SetSiblingIndex(0);
            if (startParent.CompareTag("ItemSlot"))
            {
                startParent.gameObject.GetComponent<ItemSlot>().itemCountText.gameObject.SetActive(true);
            }
            else if (startParent.CompareTag("QuickSlot"))
            {
                startParent.gameObject.GetComponent<QuickSlot>().SetBackgroundActive(false);
            }
        }

        transform.localPosition = new Vector2(0, 0);

        itemBeingDragged = null;
        itemBeingDraggedParent = null;
        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }



}