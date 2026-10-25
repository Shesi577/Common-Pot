using UnityEngine;

/// Вешается на корень ContainerCounter_Visual (там есть Animator).
public class ContainerCounterVisual : MonoBehaviour
{
    // Триггер из ContainerCounter.controller.
    [SerializeField] private string openCloseTrigger = "OpenClose";
    [SerializeField] private ContainerCounter containerCounter; // если пусто — берётся у родителя

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (containerCounter == null)
            containerCounter = GetComponentInParent<ContainerCounter>();
    }

    private void Start() => containerCounter.OnPlayerGrabbedObject += HandleGrabbed;

    private void OnDestroy()
    {
        if (containerCounter != null)
            containerCounter.OnPlayerGrabbedObject -= HandleGrabbed;
    }

    private void HandleGrabbed() => animator.SetTrigger(openCloseTrigger);
}
