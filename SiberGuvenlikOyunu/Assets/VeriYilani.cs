using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class VeriYilani : MonoBehaviour
{
    [Header("Görsel Ayarlar")]
    public GameObject govdePrefab;
    public GameObject kuyrukPrefab;
    public GameObject[] yemPrefablari;

    [Header("UI Ayarlarý")]
    public TextMeshProUGUI puanYazisi;
    public GameObject[] kalpGorselleri;

    // Game Over Ekraný
    public GameObject oyunBittiPaneli;
    public TextMeshProUGUI sonucSkorYazisi;

    [Header("Oyun Ayarlarý")]
    public float hareketHizi = 0.2f;
    public int kacYemdeBuyusun = 5;

    // Sinirlar
    public float xSinir = 20f;
    public float ySinir = 11f;

    // Hreket degiskenleri
    private Vector2 yon;
    private Vector2 sonHareketYonu;
    private List<Vector2> hareketHafizasi = new List<Vector2>();

    private List<Transform> kuyrukListesi = new List<Transform>();
    private List<GameObject> sahnedekiYemler = new List<GameObject>();

    private bool oyunDevamEdiyor = true;
    private int puan = 0;
    private int mevcutCan = 3;
    private int yenenYemSayisi = 0;
    private bool hasarAlabilir = true;

   
    private bool oyunDurduMu = false;

    void Start()
    {
        // Oyun yeniden basladýðýnda akis kontrolu
        Time.timeScale = 1f;
        oyunDurduMu = false;

        yon = Vector2.right;
        sonHareketYonu = Vector2.right;

        if (oyunBittiPaneli != null) oyunBittiPaneli.SetActive(false);

        // Kafa + Govde + Kuyruk kurulumu
        GameObject govde = Instantiate(govdePrefab, new Vector3(-1, 0, 0), Quaternion.identity);
        kuyrukListesi.Add(govde.transform);
        GameObject kuyruk = Instantiate(kuyrukPrefab, new Vector3(-2, 0, 0), Quaternion.identity);
        kuyrukListesi.Add(kuyruk.transform);

        InvokeRepeating("HareketEt", 0.1f, hareketHizi);

        // Baþlangýç yemleri (15 tane)
        for (int i = 0; i < 15; i++) TekBirYemYarat();

        // Rotasyon (15 saniyede bir)
        InvokeRepeating("YemRotasyonu", 15f, 15f);

        PuaniGuncelle();
    }

    void Update()
    {
       
        if (!oyunDevamEdiyor || oyunDurduMu) return;

        if (Input.GetKeyDown(KeyCode.UpArrow)) YonuHafizayaEkle(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) YonuHafizayaEkle(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) YonuHafizayaEkle(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) YonuHafizayaEkle(Vector2.right);
    }

    // Oyunu durdurma
    public void OyunuDurdurDevamEttir()
    {
        // oyun sonlandýysa durdur butonu calismasin
        if (!oyunDevamEdiyor) return;

        if (oyunDurduMu)
        {
            // oyun durmus, devam ettir
            Time.timeScale = 1f;
            oyunDurduMu = false;
        }
        else
        {
            // Oyun çaktif, durdur
            Time.timeScale = 0f;
            oyunDurduMu = true;
        }
    }

    void YonuHafizayaEkle(Vector2 yeniYon)
    {
        Vector2 referansYon = (hareketHafizasi.Count > 0) ? hareketHafizasi[hareketHafizasi.Count - 1] : sonHareketYonu;
        if (yeniYon == -referansYon) return;
        if (yeniYon == referansYon) return;

        if (hareketHafizasi.Count < 2)
        {
            hareketHafizasi.Add(yeniYon);
        }
    }

    public void YukariBas() { YonuHafizayaEkle(Vector2.up); }
    public void AsagiBas() { YonuHafizayaEkle(Vector2.down); }
    public void SolaBas() { YonuHafizayaEkle(Vector2.left); }
    public void SagaBas() { YonuHafizayaEkle(Vector2.right); }

    void HareketEt()
    {
        if (!oyunDevamEdiyor) return;

        if (hareketHafizasi.Count > 0)
        {
            yon = hareketHafizasi[0];
            hareketHafizasi.RemoveAt(0);

            float zAci = 0;
            if (yon == Vector2.right) zAci = 180;
            else if (yon == Vector2.left) zAci = 0;
            else if (yon == Vector2.up) zAci = -90;
            else if (yon == Vector2.down) zAci = 90;
            transform.rotation = Quaternion.Euler(0, 0, zAci);
        }

        sonHareketYonu = yon;

        Vector3 oncekiKonum = transform.position;
        Quaternion oncekiAci = transform.rotation;

        transform.position += (Vector3)yon;

        if (kuyrukListesi.Count > 0)
        {
            Vector3 geciciKonum = kuyrukListesi[0].position;
            Quaternion geciciAci = kuyrukListesi[0].rotation;

            kuyrukListesi[0].position = oncekiKonum;
            kuyrukListesi[0].rotation = oncekiAci;

            for (int i = 1; i < kuyrukListesi.Count; i++)
            {
                Vector3 suankiKonum = kuyrukListesi[i].position;
                Quaternion suankiAci = kuyrukListesi[i].rotation;

                kuyrukListesi[i].position = geciciKonum;
                kuyrukListesi[i].rotation = geciciAci;

                geciciKonum = suankiKonum;
                geciciAci = suankiAci;
            }
        }
    }

    void YemRotasyonu()
    {
        if (sahnedekiYemler.Count < 3) { TekBirYemYarat(); return; }

        for (int i = 0; i < 3; i++)
        {
            if (sahnedekiYemler.Count > 0)
            {
                int rastgeleIndex = Random.Range(0, sahnedekiYemler.Count);
                GameObject silinecekYem = sahnedekiYemler[rastgeleIndex];
                sahnedekiYemler.RemoveAt(rastgeleIndex);
                if (silinecekYem != null) Destroy(silinecekYem);
            }
        }
        for (int i = 0; i < 3; i++) TekBirYemYarat();
    }

    void TekBirYemYarat()
    {
        Vector3 potansiyelKonum = Vector3.zero;
        bool uygunKonumBulundu = false;
        int denemeSayisi = 0;

        while (!uygunKonumBulundu && denemeSayisi < 50)
        {
            denemeSayisi++;
            int rX = Mathf.RoundToInt(Random.Range(-xSinir + 1, xSinir - 1));
            int rY = Mathf.RoundToInt(Random.Range(-ySinir + 1, ySinir - 1));
            potansiyelKonum = new Vector3(rX, rY, 0);

            uygunKonumBulundu = true;

            if (Vector3.Distance(transform.position, potansiyelKonum) < 5f)
            {
                uygunKonumBulundu = false;
                continue;
            }
            foreach (GameObject digerYem in sahnedekiYemler)
            {
                if (digerYem != null)
                {
                    if (Vector3.Distance(digerYem.transform.position, potansiyelKonum) < 5f)
                    {
                        uygunKonumBulundu = false;
                        break;
                    }
                }
            }
        }

        int rIndex = Random.Range(0, yemPrefablari.Length);
        GameObject yeniYem = Instantiate(yemPrefablari[rIndex], potansiyelKonum, Quaternion.identity);
        sahnedekiYemler.Add(yeniYem);
    }

    void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.CompareTag("Yem"))
        {
            YemiYe(diger.gameObject);
        }
        else if (diger.CompareTag("ZararliYem") || diger.CompareTag("Engel"))
        {
            if (diger.gameObject.name.Contains("Duvar") || diger.gameObject.name.Contains("Wall"))
            {
                transform.position -= (Vector3)yon;
                hareketHafizasi.Clear();
                yon = -yon;
                sonHareketYonu = yon;
                CanKaybet();
            }
            else
            {
                if (sahnedekiYemler.Contains(diger.gameObject)) sahnedekiYemler.Remove(diger.gameObject);
                Destroy(diger.gameObject);
                TekBirYemYarat();
                CanKaybet();
            }
        }
        else if (kuyrukListesi.Contains(diger.transform))
        {
            CanKaybet();
        }
    }

    void YemiYe(GameObject yem)
    {
        if (sahnedekiYemler.Contains(yem)) sahnedekiYemler.Remove(yem);
        Destroy(yem);
        puan += 10;
        PuaniGuncelle();
        yenenYemSayisi++;

        if (yenenYemSayisi % kacYemdeBuyusun == 0) Buyu();
        TekBirYemYarat();
    }

    void Buyu()
    {
        Transform sonParca = kuyrukListesi[kuyrukListesi.Count - 1];
        GameObject yeniGovde = Instantiate(govdePrefab, sonParca.position, sonParca.rotation);
        kuyrukListesi.Insert(kuyrukListesi.Count - 1, yeniGovde.transform);
    }

    void CanKaybet()
    {
        if (hasarAlabilir == false) return;
        mevcutCan--;
        if (mevcutCan >= 0 && mevcutCan < kalpGorselleri.Length) kalpGorselleri[mevcutCan].SetActive(false);

        if (mevcutCan <= 0) OyunBitti();
        else { hasarAlabilir = false; Invoke("HasarKorumasiniKaldir", 1.0f); }
    }

    void HasarKorumasiniKaldir() { hasarAlabilir = true; }
    void PuaniGuncelle() { if (puanYazisi != null) puanYazisi.text = "PUAN: " + puan.ToString(); }

    void OyunBitti()
    {
        oyunDevamEdiyor = false;
        CancelInvoke("HareketEt");
        CancelInvoke("YemRotasyonu");
        Debug.Log("OYUN BÝTTÝ!");

        if (oyunBittiPaneli != null)
        {
            oyunBittiPaneli.SetActive(true);
            if (sonucSkorYazisi != null)
            {
                sonucSkorYazisi.text = "SKOR: " + puan.ToString();
            }
        }
    }

    public void OyunuYenidenBaslat()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}