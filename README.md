# Eposta
Genel amaçlı eposta gönderme (smtp), alma (imap) uygulaması ArgeMup@yandex.com

## Thanks
Many thanks to the team of the [MailKit project](https://github.com/jstedfast/MailKit) for their great work. Eposta would not be possible without your work!

    Komut.mup
        Kimlik Kontrolü
            Gönderici / <Adı> / <EpostaAdresi> / <Parola> / <SmtpSunucuAdresi> / <SunucuErişimNoktası> / <SSL>
            Alıcı / <EpostaAdresi> / <Parola> / <ImapSunucuAdresi> / <SunucuErişimNoktası> / <SSL> / <Çıktıları Kaydetme Klasörü> / <Çıktıları Kaydetme Parolası Aes> 
        Komutlar
            Gönder / <Kime ; ile ayrılmış> / <Bilgi ;> / <Gizli ;> / <Konu> / <Mesaj_html> / <Mesaj_text>
                Dosya Ekleri / <Dosya Eki 1 Yolu> / <Dosya Eki 2 ...>
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
            Diğer / <Sihirli Kelime Parolası Çözülmüş İçerik - Eposta.Parola.Yazı>

    Çıktıları Kaydetme Klasörü / Epostaları Yenile / Klasör Adı / Depo.mup (Komutlar çalıştırıldıktan sonra güncel duruma göre üretilir)
        Tipi / Epostaları Yenile / <Klasör Adı> / <Son Çalıştırılma Tarihi>
        Liste
            <UID> / Tarih / Bayraklar / Konu / Kimden eposta / Kimden Adı
                Mesaj / html / text
                Ekler
                    <Dosyanın Diskteki Yolu> / <Dosyanın Gerçek Adı> / <bit - Ek dosyası mı> / Tipi / CID

    Çıktıları Kaydetme Klasörü / Epostaları Yenile / Klasör Adı / _UID
        ArGeMuP.png