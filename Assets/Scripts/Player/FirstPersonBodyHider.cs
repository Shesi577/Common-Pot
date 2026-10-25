using UnityEngine;
using UnityEngine.Rendering;

/// Прячет собственное тело от первого лица (тень остаётся).
/// Вешается на PlayerVisual. В мультиплеере включать только у владельца игрока,
/// чтобы другие игроки видели тело.
public class FirstPersonBodyHider : MonoBehaviour
{
    private void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
    }
}
