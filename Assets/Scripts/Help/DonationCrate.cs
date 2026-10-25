using System.Collections.Generic;
using UnityEngine;

/// Тяжёлый ящик с пожертвованиями. Поднимается вдвоём, несётся к ящику с продуктами
/// и пополняет его запас. Префаб: меш + коллайдер (слой Crates) + этот скрипт.
public class DonationCrate : MonoBehaviour
{
    [SerializeField] private int amount = 6;                    // сколько продуктов добавит
    [SerializeField] private float liftWindow = 4f;             // сек. на то, чтобы второй игрок подхватил
    [SerializeField] private float maxCarrierDistance = 4.5f;   // дальше — ящик падает
    [SerializeField] private float carrySpeedMultiplier = 0.6f; // скорость вдвоём
    [SerializeField] private float soloCarrySpeedMultiplier = 0.3f;
    [SerializeField] private float carryHeight = 1.1f;

    // Для теста без сети: один игрок может поднять ящик, но очень медленно.
    // Когда в игре будут 2+ игрока — выключите.
    [SerializeField] private bool allowSoloCarry = true;

    private static readonly List<DonationCrate> All = new List<DonationCrate>();

    private readonly List<PlayerInteraction> carriers = new List<PlayerInteraction>();
    private PlayerInteraction firstRequester;
    private float requestTimer;
    private float groundY;

    public int Amount => amount;
    public void SetAmount(int value) => amount = Mathf.Max(1, value);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData() => All.Clear();

    private void Awake() => groundY = transform.position.y;
    private void OnEnable() => All.Add(this);
    private void OnDisable() => All.Remove(this);

    public static DonationCrate GetCrateCarriedBy(PlayerInteraction player)
    {
        foreach (DonationCrate crate in All)
            if (crate.carriers.Contains(player)) return crate;
        return null;
    }

    /// Игрок нажал E, глядя на ящик.
    public void RequestLift(PlayerInteraction player)
    {
        if (player.HasKitchenObject()) return; // руки заняты

        // ящик уже подняли — второй игрок может присоединиться
        if (carriers.Count > 0)
        {
            if (carriers.Count < 2 && !carriers.Contains(player))
            {
                carriers.Add(player);
                ApplySpeed();
            }
            return;
        }

        if (allowSoloCarry)
        {
            StartCarry(player);
            return;
        }

        // нужно двое: первый «берётся за ящик», второй должен успеть за liftWindow секунд
        if (firstRequester == null || firstRequester == player)
        {
            firstRequester = player;
            requestTimer = liftWindow;
            return;
        }

        StartCarry(firstRequester);
        carriers.Add(player);
        firstRequester = null;
        ApplySpeed();
    }

    /// Донести ящик до контейнера с продуктами.
    public bool TryDeliver(PlayerInteraction player, ContainerCounter counter)
    {
        if (!carriers.Contains(player)) return false;
        if (!counter.IsLimited) return false;

        counter.AddStock(amount);
        if (carriers.Count >= 2)
            HelpEvents.Raise(HelpType.Carry); // награда только за совместный перенос

        ReleaseCarriers();
        Destroy(gameObject);
        return true;
    }

    public void Drop()
    {
        ReleaseCarriers();
        Vector3 p = transform.position;
        transform.position = new Vector3(p.x, groundY, p.z);
    }

    private void StartCarry(PlayerInteraction first)
    {
        carriers.Add(first);
        SetCollidersEnabled(false); // чтобы ящик не перекрывал луч выбора
        ApplySpeed();
    }

    private void Update()
    {
        if (firstRequester != null)
        {
            requestTimer -= Time.deltaTime;
            if (requestTimer <= 0f) firstRequester = null;
        }

        if (carriers.Count == 0) return;

        carriers.RemoveAll(c => c == null);
        if (carriers.Count == 0)
        {
            ReleaseCarriers();
            return;
        }

        if (carriers.Count == 2 &&
            Vector3.Distance(carriers[0].transform.position, carriers[1].transform.position) > maxCarrierDistance)
        {
            Drop(); // отошли друг от друга слишком далеко
            return;
        }

        Vector3 center = Vector3.zero;
        foreach (PlayerInteraction c in carriers) center += c.transform.position;
        center /= carriers.Count;
        center.y = groundY + carryHeight;
        transform.position = center;
    }

    private void ApplySpeed()
    {
        float mult = carriers.Count >= 2 ? carrySpeedMultiplier : soloCarrySpeedMultiplier;
        foreach (PlayerInteraction c in carriers)
            if (c != null && c.TryGetComponent(out PlayerMovement movement))
                movement.SpeedMultiplier = mult;
    }

    private void ReleaseCarriers()
    {
        foreach (PlayerInteraction c in carriers)
            if (c != null && c.TryGetComponent(out PlayerMovement movement))
                movement.SpeedMultiplier = 1f;

        carriers.Clear();
        SetCollidersEnabled(true);
    }

    private void SetCollidersEnabled(bool enabledValue)
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = enabledValue;
    }
}
