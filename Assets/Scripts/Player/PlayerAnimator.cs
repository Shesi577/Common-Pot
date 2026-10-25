using UnityEngine;

/// Вешается на корень визуала игрока вместе с Animator
/// (Controller = Player.controller; дети корня: Body и Head).
public class PlayerAnimator : MonoBehaviour
{
    private const string IsWalking = "IsWalking"; // bool из Player.controller

    [SerializeField] private PlayerMovement playerMovement; // если пусто — берётся у родителя

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (playerMovement == null)
            playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Update() => animator.SetBool(IsWalking, playerMovement.IsWalking);
}
