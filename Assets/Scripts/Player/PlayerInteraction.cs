using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// «Руки» игрока от первого лица. Луч из камеры выбирает прилавок, другого игрока или ящик пожертвований.
/// E — взять/положить/передать/поднять, F — резать.
public class PlayerInteraction : MonoBehaviour, IKitchenObjectParent
{
    /// Выбранный прилавок изменился (null — ничего не выбрано).
    public static event Action<BaseCounter> OnSelectedCounterChanged;
    public event Action OnPickedSomething; // для звука «взял предмет»

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [Tooltip("Слои Counters + Players + Crates")]
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private float handoffCooldown = 2f; // чтобы нельзя было фармить доброту передачей туда-обратно

    private KitchenObject kitchenObject;
    private BaseCounter selectedCounter;
    private PlayerInteraction selectedPlayer;
    private DonationCrate selectedCrate;
    private float lastHandoffTime = -999f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData() => OnSelectedCounterChanged = null;

    private void Update()
    {
        HandleSelection();
        HandleInput();
    }

    private void HandleSelection()
    {
        BaseCounter foundCounter = null;
        PlayerInteraction foundPlayer = null;
        DonationCrate foundCrate = null;
        Transform cam = playerCamera.transform;

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, interactDistance, countersLayerMask))
        {
            Collider col = hit.collider;
            foundCounter = col.GetComponentInParent<BaseCounter>();
            if (foundCounter == null)
            {
                foundCrate = col.GetComponentInParent<DonationCrate>();
                if (foundCrate == null)
                {
                    PlayerInteraction p = col.GetComponentInParent<PlayerInteraction>();
                    if (p != this) foundPlayer = p;
                }
            }
        }

        selectedPlayer = foundPlayer;
        selectedCrate = foundCrate;

        if (foundCounter != selectedCounter)
        {
            selectedCounter = foundCounter;
            OnSelectedCounterChanged?.Invoke(selectedCounter);
        }
    }

    private void HandleInput()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        if (kb == null) return;
        if (kb.eKey.wasPressedThisFrame) TryInteract();
        if (kb.fKey.wasPressedThisFrame) TryInteractAlternate();
#else
        if (Input.GetKeyDown(KeyCode.E)) TryInteract();
        if (Input.GetKeyDown(KeyCode.F)) TryInteractAlternate();
#endif
    }

    public void TryInteract()
    {
        // несём ящик: донести до контейнера или поставить на землю
        DonationCrate carried = DonationCrate.GetCrateCarriedBy(this);
        if (carried != null)
        {
            if (selectedCounter is ContainerCounter container) carried.TryDeliver(this, container);
            else if (selectedCounter == null) carried.Drop();
            return;
        }

        if (selectedCrate != null) { selectedCrate.RequestLift(this); return; }
        if (selectedPlayer != null) { TryGiveTo(selectedPlayer); return; }
        if (selectedCounter != null) selectedCounter.Interact(this);
    }

    public void TryInteractAlternate()
    {
        if (DonationCrate.GetCrateCarriedBy(this) != null) return;
        if (selectedCounter != null) selectedCounter.InteractAlternate(this);
    }

    /// Передать предмет напарнику (у него должны быть пустые руки).
    private void TryGiveTo(PlayerInteraction other)
    {
        if (!HasKitchenObject() || other.HasKitchenObject()) return;
        if (Time.time - lastHandoffTime < handoffCooldown) return;

        lastHandoffTime = Time.time;
        kitchenObject.SetParent(other);
        HelpEvents.Raise(HelpType.Handoff);
    }

    // ---- IKitchenObjectParent ----
    public Transform GetKitchenObjectFollowTransform() => kitchenObjectHoldPoint;
    public KitchenObject GetKitchenObject() => kitchenObject;
    public bool HasKitchenObject() => kitchenObject != null;
    public void ClearKitchenObject() => kitchenObject = null;

    public void SetKitchenObject(KitchenObject newObject)
    {
        kitchenObject = newObject;
        if (newObject != null)
            OnPickedSomething?.Invoke();
    }
}
