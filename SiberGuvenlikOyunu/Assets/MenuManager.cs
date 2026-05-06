using UnityEngine;
using UnityEngine.SceneManagement; // Sahne degisimi

public class AnaMenuYoneticisi : MonoBehaviour
{
    
    public void VeriYilaniAc()
    {
     
        SceneManager.LoadScene("VeriYilaniMenu");
    }

   
    public void KelimeAviAc()
    {
        SceneManager.LoadScene("KelimeAvi");
    }

    public void OyundanCik()
    {
        Debug.Log("Oyundan Çýkýldý!");
        Application.Quit();
    }
}