using System;
using UnityEngine;

/// Разделочный стол. E — положить/взять, F — резать.
public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event Action<CuttingCounter> OnAnyCut; // для звука
    public event Action<float> OnProgressChanged;
    public event Action OnCut;                           // для анимации ножа

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    private int cuttingProgress;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData() => OnAnyCut = null;

    public override void Interact(IKitchenObjectParent actor)
    {
        if (!HasKitchenObject())
        {
            // кладём только то, что можно резать
            if (actor.HasKitchenObject() &&
                TryGetRecipe(actor.GetKitchenObject().GetKitchenObjectSO(), out _))
            {
                actor.GetKitchenObject().SetParent(this);
                cuttingProgress = 0;
                OnProgressChanged?.Invoke(0f);
            }
            return;
        }

        if (!actor.HasKitchenObject())
        {
            GetKitchenObject().SetParent(actor);
            cuttingProgress = 0;
            OnProgressChanged?.Invoke(0f);
        }
        else if (TryMoveCounterObjectOntoActorPlate(actor))
        {
            cuttingProgress = 0;
            OnProgressChanged?.Invoke(0f);
        }
    }

    public override void InteractAlternate(IKitchenObjectParent actor)
    {
        if (!HasKitchenObject()) return;
        if (!TryGetRecipe(GetKitchenObject().GetKitchenObjectSO(), out CuttingRecipeSO recipe)) return;

        cuttingProgress++;
        OnCut?.Invoke();
        OnAnyCut?.Invoke(this);
        OnProgressChanged?.Invoke((float)cuttingProgress / recipe.cuttingProgressMax);

        if (cuttingProgress >= recipe.cuttingProgressMax)
        {
            KitchenObjectSO output = recipe.output;
            GetKitchenObject().DestroySelf();
            KitchenObject.Spawn(output, this);
            cuttingProgress = 0; // если у результата есть свой рецепт — можно резать дальше
        }
    }

    private bool TryGetRecipe(KitchenObjectSO input, out CuttingRecipeSO recipe)
    {
        foreach (CuttingRecipeSO r in cuttingRecipeSOArray)
        {
            if (r.input == input)
            {
                recipe = r;
                return true;
            }
        }
        recipe = null;
        return false;
    }
}
