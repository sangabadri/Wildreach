using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private Text healthCounter;

    private float maxHealth;
    private float currentHealth;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        currentHealth = PlayerState.Instance.GetHealth();
        maxHealth = PlayerState.Instance.GetMaxHealth();

        slider.value = currentHealth / maxHealth;
        healthCounter.text = $"{currentHealth} / {maxHealth}";
    }

}
