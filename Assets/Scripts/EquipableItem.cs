using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EquipableItem : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && SelectionManager.Instance.handIsActive == false && InventorySystem.Instance.isOpen == false && CraftingSystem.Instance.isOpen == false)
        {
            animator.SetTrigger("hit");
        }
    }
}
