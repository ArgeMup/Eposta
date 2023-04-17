using ArgeMup.HazirKod;
using ArgeMup.HazirKod.Ekİşlemler;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Eposta
{
    internal class Program
    {
        static bool Çalışsın = true;
        static DateTime KomutDosyası_DeğiştirilmeTarihi;
        static string KomutDosyası_Yolu = Kendi.Klasörü + @"\Komut.mup";
        static Depo_ Depo_Komut;
        static DahaCokKarmasiklastirma_ Dçk_Aes;
        static DahaCokKarmasiklastirma_Asimetrik_ Dçk_Rsa;
        static Gönderici_ Gönderici = null;
        static Alıcı_ Alıcı = null;
        static byte[] ParolaAes = null;
        static string KendisiİçinBenzersizAnahtar;

        static void Main(string[] args)
        {
            W32_Konsol.KapatıldığındaHaberVer(UygulamaKapatıldı);
            GerekliDosyalar.Başlat();

            Dçk_Aes = new DahaCokKarmasiklastirma_();
            Dçk_Rsa = new DahaCokKarmasiklastirma_Asimetrik_(2048);
            KendisiİçinBenzersizAnahtar = Dçk_Rsa.ParolaÜret(5).Taban64e();

            Depo_Komut = new Depo_();
            Depo_Komut["Kimlik Kontrolü"].İçeriği = new string[] { Dçk_Rsa.AçıkAnahtar, KendisiİçinBenzersizAnahtar };

            #if DEBUG
                Deneme_Başlat(KomutDosyası_Yolu);    

                Depo_Komut_Çalıştır();
                File.WriteAllText(KomutDosyası_Yolu, Depo_Komut.YazıyaDönüştür());
            #else
                while (Çalışsın)
                {
                    File.WriteAllText(KomutDosyası_Yolu, Depo_Komut.YazıyaDönüştür());
                    KomutDosyası_DeğiştirilmeTarihi = DateTime.Now;

                    Depo_Kommut_GüncellenmesiniBekle();
                    if (!Çalışsın) break;

                    Depo_Komut_Çalıştır();
                }
            #endif

            #region Çıkış
            Dçk_Aes?.Dispose();
            Dçk_Rsa?.Dispose();
            Alıcı?.Dispose();
            #endregion

            void Başlat_Gönderici()
            {
                if (Gönderici == null)
                {
                    if (ParolaAes == null) ParolaAes = Dçk_Rsa.Düzelt(Depo_Komut["Kimlik Kontrolü/ParolaRsa", 0].Taban64ten());

                    IDepo_Eleman a = Depo_Komut["Kimlik Kontrolü/Gönderici"];
                    Gönderici = new Gönderici_(
                        Çöz(a[0]),
                        Çöz(a[1]),
                        Çöz(a[2]),
                        Çöz(a[3]),
                        a.Oku_TamSayı(null, 465, 4), a.Oku_Bit(null, true, 5));
                }
            }
            void Başlat_Alıcı()
            {
                if (Alıcı == null)
                {
                    if (ParolaAes == null) ParolaAes = Dçk_Rsa.Düzelt(Depo_Komut["Kimlik Kontrolü/ParolaRsa", 0].Taban64ten());

                    IDepo_Eleman a = Depo_Komut["Kimlik Kontrolü/Alıcı"];
                    Alıcı = new Alıcı_(
                        Çöz(a[0]),
                        Çöz(a[1]),
                        Çöz(a[2]),
                        a.Oku_TamSayı(null, 993, 3), a.Oku_Bit(null, true, 4),
                        Çöz(a[5]));
                }
            }
            void Depo_Kommut_GüncellenmesiniBekle()
            {
                while (Çalışsın)
                {
                    if (File.Exists(KomutDosyası_Yolu))
                    {
                        if (File.GetLastWriteTime(KomutDosyası_Yolu) > KomutDosyası_DeğiştirilmeTarihi)
                        {
                            return;
                        }
                    }

                    Thread.Sleep(1000);
                }
            }
            void Depo_Komut_Çalıştır()
            {
                Depo_Komut = new Depo_(File.ReadAllText(KomutDosyası_Yolu));
                if (Depo_Komut["Kimlik Kontrolü", 1] != KendisiİçinBenzersizAnahtar)
                {
                    Çalışsın = false;
                    return;
                }
                Depo_Komut.Yaz("Komutlar", DateTime.Now);

                foreach (IDepo_Eleman komut in Depo_Komut["Komutlar"].Elemanları)
                {
                    try
                    {
                        switch (komut.Adı)
                        {
                            case "Gönder":
                                Başlat_Gönderici();
                                string snç;

                                if (komut.Oku_Bit(null, default, 0))
                                {
                                    //şifreli içerik
                                    string[] ekler = new string[komut["Dosya Ekleri"].İçeriği.Length];
                                    for (int i = 0; i < ekler.Length; i++)
                                    {
                                        ekler[i] = Çöz(komut["Dosya Ekleri", i]);
                                    }

                                    snç = Gönderici.Gönder(
                                        Çöz(komut[1]),
                                        Çöz(komut[2]),
                                        Çöz(komut[3]),
                                        Çöz(komut[4]),
                                        Çöz(komut[5]),
                                        Çöz(komut[5]),
                                        ekler);
                                }
                                else
                                {
                                    //açık içerik
                                    snç = Gönderici.Gönder(
                                        komut[1],
                                        komut[2],
                                        komut[3],
                                        komut[4],
                                        komut[5],
                                        komut[5],
                                        komut["Dosya Ekleri"].İçeriği);
                                }

                                if (snç.DoluMu()) throw new Exception(snç);
                                break;

                            case "Kalıcı Olarak Sil":
                            case "Okundu Olarak İşaretle":
                            case "Okunmadı Olarak İşaretle":
                                Başlat_Alıcı();

                                foreach (string uid in komut.İçeriği)
                                {
                                    uint uuid = uid.İşaretsizTamSayıya();
                                    switch (komut.Adı)
                                    {
                                        case "Kalıcı Olarak Sil": Alıcı.Sil(uuid); break;
                                        case "Okundu Olarak İşaretle": Alıcı.İşaretle(uuid, true); break;
                                        case "Okunmadı Olarak İşaretle": Alıcı.İşaretle(uuid, false); break;
                                    }
                                }
                                break;

                            case "Listele":
                                Başlat_Alıcı();

                                Alıcı.Listele(komut.Oku_Bit(null, false, 0) ? ParolaAes : null,
                                    komut.Oku_Bit(null, false, 1),
                                    komut.Oku_TarihSaat(null, DateTime.MinValue, 2),
                                    komut.Oku_TarihSaat(null, DateTime.MaxValue, 3),
                                    komut.Oku_Bit(null, true, 4));
                                break;
                        }

                        komut.Sil(null);
                    }
                    catch (Exception ex)
                    {
                        Depo_Komut["Hatalar/" + Depo_Komut["Hatalar"].Elemanları.Length].İçeriği = new string[] { komut.Adı, ex.Message.TrimEnd('\r', '\n') };
                    }
                }
            }
            string Çöz(string Girdi)
            {
                if (Girdi.BoşMu(true)) return Girdi;

                return Girdi.Taban64ten().Düzelt(ParolaAes).Yazıya();
            }
        }

        static void UygulamaKapatıldı(W32_Konsol.CtrlType Kaynağı)
        {
            Çalışsın = false;
        }
        #if DEBUG
            static void Deneme_Başlat(string DosyaYolu /*Komut.mup*/)
            {
                Depo_ Depo_Komut = new Depo_(System.IO.File.Exists(DosyaYolu) ? System.IO.File.ReadAllText(DosyaYolu) : null);
                string RsaAçıkAnahtar = Depo_Komut.Oku("Kimlik Kontrolü");

                ArgeMup.HazirKod.DahaCokKarmasiklastirma_Asimetrik_ Dçk_Rsa = new ArgeMup.HazirKod.DahaCokKarmasiklastirma_Asimetrik_(RsaAçıkAnahtar);
                byte[] ParolaAes = Dçk_Rsa.ParolaÜret();
                byte[] ParolaRsa = Dçk_Rsa.Karıştır(ParolaAes);
                Depo_Komut.Yaz("Kimlik Kontrolü/ParolaRsa", ParolaRsa.Taban64e());

                Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", _Karıştır_("Adı"), 0);
                Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", _Karıştır_("Eposta Adresi"), 1);
                Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", _Karıştır_("Eposta Parola"), 2);
                Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", _Karıştır_("Smtp Sunucu Adresi"), 3);
                Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", 465 /*Sunucu Erişim Noktası*/, 4);
                Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", true /*SSL*/, 5);

                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", _Karıştır_("Eposta Adresi"), 0);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", _Karıştır_("Eposta Parola"), 1);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", _Karıştır_("Imap Sunucu Adresi"), 2);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", 993 /*Sunucu Erişim Noktası*/, 3);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", true /*SSL*/, 4);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", _Karıştır_("Epostaları İndirme Klasörü"), 5);

                Depo_Komut.Yaz("Komutlar/Gönder", false, 0); //Tüm İçerik Açık
                Depo_Komut.Yaz("Komutlar/Gönder", "Kime", 1);
                Depo_Komut.Yaz("Komutlar/Gönder", "Bilgi", 2);
                Depo_Komut.Yaz("Komutlar/Gönder", "Gizli", 3);
                Depo_Komut.Yaz("Komutlar/Gönder", "Konu", 4);
                Depo_Komut.Yaz("Komutlar/Gönder", "Mesaj_html", 5);
                Depo_Komut.Yaz("Komutlar/Gönder", "Mesaj_text", 6);
                Depo_Komut["Komutlar/Gönder/Dosya Ekleri"].İçeriği = new string[] { "Ek 1 Dosya Yolu", "Ek 2 Dosya Yolu" };

                Depo_Komut.Yaz("Komutlar/Gönder", true, 0); //Tüm İçerik Şifreli
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("epst1@com;epst2@com"), 1); //Kime
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("Bilgi"), 2);
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("Gizli"), 3);
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("Konu"), 4);
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("Mesaj_html"), 5);
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("Mesaj_text"), 6);
                Depo_Komut["Komutlar/Gönder/Dosya Ekleri"].İçeriği = new string[] { _Karıştır_(@"Ek 1 Dosya Yolu"), _Karıştır_(@"Ek 2 Dosya Yolu") };

                Depo_Komut["Komutlar/Kalıcı Olarak Sil"].İçeriği = new string[] { "UID1", "UID2" };
                Depo_Komut["Komutlar/Okundu Olarak İşaretle"].İçeriği = new string[] { "UID1", "UID2" };
                Depo_Komut["Komutlar/Okunmadı Olarak İşaretle"].İçeriği = new string[] { "UID1", "UID2" };

                Depo_Komut.Yaz("Komutlar/Listele", false, 0); //Üretilen dosyaları ParolaAes ile şifrele
                Depo_Komut.Yaz("Komutlar/Listele", false, 1); //Sadece okunmamışları al
                Depo_Komut.Yaz("Komutlar/Listele", DateTime.Now.AddDays(-7), 2); //Aralık Başlangıç
                Depo_Komut.Yaz("Komutlar/Listele", DateTime.Now, 3); //Aralık Bitiş
                Depo_Komut.Yaz("Komutlar/Listele", true, 4); //Dosya eklerini al

                System.IO.File.WriteAllText(DosyaYolu, Depo_Komut.YazıyaDönüştür());

                string _Karıştır_(string Girdi)
                {
                    if (Girdi.BoşMu(true)) return Girdi;

                    return Girdi.BaytDizisine().Karıştır(ParolaAes).Taban64e();
                }
            }
        #endif
    }
}