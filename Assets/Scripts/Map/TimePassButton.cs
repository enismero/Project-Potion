using UnityEngine;

public class TimePassButton : MonoBehaviour
{
    [Header("Kaç Dakika Geçsin")]
    public int minutesToPass=60;

    public void ClickToPassTime()
    {
        
        if (GameManager.instance != null)
        {
            GameManager.instance.PassTime(minutesToPass);
        }
    }
}
