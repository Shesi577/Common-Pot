using System;
using UnityEngine;

/// Плита: Idle -> Frying -> Fried -> Burned.
public class StoveCounter : BaseCounter, IHasProgress
{
    public enum State { Idle, Frying, Fried, Burned }

    public event Action<State> OnStateChanged;
    public event Action<float> OnProgressChanged;

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State state = State.Idle;
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;
    private IKitchenObjectParent placedBy; // кто положил продукт на плиту

    public State GetState() => state;
    public bool IsFried() => state == State.Fried;

    private void Update()
    {
        if (!HasKitchenObject()) return;

        switch (state)
        {
            case State.Frying:
                fryingTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(fryingTimer / fryingRecipeSO.fryingTimerMax);

                if (fryingTimer >= fryingRecipeSO.fryingTimerMax)
                {
                    GetKitchenObject().DestroySelf();
                    KitchenObject fried = KitchenObject.Spawn(fryingRecipeSO.output, this);

                    burningRecipeSO = FindBurningRecipe(fried.GetKitchenObjectSO());
                    burningTimer = 0f;
                    if (burningRecipeSO != null)
                    {
                        SetState(State.Fried);
                    }
                    else
                    {
                        // у готового блюда нет рецепта пригорания — оно просто лежит на плите
                        SetState(State.Idle);
                        OnProgressChanged?.Invoke(0f);
                    }
                }
                break;

            case State.Fried:
                burningTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(burningTimer / burningRecipeSO.burningTimerMax);

                if (burningTimer >= burningRecipeSO.burningTimerMax)
                {
                    KitchenObjectSO burnedSO = burningRecipeSO.output;
                    GetKitchenObject().DestroySelf();
                    KitchenObject.Spawn(burnedSO, this);
                    SetState(State.Burned);
                    OnProgressChanged?.Invoke(0f);
                }
                break;
        }
    }

    public override void Interact(IKitchenObjectParent actor)
    {
        if (!HasKitchenObject())
        {
            if (actor.HasKitchenObject() &&
                TryGetFryingRecipe(actor.GetKitchenObject().GetKitchenObjectSO(), out FryingRecipeSO recipe))
            {
                actor.GetKitchenObject().SetParent(this);
                placedBy = actor;
                fryingRecipeSO = recipe;
                fryingTimer = 0f;
                SetState(State.Frying);
                OnProgressChanged?.Invoke(0f);
            }
            return;
        }

        if (!actor.HasKitchenObject())
        {
            // чужое блюдо снято, пока оно почти пригорело — это спасение
            if (state == State.Fried && burningRecipeSO != null && placedBy != null && actor != placedBy
                && burningTimer >= 0.5f * burningRecipeSO.burningTimerMax)
                HelpEvents.Raise(HelpType.Rescue);

            GetKitchenObject().SetParent(actor);
            SetState(State.Idle);
            OnProgressChanged?.Invoke(0f);
        }
        else if (TryMoveCounterObjectOntoActorPlate(actor))
        {
            SetState(State.Idle);
            OnProgressChanged?.Invoke(0f);
        }
    }

    private void SetState(State newState)
    {
        state = newState;
        OnStateChanged?.Invoke(state);
    }

    private bool TryGetFryingRecipe(KitchenObjectSO input, out FryingRecipeSO recipe)
    {
        foreach (FryingRecipeSO r in fryingRecipeSOArray)
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

    private BurningRecipeSO FindBurningRecipe(KitchenObjectSO input)
    {
        foreach (BurningRecipeSO r in burningRecipeSOArray)
            if (r.input == input) return r;
        return null;
    }
}
