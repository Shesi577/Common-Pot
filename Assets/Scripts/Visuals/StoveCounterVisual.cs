using UnityEngine;

/// Вешается на StoveCounter_Visual.
/// stoveOnGameObject = "StoveOnVisual", particlesGameObject = "SizzlingParticles".
public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter; // если пусто — берётся у родителя
    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Awake()
    {
        if (stoveCounter == null)
            stoveCounter = GetComponentInParent<StoveCounter>();
    }

    private void Start()
    {
        stoveCounter.OnStateChanged += HandleStateChanged;
        SetVisuals(false); // стартуем выключенными
    }

    private void OnDestroy()
    {
        if (stoveCounter != null)
            stoveCounter.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(StoveCounter.State state)
    {
        bool on = state == StoveCounter.State.Frying || state == StoveCounter.State.Fried;
        SetVisuals(on);
    }

    private void SetVisuals(bool on)
    {
        if (stoveOnGameObject != null) stoveOnGameObject.SetActive(on);
        if (particlesGameObject != null) particlesGameObject.SetActive(on);
    }
}
