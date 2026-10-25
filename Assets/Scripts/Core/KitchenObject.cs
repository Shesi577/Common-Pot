using UnityEngine;

/// Компонент на префабе каждого предмета (помидор, тарелка, нарезанный хлеб...).
public class KitchenObject : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent parent;

    public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;
    public IKitchenObjectParent GetParent() => parent;

    /// Перекладывает предмет к новому владельцу (прилавок/игрок).
    public void SetParent(IKitchenObjectParent newParent)
    {
        if (newParent == null)
        {
            Debug.LogError("KitchenObject.SetParent: newParent == null");
            return;
        }
        if (newParent.HasKitchenObject())
        {
            Debug.LogError("KitchenObject.SetParent: у нового владельца уже есть предмет");
            return;
        }

        if (parent != null)
            parent.ClearKitchenObject();

        parent = newParent;
        parent.SetKitchenObject(this);

        transform.SetParent(parent.GetKitchenObjectFollowTransform());
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void DestroySelf()
    {
        if (parent != null)
            parent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plate)
    {
        plate = this as PlateKitchenObject;
        return plate != null;
    }

    /// Создаёт предмет сразу у владельца.
    public static KitchenObject Spawn(KitchenObjectSO so, IKitchenObjectParent newParent)
    {
        KitchenObject obj = Instantiate(so.prefab);
        obj.SetParent(newParent);
        return obj;
    }
}
