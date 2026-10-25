using System;
using UnityEngine;

/// Привозит ящик пожертвований к «двери» кухни. Новый ящик появляется,
/// только когда предыдущий уже использован.
public class DonationSpawner : MonoBehaviour
{
    public event Action OnCrateArrived; // для звука/сообщения «Привезли пожертвования!»

    [SerializeField] private DonationCrate cratePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float firstDelay = 20f;
    [SerializeField] private float interval = 45f;
    [SerializeField] private int amountMin = 4;
    [SerializeField] private int amountMax = 8;

    private DonationCrate current;
    private float timer;

    private void Start() => timer = firstDelay;

    private void Update()
    {
        if (current != null) return; // ящик ещё лежит или несётся

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        timer = interval;
        current = Instantiate(cratePrefab, spawnPoint.position, spawnPoint.rotation);
        current.SetAmount(UnityEngine.Random.Range(amountMin, amountMax + 1));
        OnCrateArrived?.Invoke();
    }
}
