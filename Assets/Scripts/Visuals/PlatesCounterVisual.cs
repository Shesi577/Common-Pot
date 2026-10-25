using System.Collections.Generic;
using UnityEngine;

/// Рисует стопку тарелок на PlatesCounter.
/// plateVisualPrefab — префаб тарелки ТОЛЬКО с мешем (без KitchenObject).
public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter; // если пусто — берётся у родителя
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualPrefab;
    [SerializeField] private float plateOffsetY = 0.1f;

    private readonly List<GameObject> plateVisuals = new List<GameObject>();

    private void Awake()
    {
        if (platesCounter == null)
            platesCounter = GetComponentInParent<PlatesCounter>();
    }

    private void Start()
    {
        platesCounter.OnPlateSpawned += HandlePlateSpawned;
        platesCounter.OnPlateRemoved += HandlePlateRemoved;
    }

    private void OnDestroy()
    {
        if (platesCounter == null) return;
        platesCounter.OnPlateSpawned -= HandlePlateSpawned;
        platesCounter.OnPlateRemoved -= HandlePlateRemoved;
    }

    private void HandlePlateSpawned()
    {
        Transform plate = Instantiate(plateVisualPrefab, counterTopPoint);
        plate.localPosition = new Vector3(0f, plateOffsetY * plateVisuals.Count, 0f);
        plateVisuals.Add(plate.gameObject);
    }

    private void HandlePlateRemoved()
    {
        if (plateVisuals.Count == 0) return;
        GameObject last = plateVisuals[plateVisuals.Count - 1];
        plateVisuals.RemoveAt(plateVisuals.Count - 1);
        Destroy(last);
    }
}
