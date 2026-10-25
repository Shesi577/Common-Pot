using System;
using UnityEngine;

/// Мусорка: уничтожает всё, что принёс игрок.
public class TrashCounter : BaseCounter
{
    public static event Action<TrashCounter> OnAnyObjectTrashed; // для звука

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData() => OnAnyObjectTrashed = null;

    public override void Interact(IKitchenObjectParent actor)
    {
        if (!actor.HasKitchenObject()) return;

        actor.GetKitchenObject().DestroySelf();
        OnAnyObjectTrashed?.Invoke(this);
    }
}
