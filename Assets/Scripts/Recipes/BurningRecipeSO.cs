using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen/Recipes/Burning Recipe")]
public class BurningRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;   // готовое блюдо
    public KitchenObjectSO output;  // подгоревшее
    [Min(0.1f)] public float burningTimerMax = 5f; // секунд до пригорания
}
