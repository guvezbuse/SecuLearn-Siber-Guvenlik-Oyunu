using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using System;

public class GirisYoneticisi : MonoBehaviour
{
    [Header("UI Elemanlarý")]
    public TMP_InputField emailInput;
    public TMP_InputField sifreInput;
    public TextMeshProUGUI hataYazisi;

    private FirebaseAuth auth;

    private bool girisBasariliMi = false;
    private bool hataVarMi = false;
    private string guncelHataMesaji = "";

    
    private Color guncelRenk = Color.red;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    void Update()
    {
        if (girisBasariliMi == true)
        {
            girisBasariliMi = false;
            SceneManager.LoadScene("AnaMenu");
        }

        if (hataVarMi == true)
        {
            hataVarMi = false;
            hataYazisi.text = guncelHataMesaji;

           
            hataYazisi.color = guncelRenk;
        }
    }

    public void KayitOlBasildi()
    {
        string email = emailInput.text;
        string sifre = sifreInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre))
        {
            HataVer("Lütfen tüm alanlarý doldurunuz!");
            return;
        }

        if (!SifreGucluMu(sifre)) return;

        hataYazisi.color = Color.yellow;
        hataYazisi.text = "Kayýt olunuyor...";
        
        //firebase.auth hazýr metot: kullanýcý kaydý
        auth.CreateUserWithEmailAndPasswordAsync(email, sifre).ContinueWith(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                string hataMesaji = "";
                if (task.Exception != null)
                {
                    hataMesaji = task.Exception.InnerExceptions[0].Message;
                }

                if (hataMesaji.Contains("already in use"))
                {
                    HataVer("Bu e-posta adresi zaten kayýtlý! Giriþ yapmayý dene.");
                }
                else
                {
                    HataVer("Kayýt hatasý: " + hataMesaji);
                }
                return;
            }

            FirebaseUser yeniKullanici = task.Result.User;
            if (yeniKullanici != null)
            {
                yeniKullanici.SendEmailVerificationAsync();
            }

            
            BilgiVer("Kayýt Baþarýlý! Mail kutunuza gelen linke týklayýp onaylayýn.");
        });
    }

    public void GirisYapBasildi()
    {
        string email = emailInput.text;
        string sifre = sifreInput.text;

        hataYazisi.color = Color.yellow;
        hataYazisi.text = "Giriþ yapýlýyor...";

        //firebase.auth kullanýcý girisi metodu
        auth.SignInWithEmailAndPasswordAsync(email, sifre).ContinueWith(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                HataVer("Giriþ Baþarýsýz! E-posta veya þifre yanlýþ.");
                return;
            }

            FirebaseUser kullanici = task.Result.User;

            if (kullanici.IsEmailVerified)
            {
                girisBasariliMi = true;
            }
            else
            {
                HataVer("Lütfen önce mail adresinize gelen linke týklayýp hesabýnýzý onaylayýn!");
                auth.SignOut();
            }
        });
    }

    bool SifreGucluMu(string sifre)
    {
        if (sifre.Length < 8) { HataVer("Þifre en az 8 karakter olmalý!"); return false; }

        bool buyuk = false, kucuk = false, rakam = false;
        foreach (char c in sifre)
        {
            if (char.IsUpper(c)) buyuk = true;
            else if (char.IsLower(c)) kucuk = true;
            else if (char.IsDigit(c)) rakam = true;
        }

        if (!buyuk) { HataVer("En az 1 BÜYÜK harf gerekli!"); return false; }
        if (!kucuk) { HataVer("En az 1 küçük harf gerekli!"); return false; }
        if (!rakam) { HataVer("En az 1 rakam gerekli!"); return false; }

        return true;
    }

    void HataVer(string mesaj)
    {
        Debug.LogWarning("UI Mesajý: " + mesaj);
        guncelHataMesaji = mesaj;
        guncelRenk = Color.red; // Hata ise Kýrmýzý
        hataVarMi = true;
    }

   
    void BilgiVer(string mesaj)
    {
        Debug.Log("UI Bilgi: " + mesaj);
        guncelHataMesaji = mesaj;
        guncelRenk = Color.green; // Bilgi ise Yeþil
        hataVarMi = true;
    }
}