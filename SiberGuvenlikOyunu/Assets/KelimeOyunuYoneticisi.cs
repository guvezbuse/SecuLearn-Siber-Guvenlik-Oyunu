using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class KelimeOyunuYoneticisi : MonoBehaviour
{
    [System.Serializable]
    public class Soru
    {
        public string kelime;
        public string ipucu;

        public Soru(string k, string i)
        {
            kelime = k;
            ipucu = i;
        }
    }

    [Header("Oyun Ayarlari")]
    public List<Soru> soruListesi = new List<Soru>();

    private string gizliKelime;
    public int kalanCan = 4;
    private int maksimumCan = 4;

    [Header("Ekran Baglantilari")]
    public TMP_Text kelimeGostergeYazisi;
    public TMP_Text canGostergeYazisi;
    public TMP_Text bilgiYazisi;

   
    public GameObject tekrarOynaButonu;

    [Header("Gorsel Ayarlari")]
    public Image durumGorseli;

    public Sprite baslangicResmi;
    public List<Sprite> hasarResimleri;
    public Sprite kazanmaResmi;

    private List<char> bilinenHarfler = new List<char>();
    private bool oyunBittiMi = false;

    void Start()
    {
        // oyun baslarken butonu gizle
        if (tekrarOynaButonu != null)
        {
            tekrarOynaButonu.SetActive(false);
        }

        ListeyiDoldur();
        YeniOyunBaslat();
    }

    void ListeyiDoldur()
    {
        soruListesi.Clear();
        soruListesi.Add(new Soru("GÝZLÝLÝK", "Bilginin yetkisiz kiþilerin eline geçmesini engellemek."));
        soruListesi.Add(new Soru("BÜTÜNLÜK", "Verinin izinsiz deðiþtirilmesini veya silinmesini önlemek."));
        soruListesi.Add(new Soru("ERÝÞÝLEBÝLÝRLÝK", "Bilgiye ihtiyaç duyulduðunda ulaþýlabilir olmasý."));
        soruListesi.Add(new Soru("PHISHING", "Sahte e-postalarla þifre çalmaya çalýþan saldýrý (Yemleme)."));
        soruListesi.Add(new Soru("KEYLOGGER", "Klavyede basýlan tuþlarý kaydeden casus yazýlým."));
        soruListesi.Add(new Soru("TROJAN", "Kendini yararlý gibi gösteren ama zararlý olan yazýlým (Truva Atý)."));
        soruListesi.Add(new Soru("VÝRÜS", "Bilgisayara bulaþýp dosyalara zarar veren kötü amaçlý kod."));
        soruListesi.Add(new Soru("FIREWALL", "Að güvenliðini saðlayan, gelen ve giden trafiði süzen duvar."));
        soruListesi.Add(new Soru("PAROLA", "Hesaplarýmýza girmek için kullandýðýmýz gizli anahtar kelime."));
        soruListesi.Add(new Soru("YEDEKLEME", "Verilerin kaybolmasýna karþý kopyasýnýn alýnmasý."));
        soruListesi.Add(new Soru("WHOIS", "Bir web sitesinin (alan adýnýn) kime ait olduðunu sorgulama servisi."));
        // arttirilabilir...
    }

    void YeniOyunBaslat()
    {
        int rastgeleSayi = Random.Range(0, soruListesi.Count);
        Soru secilenSoru = soruListesi[rastgeleSayi];

        gizliKelime = secilenSoru.kelime;

        if (bilgiYazisi != null)
        {
            bilgiYazisi.color = Color.white;
            bilgiYazisi.text = secilenSoru.ipucu;
        }

        bilinenHarfler.Clear();
        kalanCan = maksimumCan;
        oyunBittiMi = false;

        EkraniGuncelle();
    }

    public void HarfTahminEt(string tiklananHarf)
    {
        if (oyunBittiMi) return;

        string gelenHarfString = tiklananHarf.ToUpper();
        char harf = gelenHarfString[0];

        if (bilinenHarfler.Contains(harf)) return;

        bilinenHarfler.Add(harf);

        if (!gizliKelime.Contains(gelenHarfString))
        {
            kalanCan--;
        }

        EkraniGuncelle();
        OyunBittiMiKontrolEt();
    }

    void EkraniGuncelle()
    {
        canGostergeYazisi.text = "KALAN CAN: " + kalanCan;

        if (durumGorseli != null && !oyunBittiMi)
        {
            if (kalanCan == maksimumCan)
            {
                durumGorseli.sprite = baslangicResmi;
            }
            else
            {
                int hasarIndex = (maksimumCan - kalanCan) - 1;
                if (hasarIndex >= 0 && hasarIndex < hasarResimleri.Count)
                {
                    durumGorseli.sprite = hasarResimleri[hasarIndex];
                }
            }
        }

        string ekrandakiYazi = "";
        foreach (char h in gizliKelime)
        {
            if (bilinenHarfler.Contains(h))
                ekrandakiYazi += h + " ";
            else
                ekrandakiYazi += "_ ";
        }
        kelimeGostergeYazisi.text = ekrandakiYazi;
    }

    void OyunBittiMiKontrolEt()
    {
        bool oyunSonlandi = false; 

        if (kalanCan <= 0)
        {
            // kaybetme
            kelimeGostergeYazisi.text = gizliKelime;

            if (bilgiYazisi != null)
            {
                bilgiYazisi.color = Color.red;
                bilgiYazisi.text = "SÝSTEM HACKLENDÝ! KAYBETTÝNÝZ.";
            }

            if (durumGorseli != null && hasarResimleri.Count > 0)
            {
                durumGorseli.sprite = hasarResimleri[hasarResimleri.Count - 1];
            }

            oyunSonlandi = true;
        }
        else if (!kelimeGostergeYazisi.text.Contains("_"))
        {
            // kazanma
            if (bilgiYazisi != null)
            {
                bilgiYazisi.color = Color.green;
                bilgiYazisi.text = "TEBRÝKLER! GÜVENLÝK SAÐLANDI.";
            }

            if (durumGorseli != null && kazanmaResmi != null)
            {
                durumGorseli.sprite = kazanmaResmi;
            }

            oyunSonlandi = true;
        }

        // tekrar oyna butonunu goster
        if (oyunSonlandi)
        {
            oyunBittiMi = true;
            if (tekrarOynaButonu != null)
            {
                tekrarOynaButonu.SetActive(true);
            }
        }
    }

    public void OyunuYenidenBaslat()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}