using UnityEngine;

/// Подсветка прилавка, на который смотрит игрок.
/// В visualGameObjects положите «подсвеченную» копию меша стола
/// (дубликат KitchenCounter, светлый материал, scale ×1.02, по умолчанию выключен).
public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter; // если пусто — берётся у родителя
    [SerializeField] private GameObject[] visualGameObjects;

    private void Awake()
    {
        if (baseCounter == null)
            baseCounter = GetComponentInParent<BaseCounter>();
    }

    private void Start()
    {
        PlayerInteraction.OnSelectedCounterChanged += HandleSelectedChanged;
        SetVisible(false);
    }

    private void OnDestroy() => PlayerInteraction.OnSelectedCounterChanged -= HandleSelectedChanged;

    private void HandleSelectedChanged(BaseCounter selected) => SetVisible(selected == baseCounter);

    private void SetVisible(bool visible)
    {
        foreach (GameObject go in visualGameObjects)
            go.SetActive(visible);
    }
}
