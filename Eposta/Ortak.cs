using ArgeMup.HazirKod;
using System.IO;
using System;
using ArgeMup.HazirKod.Ekİşlemler;
using System.Linq;
using MimeKit;
using MailKit;
using ArgeMup.HazirKod.Dönüştürme;
using System.Collections.Generic;

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

        //SSL 465
        public Gönderici_(string Adı, string EpostaAdresi, string Parola, string SunucuAdresi, int ErişimNoktası, bool SSL)
        {
            this.Adı = Adı;
            this.EpostaAdresi = EpostaAdresi;
            this.Parola = Parola;

            this.SunucuAdresi = SunucuAdresi;
            this.ErişimNoktası = ErişimNoktası;
            this.SSL = SSL;
        }

        public void Gönder(string Kime, string Bilgi, string Gizli, string Konu, string Mesaj_Html = null, string Mesaj = null, string[] DosyaEkleri = null)
        {
            using (MimeMessage message = new MimeMessage())
            {
                message.From.Add(new MimeKit.MailboxAddress(Adı, EpostaAdresi));
                if (Konu != null) message.Subject = Konu;

                if (Kime != null) { foreach (var biri in Kime.Split(';')) { message.To.Add(new MimeKit.MailboxAddress(null, biri)); } }
                if (Bilgi != null) { foreach (var biri in Bilgi.Split(';')) { message.To.Add(new MimeKit.MailboxAddress(null, biri)); } }
                if (Gizli != null) { foreach (var biri in Gizli.Split(';')) { message.To.Add(new MimeKit.MailboxAddress(null, biri)); } }

                BodyBuilder builder = new BodyBuilder();
                if (Mesaj != null) builder.TextBody = Mesaj;
                if (Mesaj_Html != null) builder.HtmlBody = Mesaj_Html;
                if (DosyaEkleri != null) { foreach (var biri in DosyaEkleri) builder.Attachments.Add(biri); }
                message.Body = builder.ToMessageBody();

                using (MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect(SunucuAdresi, ErişimNoktası, SSL);
                    client.Authenticate(EpostaAdresi, Parola);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
        }
    }

    public class Alıcı_ : IDisposable
    {
        public string EpostaAdresi, Parola, SunucuAdresi, ÇıktılarıKaydetmeKlasörü;
        public int ErişimNoktası;
        public bool SSL;
        byte[] ParolaAes;
        MailKit.Net.Imap.ImapClient İstemci = null;

        //SSL 993
        public Alıcı_(string EpostaAdresi, string Parola, string SunucuAdresi, int ErişimNoktası, bool SSL, string ÇıktılarıKaydetmeKlasörü, byte[] ParolaAes)
        {
            this.EpostaAdresi = EpostaAdresi;
            this.Parola = Parola;

            this.SunucuAdresi = SunucuAdresi;
            this.ErişimNoktası = ErişimNoktası;
            this.SSL = SSL;

            this.ParolaAes = ParolaAes; //Çıktıları şifrelemek için

            this.ÇıktılarıKaydetmeKlasörü = ÇıktılarıKaydetmeKlasörü.BoşMu() ? Kendi.Klasörü + @"\Çıktılar" : ÇıktılarıKaydetmeKlasörü;

            Directory.CreateDirectory(this.ÇıktılarıKaydetmeKlasörü);
            this.ÇıktılarıKaydetmeKlasörü += @"\";
        }
        IMailFolder KlasörüAç(string KlasörAdı, bool YazmaİzniGerekiyor)
        {
            if (İstemci == null || !İstemci.IsConnected)
            {
                İstemci = new MailKit.Net.Imap.ImapClient();
                İstemci.Connect(SunucuAdresi, ErişimNoktası, SSL);
                İstemci.Authenticate(EpostaAdresi, Parola);
            }

            IMailFolder mf;
            if (KlasörAdı.BoşMu(true)) mf = İstemci.Inbox;
            else
            {
                string[] dizi = KlasörAdı.Split(İstemci.Inbox.DirectorySeparator);
                if (dizi == null || dizi.Length != 2) throw new Exception("Hatalı Klasör Adı " + KlasörAdı);

                FolderNamespace fns = İstemci.PersonalNamespaces.First(x => x.Path == dizi[0]);
                if (fns == null) throw new Exception("Hatalı Klasör Adı " + KlasörAdı);

                mf = İstemci.GetFolder(fns);
                if (mf == null) throw new Exception("Hatalı Klasör Adı " + KlasörAdı);

                mf = mf.GetSubfolders(false).First(x => x.FullName == dizi[1]);
            }

            if (mf == null) throw new Exception("Hatalı Klasör Adı " + KlasörAdı);
            if (YazmaİzniGerekiyor && mf.IsOpen && mf.Access != FolderAccess.ReadWrite) mf.Close();
            if (!mf.IsOpen) mf.Open(YazmaİzniGerekiyor ? FolderAccess.ReadWrite : FolderAccess.ReadOnly);

            if (!mf.IsOpen) throw new Exception("Klasör açılamadı " + KlasörAdı);
            else if (YazmaİzniGerekiyor && mf.Access != FolderAccess.ReadWrite) throw new Exception("Klasör okuma yazma izni ile açılamadı " + KlasörAdı);

            return mf;
        }

        public void KlasörleriListele(IDepo_Eleman KomutMupCevaplarKomutDalı, bool Detaylar)
        {
            KlasörüAç(null, false);
            foreach (var kls in İstemci.PersonalNamespaces)
            {
                IMailFolder mf = İstemci.GetFolder(kls);

                foreach (IMailFolder mf_alt in mf.GetSubfolders(false))
                {
                    int Toplam = -1, Okunmadı = -1, YeniGelen = -1;
                    ulong Boyut = 0;

                    if (Detaylar)
                    {
                        if (!mf_alt.IsOpen) mf_alt.Open(FolderAccess.ReadOnly);
                        mf_alt.Status(StatusItems.Count | StatusItems.Recent | StatusItems.Unread | StatusItems.Size);

                        Toplam = mf_alt.Count;
                        Okunmadı = mf_alt.Unread;
                        YeniGelen = mf_alt.Recent;
                        Boyut = mf_alt.Size == null ? 0 : mf_alt.Size.Value;
                    }

                    KomutMupCevaplarKomutDalı[kls.Path + kls.DirectorySeparator + mf_alt.FullName].İçeriği = new string[] { Okunmadı.Yazıya(), Toplam.Yazıya(), YeniGelen.Yazıya(), Boyut.ToString() };
                }
            }
        }
        string _KlasörAdıBoşİseDüzelt_(string KlasörAdı)
        {
            return KlasörAdı.BoşMu(true) ? "Gelen Kutusu" : KlasörAdı;
        }
        string _EpostalarıYenile_DepoDosyasıAdı_(string KlasörAdı)
        {
            return D_DosyaKlasörAdı.Düzelt(ÇıktılarıKaydetmeKlasörü + "Epostaları Yenile\\" + _KlasörAdıBoşİseDüzelt_(KlasörAdı) + "\\Depo.mup");
        }
        Depo_ _EpostalarıYenile_DepoDosyasınıAç_UIDyiSil_(string KlasörAdı, string UID = null)
        {
            string çıktı_dosyası_adı = _EpostalarıYenile_DepoDosyasıAdı_(KlasörAdı);
            byte[] çıktı_dosyası_İçeriği = File.Exists(çıktı_dosyası_adı) ? File.ReadAllBytes(çıktı_dosyası_adı) : null;
            if (çıktı_dosyası_İçeriği != null && ParolaAes != null) çıktı_dosyası_İçeriği = çıktı_dosyası_İçeriği.Düzelt(ParolaAes);
            Depo_ depo = new Depo_(çıktı_dosyası_İçeriği?.Yazıya());
            
            if (UID.DoluMu()) depo.Sil("Liste/" + UID);

            return depo;
        }
        public void EpostalarıYenile(string KlasörAdı, bool SadeceOkunmamışlar, DateTime Başlangıç, DateTime Bitiş, bool EkleriAl)
        {
            IMailFolder mf = KlasörüAç(KlasörAdı, false);
            Depo_ depo = _EpostalarıYenile_DepoDosyasınıAç_UIDyiSil_(KlasörAdı);
            depo["Tipi"].İçeriği = new string[] { "EpostalarıYenile", _KlasörAdıBoşİseDüzelt_(KlasörAdı), DateTime.Now.Yazıya() };
            string ÇıktıDosyalarıKlasörü = Path.GetDirectoryName(_EpostalarıYenile_DepoDosyasıAdı_(KlasörAdı)) + "\\";
            List<string> İstenen_uid_ler = new List<string>();

            foreach (IMessageSummary summary in mf.Fetch(0, -1, new FetchRequest(MessageSummaryItems.UniqueId | MessageSummaryItems.Flags | MessageSummaryItems.InternalDate)))
            {
                string kimlik = summary.UniqueId.Id.ToString();

                if ((SadeceOkunmamışlar && summary.Flags.Value.HasFlag(MessageFlags.Seen)) ||
                    summary.Date.DateTime < Başlangıç ||
                    summary.Date.DateTime > Bitiş)
                {
                    depo.Sil("Liste/" + kimlik);
                    continue; //istenmiyor
                }

                //isteniyor
                İstenen_uid_ler.Add(kimlik);
                IDepo_Eleman uid = depo["Liste/" + summary.UniqueId.Id.ToString()];
                
                if (depo.Oku("Liste/" + kimlik).DoluMu())
                {
                    uid.Yaz(null, summary.Date.DateTime, 0);
                    uid.Yaz(null, summary.Flags?.ToString(), 1);
                    continue; //isteniyor fakat zaten indirilmiş
                }
                kimlik = "_" + kimlik;

                MimeMessage içerik = mf.GetMessage(summary.UniqueId);
                uid.İçeriği = new string[] {
                    içerik.Date.DateTime.Yazıya(),
                    summary.Flags?.ToString(),
                    içerik.Subject };

                MailboxAddress Gönderen = null;
                if (içerik.Sender != null) Gönderen = içerik.Sender;
                else if (içerik.From.Mailboxes != null && içerik.From.Mailboxes.Count() > 0) Gönderen = içerik.From.Mailboxes.First();
                if (Gönderen != null)
                {
                    uid[3] = Gönderen.Address;
                    uid[4] = Gönderen.Name;
                }
                else uid[3] = içerik.From.ToString();
                
                uid["Mesaj"].İçeriği = new string[] { içerik.HtmlBody, içerik.TextBody };

                if (!EkleriAl) uid.Sil("Ekler");
                else
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
                            string asıl_adı = D_DosyaKlasörAdı.Düzelt(Girdi.ContentDisposition?.FileName ?? Girdi.ContentType.Name);
                            if (string.IsNullOrWhiteSpace(asıl_adı)) return;

                            Klasör.Oluştur(ÇıktıDosyalarıKlasörü + kimlik);
                            string kontrol_edilmiş_adı = asıl_adı;
                            int no = 0;
                            while (File.Exists(ÇıktıDosyalarıKlasörü + kimlik + "\\" + kontrol_edilmiş_adı)) kontrol_edilmiş_adı = "_" + no++ + "_" + asıl_adı;

                            uid["Ekler/" + ÇıktıDosyalarıKlasörü + kimlik + "\\" + kontrol_edilmiş_adı].İçeriği = new string[] { asıl_adı, Girdi.IsAttachment.ToString(), Girdi.ContentType.MediaType + "/" + Girdi.ContentType.MediaSubtype, Girdi.ContentId };

                            using (MemoryStream stream = new MemoryStream())
                            {
                                if (Girdi is MessagePart)
                                {
                                    MessagePart rfc822 = (MessagePart)Girdi;
                                    rfc822.Message.WriteTo(stream);
                                }
                                else
                                {
                                    MimePart part = (MimePart)Girdi;
                                    part.Content.DecodeTo(stream);
                                }

                                byte[] çıktı_e = stream.ToArray();
                                if (ParolaAes != null) çıktı_e = çıktı_e.Karıştır(ParolaAes);
                                File.WriteAllBytes(ÇıktıDosyalarıKlasörü + kimlik + "\\" + kontrol_edilmiş_adı, çıktı_e);
                            }
                        }
                    }
                }
            }

            //Önceden kalma uid leri silme
            foreach (IDepo_Eleman biri in depo["Liste"].Elemanları)
            {
                if (İstenen_uid_ler.Contains(biri.Adı)) continue;
                biri.Sil(null);
            }

            if (!Directory.Exists(ÇıktıDosyalarıKlasörü)) Klasör.Oluştur(ÇıktıDosyalarıKlasörü);
            else
            {
                foreach (string kls in Directory.GetDirectories(ÇıktıDosyalarıKlasörü, "_*", SearchOption.TopDirectoryOnly))
                {
                    if (!EkleriAl) Klasör.Sil(kls);
                    else
                    {
                        int knm = kls.LastIndexOf("\\_");
                        if (knm < 0) continue;

                        string uid_ = kls.Substring(knm + 2);
                        if (depo.Oku("Liste/" + uid_).BoşMu()) Klasör.Sil(kls);
                    }
                }
            }
            
            byte[] çıktı_d = depo.YazıyaDönüştür().BaytDizisine();
            if (ParolaAes != null) çıktı_d = çıktı_d.Karıştır(ParolaAes);
            File.WriteAllBytes(_EpostalarıYenile_DepoDosyasıAdı_(KlasörAdı), çıktı_d);
        }
        public void İşaretle(string KlasörAdı, uint UID, bool Okundu)
        {
            IMailFolder mf = KlasörüAç(KlasörAdı, true);
            _EpostalarıYenile_DepoDosyasınıAç_UIDyiSil_(KlasörAdı, UID.ToString());

            UniqueId uid = new UniqueId(UID);
            if (Okundu) mf.Store(uid, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Seen) { Silent = true });
            else mf.Store(uid, new StoreFlagsRequest(StoreAction.Remove, MessageFlags.Seen) { Silent = true });
        }
        public string Taşı(uint UID, string KaynakKlasör, string HedefKlasör)
        {
            _EpostalarıYenile_DepoDosyasınıAç_UIDyiSil_(KaynakKlasör, UID.ToString());

            IMailFolder Hedef = KlasörüAç(HedefKlasör, true), Kaynak = KlasörüAç(KaynakKlasör, true);
            UniqueId? yeni = Kaynak.MoveTo(new UniqueId(UID), Hedef);
            return yeni.ToString();
        }
        public void Sil(string KlasörAdı, uint UID)
        {
            IMailFolder mf = KlasörüAç(KlasörAdı, true);
            _EpostalarıYenile_DepoDosyasınıAç_UIDyiSil_(KlasörAdı, UID.ToString());

            UniqueId uid = new UniqueId(UID);
            mf.Store(uid, new StoreFlagsRequest(StoreAction.Add, MessageFlags.Deleted) { Silent = true });
            List<UniqueId> l = new List<UniqueId> { uid };
            mf.Expunge(l);
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
