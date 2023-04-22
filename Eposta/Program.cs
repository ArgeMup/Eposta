using ArgeMup.HazirKod;
using ArgeMup.HazirKod.Ekİşlemler;
using System;
using System.IO;
using System.Threading;

namespace Eposta
{
    internal class Program
    {
        static bool Çalışsın = true, Gönderici_TümİçerikŞifreli = true;
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
                File.WriteAllText(KomutDosyası_Yolu, Depo_Komut.YazıyaDönüştür());
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
                    if (ParolaAes == null)
                    {
                        ParolaAes = Dçk_Rsa.Düzelt(Depo_Komut["Kimlik Kontrolü/ParolaRsa", 0].Taban64ten());
                        Depo_Komut.Sil("Kimlik Kontrolü/ParolaRsa");
                    }

                    IDepo_Eleman a = Depo_Komut["Kimlik Kontrolü/Gönderici"];
                    Gönderici = new Gönderici_(
                        Çöz(a[0]),
                        Çöz(a[1]),
                        Çöz(a[2]),
                        Çöz(a[3]),
                        a.Oku_TamSayı(null, 465, 4),
                        a.Oku_Bit(null, true, 5));

                    Gönderici_TümİçerikŞifreli = a.Oku_Bit(null, true, 6);

                    Depo_Komut.Sil("Kimlik Kontrolü/Gönderici");
                }
            }
            void Başlat_Alıcı()
            {
                if (Alıcı == null)
                {
                    if (ParolaAes == null)
                    {
                        ParolaAes = Dçk_Rsa.Düzelt(Depo_Komut["Kimlik Kontrolü/ParolaRsa", 0].Taban64ten());
                        Depo_Komut.Sil("Kimlik Kontrolü/ParolaRsa");
                    }

                    IDepo_Eleman a = Depo_Komut["Kimlik Kontrolü/Alıcı"];
                    Alıcı = new Alıcı_(
                        Çöz(a[0]),
                        Çöz(a[1]),
                        Çöz(a[2]),
                        a.Oku_TamSayı(null, 993, 3), 
                        a.Oku_Bit(null, true, 4),
                        Çöz(a[5]),
                        a.Oku_Bit(null, false, 6) ? ParolaAes : null );

                    Depo_Komut.Sil("Kimlik Kontrolü/Alıcı");
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
                Depo_Komut.Yaz("Cevaplar", DateTime.Now);

                foreach (IDepo_Eleman komut in Depo_Komut["Komutlar"].Elemanları)
                {
                    try
                    {
                        if (komut.Adı.StartsWith("Gönder"))
                        {
                            Başlat_Gönderici();

                            if (Gönderici_TümİçerikŞifreli)
                            {
                                //şifreli içerik
                                string[] ekler = new string[komut["Dosya Ekleri"].İçeriği.Length];
                                for (int i = 0; i < ekler.Length; i++)
                                {
                                    ekler[i] = Çöz(komut["Dosya Ekleri", i]);
                                }

                                Gönderici.Gönder(
                                    Çöz(komut[0]),
                                    Çöz(komut[1]),
                                    Çöz(komut[2]),
                                    Çöz(komut[3]),
                                    Çöz(komut[4]),
                                    Çöz(komut[5]),
                                    ekler);
                            }
                            else
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
                        }
                        else if (komut.Adı.StartsWith("Epostaları Yenile"))
                        {
                            Başlat_Alıcı();

                            Alıcı.EpostalarıYenile(
                                komut.Oku(null, null, 0), //klasör adı
                                komut.Oku_Bit(null, false, 1), //Sadece Okunmamışlar
                                komut.Oku_TarihSaat(null, DateTime.MinValue, 2),
                                komut.Oku_TarihSaat(null, DateTime.MaxValue, 3),
                                komut.Oku_Bit(null, true, 4));//Ekleri al
                        }
                        else if (komut.Adı.StartsWith("Klasörleri Listele"))
                        {
                            Başlat_Alıcı();

                            Alıcı.KlasörleriListele(Depo_Komut["Cevaplar/" + komut.Adı], komut.Oku_Bit(null));
                        }
                        else if (komut.Adı.StartsWith("Okundu Olarak İşaretle"))
                        {
                            Başlat_Alıcı();

                            Alıcı.İşaretle(komut.İçeriği[0], komut.İçeriği[1].İşaretsizTamSayıya(), true);
                        }
                        else if (komut.Adı.StartsWith("Okunmadı Olarak İşaretle"))
                        {
                            Başlat_Alıcı();

                            Alıcı.İşaretle(komut.İçeriği[0], komut.İçeriği[1].İşaretsizTamSayıya(), false);
                        }
                        else if (komut.Adı.StartsWith("Kalıcı Olarak Sil"))
                        {
                            Başlat_Alıcı();

                            Alıcı.Sil(komut.İçeriği[0], komut.İçeriği[1].İşaretsizTamSayıya());
                        }
                        else if (komut.Adı.StartsWith("Taşı"))
                        {
                            Başlat_Alıcı();

                            foreach (IDepo_Eleman taşınacak in komut.Elemanları)
                            {
                                Depo_Komut["Cevaplar/" + komut.Adı, 1] = Alıcı.Taşı(taşınacak.Adı.İşaretsizTamSayıya(), taşınacak[0], taşınacak[1]);
                            }
                        }
                        else throw new Exception("Geçersiz komut");

                        Depo_Komut.Yaz("Cevaplar/" + komut.Adı, "Başarılı");
                    }
                    catch (Exception ex)
                    {
                        Depo_Komut["Cevaplar/" + komut.Adı].İçeriği = new string[] { "Hatalı", ex.Message.TrimEnd('\r', '\n') };
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
                Depo_ Depo_Komut = new Depo_(File.ReadAllText(DosyaYolu));
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
                Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", true /*Tüm İçerik Şifreli*/, 6);

                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", _Karıştır_("Eposta Adresi"), 0);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", _Karıştır_("Eposta Parola"), 1);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", _Karıştır_("Imap Sunucu Adresi"), 2);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", 993 /*Sunucu Erişim Noktası*/, 3);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", true /*SSL*/, 4);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", _Karıştır_("Çıktıları Kaydetme Klasörü"), 5);
                Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", false /*Çıktıları ParolaAes ile şifrele*/, 6);

                //Tüm İçerik Açık - Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", false /*Tüm İçerik Şifreli*/, 6);
                Depo_Komut.Yaz("Komutlar/Gönder", "Kime", 0);
                Depo_Komut.Yaz("Komutlar/Gönder", "Bilgi", 1);
                Depo_Komut.Yaz("Komutlar/Gönder", "Gizli", 2);
                Depo_Komut.Yaz("Komutlar/Gönder", "Konu", 3);
                Depo_Komut.Yaz("Komutlar/Gönder", "Mesaj_html", 4);
                Depo_Komut.Yaz("Komutlar/Gönder", "Mesaj_text", 5);
                Depo_Komut["Komutlar/Gönder/Dosya Ekleri"].İçeriği = new string[] { "Ek 1 Dosya Yolu", "Ek 2 Dosya Yolu" };

                //Tüm İçerik Şifreli - Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", true /*Tüm İçerik Şifreli*/, 6);
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("epst1@com;epst2@com"), 0);
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("Bilgi"), 1);
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("Gizli"), 2);
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("Konu"), 3);
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("Mesaj_html"), 4);
                Depo_Komut.Yaz("Komutlar/Gönder", _Karıştır_("Mesaj_text"), 5);
                Depo_Komut["Komutlar/Gönder/Dosya Ekleri"].İçeriği = new string[] { _Karıştır_(@"Ek 1 Dosya Yolu"), _Karıştır_(@"Ek 2 Dosya Yolu") };

                Depo_Komut.Yaz("Komutlar/Klasörleri Listele", false);
                Depo_Komut.Yaz("Komutlar/Klasörleri Listele 2", true); //detaylarla birlikte

                Depo_Komut["Komutlar/Kalıcı Olarak Sil"].İçeriği = new string[] { "Bulunduğu Klasörün Adı", "UID" };
                Depo_Komut["Komutlar/Okundu Olarak İşaretle"].İçeriği = new string[] { "Bulunduğu Klasörün Adı", "UID" };
                Depo_Komut["Komutlar/Okunmadı Olarak İşaretle"].İçeriği = new string[] { "Bulunduğu Klasörün Adı", "UID" };
                Depo_Komut["Komutlar/Taşı/UID1"].İçeriği = new string[] { "Bulunduğu Klasörün Adı", "Hedef Klasörün Adı" };

                Depo_Komut.Yaz("Komutlar/Epostaları Yenile", null, 0); //Klasörün Adı, boş ise gelen kutusu
                Depo_Komut.Yaz("Komutlar/Epostaları Yenile", false, 1); //Sadece okunmamışları al
                Depo_Komut.Yaz("Komutlar/Epostaları Yenile", DateTime.Now.AddDays(-7), 2); //Aralık Başlangıç
                Depo_Komut.Yaz("Komutlar/Epostaları Yenile", DateTime.Now, 3); //Aralık Bitiş
                Depo_Komut.Yaz("Komutlar/Epostaları Yenile", true, 4); //Dosya eklerini al

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