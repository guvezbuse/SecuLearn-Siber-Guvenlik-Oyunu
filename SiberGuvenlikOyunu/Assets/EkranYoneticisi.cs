using UnityEngine;

public class EkranYoneticisi : MonoBehaviour
{
    public bool yatayOlsun = false; 

    void Start()
    {
        // Sahne baþladýðýnda ayarý kontrol et ve uygula
        EkranYonunuAyarla();
    }

    public void EkranYonunuAyarla()
    {
        if (yatayOlsun)
        {
            // Oyunu YATAY (Landscape) moda zorla
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Debug.Log("Ekran YATAY moda alýndý.");
        }
        else
        {
            // Oyunu DÝK (Portrait) moda zorla
            Screen.orientation = ScreenOrientation.Portrait;
            Debug.Log("Ekran DÝK moda alýndý.");
        }
    }
}