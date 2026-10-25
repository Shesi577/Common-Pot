using UnityEngine;

/// Вешается на КОРЕНЬ CuttingCounter_Visual вместе с Animator
/// (Controller = CuttingCounter.controller; дети корня: Knife и Chopping Board).
public class CuttingCounterVisual : MonoBehaviour
{
    private const string CutTrigger = "Cut"; // триггер из CuttingCounter.controller

    [SerializeField] private CuttingCounter cuttingCounter; // если пусто — берётся у родителя

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (cuttingCounter == null)
            cuttingCounter = GetComponentInParent<CuttingCounter>();
    }

    private void Start() => cuttingCounter.OnCut += HandleCut;

    private void OnDestroy()
    {
        if (cuttingCounter != null)
            cuttingCounter.OnCut -= HandleCut;
    }

    private void HandleCut() => animator.SetTrigger(CutTrigger);
}
