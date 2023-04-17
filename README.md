# Eposta
Genel amaçlı eposta gönderme (smtp), alma (imap) uygulaması ArgeMup@yandex.com

## Thanks
Many thanks to the team of the [MailKit project](https://github.com/jstedfast/MailKit) for their great work. Eposta would not be possible without your work!


    Adım 1 - Eposta.exe çalıştırıldığında alttaki dosyayı üretir (eskinin üzerine yazar) ve dosyanın güncellenmesini bekler.
        Komut.mup
            Kimlik Kontrolü / Rsa-2048 Açık anahtar - xml - Kısaltma:RsaAçıkAnahtar

    Adım 2 - Uygulama alttaki örnek işlem ile gerekli alanları doldurabilir.
        void Başlat(string DosyaYolu /*Komut.mup*/)
        {
            Depo_ Depo_Komut = new Depo_(System.IO.File.ReadAllText(DosyaYolu));
            string RsaAçıkAnahtar = Depo_Komut.Oku("Kimlik Kontrolü");

            ArgeMup.HazirKod.DahaCokKarmasiklastirma_Asimetrik_ Dçk_Rsa = new ArgeMup.HazirKod.DahaCokKarmasiklastirma_Asimetrik_(RsaAçıkAnahtar);
            byte[] ParolaAes = Dçk_Rsa.ParolaÜret();
            byte[] ParolaRsa = Dçk_Rsa.Karıştır(ParolaAes);
            Depo_Komut.Yaz("Kimlik Kontrolü/ParolaRsa", ParolaRsa.Taban64e());

            Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", "Adı".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 0);
            Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", "Eposta Adresi".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 1);
            Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", "Eposta Parola".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 2);
            Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", "Smtp Sunucu Adresi".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 3);
            Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", 465 /*Sunucu Erişim Noktası*/, 4);
            Depo_Komut.Yaz("Kimlik Kontrolü/Gönderici", true /*SSL*/, 5);

            Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", "Eposta Adresi".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 0);
            Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", "Eposta Parola".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 1);
            Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", "Imap Sunucu Adresi".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 2);
            Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", 993 /*Sunucu Erişim Noktası*/, 3);
            Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", true /*SSL*/, 4);
            Depo_Komut.Yaz("Kimlik Kontrolü/Alıcı", "Epostaları İndirme Klasörü".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 5);

            Depo_Komut.Yaz("Komutlar/Gönder", false, 0); //Tüm İçerik Açık
            Depo_Komut.Yaz("Komutlar/Gönder", "Kime", 1);
            Depo_Komut.Yaz("Komutlar/Gönder", "Bilgi", 2);
            Depo_Komut.Yaz("Komutlar/Gönder", "Gizli", 3);
            Depo_Komut.Yaz("Komutlar/Gönder", "Konu", 4);
            Depo_Komut.Yaz("Komutlar/Gönder", "Mesaj_html", 5);
            Depo_Komut.Yaz("Komutlar/Gönder", "Mesaj_text", 6);
            Depo_Komut["Komutlar/Gönder/Dosya Ekleri"].İçeriği = new string[] { "Ek 1 Dosya Yolu", "Ek 2 Dosya Yolu" };

            Depo_Komut.Yaz("Komutlar/Gönder", true, 0); //Tüm İçerik Şifreli
            Depo_Komut.Yaz("Komutlar/Gönder", "epst1@com;epst2@com".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 1); //Kime
            Depo_Komut.Yaz("Komutlar/Gönder", "Bilgi".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 2);
            Depo_Komut.Yaz("Komutlar/Gönder", "Gizli".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 3);
            Depo_Komut.Yaz("Komutlar/Gönder", "Konu".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 4);
            Depo_Komut.Yaz("Komutlar/Gönder", "Mesaj_html".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 5);
            Depo_Komut.Yaz("Komutlar/Gönder", "Mesaj_text".BaytDizisine().Karıştır(ParolaAes).Taban64e(), 6);
            Depo_Komut["Komutlar/Gönder/Dosya Ekleri"].İçeriği = new string[] { @"Ek 1 Dosya Yolu".BaytDizisine().Karıştır(ParolaAes).Taban64e(), @"Ek 2 Dosya Yolu".BaytDizisine().Karıştır(ParolaAes).Taban64e() };

            Depo_Komut["Komutlar/Kalıcı Olarak Sil"].İçeriği = new string[] { "UID1", "UID2" };
            Depo_Komut["Komutlar/Okundu Olarak İşaretle"].İçeriği = new string[] { "UID1", "UID2" };
            Depo_Komut["Komutlar/Okunmadı Olarak İşaretle"].İçeriği = new string[] { "UID1", "UID2" };

            Depo_Komut.Yaz("Komutlar/Listele", false, 0); //Üretilen dosyaları ParolaAes ile şifrele
            Depo_Komut.Yaz("Komutlar/Listele", false, 1); //Sadece okunmamışları al
            Depo_Komut.Yaz("Komutlar/Listele", DateTime.Now.AddDays(-7), 2); //Aralık Başlangıç
            Depo_Komut.Yaz("Komutlar/Listele", DateTime.Now, 3); //Aralık Bitiş
            Depo_Komut.Yaz("Komutlar/Listele", true, 4); //Dosya eklerini al

            System.IO.File.WriteAllText(DosyaYolu, Depo_Komut.YazıyaDönüştür());
        }

    Komut.mup
        Kimlik Kontrolü / Rsa-2048 Açık anahtar - xml - Kısaltma:RsaAçıkAnahtar / KendisiİçinBenzersizAnahtar
            Gönderici / <Adı - ParolaAes> / <EpostaAdresi - ParolaAes> / <Parola - ParolaAes> / <SmtpSunucuAdresi - ParolaAes> / SunucuErişimNoktası / SSL
            Alıcı / <EpostaAdresi - ParolaAes> / <Parola - ParolaAes> / <ImapSunucuAdresi - ParolaAes> / SunucuErişimNoktası / SSL / <Epostaları İndirme Klasörü - ParolaAes>
            ParolaRsa / Uygulamanın ürettiği ParolaAes şifresinin RsaAçıkAnahtar ile karıştırılmış hali - base64
        Komutlar / <Son Çalıştırılma Tarihi>
            Gönder / <bit - True:Tüm İçerik Şifreli> / <Kime ; ile ayrılmış - ParolaAes> / <Bilgi ; - ParolaAes> / <Gizli ; - ParolaAes> / <Konu - ParolaAes> / <Mesaj_html - ParolaAes> / <Mesaj_text - ParolaAes>
                Dosya Ekleri / <Dosya Eki 1 Yolu - ParolaAes> / <Dosya Eki 2 ...>
            Kalıcı Olarak Sil / UID1 / UID2 ... (birden fazla epostayı aynı seferde işlemek için)
            Okundu Olarak İşaretle / UID ...
            Okunmadı Olarak İşaretle / UID ...
            Listele (Sadece Gelen Kutusu) / <bit - True:Üretilen dosyaları ParolaAes ile şifrele> / <bit - sadece okunmamışları dahil et> / <tarih saat - ilk tarih> / <tarih saat - son tarih> / <bit - Dosya eklerini dahil et>
        Hatalar
            <Kaynak> / <Açıklama>

    Epostaları İndirme Klasörü / _UID / Depo.mup (Komutlar çalıştırıldıktan sonra güncel duruma göre üretilir)
        <UID> / Tarih / Kimden / Bayraklar / Konu
            Mesaj / html / text
            Ekler
                <Dosyanın Diskteki Adı> / <Dosyanın Gerçek Adı> / <bit - Ek dosyası mı> / Tipi / CID
                (Dosyanın gerçek yolu -> Epostaları İndirme Klasörü\_UID\<Dosyanın Diskteki Adı>)