using System;

/// Всё, что показывает полоску прогресса (нарезка, жарка).
/// Значение 0..1. Полоска прячется при 0 и при 1.
public interface IHasProgress
{
    event Action<float> OnProgressChanged;
}
