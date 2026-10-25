using System;
using System.Collections.Generic;
using UnityEngine;

/// Показывает на тарелке ингредиенты по мере их добавления.
/// Вешается на PlateCompleteVisual (child префаба тарелки).
/// В списке сопоставьте каждый KitchenObjectSO с его вложенным объектом
/// (Bread, MeatPattyCooked, MeatPattyBurned, CheeseSlices, TomatoSlices, CabbageSliced).
public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct IngredientVisual
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject visual;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject; // если пусто — берётся у родителя
    [SerializeField] private List<IngredientVisual> ingredientVisuals;

    private void Awake()
    {
        if (plateKitchenObject == null)
            plateKitchenObject = GetComponentInParent<PlateKitchenObject>();
    }

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += HandleIngredientAdded;

        foreach (IngredientVisual item in ingredientVisuals)
            item.visual.SetActive(false); // пустая тарелка
    }

    private void OnDestroy()
    {
        if (plateKitchenObject != null)
            plateKitchenObject.OnIngredientAdded -= HandleIngredientAdded;
    }

    private void HandleIngredientAdded(KitchenObjectSO added)
    {
        foreach (IngredientVisual item in ingredientVisuals)
        {
            if (item.kitchenObjectSO == added)
                item.visual.SetActive(true);
        }
    }
}
