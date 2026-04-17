using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class HydrationBar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private Text hydrationCounter;

    private float maxHydration;
    private float currentHydration;

    private void Start()
    {
        slider = GetComponent<Slider>();

    }

    private void Update()
    {
        currentHydration = PlayerState.Instance.GetHydration();
        maxHydration = PlayerState.Instance.GetMaxHydration();

        slider.value = currentHydration / maxHydration;
        hydrationCounter.text = $"{currentHydration} %";
    }

}
