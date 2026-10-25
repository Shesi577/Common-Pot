using System;
using UnityEngine;

/// Общий результат смены: сколько накормлено и «Доброта смены».
/// Очков на игрока нет — всё считается на всех вместе.
public class KindnessManager : MonoBehaviour
{
    public static KindnessManager Instance { get; private set; }

    public event Action<float> OnKindnessChanged; // 0..1
    public event Action<int> OnFedChanged;
    public event Action<HelpType> OnHelpCounted;  // для всплывающих подсказок

    [SerializeField] private float kindnessMax = 100f;
    [SerializeField] private float kindnessPerGuestFed = 4f;
    [SerializeField] private float kindnessPerHandoff = 3f;
    [SerializeField] private float kindnessPerRescue = 5f;
    [SerializeField] private float kindnessPerCarry = 15f;

    private float kindness;
    private int fedCount;
    private readonly int[] helpCounts = new int[Enum.GetValues(typeof(HelpType)).Length];

    public int GetFedCount() => fedCount;
    public float GetKindnessNormalized() => kindness / kindnessMax;
    public int GetHelpCount(HelpType type) => helpCounts[(int)type];

    public int GetTotalHelpCount()
    {
        int total = 0;
        foreach (int c in helpCounts) total += c;
        return total;
    }

    private void Awake() => Instance = this;

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += HandleGuestFed;
        HelpEvents.OnHelped += HandleHelp;
    }

    private void OnDestroy()
    {
        if (DeliveryManager.Instance != null)
            DeliveryManager.Instance.OnRecipeSuccess -= HandleGuestFed;
        HelpEvents.OnHelped -= HandleHelp;
    }

    private void HandleGuestFed()
    {
        fedCount++;
        OnFedChanged?.Invoke(fedCount);
        AddKindness(kindnessPerGuestFed);
    }

    private void HandleHelp(HelpType type)
    {
        helpCounts[(int)type]++;
        OnHelpCounted?.Invoke(type);

        switch (type)
        {
            case HelpType.Handoff: AddKindness(kindnessPerHandoff); break;
            case HelpType.Rescue:  AddKindness(kindnessPerRescue);  break;
            case HelpType.Carry:   AddKindness(kindnessPerCarry);   break;
        }
    }

    private void AddKindness(float amount)
    {
        kindness = Mathf.Min(kindness + amount, kindnessMax);
        OnKindnessChanged?.Invoke(GetKindnessNormalized());
    }
}
