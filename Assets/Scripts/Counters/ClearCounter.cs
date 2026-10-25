/// Обычный стол: положить / взять / переложить на тарелку.
public class ClearCounter : BaseCounter
{
    public override void Interact(IKitchenObjectParent actor)
    {
        if (!HasKitchenObject())
        {
            // стол пуст: кладём то, что в руках
            if (actor.HasKitchenObject())
                actor.GetKitchenObject().SetParent(this);
            return;
        }

        // на столе что-то лежит
        if (!actor.HasKitchenObject())
        {
            GetKitchenObject().SetParent(actor); // забираем
            return;
        }

        if (actor.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateInHands))
        {
            // в руках тарелка -> добавляем ингредиент со стола
            if (plateInHands.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                GetKitchenObject().DestroySelf();
        }
        else if (GetKitchenObject().TryGetPlate(out PlateKitchenObject plateOnCounter))
        {
            // на столе тарелка, в руках ингредиент -> кладём на тарелку
            if (plateOnCounter.TryAddIngredient(actor.GetKitchenObject().GetKitchenObjectSO()))
                actor.GetKitchenObject().DestroySelf();
        }
    }
}
