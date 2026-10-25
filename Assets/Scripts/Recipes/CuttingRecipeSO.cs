using UnityEngine;

[CreateAssetMenu(menuName = "Kitchen/Recipes/Cutting Recipe")]
public class CuttingRecipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    [Min(1)] public int cuttingProgressMax = 3; // сколько нажатий «резать»
}
