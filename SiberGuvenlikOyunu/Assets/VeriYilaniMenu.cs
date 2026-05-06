using UnityEngine;
using UnityEngine.SceneManagement; // Sahne degisimi icin

public class VeriYilaniMenu : MonoBehaviour
{
    public void OyunuBaslat()
    {
     
        SceneManager.LoadScene("VeriYilani");
    }

    public void OyundanCik()
    {
        Debug.Log("Oyundan Çýkýldý!");
        Application.Quit();
    }
}