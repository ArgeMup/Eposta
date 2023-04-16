using ArgeMup.HazirKod;
using System.IO;

namespace Eposta
{
    public static class GerekliDosyalar
    {
        public static void Başlat()
        {
            File.WriteAllText(Kendi.Klasörü + @"\Eposta.exe.config", Properties.Resources.Eposta_exe);
            File.WriteAllBytes(Kendi.Klasörü + @"\MailKit.dll", Properties.Resources.MailKit);
            File.WriteAllBytes(Kendi.Klasörü + @"\MimeKit.dll", Properties.Resources.MimeKit);
            File.WriteAllBytes(Kendi.Klasörü + @"\BouncyCastle.Cryptography.dll", Properties.Resources.BouncyCastle_Cryptography);
            File.WriteAllBytes(Kendi.Klasörü + @"\System.Buffers.dll", Properties.Resources.System_Buffers);
            File.WriteAllBytes(Kendi.Klasörü + @"\System.Memory.dll", Properties.Resources.System_Memory);
            File.WriteAllBytes(Kendi.Klasörü + @"\System.Numerics.Vectors.dll", Properties.Resources.System_Numerics_Vectors);
            File.WriteAllBytes(Kendi.Klasörü + @"\System.Threading.Tasks.Extensions.dll", Properties.Resources.System_Threading_Tasks_Extensions);
            File.WriteAllBytes(Kendi.Klasörü + @"\System.Runtime.CompilerServices.Unsafe.dll", Properties.Resources.System_Runtime_CompilerServices_Unsafe);
        }
    }

    public static class Gönderici
    {
        public static string Adı, EpostaAdresi, Parola, SunucuAdresi;
        public static int ErişimNoktası;
        public static bool SSL;

        public static void Başlat(string Adı, string EpostaAdresi, string Parola, string SunucuAdresi, int ErişimNoktası = 465, bool SSL = true)
        {
            Gönderici.Adı = Adı;
            Gönderici.EpostaAdresi = EpostaAdresi;
            Gönderici.Parola = Parola;

            Gönderici.SunucuAdresi = SunucuAdresi;
            Gönderici.ErişimNoktası = ErişimNoktası;
            Gönderici.SSL = SSL;
        }

        public static string Gönder(string Kime, string Bilgi, string Gizli, string Konu, string Mesaj = null, string Mesaj_Html = null, string[] DosyaEkleri = null)
        {
            string sonuç = null;
            MimeKit.MimeMessage message = null;
            MailKit.Net.Smtp.SmtpClient client = null;

            try
            {
                message = new MimeKit.MimeMessage();
                message.From.Add(new MimeKit.MailboxAddress(Adı, EpostaAdresi));
                if (Konu != null) message.Subject = Konu;

                if (Kime != null) { foreach (var biri in Kime.Split(';')) { message.To.Add(new MimeKit.MailboxAddress(null, biri)); } }
                if (Bilgi != null) { foreach (var biri in Bilgi.Split(';')) { message.To.Add(new MimeKit.MailboxAddress(null, biri)); } }
                if (Gizli != null) { foreach (var biri in Gizli.Split(';')) { message.To.Add(new MimeKit.MailboxAddress(null, biri)); } }

                MimeKit.BodyBuilder builder = new MimeKit.BodyBuilder();
                if (Mesaj != null) builder.TextBody = Mesaj;
                if (Mesaj_Html != null) builder.HtmlBody = Mesaj_Html;
                if (DosyaEkleri != null) { foreach (var biri in DosyaEkleri) builder.Attachments.Add(biri); }
                message.Body = builder.ToMessageBody();

                client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect(SunucuAdresi, ErişimNoktası, SSL);
                client.Authenticate(EpostaAdresi, Parola);
                client.Send(message);
                client.Disconnect(true);
            }
            catch (System.Exception ex) { sonuç = ex.Message; }

            message?.Dispose();
            client?.Dispose();

            return sonuç;
        }
    }

    public static class Alıcı
    {
        public static string EpostaAdresi, Parola, SunucuAdresi, İndirmeKlasörü;
        public static int ErişimNoktası;
        public static bool SSL;
        static MailKit.Net.Imap.ImapClient İstemci = null;
        static MailKit.IMailFolder GelenKutusu;

        public static void Başlat(string EpostaAdresi, string Parola, string SunucuAdresi, int ErişimNoktası = 993, bool SSL = true, string İndirmeKlasörü = null, bool İndirmeKlasörünüSil = true)
        {
            Alıcı.EpostaAdresi = EpostaAdresi;
            Alıcı.Parola = Parola;

            Alıcı.SunucuAdresi = SunucuAdresi;
            Alıcı.ErişimNoktası = ErişimNoktası;
            Alıcı.SSL = SSL;

            Alıcı.İndirmeKlasörü = İndirmeKlasörü == null ? Kendi.Klasörü + @"\Epostalar" : İndirmeKlasörü;
            if (İndirmeKlasörünüSil) Klasör.Sil(Alıcı.İndirmeKlasörü);

            Directory.CreateDirectory(Alıcı.İndirmeKlasörü);
            Alıcı.İndirmeKlasörü += @"\";
        }
        static void Bağlan(bool SadeceOkunabilir = true)
        {
            bool YenidenBağlan = false;
            if (İstemci == null || !İstemci.IsConnected || GelenKutusu == null || !GelenKutusu.IsOpen) YenidenBağlan = true;
            else if (!SadeceOkunabilir && GelenKutusu.Access != MailKit.FolderAccess.ReadWrite) { YenidenBağlan = true; İstemci.Disconnect(true); }

            if (YenidenBağlan)
            {
                İstemci = new MailKit.Net.Imap.ImapClient();
                İstemci.Connect(SunucuAdresi, ErişimNoktası, SSL);
                İstemci.Authenticate(EpostaAdresi, Parola);

                GelenKutusu = İstemci.Inbox;
                GelenKutusu.Open(SadeceOkunabilir ? MailKit.FolderAccess.ReadOnly : MailKit.FolderAccess.ReadWrite);
            }
        }

        public static void Listele()
        {
            Bağlan();

            System.Collections.Generic.List<string> MevcutEpostalar = new System.Collections.Generic.List<string>();
            foreach (MailKit.IMessageSummary summary in GelenKutusu.Fetch(0, -1, new MailKit.FetchRequest(MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.Flags)))
            {
                string kimlik = "_" + summary.UniqueId.Id.ToString();
                MevcutEpostalar.Add(İndirmeKlasörü + kimlik);
                if (Directory.Exists(İndirmeKlasörü + kimlik)) continue;
                Directory.CreateDirectory(İndirmeKlasörü + kimlik);

                MimeKit.MimeMessage içerik = GelenKutusu.GetMessage(summary.UniqueId);
                Depo_ depo = new Depo_();
                depo.Yaz("Hatırlatıcı", summary.UniqueId.Id);
                depo.Yaz("Konu", içerik.Subject);
                depo.Yaz("Mesaj", içerik.HtmlBody + System.Environment.NewLine + içerik.TextBody);
                depo.Yaz("Tarih", içerik.Date.DateTime);
                depo.Yaz("Kimden", (içerik.Sender == null ? içerik.From[0].ToString() : içerik.Sender.Name + " " + içerik.Sender.Address).Trim());
                depo.Yaz("Bayraklar", summary.Flags?.ToString());

                _DosyaEkiAra_(içerik.Body);
                void _DosyaEkiAra_(MimeKit.MimeEntity Girdi)
                {
                    if (Girdi.ContentType.MediaType == "text") return;
                    else if (Girdi.ContentType.MediaType == "multipart")
                    {
                        MimeKit.Multipart çoklu = (MimeKit.Multipart)Girdi;
                        foreach (MimeKit.MimeEntity biri in çoklu)
                        {
                            _DosyaEkiAra_(biri);
                        }
                    }
                    else
                    {
                        string asıl_adı = ArgeMup.HazirKod.Dönüştürme.D_DosyaKlasörAdı.Düzelt(Girdi.ContentDisposition?.FileName ?? Girdi.ContentType.Name);
                        if (string.IsNullOrWhiteSpace(asıl_adı)) return;

                        string kontrol_edilmiş_adı = asıl_adı;
                        int no = 0;
                        while (File.Exists(İndirmeKlasörü + kimlik + "\\" + kontrol_edilmiş_adı)) kontrol_edilmiş_adı = "_" + no++ + "_" + asıl_adı;

                        depo["Ekler/" + kontrol_edilmiş_adı].İçeriği = new string[] { Girdi.IsAttachment.ToString(), asıl_adı, Girdi.ContentType.MediaType + "/" + Girdi.ContentType.MediaSubtype, Girdi.ContentId };

                        using (var stream = File.Create(İndirmeKlasörü + kimlik + "\\" + kontrol_edilmiş_adı))
                        {
                            if (Girdi is MimeKit.MessagePart)
                            {
                                var rfc822 = (MimeKit.MessagePart)Girdi;

                                rfc822.Message.WriteTo(stream);
                            }
                            else
                            {
                                var part = (MimeKit.MimePart)Girdi;

                                part.Content.DecodeTo(stream);
                            }
                        }
                    }
                }

                File.WriteAllText(İndirmeKlasörü + kimlik + @"\Depo.mup", depo.YazıyaDönüştür());
            }

            foreach (string kls in Directory.GetDirectories(İndirmeKlasörü, "_*", SearchOption.TopDirectoryOnly))
            {
                if (!MevcutEpostalar.Contains(kls)) Klasör.Sil(kls);
            }
        }
        public static void İşaretle(uint Hatırlatıcı, bool Okundu)
        {
            Bağlan(false);

            MailKit.UniqueId uid = new MailKit.UniqueId(Hatırlatıcı);
            if (Okundu) GelenKutusu.Store(uid, new MailKit.StoreFlagsRequest(MailKit.StoreAction.Add, MailKit.MessageFlags.Seen) { Silent = true });
            else GelenKutusu.Store(uid, new MailKit.StoreFlagsRequest(MailKit.StoreAction.Remove, MailKit.MessageFlags.Seen) { Silent = true });
        }
        public static void Sil(uint Hatırlatıcı)
        {
            Bağlan(false);

            MailKit.UniqueId uid = new MailKit.UniqueId(Hatırlatıcı);
            GelenKutusu.Store(uid, new MailKit.StoreFlagsRequest(MailKit.StoreAction.Add, MailKit.MessageFlags.Deleted) { Silent = true });
            Klasör.Sil(İndirmeKlasörü + @"_" + Hatırlatıcı);
        }
    }
}
