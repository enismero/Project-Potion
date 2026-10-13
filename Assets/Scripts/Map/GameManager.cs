using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Time Settings")]
    public int currentDay=1;
    [Tooltip("1 dknın geçiceği saniye")]
    public float timeSpeed=10f;

    private float currentMinuteInDay=480f;//(8:00=>8.60=480)

    [Header("Money")]
    public int currentMoney=250;

    [Header("UI Referance")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI moneyText;

    private void Awake()
    {
        if (instance == null)
        {
            instance=this;
            DontDestroyOnLoad(gameObject); //sahne değişiminde silme
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        currentMinuteInDay+=Time.deltaTime*timeSpeed;
        //1day= 1440minute
        if(currentMinuteInDay >= 1440f)
        {
            currentMinuteInDay -= 1440f;
            currentDay++;
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        if(dayText!=null) dayText.text="Gün "+currentDay;

        if (timeText != null)
        {
            int hours = (int)(currentMinuteInDay/60f);
            int minutes = (int)(currentMinuteInDay % 60f);
            timeText.text = string.Format("{00:00}:{01:00}", hours, minutes);
        }

        if (moneyText != null)
            moneyText.text = currentMoney.ToString() + " G";
    }

    //para ekle
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateUI();
    }
    //para çıkar
    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateUI();
            return true;
        }
        Debug.Log("Para yetersiz");
        return false;
    }

    public void PassTime(int minutesToPass)
    {
        currentMinuteInDay += minutesToPass;

        while (currentMinuteInDay >= 1440f)
        {
            currentMinuteInDay -= 1440f; //çok uzun süre atlanabilsin diye if erine while kullanıldı
            currentDay++;
        }
        UpdateUI();
        Debug.Log(minutesToPass+" geçti");
    }
}
