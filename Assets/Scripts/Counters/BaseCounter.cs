using System;
using UnityEngine;

/// Базовый класс всех прилавков.
/// Interact — основное действие (кнопка E), InteractAlternate — дополнительное (кнопка F: резать).
public abstract class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    /// Для звуков/эффектов: что-то положили на любой прилавок.
    public static event Action<BaseCounter> OnAnyObjectPlacedHere;

    [SerializeField] private Transform counterTopPoint; // пустой child на поверхности стола

    private KitchenObject kitchenObject;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData() => OnAnyObjectPlacedHere = null;

    public virtual void Interact(IKitchenObjectParent actor) { }
    public virtual void InteractAlternate(IKitchenObjectParent actor) { }

    /// Игрок держит тарелку, на прилавке ингредиент -> ингредиент уходит на тарелку.
    protected bool TryMoveCounterObjectOntoActorPlate(IKitchenObjectParent actor)
    {
        if (!HasKitchenObject() || !actor.HasKitchenObject()) return false;
        if (!actor.GetKitchenObject().TryGetPlate(out PlateKitchenObject plate)) return false;
        if (!plate.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) return false;

        GetKitchenObject().DestroySelf();
        return true;
    }

    public Transform GetKitchenObjectFollowTransform() => counterTopPoint;
    public KitchenObject GetKitchenObject() => kitchenObject;
    public bool HasKitchenObject() => kitchenObject != null;

    public void SetKitchenObject(KitchenObject newObject)
    {
        kitchenObject = newObject;
        if (newObject != null)
            OnAnyObjectPlacedHere?.Invoke(this);
    }

    public void ClearKitchenObject() => kitchenObject = null;
}
