using System;
using UnityEngine;

public enum HelpType { Handoff, Rescue, Carry }

/// Единая точка, куда игровые системы сообщают «игрок помог другому».
/// KindnessManager слушает эти события и начисляет доброту.
public static class HelpEvents
{
    public static event Action<HelpType> OnHelped;

    public static void Raise(HelpType type) => OnHelped?.Invoke(type);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData() => OnHelped = null;
}
