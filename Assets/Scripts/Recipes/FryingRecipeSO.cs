using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen/Recipes/Frying Recipe")]
public class FryingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    [Min(0.1f)] public float fryingTimerMax = 5f; // секунд до готовности
}
