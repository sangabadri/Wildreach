using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameObject itemInfoPanel;
    private GameObject consumableAlertUI;

    private Text itemInfoPanel_ItemName;
    private Text itemInfoPanel_ItemDescription;
    private Text itemInfoPanel_ItemFunctionality;
    private Text itemInfoPanel_ItemEffectsHealth;
    private Text itemInfoPanel_ItemEffectsStamina;
    private Text itemInfoPanel_ItemEffectsHydration;

    public string itemName;
    public string itemDesription;
    public string itemFunctionality;

    public bool isTrashable;
    public bool isEquippable;

    private bool isInQuickSlot = false;
    private bool isEquipped = false;

    public int itemsPerSlot;

    private Button YesBTN, NoBTN;
    private Text question;
    private Slider slider;
    private Text minValue, maxValue;
    private Text text;

    private ItemSlot slot;

    private bool pointerClicked = false;

    [SerializeField] private bool isConsumable;
    [SerializeField] private float healthEffect;
    [SerializeField] private float caloriesEffect;
    [SerializeField] private float hydrationEffect;

    private void Start()
    {
        itemInfoPanel = InventorySystem.Instance.itemInfoPanel;
        itemInfoPanel_ItemName = itemInfoPanel.transform.Find("ItemName").GetComponent<Text>();
        itemInfoPanel_ItemDescription = itemInfoPanel.transform.Find("ItemDescription").GetComponent<Text>();
        itemInfoPanel_ItemFunctionality = itemInfoPanel.transform.Find("ItemFunctionality").GetComponent<Text>();
        itemInfoPanel_ItemEffectsHealth = itemInfoPanel.transform.Find("ItemEffects").Find("Health").GetComponent<Text>();
        itemInfoPanel_ItemEffectsStamina = itemInfoPanel.transform.Find("ItemEffects").Find("Stamina").GetComponent<Text>();
        itemInfoPanel_ItemEffectsHydration = itemInfoPanel.transform.Find("ItemEffects").Find("Hydration").GetComponent<Text>();

        consumableAlertUI = InventorySystem.Instance.canvas.transform.Find("ConsumableAlert").gameObject;

        YesBTN = consumableAlertUI.transform.Find("YesBTN").GetComponent<Button>();
        YesBTN.onClick.AddListener(delegate { ConsumeItem(); });

        NoBTN = consumableAlertUI.transform.Find("NoBTN").GetComponent<Button>();
        NoBTN.onClick.AddListener(delegate { CancelConsumption(); });

        question = consumableAlertUI.transform.Find("Text").GetComponent<Text>();
        minValue = consumableAlertUI.transform.Find("Quantity").Find("MinValue").GetComponent<Text>();
        maxValue = consumableAlertUI.transform.Find("Quantity").Find("MaxValue").GetComponent<Text>();
        slider = consumableAlertUI.transform.Find("Quantity").Find("Slider").GetComponent<Slider>();
        text = consumableAlertUI.transform.Find("Quantity").Find("Slider").Find("HandleSlideArea").Find("Handle").Find("Text").GetComponent<Text>();

        slider.onValueChanged.AddListener(UpdateSlider);

        text.text = "1";
        slider.value = 1;
        slider.minValue = 1;


        itemInfoPanel.SetActive(false);

        slot = transform.parent.GetComponent<ItemSlot>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isInQuickSlot) return;
        itemInfoPanel_ItemName.text = itemName;
        itemInfoPanel_ItemDescription.text = itemDesription;
        itemInfoPanel_ItemFunctionality.text = itemFunctionality;
        if (isConsumable)
        {
            itemInfoPanel_ItemEffectsHealth.text = "Health Effect: " + ((int)healthEffect);
            itemInfoPanel_ItemEffectsStamina.text = "Calories Effect: " + ((int)caloriesEffect);
            itemInfoPanel_ItemEffectsHydration.text = "Hydration Effect: " + ((int)hydrationEffect);
        }
        else
        {
            itemInfoPanel_ItemEffectsHealth.text = "Not Consumable";
            itemInfoPanel_ItemEffectsStamina.gameObject.SetActive(false);
            itemInfoPanel_ItemEffectsHydration.gameObject.SetActive(false);
        }
        itemInfoPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoPanel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right && isConsumable)
        {
            maxValue.text = slot.GetItemCount().ToString();
            slider.maxValue = slot.GetItemCount();
            question.text = "How many of " + transform.GetComponent<InventoryItem>().itemName + " do you want to consume?";
            consumableAlertUI.SetActive(true);
            pointerClicked = true;
        }
    }

    private void CancelConsumption()
    {
        text.text = "1";
        slider.value = 1;
        consumableAlertUI.SetActive(false);
    }

    private void ConsumeItem()
    {
        if (pointerClicked)
        {
            ConsumingFunction((int)slider.value);
            if (slider.value == slider.maxValue)
            {
                DestroyImmediate(gameObject);
                InventorySystem.Instance.UnMapItemList(slot.gameObject);
            }
            else
            {
                slot.SetItemCount(slot.GetItemCount() - (int)slider.value);
            }
            CraftingSystem.Instance.RefreshNeededItems();
            text.text = "1";
            slider.value = 1;
            consumableAlertUI.SetActive(false);
        }
    }


    private void ConsumingFunction(int count)
    {
        itemInfoPanel.SetActive(false);
        for(int i=0; i<count; i++)
        {
            HealthEffectCalculation();
            CaloriesEffectCalculation();
            HydrationEffectCalculation();
        }
    }

    private void HealthEffectCalculation()
    {
        float currentHealth = PlayerState.Instance.GetHealth();
        float maxHealth = PlayerState.Instance.GetMaxHealth();

        if(currentHealth + healthEffect > maxHealth)
        {
            PlayerState.Instance.SetHealth(maxHealth);
        }
        else
        {
            PlayerState.Instance.SetHealth(currentHealth + healthEffect);
        }
    }

    private void CaloriesEffectCalculation()
    {
        float currentCalories = PlayerState.Instance.GetCalories();
        float maxCalories = PlayerState.Instance.GetMaxCalories();

        if (currentCalories + caloriesEffect > maxCalories)
        {
            PlayerState.Instance.SetCalories(maxCalories);
        }
        else
        {
            PlayerState.Instance.SetCalories(currentCalories + caloriesEffect);
        }
    }

    private void HydrationEffectCalculation()
    {
        float currentHydration = PlayerState.Instance.GetHydration();
        float maxHydration = PlayerState.Instance.GetMaxHydration();

        if (currentHydration + hydrationEffect > maxHydration)
        {
            PlayerState.Instance.SetHydration(maxHydration);
        }
        else
        {
            PlayerState.Instance.SetHydration(currentHydration + hydrationEffect);
        }
    }

    void UpdateSlider(float value)
    {
        text.text = ((int)slider.value).ToString();
    }

    public void SetIsInQuickSlot(bool value)
    {
        isInQuickSlot = value;
    }

    public bool GetIsInQuickSlot()
    {
        return isInQuickSlot;
    }

    public void SetIsEquipped(bool value)
    {
        isEquipped = value;
    }

    public bool GetIsEquipped()
    {
        return isEquipped;
    }

}
