using UnityEngine;
using UnityEngine.UI;

/// Полоска прогресса над прилавком (World Space Canvas).
/// hasProgressGameObject — объект, у которого есть CuttingCounter или StoveCounter.
public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image barImage; // Image Type = Filled, Horizontal

    private IHasProgress hasProgress;

    private void Start()
    {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (hasProgress == null)
        {
            Debug.LogError(hasProgressGameObject + " не реализует IHasProgress");
            return;
        }

        hasProgress.OnProgressChanged += HandleProgressChanged;
        barImage.fillAmount = 0f;
        Hide();
    }

    private void OnDestroy()
    {
        if (hasProgress != null)
            hasProgress.OnProgressChanged -= HandleProgressChanged;
    }

    private void HandleProgressChanged(float progress)
    {
        barImage.fillAmount = progress;
        if (progress <= 0f || progress >= 1f) Hide();
        else Show();
    }

    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}
