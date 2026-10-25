using System.Collections.Generic;
using UnityEngine;

/// Блюдо, которое нужно подать: набор ингредиентов на тарелке (порядок не важен).
[CreateAssetMenu(menuName = "Kitchen/Recipes/Dish Recipe")]
public class RecipeSO : ScriptableObject
{
    public string recipeName;
    public List<KitchenObjectSO> kitchenObjectSOList;
}
