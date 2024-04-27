using ArgeMup.HazirKod;
using ArgeMup.HazirKod.Ekİşlemler;
using System;
using System.Threading;

namespace Eposta
{
    internal class Program
    {
        static bool Çalışsın = true;
        static YanUygulama.Şube_ Şube;
        static Mutex Kilit;
        static Gönderici_ Gönderici = null;
        static Alıcı_ Alıcı = null;

        static void Main(string[] BaşlangıçParamaetreleri)
        {
#if DEBUG
            BaşlangıçParamaetreleri = new string[] { "5555" };
#endif

            if (BaşlangıçParamaetreleri == null || 
                BaşlangıçParamaetreleri.Length != 1 || 
                !int.TryParse(BaşlangıçParamaetreleri[0], out int ŞubeErişimNoktası) ||
                ŞubeErişimNoktası <= 0)
            {
                Console.WriteLine("BaşlangıçParamaetreleri hatalı");
                return;
            }

            GerekliDosyalar.Başlat();
            Şube = new YanUygulama.Şube_(ŞubeErişimNoktası, GeriBildirim_İşlemi_Uygulama);
            Kilit = new Mutex();

#if DEBUG
            GeriBildirim_İşlemi_Uygulama(true, Deneme_Başlat(), null);
            Çalışsın = false;
#endif

            while (Çalışsın) Thread.Sleep(1000);
            Alıcı?.Dispose();
            Şube.Dispose();
            Kilit.Dispose();
        }
        static void GeriBildirim_İşlemi_Uygulama(bool BağlantıKuruldu, byte[] Bilgi, string Açıklama)
        {
            if (!BağlantıKuruldu)
            {
#if !DEBUG
                Çalışsın = false;
#endif
                return;
            }

            string içerik = Bilgi.Yazıya();
            if (içerik.BoşMu() || !Kilit.WaitOne(5000)) return;

            Depo_ Depo_Komut = new Depo_(içerik);
            Depo_Komut.Yaz("Cevaplar", DateTime.Now);
            foreach (IDepo_Eleman komut in Depo_Komut["Komutlar"].Elemanları)
            {
                try
                {
                    if (komut.Adı.StartsWith("Gönder"))
                    {
                        //açık içerik
                        Gönderici.Gönder(
                            komut[0], //kime
                            komut[1], //bilgi
                            komut[2], //gizli
                            komut[3], //konu
                            komut[4], //html
                            komut[5], //normal
                            komut["Dosya Ekleri"].İçeriği);
                    }
                    else if (komut.Adı.StartsWith("Epostaları Yenile"))
                    {
                        Alıcı.EpostalarıYenile(
                            komut.Oku(null, "Gelen Kutusu", 0), //klasör adı
                            komut.Oku_Bit(null, false, 1), //Sadece Okunmamışlar
                            komut.Oku_TarihSaat(null, DateTime.MinValue, 2),
                            komut.Oku_TarihSaat(null, DateTime.MaxValue, 3),
                            komut.Oku_Bit(null, true, 4));//Ekleri al
                    }
                    else if (komut.Adı.StartsWith("Klasörleri Listele"))
                    {
                        Alıcı.KlasörleriListele(Depo_Komut["Cevaplar/" + komut.Adı], komut.Oku_Bit(null));
                    }
                    else if (komut.Adı.StartsWith("Okundu Olarak İşaretle"))
                    {
                        Alıcı.İşaretle(komut.İçeriği[0], komut.İçeriği[1].İşaretsizTamSayıya(), true);
                    }
                    else if (komut.Adı.StartsWith("Okunmadı Olarak İşaretle"))
                    {
                        Alıcı.İşaretle(komut.İçeriği[0], komut.İçeriği[1].İşaretsizTamSayıya(), false);
                    }
                    else if (komut.Adı.StartsWith("Kalıcı Olarak Sil"))
                    {
                        Alıcı.Sil(komut.İçeriği[0], komut.İçeriği[1].İşaretsizTamSayıya());
                    }
                    else if (komut.Adı.StartsWith("Taşı"))
                    {
                        foreach (IDepo_Eleman taşınacak in komut.Elemanları)
                        {
                            Depo_Komut["Cevaplar/" + komut.Adı, 1] = Alıcı.Taşı(taşınacak.Adı.İşaretsizTamSayıya(), taşınacak[0], taşınacak[1]);
                        }
                    }
                    else if (komut.Adı == "Başlat Gönderici")
                    {
                        if (Gönderici == null)
                        {
                            Gönderici = new Gönderici_(
                                komut[0],
                                komut[1],
                                komut[2],
                                komut[3],
                                komut.Oku_TamSayı(null, 465, 4),
                                komut.Oku_Bit(null, true, 5));
                        }
                    }
                    else if (komut.Adı == "Başlat Alıcı")
                    {
                        if (Alıcı == null)
                        {
                            Alıcı = new Alıcı_(
                                komut[0],
                                komut[1],
                                komut[2],
                                komut.Oku_TamSayı(null, 993, 3),
                                komut.Oku_Bit(null, true, 4),
                                komut[5],
                                komut.Oku_BaytDizisi(null, null, 6));
                        }
                    }
                    else throw new Exception("Geçersiz komut");

                    Depo_Komut.Yaz("Cevaplar/" + komut.Adı, "Başarılı");
                }
                catch (Exception ex)
                {
                    Depo_Komut["Cevaplar/" + komut.Adı].İçeriği = new string[] { "Hatalı", ex.ToString().TrimEnd('\r', '\n') };
                }
            }

            Kilit.ReleaseMutex();

#if DEBUG
            Depo_Komut.YazıyaDönüştür().Dosyaİçeriği_Yaz("epst.txt");
#else
            byte[] cevap_dizi = Depo_Komut.YazıyaDönüştür().BaytDizisine();
            if (cevap_dizi != null && cevap_dizi.Length > 0) Şube.Gönder(cevap_dizi);
#endif
        }

#if DEBUG
        static byte[] Deneme_Başlat()
        {
            Depo_ Depo_Komut = new Depo_();

            Depo_Komut.Yaz("Komutlar/Başlat Gönderici", "Adı", 0);
            Depo_Komut.Yaz("Komutlar/Başlat Gönderici", "Eposta Adresi", 1);
            Depo_Komut.Yaz("Komutlar/Başlat Gönderici", "Eposta Parola", 2);
            Depo_Komut.Yaz("Komutlar/Başlat Gönderici", "Smtp Sunucu Adresi", 3);
            Depo_Komut.Yaz("Komutlar/Başlat Gönderici", 465 /*Sunucu Erişim Noktası*/, 4);
            Depo_Komut.Yaz("Komutlar/Başlat Gönderici", true /*SSL*/, 5);

            Depo_Komut.Yaz("Komutlar/Başlat Alıcı", "Eposta Adresi", 0);
            Depo_Komut.Yaz("Komutlar/Başlat Alıcı", "Eposta Parola", 1);
            Depo_Komut.Yaz("Komutlar/Başlat Alıcı", "Imap Sunucu Adresi", 2);
            Depo_Komut.Yaz("Komutlar/Başlat Alıcı", 993 /*Sunucu Erişim Noktası*/, 3);
            Depo_Komut.Yaz("Komutlar/Başlat Alıcı", true /*SSL*/, 4);
            Depo_Komut.Yaz("Komutlar/Başlat Alıcı", "Çıktıları Kaydetme Klasörü", 5);
            Depo_Komut.Yaz("Komutlar/Başlat Alıcı", Rastgele.BaytDizisi() /*Çıktıları Kaydetme Şifresi*/, 6);

            Depo_Komut.Yaz("Komutlar/Gönder", "Kime", 0);
            Depo_Komut.Yaz("Komutlar/Gönder", "Bilgi", 1);
            Depo_Komut.Yaz("Komutlar/Gönder", "Gizli", 2);
            Depo_Komut.Yaz("Komutlar/Gönder", "Konu", 3);
            Depo_Komut.Yaz("Komutlar/Gönder", "Mesaj_html", 4);
            Depo_Komut.Yaz("Komutlar/Gönder", "Mesaj_text", 5);
            Depo_Komut["Komutlar/Gönder/Dosya Ekleri"].İçeriği = new string[] { "Ek 1 Dosya Yolu", "Ek 2 Dosya Yolu" };

            Depo_Komut.Yaz("Komutlar/Klasörleri Listele", false);
            Depo_Komut.Yaz("Komutlar/Klasörleri Listele 2", true); //detaylarla birlikte

            Depo_Komut["Komutlar/Kalıcı Olarak Sil"].İçeriği = new string[] { "Bulunduğu Klasörün Adı", "UID" };
            Depo_Komut["Komutlar/Okundu Olarak İşaretle"].İçeriği = new string[] { "Bulunduğu Klasörün Adı", "UID" };
            Depo_Komut["Komutlar/Okunmadı Olarak İşaretle"].İçeriği = new string[] { "Bulunduğu Klasörün Adı", "UID" };
            Depo_Komut["Komutlar/Taşı/UID1"].İçeriği = new string[] { "Bulunduğu Klasörün Adı", "Hedef Klasörün Adı" };

            Depo_Komut.Yaz("Komutlar/Epostaları Yenile", (string)null, 0); //Klasörün Adı, boş ise gelen kutusu
            Depo_Komut.Yaz("Komutlar/Epostaları Yenile", false, 1); //Sadece okunmamışları al
            Depo_Komut.Yaz("Komutlar/Epostaları Yenile", DateTime.Now.AddDays(-7), 2); //Aralık Başlangıç
            Depo_Komut.Yaz("Komutlar/Epostaları Yenile", DateTime.Now, 3); //Aralık Bitiş
            Depo_Komut.Yaz("Komutlar/Epostaları Yenile", true, 4); //Dosya eklerini al

            return Depo_Komut.YazıyaDönüştür().BaytDizisine();
        }
#endif
    }
}


