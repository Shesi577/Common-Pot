using UnityEngine;

/// Всё, что может держать предмет: прилавок, игрок, (позже) NPC-волонтёр.
public interface IKitchenObjectParent
{
    Transform GetKitchenObjectFollowTransform();
    void SetKitchenObject(KitchenObject kitchenObject);
    KitchenObject GetKitchenObject();
    void ClearKitchenObject();
    bool HasKitchenObject();
}
