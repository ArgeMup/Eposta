using ArgeMup.HazirKod;
using System.IO;
using System;
using ArgeMup.HazirKod.Ekİşlemler;

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

    public class Gönderici_
    {
        public string Adı, EpostaAdresi, Parola, SunucuAdresi;
        public int ErişimNoktası;
        public bool SSL;

        public Gönderici_(string Adı, string EpostaAdresi, string Parola, string SunucuAdresi, int ErişimNoktası = 465, bool SSL = true)
        {
            this.Adı = Adı;
            this.EpostaAdresi = EpostaAdresi;
            this.Parola = Parola;

            this.SunucuAdresi = SunucuAdresi;
            this.ErişimNoktası = ErişimNoktası;
            this.SSL = SSL;
        }

        public string Gönder(string Kime, string Bilgi, string Gizli, string Konu, string Mesaj_Html = null, string Mesaj = null, string[] DosyaEkleri = null)
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

    public class Alıcı_ : IDisposable
    {
        public string EpostaAdresi, Parola, SunucuAdresi, İndirmeKlasörü;
        public int ErişimNoktası;
        public bool SSL;
        MailKit.Net.Imap.ImapClient İstemci = null;
        MailKit.IMailFolder GelenKutusu;

        public Alıcı_(string EpostaAdresi, string Parola, string SunucuAdresi, int ErişimNoktası = 993, bool SSL = true, string İndirmeKlasörü = null)
        {
            this.EpostaAdresi = EpostaAdresi;
            this.Parola = Parola;

            this.SunucuAdresi = SunucuAdresi;
            this.ErişimNoktası = ErişimNoktası;
            this.SSL = SSL;

            this.İndirmeKlasörü = İndirmeKlasörü == null ? Kendi.Klasörü + @"\Epostalar" : İndirmeKlasörü;

            Directory.CreateDirectory(this.İndirmeKlasörü);
            this.İndirmeKlasörü += @"\";
        }
        void Bağlan(bool SadeceOkunabilir = true)
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

        public void Listele(byte[] ParolaAes, bool SadeceOkunmamışlar, DateTime Başlangıç, DateTime Bitiş, bool EkleriAl)
        {
            Bağlan();
 
            System.Collections.Generic.List<string> MevcutEpostalar = new System.Collections.Generic.List<string>();
            foreach (MailKit.IMessageSummary summary in GelenKutusu.Fetch(0, -1, new MailKit.FetchRequest(MailKit.MessageSummaryItems.UniqueId | MailKit.MessageSummaryItems.Flags | MailKit.MessageSummaryItems.InternalDate)))
            {
                if (SadeceOkunmamışlar && summary.Flags.Value.HasFlag(MailKit.MessageFlags.Seen)) continue;
                if (summary.Date.DateTime < Başlangıç || summary.Date.DateTime > Bitiş) continue;

                string kimlik = "_" + summary.UniqueId.Id.ToString();
                MevcutEpostalar.Add(İndirmeKlasörü + kimlik);
                if (Directory.Exists(İndirmeKlasörü + kimlik)) continue;
                Directory.CreateDirectory(İndirmeKlasörü + kimlik);

                MimeKit.MimeMessage içerik = GelenKutusu.GetMessage(summary.UniqueId);

                Depo_ depo = new Depo_();
                depo.Yaz("Kontrol Edildi", DateTime.Now);
                IDepo_Eleman uid = depo[summary.UniqueId.Id.ToString()];
                uid.İçeriği = new string[] { 
                    içerik.Date.DateTime.Yazıya(),
                    (içerik.Sender == null ? içerik.From[0].ToString() : içerik.Sender.Name + " " + içerik.Sender.Address).Trim(),
                    summary.Flags?.ToString(),
                    içerik.Subject };
                uid["Mesaj"].İçeriği = new string[] { içerik.HtmlBody, içerik.TextBody };
                    
                if (EkleriAl)
                {
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

                            uid["Ekler/" + kontrol_edilmiş_adı].İçeriği = new string[] { asıl_adı, Girdi.IsAttachment.ToString(), Girdi.ContentType.MediaType + "/" + Girdi.ContentType.MediaSubtype, Girdi.ContentId };

                            using (MemoryStream stream = new MemoryStream())
                            {
                                if (Girdi is MimeKit.MessagePart)
                                {
                                    MimeKit.MessagePart rfc822 = (MimeKit.MessagePart)Girdi;

                                    rfc822.Message.WriteTo(stream);
                                }
                                else
                                {
                                    MimeKit.MimePart part = (MimeKit.MimePart)Girdi;

                                    part.Content.DecodeTo(stream);
                                }

                                byte[] çıktı_e = stream.ToArray();

                                if (ParolaAes != null) çıktı_e = çıktı_e.Karıştır(ParolaAes);

                                File.WriteAllBytes(İndirmeKlasörü + kimlik + "\\" + kontrol_edilmiş_adı, çıktı_e);
                            }
                        }
                    }
                }

                byte[] çıktı_d = depo.YazıyaDönüştür().BaytDizisine();
                if (ParolaAes != null) çıktı_d = çıktı_d.Karıştır(ParolaAes);
                File.WriteAllBytes(İndirmeKlasörü + kimlik + "\\Depo.mup", çıktı_d);
            }

            foreach (string kls in Directory.GetDirectories(İndirmeKlasörü, "_*", SearchOption.TopDirectoryOnly))
            {
                if (!MevcutEpostalar.Contains(kls)) Klasör.Sil(kls);
            }
        }
        public void İşaretle(uint UID, bool Okundu)
        {
            Bağlan(false);

            MailKit.UniqueId uid = new MailKit.UniqueId(UID);
            if (Okundu) GelenKutusu.Store(uid, new MailKit.StoreFlagsRequest(MailKit.StoreAction.Add, MailKit.MessageFlags.Seen) { Silent = true });
            else GelenKutusu.Store(uid, new MailKit.StoreFlagsRequest(MailKit.StoreAction.Remove, MailKit.MessageFlags.Seen) { Silent = true });
        }
        public void Sil(uint UID)
        {
            Bağlan(false);

            MailKit.UniqueId uid = new MailKit.UniqueId(UID);
            GelenKutusu.Store(uid, new MailKit.StoreFlagsRequest(MailKit.StoreAction.Add, MailKit.MessageFlags.Deleted) { Silent = true });
            Klasör.Sil(İndirmeKlasörü + @"_" + UID);
        }

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    İstemci?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Alıcı_()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
