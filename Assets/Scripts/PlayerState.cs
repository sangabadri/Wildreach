using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }

    [SerializeField] private float maxHealth;
    private float currentHealth;


    [SerializeField] private float maxCalories;
    private float currentCalories;

    private float distanceTraveled = 0;
    private Vector3 lastPosition;

    [SerializeField] private GameObject player;


    [SerializeField] private float maxHydration;
    private float currentHydration;


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
        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydration = maxHydration;

        lastPosition = player.transform.position;
        StartCoroutine(DecreaseHydration());

    }
    IEnumerator DecreaseHydration()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            currentHydration -= 1;
        }
    }

    private void Update()
    {
        distanceTraveled+= Vector3.Distance(player.transform.position, lastPosition);
        lastPosition = player.transform.position;

        if (distanceTraveled > 50)
        {
            currentCalories -= 1;
            distanceTraveled = 0;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            currentHealth -= 10;
        }
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
    }

    public void SetCalories(float calories)
    {
        currentCalories = calories;
    }

    public void SetHydration(float hydration)
    {
        currentHydration = hydration;
    }

    public float GetHealth()
    {
        return currentHealth; 
    }

    public float GetCalories()
    {
        return currentCalories;
    }

    public float GetHydration()
    {
        return currentHydration;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetMaxCalories()
    {
        return maxCalories;
    }

    public float GetMaxHydration()
    {
        return maxHydration;
    }
}
