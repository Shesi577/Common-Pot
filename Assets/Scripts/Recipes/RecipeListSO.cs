using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen/Recipes/Dish Recipe List")]
public class RecipeListSO : ScriptableObject
{
    public List<RecipeSO> recipeSOList;
}
