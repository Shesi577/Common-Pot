using System;
using UnityEngine;

/// Стопка тарелок: тарелки появляются по таймеру, игрок берёт пустыми руками.
public class PlatesCounter : BaseCounter
{
    public event Action OnPlateSpawned;
    public event Action OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    [SerializeField] private float spawnPlateTimerMax = 4f;
    [SerializeField] private int platesSpawnedAmountMax = 4;

    private float spawnPlateTimer;
    private int platesSpawnedAmount;

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer < spawnPlateTimerMax) return;

        spawnPlateTimer = 0f;
        if (platesSpawnedAmount < platesSpawnedAmountMax)
        {
            platesSpawnedAmount++;
            OnPlateSpawned?.Invoke();
        }
    }

    public override void Interact(IKitchenObjectParent actor)
    {
        if (actor.HasKitchenObject()) return; // руки заняты
        if (platesSpawnedAmount <= 0) return; // тарелок нет

        platesSpawnedAmount--;
        KitchenObject.Spawn(plateKitchenObjectSO, actor);
        OnPlateRemoved?.Invoke();
    }
}
