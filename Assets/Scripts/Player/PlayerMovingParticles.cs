using UnityEngine;

/// Пыль из-под ног: эмиссия включена, только пока игрок идёт.
/// Вешается на PlayerMovingParticles (child корня игрока, у ног).
public class PlayerMovingParticles : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement; // если пусто — берётся у родителя

    private ParticleSystem particles;
    private bool emitting;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        if (playerMovement == null)
            playerMovement = GetComponentInParent<PlayerMovement>();
        SetEmitting(false);
    }

    private void Update()
    {
        if (playerMovement.IsWalking != emitting)
            SetEmitting(playerMovement.IsWalking);
    }

    private void SetEmitting(bool value)
    {
        emitting = value;
        ParticleSystem.EmissionModule emission = particles.emission;
        emission.enabled = value;
    }
}
