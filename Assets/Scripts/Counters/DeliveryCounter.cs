/// Раздача: сюда приносят тарелку для гостя.
public class DeliveryCounter : BaseCounter
{
    public override void Interact(IKitchenObjectParent actor)
    {
        if (!actor.HasKitchenObject()) return;

        if (actor.GetKitchenObject().TryGetPlate(out PlateKitchenObject plate))
        {
            DeliveryManager.Instance.DeliverRecipe(plate);
            actor.GetKitchenObject().DestroySelf();
        }
    }
}
