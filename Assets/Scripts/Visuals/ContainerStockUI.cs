using TMPro;
using UnityEngine;

/// Число оставшихся продуктов над ящиком (World Space Canvas + LookAtCamera).
public class ContainerStockUI : MonoBehaviour
{
    [SerializeField] private ContainerCounter containerCounter; // если пусто — берётся у родителя
    [SerializeField] private TMP_Text label;

    private void Awake()
    {
        if (containerCounter == null)
            containerCounter = GetComponentInParent<ContainerCounter>();
    }

    private void Start()
    {
        if (!containerCounter.IsLimited)
        {
            gameObject.SetActive(false); // безлимитный ящик — число не нужно
            return;
        }

        containerCounter.OnStockChanged += Refresh;
        Refresh(containerCounter.Stock);
    }

    private void OnDestroy()
    {
        if (containerCounter != null)
            containerCounter.OnStockChanged -= Refresh;
    }

    private void Refresh(int stock)
    {
        label.text = stock.ToString();
        label.color = stock == 0 ? Color.red : Color.white;
    }
}
