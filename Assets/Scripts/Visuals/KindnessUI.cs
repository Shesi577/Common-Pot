using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// HUD: шкала «Доброта смены», счётчик «Накормлено» и короткое сообщение о помощи.
public class KindnessUI : MonoBehaviour
{
    [SerializeField] private Image kindnessBar; // Image Type = Filled
    [SerializeField] private TMP_Text fedText;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private float popupDuration = 2f;

    private float popupTimer;

    private void Start()
    {
        KindnessManager m = KindnessManager.Instance;
        m.OnKindnessChanged += HandleKindnessChanged;
        m.OnFedChanged += HandleFedChanged;
        m.OnHelpCounted += ShowPopup;

        HandleKindnessChanged(m.GetKindnessNormalized());
        HandleFedChanged(m.GetFedCount());
        if (popupText != null) popupText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (KindnessManager.Instance == null) return;
        KindnessManager m = KindnessManager.Instance;
        m.OnKindnessChanged -= HandleKindnessChanged;
        m.OnFedChanged -= HandleFedChanged;
        m.OnHelpCounted -= ShowPopup;
    }

    private void Update()
    {
        if (popupTimer <= 0f) return;
        popupTimer -= Time.unscaledDeltaTime;
        if (popupTimer <= 0f && popupText != null) popupText.gameObject.SetActive(false);
    }

    private void HandleKindnessChanged(float normalized)
    {
        if (kindnessBar != null) kindnessBar.fillAmount = normalized;
    }

    private void HandleFedChanged(int fed)
    {
        if (fedText != null) fedText.text = $"Накормлено: {fed}";
    }

    private void ShowPopup(HelpType type)
    {
        if (popupText == null) return;

        popupText.text = type switch
        {
            HelpType.Handoff => "Передал еду напарнику!",
            HelpType.Rescue  => "Спас блюдо вовремя!",
            HelpType.Carry   => "Вдвоём донесли пожертвования!",
            _ => ""
        };
        popupText.gameObject.SetActive(true);
        popupTimer = popupDuration;
    }
}
