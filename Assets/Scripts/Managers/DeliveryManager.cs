using System;
using System.Collections.Generic;
using UnityEngine;

/// Очередь заказов (гостей) и проверка поданных тарелок.
public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    public event Action OnRecipeSpawned;
    public event Action OnRecipeCompleted;
    public event Action OnRecipeSuccess;
    public event Action OnRecipeFailed;

    [SerializeField] private RecipeListSO recipeListSO;
    [SerializeField] private float spawnRecipeTimerMax = 6f;
    [SerializeField] private int waitingRecipesMax = 4;

    private readonly List<RecipeSO> waitingRecipeSOList = new List<RecipeSO>();
    private float spawnRecipeTimer = 2f; // первый заказ через 2 сек
    private int successfulRecipesAmount;

    public List<RecipeSO> GetWaitingRecipeSOList() => waitingRecipeSOList;
    public int GetSuccessfulRecipesAmount() => successfulRecipesAmount;

    private void Awake() => Instance = this;

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer > 0f) return;

        spawnRecipeTimer = spawnRecipeTimerMax;
        if (waitingRecipeSOList.Count >= waitingRecipesMax) return;
        if (recipeListSO == null || recipeListSO.recipeSOList.Count == 0) return;

        RecipeSO recipe = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
        waitingRecipeSOList.Add(recipe);
        OnRecipeSpawned?.Invoke();
    }

    public void DeliverRecipe(PlateKitchenObject plate)
    {
        List<KitchenObjectSO> onPlate = plate.GetIngredients();

        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waiting = waitingRecipeSOList[i];
            if (waiting.kitchenObjectSOList.Count != onPlate.Count) continue;

            bool matches = true;
            foreach (KitchenObjectSO needed in waiting.kitchenObjectSOList)
            {
                if (!onPlate.Contains(needed))
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
            {
                waitingRecipeSOList.RemoveAt(i);
                successfulRecipesAmount++;
                OnRecipeCompleted?.Invoke();
                OnRecipeSuccess?.Invoke();
                return;
            }
        }

        OnRecipeFailed?.Invoke(); // подходящего заказа нет
    }
}
