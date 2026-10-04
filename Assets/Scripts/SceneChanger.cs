using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void LoadTargetScene(string sceneName)
    {
        if (InventorySaver.instance != null)
        {
           InventorySaver.instance.Kaydet();
        }
        
        //kayıt tamamlandıktan sonra yeni sahneye geç
        SceneManager.LoadScene(sceneName);
    }
}
