using System;
using System.Collections.Generic;
using UnityEngine;

/// Тарелка. Ставится на префаб тарелки ВМЕСТО KitchenObject.
/// В validKitchenObjectSOList перечислите, что можно класть на тарелку.
public class PlateKitchenObject : KitchenObject
{
    public event Action<KitchenObjectSO> OnIngredientAdded;

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    private readonly List<KitchenObjectSO> ingredients = new List<KitchenObjectSO>();

    public bool TryAddIngredient(KitchenObjectSO so)
    {
        if (!validKitchenObjectSOList.Contains(so)) return false; // не подходит для тарелки
        if (ingredients.Contains(so)) return false;               // дубликаты нельзя

        ingredients.Add(so);
        OnIngredientAdded?.Invoke(so);
        return true;
    }

    public List<KitchenObjectSO> GetIngredients() => ingredients;
}
