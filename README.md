# Eposta
Genel amaçlı eposta gönderme (smtp), alma (imap) uygulaması ArgeMup@yandex.com

## Thanks
Many thanks to the team of the [MailKit project](https://github.com/jstedfast/MailKit) for their great work. Eposta would not be possible without your work!

    Adım 1 - Eposta.exe çalıştırıldığında alttaki dosyayı üretir (eskinin üzerine yazar) ve dosyanın güncellenmesini bekler.
        Komut.mup
            Sunucu Kimlik Bilgileri / Rsa-4096 Açık anahtar - xml

    Adım 2 - Uygulama <Saklı İçerik> işlemlerinde kullanılmak üzere rasgele bir Aes-Cbc parolası üretir. Kısaltma:ParolaAes

    Adım 3 - Uygulama Eposta.exe nin sağladığı Rsa-4096 açık anahtarını kullanarak ParolaAes içeriğini şifreler. Kısaltma:ParolaRsa

    Adım 4 - Uygulama Komut.mup dosyasını açıp kimlik işlemleri için gerekli alanları doldurur ve komutları da ekleyerek sistemi çalıştırır.
        Komut.mup
            Sunucu Kimlik Bilgileri / Rsa-4096 Açık anahtar - xml
                Gönderici / <Saklı İçerik - ParolaAes ile şifrelenmiş - base64> ( Adı / EpostaAdresi / Parola / SmtpSunucuAdresi / SunucuErişimNoktası / SSL )
                    Gönder İçeriği Şifreli / <boş veya True> (Komutlar / Gönder içeriği ParolaAes ile şifrelenerek korunabilir.)
                Alıcı / <Saklı İçerik - ParolaAes ile şifrelenmiş - base64> ( EpostaAdresi / Parola / ImapSunucuAdresi / SunucuErişimNoktası / SSL / Epostaları İndirme Klasörü )
                    Eposta İçeriği Şifreli / <boş veya True> (Epostaları İndirme Klasörü içerisinde dosya oluşturulmadan önce ParolaAes ile şifrelenir.)
                Saklı İçerik Parolası / <ParolaRsa>
            Komutlar
                Gönder / <Açık veya Saklı İçerik> ( <Kime ; ile ayrılmı> / <Bilgi ;> / <Gizli ;> / <Konu> / Mesaj_html / Mesaj_text / Dosya Eki 1 / Dosya Eki 2 ... )
                Kalıcı Olarak Sil / Hatırlatıcı1 / Hatırlatıcı2 ... (birden fazla epostayı aynı seferde işlemek için)
                Okundu Olarak İşaretle / Hatırlatıcı ...
                Okunmadı Olarak İşaretle / Hatırlatıcı ...
                Listele (Sadece Gelen Kutusu) / <bit - okunmamışlar> / <bit - okunmuşlar> / <tarih saat - ilk tarih> / <tarih saat - son tarih>
                
    Adım 5 - Eposta.exe kimlik bilgileri ışığında komutları çalıştırır ve tekrardan Komut.mup dosyanın güncellenmesini bekler.
        Komut.mup
            Sunucu Kimlik Bilgileri / Rsa-4096 Açık anahtar - xml
                Gönderici / <Saklı İçerik - ParolaAes ile şifrelenmiş - base64> ( Adı / EpostaAdresi / Parola / SmtpSunucuAdresi / SunucuErişimNoktası / SSL )
                    Gönder İçeriği Şifreli / <boş veya True> (Komutlar / Gönder içeriği ParolaAes ile şifrelenerek korunabilir.)
                Alıcı / <Saklı İçerik - ParolaAes ile şifrelenmiş - base64> ( EpostaAdresi / Parola / ImapSunucuAdresi / SunucuErişimNoktası / SSL / Epostaları İndirme Klasörü )
                    Eposta İçeriği Şifreli / <boş veya True> (Epostaları İndirme Klasörü içerisinde dosya oluşturulmadan önce ParolaAes ile şifrelenir.)
                Saklı İçerik Parolası / <ParolaRsa>
            Komutlar
                Gönder / <Açık veya Saklı İçerik> ( <Kime ; ile ayrılmı> / <Bilgi ;> / <Gizli ;> / <Konu> / Mesaj_html / Mesaj_text / Dosya Eki 1 / Dosya Eki 2 ... )
                Kalıcı Olarak Sil / Hatırlatıcı1 / Hatırlatıcı2 ... (birden fazla epostayı aynı seferde işlemek için)
                Okundu Olarak İşaretle / Hatırlatıcı ...
                Okunmadı Olarak İşaretle / Hatırlatıcı ...
                Listele (Sadece Gelen Kutusu) / <bit - okunmamışlar> / <bit - okunmuşlar> / <tarih saat - ilk tarih> / <tarih saat - son tarih>
            Hatalar
                Genel (Kaynağı belirsiz yada genel)
                    Hata 1
                    Hata 2
                <Hatırlatıcı> / Hata Açıklaması
        Epostaları İndirme Klasörü / Epostalar.mup (Komutlar çalıştırıldıktan sonra güncel duruma göre üretilir)
            <Hatırlatıcı> / Tarih / Kimden / Bayraklar / Konu
                Mesaj / html / text
                Ekler
                    <Dosyanın Diskteki Adı> / <Dosyanın Gerçek Adı> / <bit - Ek dosyası mı> / Tipi / cid
                    (Dosyanın gerçek yolu -> Epostaları İndirme Klasörü\_Hatırlatıcı\<Dosyanın Diskteki Adı>)
