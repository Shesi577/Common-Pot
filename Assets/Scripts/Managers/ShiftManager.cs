using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Таймер смены и экран итога без рейтингов.
public class ShiftManager : MonoBehaviour
{
    [SerializeField] private float shiftDuration = 180f;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private string[] guestStories =
    {
        "Бабушка Анна пришла с внуком и сказала, что суп — как дома.",
        "Студент Миша впервые за неделю поел горячее.",
        "Семья из соседнего дома принесла нам цветы в благодарность.",
        "Сегодня за столом незнакомые люди разговорились и договорились встретиться снова."
    };

    private float timeLeft;
    private bool ended;

    private void Start()
    {
        Time.timeScale = 1f;
        timeLeft = shiftDuration;
        if (summaryPanel != null) summaryPanel.SetActive(false);
        UpdateTimerText();
    }

    private void Update()
    {
        if (ended) return;

        timeLeft -= Time.deltaTime;
        UpdateTimerText();
        if (timeLeft <= 0f) EndShift();
    }

    private void UpdateTimerText()
    {
        if (timerText == null) return;
        int seconds = Mathf.CeilToInt(Mathf.Max(timeLeft, 0f));
        timerText.text = $"{seconds / 60:00}:{seconds % 60:00}";
    }

    private void EndShift()
    {
        ended = true;
        Time.timeScale = 0f;

        // отключаем управление и отпускаем курсор для кнопки
        foreach (PlayerMovement m in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None)) m.enabled = false;
        foreach (PlayerLook l in FindObjectsByType<PlayerLook>(FindObjectsSortMode.None)) l.enabled = false;
        foreach (PlayerInteraction i in FindObjectsByType<PlayerInteraction>(FindObjectsSortMode.None)) i.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        KindnessManager k = KindnessManager.Instance;
        string story = guestStories.Length > 0 ? guestStories[UnityEngine.Random.Range(0, guestStories.Length)] : "";

        if (summaryText != null)
        {
            summaryText.text =
                "Смена окончена!\n" +
                $"Накормлено: {k.GetFedCount()} чел.\n" +
                $"Помогли друг другу: {k.GetTotalHelpCount()} раз\n\n" +
                story;
        }
        if (summaryPanel != null) summaryPanel.SetActive(true);
    }

    /// Привяжите к кнопке «Новая смена».
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
