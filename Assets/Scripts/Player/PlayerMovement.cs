using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// Движение от первого лица: WASD относительно направления взгляда (поворот тела делает PlayerLook).
/// Вешается на корень игрока (нужен CharacterController).
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private const float Gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;

    public bool IsWalking { get; private set; }

    /// Меняется при переносе тяжёлого ящика (DonationCrate).
    public float SpeedMultiplier { get; set; } = 1f;

    private void Awake() => controller = GetComponent<CharacterController>();

    private void Update()
    {
        Vector2 input = ReadMoveInput();
        Vector3 moveDir = transform.right * input.x + transform.forward * input.y;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        IsWalking = input.sqrMagnitude > 0.0001f;

        verticalVelocity = controller.isGrounded ? -1f : verticalVelocity + Gravity * Time.deltaTime;

        Vector3 velocity = moveDir * moveSpeed * SpeedMultiplier + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private static Vector2 ReadMoveInput()
    {
        Vector2 v = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
        Keyboard kb = Keyboard.current;
        if (kb == null) return v;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) v.y += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) v.y -= 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) v.x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) v.x += 1f;
#else
        v.x = Input.GetAxisRaw("Horizontal");
        v.y = Input.GetAxisRaw("Vertical");
#endif
        return v;
    }
}
