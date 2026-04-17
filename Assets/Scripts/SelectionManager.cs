using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    public GameObject interaction_Info_UI;
    public bool playerInRange = false;
    private Text interaction_text;
    public GameObject selectedObject;

    public GameObject centreDot;
    public GameObject centreHand;

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
        interaction_text = interaction_Info_UI.GetComponent<Text>();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var interactable = hit.transform.GetComponent<InteractableObject>();

            if (interactable && interactable.playerInRange)
            {
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);
                playerInRange = true;
                selectedObject = interactable.gameObject;

                if(interactable.CompareTag("Pickable"))
                {
                    centreDot.SetActive(false);
                    centreHand.SetActive(true);
                }
                else
                {
                    centreDot.SetActive(true);
                    centreHand.SetActive(false);
                }
            }
            else
            {
                interaction_Info_UI.SetActive(false);
                playerInRange= false;
                selectedObject= null;

                centreDot.SetActive(true);
                centreHand.SetActive(false);
            }
        }
        else
        {
            interaction_Info_UI.SetActive(false);
            playerInRange = false;
            selectedObject = null;

            centreDot.SetActive(true);
            centreHand.SetActive(false);
        }
    }

    public void DisableSelection()
    {
        centreDot.SetActive(false);
        centreHand.SetActive(false);
        interaction_Info_UI.SetActive(false);
        selectedObject = null;
    }

    public void EnableSelection()
    {
        centreDot.SetActive(true);
        centreHand.SetActive(true);
        interaction_Info_UI.SetActive(true);
    }
}