# Eposta
Genel amaçlı eposta gönderme (smtp), alma (imap) uygulaması ArgeMup@yandex.com

## Thanks
Many thanks to the team of the [MailKit project](https://github.com/jstedfast/MailKit) for their great work. Eposta would not be possible without your work!

    Adım 1 - Eposta.exe çalıştırıldığında alttaki dosyayı üretir (eskinin üzerine yazar) ve dosyanın güncellenmesini bekler.
        Komut.mup
            Kimlik Kontrolü / Rsa-2048 Açık anahtar - xml - Kısaltma:RsaAçıkAnahtar

    Adım 2 - Uygulama alttaki örnek işlem ile gerekli alanları doldurabilir.
        void Deneme_Başlat(string DosyaYolu /*Komut.mup*/)
        {
            Depo_ Depo_Komut = new Depo_(System.IO.File.ReadAllText(DosyaYolu));
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

    Komut.mup
        Kimlik Kontrolü / Rsa-2048 Açık anahtar - xml - Kısaltma:RsaAçıkAnahtar / KendisiİçinBenzersizAnahtar
            Gönderici / <Adı - ParolaAes> / <EpostaAdresi - ParolaAes> / <Parola - ParolaAes> / <SmtpSunucuAdresi - ParolaAes> / SunucuErişimNoktası / SSL / <bit - Tüm İçerik Şifreli - ParolaAes>
            Alıcı / <EpostaAdresi - ParolaAes> / <Parola - ParolaAes> / <ImapSunucuAdresi - ParolaAes> / SunucuErişimNoktası / SSL / <Çıktıları Kaydetme Klasörü - ParolaAes> / <bit - Çıktıları ParolaAes ile şifrele> 
            ParolaRsa / Uygulamanın ürettiği ParolaAes şifresinin RsaAçıkAnahtar ile karıştırılmış hali - base64
        Komutlar
            Gönder / <Kime ; ile ayrılmış - ParolaAes> / <Bilgi ; - ParolaAes> / <Gizli ; - ParolaAes> / <Konu - ParolaAes> / <Mesaj_html - ParolaAes> / <Mesaj_text - ParolaAes>
                Dosya Ekleri / <Dosya Eki 1 Yolu - ParolaAes> / <Dosya Eki 2 ...>
            Kalıcı Olarak Sil / <Klasör Adı - boş ise gelen kutusu> / UID
            Okundu Olarak İşaretle / <Klasör Adı> / UID
            Okunmadı Olarak İşaretle / <Klasör Adı> / UID
            Taşı
                UID1 / <Bulunduğu Klasörün Adı> / <Hedef Klasörün Adı>
                UID2 / ...
            Klasörleri Listele / <bit - detayları al>
            Epostaları Yenile / <Klasör Adı> / <bit - sadece okunmamışları dahil et> / <tarih saat - ilk tarih> / <tarih saat - son tarih> / <bit - Dosya eklerini dahil et>
            (Aynı komutu birden fazla kez çağırabilmek için sonuna birşeyler eklenmelidir)
            Kalıcı Olarak Sil 1 / <Klasör Adı> / UID1
            Kalıcı Olarak Sil ArGeMuP / <Klasör Adı> / UID2 gibi
        Cevaplar / <Son Çalıştırılma Tarihi>    
            Gönder / Başarılı veya Hatalı / Hata Açıklaması
            Kalıcı Olarak Sil / Başarılı veya Hatalı / Hata Açıklaması
            Okundu Olarak İşaretle / Başarılı veya Hatalı / Hata Açıklaması
            Okunmadı Olarak İşaretle / Başarılı veya Hatalı / Hata Açıklaması
            Taşı / Başarılı veya Hatalı / Yeni UID veya Hata Açıklaması
            Klasörleri Listele / Başarılı veya Hatalı / Hata Açıklaması
                Klasör1 / <Okunmamış eposta sayısı> / <Toplam eposta sayısı> / <Yeni gelen eposta sayısı> / <Klasör boyutu - bayt>
                Klasör2 ...
            Epostaları Yenile / Başarılı veya Hatalı / Hata Açıklaması

    Çıktıları Kaydetme Klasörü / Epostaları Yenile / Klasör Adı / Depo.mup (Komutlar çalıştırıldıktan sonra güncel duruma göre üretilir)
        Tipi / Epostaları Yenile / <Klasör Adı> / <Son Çalıştırılma Tarihi>
        Liste
            <UID> / Tarih / Bayraklar / Konu / Kimden eposta / Kimden Adı
                Mesaj / html / text
                Ekler
                    <Dosyanın Diskteki Yolu> / <Dosyanın Gerçek Adı> / <bit - Ek dosyası mı> / Tipi / CID

    Çıktıları Kaydetme Klasörü / Epostaları Yenile / Klasör Adı / _UID
        ArGeMuP.png