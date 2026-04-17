using UnityEngine;
using UnityEngine.UI;


public class CaloriesBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private Text caloriesCounter;

    private float maxCalories;
    private float currentCalories;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        currentCalories = PlayerState.Instance.GetCalories();
        maxCalories = PlayerState.Instance.GetMaxCalories();

        slider.value = currentCalories / maxCalories;
        caloriesCounter.text = $"{currentCalories} / {maxCalories}";
    }

}
