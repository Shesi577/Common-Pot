using System;
using UnityEngine;

/// Ящик с пожертвованиями. Запас ограничен и случаен: пополняется ящиком DonationCrate.
public class ContainerCounter : BaseCounter
{
    public event Action OnPlayerGrabbedObject; // для анимации открытия крышки
    public event Action<int> OnStockChanged;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private bool limitedStock = true;
    [SerializeField] private int initialStockMin = 4;
    [SerializeField] private int initialStockMax = 10;
    [SerializeField] private int stockCap = 20;

    private int stock;

    public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;
    public bool IsLimited => limitedStock;
    public int Stock => stock;

    private void Awake()
    {
        if (limitedStock)
            stock = UnityEngine.Random.Range(initialStockMin, initialStockMax + 1);
    }

    public override void Interact(IKitchenObjectParent actor)
    {
        if (actor.HasKitchenObject()) return;       // руки заняты
        if (limitedStock && stock <= 0) return;     // ящик пуст

        KitchenObject.Spawn(kitchenObjectSO, actor);

        if (limitedStock)
        {
            stock--;
            OnStockChanged?.Invoke(stock);
        }
        OnPlayerGrabbedObject?.Invoke();
    }

    public void AddStock(int amount)
    {
        if (!limitedStock) return;
        stock = Mathf.Min(stock + amount, stockCap);
        OnStockChanged?.Invoke(stock);
    }
}
