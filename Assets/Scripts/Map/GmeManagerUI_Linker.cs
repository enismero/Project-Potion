using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GmeManagerUI_Linker : MonoBehaviour
{
    [Header("sahnedeki yazı objeleri")]
    public TextMeshProUGUI localDayText;
    public TextMeshProUGUI localTimeText;
    public TextMeshProUGUI localMoneyText;

    private void Start()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.dayText = localDayText;
            GameManager.instance.timeText = localTimeText;
            GameManager.instance.moneyText = localMoneyText;

            GameManager.instance.UpdateUI();
        }
    }
}
