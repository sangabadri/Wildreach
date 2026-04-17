using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool playerInRange = false;
    public string itemName;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && (SelectionManager.Instance.playerInRange) && (SelectionManager.Instance.selectedObject == gameObject))
        {
            if(!InventorySystem.Instance.IsFull(itemName))
            {
                InventorySystem.Instance.AddToInventory(itemName);
                Destroy(gameObject);
            }
            else
            {
                InventorySystem.Instance.TriggerPickupAlert(true, null, null);
            }
        }
    }

    public string GetItemName()
    {
        return itemName;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

}