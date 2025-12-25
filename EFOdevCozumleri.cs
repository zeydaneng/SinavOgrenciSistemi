/*
 * ================================================================
 * ENTITY FRAMEWORK ÖDEV ÇÖZÜMLERİ
 * 45 Soru - Gelişmiş Seviye
 * Veritabanı: dbSinavOgrenci
 * ================================================================
 * 
 * KULLANIMDAN ÖNCE:
 * 1. Visual Studio'da yeni bir Windows Forms projesi oluşturun
 * 2. NuGet üzerinden EntityFramework paketini yükleyin
 * 3. Add > New Item > ADO.NET Entity Data Model ekleyin
 * 4. Database First yaklaşımı ile dbSinavOgrenci veritabanını seçin
 * 5. Tüm tabloları modele dahil edin
 * 
 * Bu dosyadaki kodları ilgili formlara kopyalayarak kullanabilirsiniz.
 * ================================================================
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Windows.Forms;

namespace EFOdevCozumleri
{
    public partial class FormMain : Form
    {
        // ================================================================
        // SORU 1: Form seviyesinde DbContext örneği
        // ================================================================
        /*
         * AÇIKLAMA:
         * DbContext, Entity Framework'ün veritabanı ile iletişim kurmasını sağlayan
         * ana sınıftır. Form seviyesinde tanımlandığında:
         * - Form açıldığında context oluşturulur
         * - Form boyunca aynı context kullanılır (tek bağlantı)
         * - Form kapandığında Dispose edilmelidir
         * 
         * YAŞAM DÖNGÜSÜ:
         * 1. Form.Load   → Context oluşturulur
         * 2. Form çalışır → Tüm işlemler aynı context ile yapılır
         * 3. Form.Close  → Context Dispose edilir
         */
        dbSinavOgrenciEntities db = new dbSinavOgrenciEntities();

        public FormMain()
        {
            InitializeComponent();
        }

        // Form kapanırken context'i temizle
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            db.Dispose();
            base.OnFormClosed(e);
        }

        // ================================================================
        // SORU 2: Tüm öğrencileri listeleme
        // ================================================================
        private void btnOgrenciListele_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = db.TBLOGRENCI.ToList();
        }

        // ================================================================
        // SORU 3: OgrenciFoto alanını gizleme
        // ================================================================
        private void btnOgrenciListeleGizli_Click(object sender, EventArgs e)
        {
            // Yöntem 1: DataGridView yüklendikten sonra sütunu gizle
            dataGridView1.DataSource = db.TBLOGRENCI.ToList();
            if (dataGridView1.Columns.Contains("OgrenciFoto"))
            {
                dataGridView1.Columns["OgrenciFoto"].Visible = false;
            }

            // Yöntem 2: Select ile sadece istenen alanları seç
            var ogrenciler = db.TBLOGRENCI
                .Select(o => new
                {
                    o.OgrenciID,
                    o.OgrenciAd,
                    o.OgrenciSoyad,
                    o.KulupID
                })
                .ToList();
            dataGridView1.DataSource = ogrenciler;
        }

        // ================================================================
        // SORU 4: Dersleri listeleme
        // ================================================================
        private void btnDersListele_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = db.TBLDERSLER.ToList();
        }

        // ================================================================
        // SORU 5: ADO.NET vs Entity Framework karşılaştırması
        // ================================================================
        /*
         * ADO.NET YÖNTEMİ:
         * - Manuel bağlantı yönetimi gerektirir
         * - SQL sorguları string olarak yazılır
         * - DataReader veya DataAdapter kullanılır
         * - Daha fazla kod yazılması gerekir
         * - Daha düşük seviyeli kontrol sağlar
         * 
         * ENTITY FRAMEWORK YÖNTEMİ:
         * - Otomatik bağlantı yönetimi
         * - LINQ ile tip güvenli sorgular
         * - Nesne tabanlı çalışma
         * - Daha az kod, daha okunabilir
         * - ORM avantajları (mapping, tracking)
         */

        // ADO.NET ile listeleme
        private void btnDersListeleADO_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=.;Database=dbSinavOgrenci;Integrated Security=True;";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM TBLDERSLER", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        // Entity Framework ile listeleme (daha kısa ve temiz)
        private void btnDersListeleEF_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = db.TBLDERSLER.ToList();
        }

        // ================================================================
        // SORU 6: TBLNOTLAR anonim tip ile listeleme
        // ================================================================
        private void btnNotListele_Click(object sender, EventArgs e)
        {
            var notlar = db.TBLNOTLAR
                .Select(n => new
                {
                    n.NOTID,
                    n.DERS,
                    n.SINAV1,
                    n.SINAV2,
                    n.SINAV3,
                    OgrenciAdi = n.TBLOGRENCI.OgrenciAd
                })
                .ToList();

            dataGridView1.DataSource = notlar;
        }

        // ================================================================
        // SORU 7: Navigation property ile ders adı getirme
        // ================================================================
        private void btnNotListeleDersAdi_Click(object sender, EventArgs e)
        {
            var notlar = db.TBLNOTLAR
                .Select(n => new
                {
                    n.NOTID,
                    DersAdi = n.TBLDERSLER.DERSAD,  // Navigation property
                    n.SINAV1,
                    n.SINAV2,
                    n.SINAV3,
                    OgrenciAdi = n.TBLOGRENCI.OgrenciAd  // Navigation property
                })
                .ToList();

            dataGridView1.DataSource = notlar;
        }

        // ================================================================
        // SORU 8: Navigation Property Açıklaması
        // ================================================================
        /*
         * NAVIGATION PROPERTY NEDİR?
         * 
         * Navigation Property, Entity Framework'te ilişkili tablolar arasında
         * geçiş yapmamızı sağlayan özelliklerdir.
         * 
         * TBLOGRENCI tablosunda KulupID foreign key olarak tanımlıdır.
         * Entity Framework, bu ilişkiyi otomatik olarak algılar ve
         * TBLOGRENCI entity'sine TBLKULUPLER tipinde bir navigation property ekler.
         * 
         * ÖRNEK:
         * - TBLNOTLAR entity'sinde OGRENCI (int) → Foreign Key
         * - TBLNOTLAR entity'sinde TBLOGRENCI → Navigation Property
         * 
         * Bu sayede n.TBLOGRENCI.OgrenciAd şeklinde doğrudan ilişkili
         * tablodaki verilere erişebiliriz.
         * 
         * AVANTAJLARI:
         * 1. JOIN yazmaya gerek kalmaz
         * 2. Tip güvenliği sağlar
         * 3. IntelliSense desteği sunar
         * 4. Kod okunabilirliğini artırır
         */

        // ================================================================
        // SORU 9: Ad + Soyad birleştirme
        // ================================================================
        private void btnAdSoyadBirlestir_Click(object sender, EventArgs e)
        {
            var ogrenciler = db.TBLOGRENCI
                .Select(o => new
                {
                    o.OgrenciID,
                    AdSoyad = o.OgrenciAd + " " + o.OgrenciSoyad,
                    o.KulupID
                })
                .ToList();

            dataGridView1.DataSource = ogrenciler;
        }

        // ================================================================
        // SORU 10: Öğrenci - Kulüp JOIN
        // ================================================================
        private void btnOgrenciKulup_Click(object sender, EventArgs e)
        {
            // Yöntem 1: Navigation Property (Önerilen)
            var liste1 = db.TBLOGRENCI
                .Select(o => new
                {
                    AdSoyad = o.OgrenciAd + " " + o.OgrenciSoyad,
                    KulupAdi = o.TBLKULUPLER.KULUPAD
                })
                .ToList();

            // Yöntem 2: Explicit Join
            var liste2 = db.TBLOGRENCI
                .Join(db.TBLKULUPLER,
                    o => o.KulupID,
                    k => k.KULUPID,
                    (o, k) => new
                    {
                        AdSoyad = o.OgrenciAd + " " + o.OgrenciSoyad,
                        KulupAdi = k.KULUPAD
                    })
                .ToList();

            dataGridView1.DataSource = liste1;
        }

        // ================================================================
        // SORU 11: Üç tablo birleştirme
        // ================================================================
        private void btnUcTabloJoin_Click(object sender, EventArgs e)
        {
            var liste = db.TBLNOTLAR
                .Select(n => new
                {
                    OgrenciAdi = n.TBLOGRENCI.OgrenciAd + " " + n.TBLOGRENCI.OgrenciSoyad,
                    DersAdi = n.TBLDERSLER.DERSAD,
                    n.ORTALAMA,
                    Durum = n.DURUM == true ? "GEÇTİ" : "KALDI"
                })
                .ToList();

            dataGridView1.DataSource = liste;
        }

        // ================================================================
        // SORU 12: Öğrencinin tüm notlarını navigation ile getirme
        // ================================================================
        private void btnOgrenciNotlari_Click(object sender, EventArgs e)
        {
            int ogrenciID = Convert.ToInt32(txtOgrenciID.Text);

            var ogrenci = db.TBLOGRENCI.Find(ogrenciID);
            if (ogrenci != null)
            {
                // Navigation property üzerinden notlara erişim
                var notlar = ogrenci.TBLNOTLAR
                    .Select(n => new
                    {
                        DersAdi = n.TBLDERSLER.DERSAD,
                        n.SINAV1,
                        n.SINAV2,
                        n.SINAV3,
                        n.ORTALAMA,
                        Durum = n.DURUM == true ? "GEÇTİ" : "KALDI"
                    })
                    .ToList();

                dataGridView1.DataSource = notlar;
            }
            else
            {
                MessageBox.Show("Öğrenci bulunamadı!");
            }
        }

        // ================================================================
        // SORU 13: Öğrenci ekleme
        // ================================================================
        /*
         * Form Tasarımı:
         * - txtOgrenciAd: TextBox
         * - txtOgrenciSoyad: TextBox
         * - txtFotoYol: TextBox
         * - cmbKulup: ComboBox (ValueMember=KULUPID, DisplayMember=KULUPAD)
         * - btnEkle: Button
         */
        private void FormMain_Load(object sender, EventArgs e)
        {
            // ComboBox'ları doldur
            cmbKulup.DataSource = db.TBLKULUPLER.ToList();
            cmbKulup.ValueMember = "KULUPID";
            cmbKulup.DisplayMember = "KULUPAD";

            cmbDers.DataSource = db.TBLDERSLER.ToList();
            cmbDers.ValueMember = "DERSID";
            cmbDers.DisplayMember = "DERSAD";
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            TBLOGRENCI yeniOgrenci = new TBLOGRENCI
            {
                OgrenciAd = txtOgrenciAd.Text,
                OgrenciSoyad = txtOgrenciSoyad.Text,
                OgrenciFoto = string.IsNullOrEmpty(txtFotoYol.Text) ? null : txtFotoYol.Text,
                KulupID = Convert.ToInt32(cmbKulup.SelectedValue)
            };

            db.TBLOGRENCI.Add(yeniOgrenci);
            db.SaveChanges();

            MessageBox.Show("Öğrenci başarıyla eklendi!");

            // DataGridView'i güncelle (SORU 14)
            OgrenciListesiGuncelle();
        }

        // ================================================================
        // SORU 14: DataGridView otomatik güncelleme
        // ================================================================
        private void OgrenciListesiGuncelle()
        {
            dataGridView1.DataSource = db.TBLOGRENCI
                .Select(o => new
                {
                    o.OgrenciID,
                    o.OgrenciAd,
                    o.OgrenciSoyad,
                    KulupAdi = o.TBLKULUPLER.KULUPAD
                })
                .ToList();
        }

        // ================================================================
        // SORU 15: Öğrenci silme
        // ================================================================
        private void btnSil_Click(object sender, EventArgs e)
        {
            int ogrenciID;
            if (!int.TryParse(txtOgrenciID.Text, out ogrenciID))
            {
                MessageBox.Show("Geçerli bir ID giriniz!");
                return;
            }

            var silinecek = db.TBLOGRENCI.Find(ogrenciID);

            if (silinecek == null)
            {
                MessageBox.Show("Bu ID'ye sahip öğrenci bulunamadı!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Önce öğrencinin notlarını sil (foreign key kısıtlaması)
            var notlar = db.TBLNOTLAR.Where(n => n.OGRENCI == ogrenciID).ToList();
            db.TBLNOTLAR.RemoveRange(notlar);

            db.TBLOGRENCI.Remove(silinecek);
            db.SaveChanges();

            MessageBox.Show("Öğrenci başarıyla silindi!");
            OgrenciListesiGuncelle();
        }

        // ================================================================
        // SORU 16: Öğrenci güncelleme
        // ================================================================
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            int ogrenciID = Convert.ToInt32(txtOgrenciID.Text);

            var ogrenci = db.TBLOGRENCI.Find(ogrenciID);

            if (ogrenci == null)
            {
                MessageBox.Show("Öğrenci bulunamadı!");
                return;
            }

            ogrenci.OgrenciAd = txtOgrenciAd.Text;
            ogrenci.OgrenciSoyad = txtOgrenciSoyad.Text;
            ogrenci.OgrenciFoto = txtFotoYol.Text;
            ogrenci.KulupID = Convert.ToInt32(cmbKulup.SelectedValue);

            db.SaveChanges();

            MessageBox.Show("Öğrenci başarıyla güncellendi!");
            OgrenciListesiGuncelle();
        }

        // ================================================================
        // SORU 17: Güncelleme öncesi varlık kontrolü
        // ================================================================
        private bool OgrenciVarMi(int ogrenciID)
        {
            return db.TBLOGRENCI.Any(o => o.OgrenciID == ogrenciID);
        }

        private void btnKontrolluGuncelle_Click(object sender, EventArgs e)
        {
            int ogrenciID = Convert.ToInt32(txtOgrenciID.Text);

            if (!OgrenciVarMi(ogrenciID))
            {
                MessageBox.Show("Bu ID'ye sahip öğrenci bulunamadı!");
                return;
            }

            // Güncelleme işlemi...
            var ogrenci = db.TBLOGRENCI.Find(ogrenciID);
            ogrenci.OgrenciAd = txtOgrenciAd.Text;
            ogrenci.OgrenciSoyad = txtOgrenciSoyad.Text;
            db.SaveChanges();
        }

        // ================================================================
        // SORU 18: EntityState Kavramı
        // ================================================================
        /*
         * ENTITYSTATE AÇIKLAMASI:
         * 
         * Entity Framework, her entity'nin durumunu takip eder.
         * Bu durum, SaveChanges() çağrıldığında hangi SQL komutunun
         * çalıştırılacağını belirler.
         * 
         * DURUMLAR:
         * 
         * 1. Unchanged (Değişmemiş)
         *    - Veritabanından çekilen, değiştirilmemiş kayıt
         *    - SaveChanges() hiçbir işlem yapmaz
         * 
         * 2. Modified (Değiştirilmiş)
         *    - Bir property'si değiştirilmiş kayıt
         *    - SaveChanges() UPDATE sorgusu çalıştırır
         * 
         * 3. Added (Eklenmiş)
         *    - DbSet.Add() ile eklenen yeni kayıt
         *    - SaveChanges() INSERT sorgusu çalıştırır
         * 
         * 4. Deleted (Silinmiş)
         *    - DbSet.Remove() ile işaretlenen kayıt
         *    - SaveChanges() DELETE sorgusu çalıştırır
         * 
         * 5. Detached (Bağlantısız)
         *    - Context tarafından takip edilmeyen entity
         *    - SaveChanges() hiçbir işlem yapmaz
         * 
         * ÖRNEK KULLANIM:
         */
        private void EntityStateDemosu()
        {
            var ogrenci = db.TBLOGRENCI.Find(1);
            var state = db.Entry(ogrenci).State;  // Unchanged

            ogrenci.OgrenciAd = "Yeni Ad";
            state = db.Entry(ogrenci).State;  // Modified

            var yeni = new TBLOGRENCI { OgrenciAd = "Test" };
            db.TBLOGRENCI.Add(yeni);
            state = db.Entry(yeni).State;  // Added

            db.TBLOGRENCI.Remove(ogrenci);
            state = db.Entry(ogrenci).State;  // Deleted
        }

        // ================================================================
        // SORU 19: TextChanged ile filtreleme
        // ================================================================
        private void txtOgrenciAd_TextChanged(object sender, EventArgs e)
        {
            string aramaMetni = txtOgrenciAd.Text;

            var sonuc = db.TBLOGRENCI
                .Where(o => o.OgrenciAd.Contains(aramaMetni))
                .Select(o => new
                {
                    o.OgrenciID,
                    o.OgrenciAd,
                    o.OgrenciSoyad
                })
                .ToList();

            dataGridView1.DataSource = sonuc;
        }

        // ================================================================
        // SORU 20: Ada göre artan sıralama
        // ================================================================
        private void btnArtanSirala_Click(object sender, EventArgs e)
        {
            var ogrenciler = db.TBLOGRENCI
                .OrderBy(o => o.OgrenciAd)
                .ToList();

            dataGridView1.DataSource = ogrenciler;
        }

        // ================================================================
        // SORU 21: Soyada göre azalan sıralama
        // ================================================================
        private void btnAzalanSirala_Click(object sender, EventArgs e)
        {
            var ogrenciler = db.TBLOGRENCI
                .OrderByDescending(o => o.OgrenciSoyad)
                .ToList();

            dataGridView1.DataSource = ogrenciler;
        }

        // ================================================================
        // SORU 22: İlk 3 öğrenci (Take)
        // ================================================================
        private void btnIlkUc_Click(object sender, EventArgs e)
        {
            var ogrenciler = db.TBLOGRENCI
                .OrderBy(o => o.OgrenciID)
                .Take(3)
                .ToList();

            dataGridView1.DataSource = ogrenciler;
        }

        // ================================================================
        // SORU 23: ID ile öğrenci getirme
        // ================================================================
        private void btnIdIleGetir_Click(object sender, EventArgs e)
        {
            int aramaID = Convert.ToInt32(txtOgrenciID.Text);

            var ogrenci = db.TBLOGRENCI
                .Where(o => o.OgrenciID == aramaID)
                .Select(o => new
                {
                    o.OgrenciID,
                    o.OgrenciAd,
                    o.OgrenciSoyad,
                    KulupAdi = o.TBLKULUPLER.KULUPAD
                })
                .ToList();

            dataGridView1.DataSource = ogrenci;
        }

        // ================================================================
        // SORU 24: "A" ile başlayan öğrenciler
        // ================================================================
        private void btnAileBaslayan_Click(object sender, EventArgs e)
        {
            var ogrenciler = db.TBLOGRENCI
                .Where(o => o.OgrenciAd.StartsWith("A"))
                .Select(o => new
                {
                    o.OgrenciID,
                    o.OgrenciAd,
                    o.OgrenciSoyad
                })
                .ToList();

            dataGridView1.DataSource = ogrenciler;
        }

        // ================================================================
        // SORU 25: "n" ile biten öğrenciler
        // ================================================================
        private void btnNileBiten_Click(object sender, EventArgs e)
        {
            var ogrenciler = db.TBLOGRENCI
                .Where(o => o.OgrenciAd.EndsWith("n"))
                .Select(o => new
                {
                    o.OgrenciID,
                    o.OgrenciAd,
                    o.OgrenciSoyad
                })
                .ToList();

            dataGridView1.DataSource = ogrenciler;
        }

        // ================================================================
        // SORU 26: SINAV1 notlarının toplamı
        // ================================================================
        private void btnSinav1Toplam_Click(object sender, EventArgs e)
        {
            var toplam = db.TBLNOTLAR.Sum(n => n.SINAV1);
            MessageBox.Show($"SINAV1 Notları Toplamı: {toplam}");
        }

        // ================================================================
        // SORU 27: SINAV1 notlarının ortalaması
        // ================================================================
        private void btnSinav1Ortalama_Click(object sender, EventArgs e)
        {
            var ortalama = db.TBLNOTLAR.Average(n => n.SINAV1);
            MessageBox.Show($"SINAV1 Notları Ortalaması: {ortalama:F2}");
        }

        // ================================================================
        // SORU 28: Ortalama üzeri not alan öğrenciler
        // ================================================================
        private void btnOrtalamaUzeri_Click(object sender, EventArgs e)
        {
            var ortalama = db.TBLNOTLAR.Average(n => n.SINAV1);

            var ogrenciler = db.TBLNOTLAR
                .Where(n => n.SINAV1 > ortalama)
                .Select(n => new
                {
                    OgrenciAdi = n.TBLOGRENCI.OgrenciAd + " " + n.TBLOGRENCI.OgrenciSoyad,
                    n.SINAV1,
                    Ortalama = ortalama
                })
                .ToList();

            dataGridView1.DataSource = ogrenciler;
        }

        // ================================================================
        // SORU 29: Anonim tip ile durum listesi
        // ================================================================
        private void btnDurumListesi_Click(object sender, EventArgs e)
        {
            var liste = db.TBLNOTLAR
                .Select(n => new
                {
                    Ogrenci = n.TBLOGRENCI.OgrenciAd + " " + n.TBLOGRENCI.OgrenciSoyad,
                    n.ORTALAMA,
                    Durum = n.DURUM == true ? "GEÇTİ" : "KALDI"
                })
                .ToList();

            dataGridView1.DataSource = liste;
        }

        // ================================================================
        // SORU 30: Geçen ve kalan öğrencileri ayrı listeleme
        // ================================================================
        private void btnGecenKalan_Click(object sender, EventArgs e)
        {
            // Geçen öğrenciler
            var gecenler = db.TBLNOTLAR
                .Where(n => n.DURUM == true)
                .Select(n => new
                {
                    Ogrenci = n.TBLOGRENCI.OgrenciAd + " " + n.TBLOGRENCI.OgrenciSoyad,
                    DersAdi = n.TBLDERSLER.DERSAD,
                    n.ORTALAMA
                })
                .ToList();

            // Kalan öğrenciler
            var kalanlar = db.TBLNOTLAR
                .Where(n => n.DURUM == false)
                .Select(n => new
                {
                    Ogrenci = n.TBLOGRENCI.OgrenciAd + " " + n.TBLOGRENCI.OgrenciSoyad,
                    DersAdi = n.TBLDERSLER.DERSAD,
                    n.ORTALAMA
                })
                .ToList();

            // İki ayrı DataGridView'e bağlama
            dgvGecenler.DataSource = gecenler;
            dgvKalanlar.DataSource = kalanlar;
        }

        // ================================================================
        // SORU 30 (Gelişmiş): SP ile kontrollü not ekleme
        // ================================================================
        private void btnSPNotEkle_Click(object sender, EventArgs e)
        {
            try
            {
                int ogrenciID = Convert.ToInt32(txtOgrenciID.Text);
                int dersID = Convert.ToInt32(cmbDers.SelectedValue);
                int sinav1 = Convert.ToInt32(txtSinav1.Text);
                int sinav2 = Convert.ToInt32(txtSinav2.Text);
                int sinav3 = Convert.ToInt32(txtSinav3.Text);

                // Stored Procedure çağırma
                db.Database.ExecuteSqlCommand(
                    "EXEC sp_NotEkleKontrollu @OgrenciID, @DersID, @Sinav1, @Sinav2, @Sinav3",
                    new SqlParameter("@OgrenciID", ogrenciID),
                    new SqlParameter("@DersID", dersID),
                    new SqlParameter("@Sinav1", sinav1),
                    new SqlParameter("@Sinav2", sinav2),
                    new SqlParameter("@Sinav3", sinav3)
                );

                MessageBox.Show("Not başarıyla eklendi!");
            }
            catch (SqlException ex)
            {
                // SP hata mesajını yakalama
                MessageBox.Show($"Hata: {ex.Message}", "SP Hatası",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================================================================
        // SORU 31: SP ile öğrenci detay raporu
        // ================================================================
        private void btnOgrenciDetay_Click(object sender, EventArgs e)
        {
            int ogrenciID = Convert.ToInt32(txtOgrenciID.Text);

            var sonuc = db.Database.SqlQuery<OgrenciDetay>(
                "EXEC sp_OgrenciDetayGetir @OgrenciID",
                new SqlParameter("@OgrenciID", ogrenciID)
            ).ToList();

            dataGridView1.DataSource = sonuc;
        }

        // SP sonucu için DTO sınıfı
        public class OgrenciDetay
        {
            public string OgrenciAdSoyad { get; set; }
            public string KulupAdi { get; set; }
            public string DersAdi { get; set; }
            public int? SINAV1 { get; set; }
            public int? SINAV2 { get; set; }
            public int? SINAV3 { get; set; }
            public decimal? ORTALAMA { get; set; }
            public string Durum { get; set; }
        }

        // ================================================================
        // SORU 32: SP ile ders başarı raporu
        // ================================================================
        /*
         * LINQ İLE YAPILABİLİRLİK DEĞERLENDİRMESİ:
         * 
         * Bu rapor LINQ ile de yapılabilir ancak:
         * 
         * AVANTAJLAR (SP):
         * - Daha performanslı (sunucu tarafında hesaplama)
         * - Karmaşık mantık tek yerde toplanır
         * - Network trafiği azalır
         * 
         * DEZAVANTAJLAR (SP):
         * - Debug edilmesi daha zor
         * - Versiyon kontrolü zorlaşır
         * - Veritabanına bağımlılık artar
         * 
         * LINQ VERSIYONU:
         */
        private void btnDersBasariLinq_Click(object sender, EventArgs e)
        {
            int dersID = Convert.ToInt32(cmbDers.SelectedValue);

            var notlar = db.TBLNOTLAR.Where(n => n.DERS == dersID);

            var rapor = new
            {
                ToplamOgrenci = notlar.Count(),
                GecenSayisi = notlar.Count(n => n.DURUM == true),
                KalanSayisi = notlar.Count(n => n.DURUM == false),
                BasariYuzdesi = notlar.Count() > 0
                    ? (double)notlar.Count(n => n.DURUM == true) / notlar.Count() * 100
                    : 0,
                EnYuksekOrtalama = notlar.Max(n => n.ORTALAMA),
                EnBasariliOgrenci = notlar
                    .OrderByDescending(n => n.ORTALAMA)
                    .Select(n => n.TBLOGRENCI.OgrenciAd + " " + n.TBLOGRENCI.OgrenciSoyad)
                    .FirstOrDefault()
            };

            MessageBox.Show($@"
                Toplam Öğrenci: {rapor.ToplamOgrenci}
                Geçen: {rapor.GecenSayisi}
                Kalan: {rapor.KalanSayisi}
                Başarı %: {rapor.BasariYuzdesi:F2}
                En Yüksek: {rapor.EnYuksekOrtalama}
                En Başarılı: {rapor.EnBasariliOgrenci}
            ");
        }

        // SP ile
        private void btnDersBasariSP_Click(object sender, EventArgs e)
        {
            int dersID = Convert.ToInt32(cmbDers.SelectedValue);

            var sonuc = db.Database.SqlQuery<DersBasariRapor>(
                "EXEC sp_DersBasariRaporu @DersID",
                new SqlParameter("@DersID", dersID)
            ).FirstOrDefault();

            if (sonuc != null)
            {
                MessageBox.Show($@"
                    Toplam Öğrenci: {sonuc.ToplamOgrenci}
                    Geçen: {sonuc.GecenSayisi}
                    Kalan: {sonuc.KalanSayisi}
                    Başarı %: {sonuc.BasariYuzdesi:F2}
                    En Yüksek: {sonuc.EnYuksekOrtalama}
                    En Başarılı: {sonuc.EnBasariliOgrenci}
                ");
            }
        }

        public class DersBasariRapor
        {
            public int ToplamOgrenci { get; set; }
            public int GecenSayisi { get; set; }
            public int KalanSayisi { get; set; }
            public decimal BasariYuzdesi { get; set; }
            public decimal EnYuksekOrtalama { get; set; }
            public string EnBasariliOgrenci { get; set; }
        }

        // ================================================================
        // SORU 33: SP ile not güncelleme
        // ================================================================
        private void btnSPNotGuncelle_Click(object sender, EventArgs e)
        {
            int notID = Convert.ToInt32(txtNotID.Text);
            int s1 = Convert.ToInt32(txtSinav1.Text);
            int s2 = Convert.ToInt32(txtSinav2.Text);
            int s3 = Convert.ToInt32(txtSinav3.Text);

            var sonuc = db.Database.SqlQuery<TBLNOTLAR>(
                "EXEC sp_NotGuncelle @NotID, @S1, @S2, @S3",
                new SqlParameter("@NotID", notID),
                new SqlParameter("@S1", s1),
                new SqlParameter("@S2", s2),
                new SqlParameter("@S3", s3)
            ).ToList();

            dataGridView1.DataSource = sonuc;
            MessageBox.Show("Not güncellendi!");
        }

        // ================================================================
        // SORU 34: Toplu silme ve kayıt sayısı
        // ================================================================
        private void btnTopluSil_Click(object sender, EventArgs e)
        {
            int kulupID = Convert.ToInt32(cmbKulup.SelectedValue);

            var sonuc = db.Database.SqlQuery<int>(
                "EXEC sp_TopluSil @KulupID",
                new SqlParameter("@KulupID", kulupID)
            ).FirstOrDefault();

            MessageBox.Show($"{sonuc} kayıt silindi!");
        }

        // ================================================================
        // SORU 35: Scalar Function Açıklaması
        // ================================================================
        /*
         * SCALAR FUNCTION EF ÜZERİNDEN ÇAĞRILAMAMASİNİN TEKNİK NEDENLERİ:
         * 
         * Entity Framework (özellikle EF6), scalar function'ları doğrudan
         * desteklemez. Bunun nedenleri:
         * 
         * 1. FUNCTION IMPORT KISITLAMASI:
         *    - EF Designer sadece Stored Procedure'leri Function Import
         *      olarak eklemeye izin verir
         *    - Scalar function'lar bu mekanizma dışındadır
         * 
         * 2. LINQ TO ENTITIES KISITLAMASI:
         *    - LINQ sorguları SQL'e çevrilirken, bilinmeyen metotlar
         *      desteklenmez
         *    - Scalar function'lar "bilinmeyen metot" olarak değerlendirilir
         * 
         * ÇÖZÜMLER:
         * 
         * a) Raw SQL Query:
         *    var sonuc = db.Database.SqlQuery<decimal>(
         *        "SELECT dbo.fn_GenelBasariPuani(@id)",
         *        new SqlParameter("@id", ogrenciID)
         *    ).FirstOrDefault();
         * 
         * b) EDMX üzerinde Model Defined Function:
         *    [EdmFunction("dbSinavOgrenciModel.Store", "fn_GenelBasariPuani")]
         *    public static decimal GenelBasariPuani(int ogrenciID) { ... }
         * 
         * c) EF Core ile (EF6'da yok):
         *    - HasDbFunction() ile mapping yapılabilir
         */
        private void btnScalarFunction_Click(object sender, EventArgs e)
        {
            int ogrenciID = Convert.ToInt32(txtOgrenciID.Text);

            var sonuc = db.Database.SqlQuery<decimal>(
                "SELECT dbo.fn_GenelBasariPuani(@OgrenciID)",
                new SqlParameter("@OgrenciID", ogrenciID)
            ).FirstOrDefault();

            MessageBox.Show($"Genel Başarı Puanı: {sonuc:F2}");
        }

        // ================================================================
        // SORU 36: Table-Valued Function çağırma
        // ================================================================
        private void btnTVFDersNot_Click(object sender, EventArgs e)
        {
            int dersID = Convert.ToInt32(cmbDers.SelectedValue);

            var sonuc = db.Database.SqlQuery<DersNotListesi>(
                "SELECT * FROM dbo.fn_DersNotListesi(@DersID)",
                new SqlParameter("@DersID", dersID)
            ).ToList();

            dataGridView1.DataSource = sonuc;
        }

        public class DersNotListesi
        {
            public string OgrenciAdSoyad { get; set; }
            public int? SINAV1 { get; set; }
            public int? SINAV2 { get; set; }
            public int? SINAV3 { get; set; }
            public decimal? ORTALAMA { get; set; }
            public string Durum { get; set; }
        }

        // ================================================================
        // SORU 37: Çok parametreli TVF ile dinamik filtreleme
        // ================================================================
        private void btnCokParametreliTVF_Click(object sender, EventArgs e)
        {
            decimal minOrt = Convert.ToDecimal(txtMinOrtalama.Text);
            decimal maxOrt = Convert.ToDecimal(txtMaxOrtalama.Text);

            // Opsiyonel parametreler
            object dersID = cmbDers.SelectedValue ?? DBNull.Value;
            object durum = chkGecti.Checked ? (object)1 : (chkKaldi.Checked ? (object)0 : DBNull.Value);

            var sonuc = db.Database.SqlQuery<NotFiltreSonuc>(
                "SELECT * FROM dbo.fn_NotFiltrele(@Min, @Max, @Ders, @Durum)",
                new SqlParameter("@Min", minOrt),
                new SqlParameter("@Max", maxOrt),
                new SqlParameter("@Ders", dersID),
                new SqlParameter("@Durum", durum)
            ).ToList();

            dataGridView1.DataSource = sonuc;
        }

        public class NotFiltreSonuc
        {
            public string OgrenciAdSoyad { get; set; }
            public string DersAdi { get; set; }
            public decimal? ORTALAMA { get; set; }
            public string Durum { get; set; }
        }

        // ================================================================
        // SORU 38: Kulüp başarı oranı - Dashboard gösterimi
        // ================================================================
        private void btnKulupDashboard_Click(object sender, EventArgs e)
        {
            var sonuc = db.Database.SqlQuery<KulupBasari>(
                "SELECT * FROM dbo.fn_KulupBasariOrani()"
            ).ToList();

            dataGridView1.DataSource = sonuc;

            // Panel üzerinde görsel gösterim
            foreach (var kulup in sonuc)
            {
                // Her kulüp için bir progress bar veya gauge eklenebilir
                // lblKulupAdi.Text = kulup.KulupAdi;
                // progressBar.Value = (int)kulup.BasariOrani;
            }
        }

        public class KulupBasari
        {
            public string KulupAdi { get; set; }
            public int UyeSayisi { get; set; }
            public decimal? OrtalamaBasari { get; set; }
            public decimal? BasariOrani { get; set; }
        }

        // ================================================================
        // SORU 39: Inline TVF - Zayıf öğrenciler
        // ================================================================
        /*
         * INLINE TVF PERFORMANS AVANTAJI:
         * 
         * Inline TVF'ler (RETURNS TABLE AS RETURN SELECT...):
         * 
         * 1. SORGU OPTİMİZASYONU:
         *    - SQL Server, inline TVF'yi ana sorguya "inline" eder
         *    - Tüm sorgu tek bir execution plan ile çalışır
         *    - Multi-statement TVF'de ise geçici tablo kullanılır
         * 
         * 2. PERFORMANS:
         *    - Ara tablo oluşturulmaz
         *    - Index'ler doğrudan kullanılabilir
         *    - İstatistikler daha doğru tahminde bulunur
         * 
         * 3. PARAMETRE KOKLAMASİ:
         *    - Parametreler doğrudan sorguya enjekte edilir
         *    - Daha iyi execution plan oluşur
         */
        private void btnZayifOgrenciler_Click(object sender, EventArgs e)
        {
            var sonuc = db.Database.SqlQuery<ZayifOgrenci>(
                "SELECT * FROM dbo.fn_ZayifOgrenciler()"
            ).ToList();

            dataGridView1.DataSource = sonuc;
        }

        public class ZayifOgrenci
        {
            public string OgrenciAdSoyad { get; set; }
            public int? SINAV1 { get; set; }
            public decimal GenelOrtalama { get; set; }
            public decimal EsikDeger { get; set; }
        }

        // ================================================================
        // SORU 40: Function çıkışını DTO'ya map etme
        // ================================================================
        private void btnOgrenciNotOzet_Click(object sender, EventArgs e)
        {
            int ogrenciID = Convert.ToInt32(txtOgrenciID.Text);

            var sonuc = db.Database.SqlQuery<OgrenciNotOzetDTO>(
                "SELECT * FROM dbo.fn_OgrenciNotOzet(@OgrenciID)",
                new SqlParameter("@OgrenciID", ogrenciID)
            ).ToList();

            dataGridView1.DataSource = sonuc;

            if (sonuc.Any())
            {
                var ozet = sonuc.First();
                lblToplamDers.Text = $"Toplam Ders: {ozet.ToplamDers}";
                lblGecenDers.Text = $"Geçen: {ozet.GecenDers}";
                lblKalanDers.Text = $"Kalan: {ozet.KalanDers}";
                lblGenelOrt.Text = $"Genel Ort: {ozet.GenelOrt:F2}";
            }
        }

        // ================================================================
        // SORU 41: Transaction - Çok adımlı öğrenci kayıt süreci
        // ================================================================
        /*
         * SAVECHANGES KONUMU TARTIŞMASI:
         * 
         * YÖNTEMLERİN KARŞILAŞTIRILMASI:
         * 
         * 1. HER ADIMDA SAVECHANGES:
         *    - Her işlem anında veritabanına yazılır
         *    - Hata durumunda kısmi veri kalabilir
         *    - Transaction ile sarmalanmazsa tutarsızlık oluşur
         * 
         * 2. EN SONDA TEK SAVECHANGES (ÖNERİLEN):
         *    - Tüm değişiklikler bellekte birikir
         *    - Tek seferde veritabanına yazılır
         *    - EF otomatik transaction yönetimi yapar
         *    - Ya hepsi ya hiçbiri garantisi
         * 
         * NOT: Aşağıdaki örnekte SaveChanges en sonda çağrılıyor.
         */
        private void btnCokAdimliKayit_Click(object sender, EventArgs e)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // ADIM 1: Yeni öğrenci ekle
                    var yeniOgrenci = new TBLOGRENCI
                    {
                        OgrenciAd = txtOgrenciAd.Text,
                        OgrenciSoyad = txtOgrenciSoyad.Text,
                        KulupID = Convert.ToInt32(cmbKulup.SelectedValue)
                    };
                    db.TBLOGRENCI.Add(yeniOgrenci);
                    db.SaveChanges(); // ID almak için gerekli

                    // Doğrulama
                    if (yeniOgrenci.OgrenciID <= 0)
                        throw new Exception("Öğrenci eklenemedi!");

                    // ADIM 2: Üç farklı ders için kayıt oluştur
                    var dersler = db.TBLDERSLER.Take(3).ToList();
                    if (dersler.Count < 3)
                        throw new Exception("Yeterli ders bulunamadı!");

                    // ADIM 3: Her ders için not kaydı oluştur
                    foreach (var ders in dersler)
                    {
                        var yeniNot = new TBLNOTLAR
                        {
                            OGRENCI = yeniOgrenci.OgrenciID,
                            DERS = ders.DERSID,
                            SINAV1 = 0,
                            SINAV2 = 0,
                            SINAV3 = 0,
                            ORTALAMA = 0,
                            DURUM = false
                        };
                        db.TBLNOTLAR.Add(yeniNot);
                    }

                    // ADIM 4: Son doğrulama
                    db.SaveChanges();

                    // ADIM 5: Başarılıysa commit
                    transaction.Commit();
                    MessageBox.Show("Öğrenci ve notlar başarıyla eklendi!");
                }
                catch (Exception ex)
                {
                    // Hata olursa rollback
                    transaction.Rollback();
                    MessageBox.Show($"Hata oluştu, işlem geri alındı: {ex.Message}");
                }
            }
        }

        // ================================================================
        // SORU 42: TransactionScope ile toplu not güncelleme
        // ================================================================
        /*
         * TRANSACTIONSCOPE Options FARKLARI:
         * 
         * TransactionScopeOption.Required (Varsayılan):
         * - Mevcut bir transaction varsa ona katılır
         * - Yoksa yeni transaction başlatır
         * - İç içe kullanımda dış transaction'a bağlıdır
         * 
         * TransactionScopeOption.RequiresNew:
         * - Her zaman yeni bir transaction başlatır
         * - Dış transaction'dan bağımsız çalışır
         * - İç transaction başarısız olsa bile dış devam edebilir
         * 
         * TransactionScopeOption.Suppress:
         * - Transaction olmadan çalışır
         * - Mevcut transaction'ı askıya alır
         */
        private void btnTopluNotGuncelle_Click(object sender, EventArgs e)
        {
            // 10 öğrencinin notlarını toplu güncelleme
            var guncellenecekNotlar = db.TBLNOTLAR.Take(10).ToList();

            using (var scope = new TransactionScope())
            {
                try
                {
                    foreach (var not in guncellenecekNotlar)
                    {
                        // Her not için güncelleme
                        not.SINAV1 = (not.SINAV1 ?? 0) + 5; // Bonus puan
                        if (not.SINAV1 > 100) not.SINAV1 = 100;

                        // Ortalama yeniden hesapla
                        not.ORTALAMA = (not.SINAV1 + not.SINAV2 + not.SINAV3) / 3.0m;
                        not.DURUM = not.ORTALAMA >= 50;
                    }

                    db.SaveChanges();
                    scope.Complete(); // Transaction başarılı

                    MessageBox.Show($"{guncellenecekNotlar.Count} not güncellendi!");
                }
                catch (Exception ex)
                {
                    // scope.Complete() çağrılmadığı için otomatik rollback
                    MessageBox.Show($"Hata: {ex.Message}\nHiçbir kayıt güncellenmedi.");
                }
            }
        }

        // RequiresNew örneği
        private void btnRequiresNewDemo_Click(object sender, EventArgs e)
        {
            using (var outerScope = new TransactionScope())
            {
                // Dış işlemler...
                db.TBLOGRENCI.First().OgrenciAd = "Test";

                using (var innerScope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    // Bu transaction bağımsız çalışır
                    // İç işlemler...
                    innerScope.Complete();
                }

                outerScope.Complete();
            }
        }

        // ================================================================
        // SORU 43: Çok tablolu transaction işlemi
        // ================================================================
        private void btnCokTabloTransaction_Click(object sender, EventArgs e)
        {
            int ogrenciID = Convert.ToInt32(txtOgrenciID.Text);

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Öğrenciyi bul
                    var ogrenci = db.TBLOGRENCI.Find(ogrenciID);
                    if (ogrenci == null)
                        throw new Exception("Öğrenci bulunamadı!");

                    int kulupID = ogrenci.KulupID ?? 0;

                    // ADIM 1: TBLNOTLAR'dan 3 not sil
                    var silinecekNotlar = db.TBLNOTLAR
                        .Where(n => n.OGRENCI == ogrenciID)
                        .Take(3)
                        .ToList();

                    if (silinecekNotlar.Count > 0)
                    {
                        db.TBLNOTLAR.RemoveRange(silinecekNotlar);
                        db.SaveChanges();
                    }

                    // ADIM 2: Öğrencinin kalan notlarını sil
                    var kalanNotlar = db.TBLNOTLAR
                        .Where(n => n.OGRENCI == ogrenciID)
                        .ToList();
                    db.TBLNOTLAR.RemoveRange(kalanNotlar);

                    // ADIM 3: Öğrenciyi sil
                    db.TBLOGRENCI.Remove(ogrenci);
                    db.SaveChanges();

                    // Not: Kulüp üye sayısı TBLKULUPLER'da ayrı bir sütun olarak
                    // tutulmadığından, bu adım atlndı. Eğer bir UyeSayisi sütunu
                    // olsaydı: kulup.UyeSayisi--;

                    transaction.Commit();
                    MessageBox.Show("İşlem başarıyla tamamlandı!");
                    OgrenciListesiGuncelle();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Hata: {ex.Message}\nTüm işlemler geri alındı.");
                }
            }
        }

        // ================================================================
        // SORU 44: Transaction + SP birlikte kullanımı
        // ================================================================
        private void btnTransactionSP_Click(object sender, EventArgs e)
        {
            int ogrenciID = Convert.ToInt32(txtOgrenciID.Text);

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // SP çağır - transaction içinde
                    db.Database.ExecuteSqlCommand(
                        "EXEC sp_OgrenciTamSil @OgrenciID",
                        new SqlParameter("@OgrenciID", ogrenciID)
                    );

                    // Başarılıysa commit
                    transaction.Commit();
                    MessageBox.Show("Öğrenci ve tüm kayıtları silindi!");
                    OgrenciListesiGuncelle();
                }
                catch (Exception ex)
                {
                    // SP hata verirse rollback
                    transaction.Rollback();
                    MessageBox.Show($"SP Hatası: {ex.Message}\nİşlem geri alındı.");
                }
            }
        }

        // ================================================================
        // SORU 45: Transaction + Validation + File Check
        // ================================================================
        /*
         * BU SENARYONUN AVANTAJLARI:
         * 
         * EF TARAFI:
         * - Transaction ile atomik işlem garantisi
         * - Dosya kontrolü başarısız olursa DB değişmez
         * - Nesne tabanlı, tip güvenli kod
         * 
         * SQL TARAFI:
         * - Stored Procedure ile de yapılabilir
         * - Ancak dosya kontrolü SQL'de zor (xp_fileexist güvenlik sorunu)
         * - Uygulama katmanında kontrol daha güvenli
         * 
         * EN İYİ YAKLAŞIM:
         * - Dosya kontrolü: Uygulama katmanı
         * - Veritabanı işlemleri: EF + Transaction
         * - İki katman arasında tutarlılık: Transaction scope
         */
        private void btnFotoGuncelle_Click(object sender, EventArgs e)
        {
            int ogrenciID = Convert.ToInt32(txtOgrenciID.Text);
            string yeniFotoYolu = txtFotoYol.Text;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // ADIM 1: Dosya var mı kontrol et
                    if (!string.IsNullOrEmpty(yeniFotoYolu))
                    {
                        if (!File.Exists(yeniFotoYolu))
                        {
                            throw new FileNotFoundException(
                                "Belirtilen fotoğraf dosyası bulunamadı!",
                                yeniFotoYolu
                            );
                        }
                    }

                    // ADIM 2: Öğrenciyi bul
                    var ogrenci = db.TBLOGRENCI.Find(ogrenciID);
                    if (ogrenci == null)
                        throw new Exception("Öğrenci bulunamadı!");

                    // ADIM 3: Fotoğraf yolunu güncelle
                    ogrenci.OgrenciFoto = yeniFotoYolu;
                    db.SaveChanges();

                    // ADIM 4: Başarılıysa commit
                    transaction.Commit();
                    MessageBox.Show("Fotoğraf başarıyla güncellendi!");
                }
                catch (FileNotFoundException ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Dosya bulunamadı: {ex.FileName}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Hata: {ex.Message}");
                }
            }
        }

        // ================================================================
        // Form Controls (Tasarım için referans)
        // ================================================================
        /*
         * FORM ÜZERİNDEKİ KONTROLLER:
         * 
         * TextBox'lar:
         * - txtOgrenciID
         * - txtOgrenciAd
         * - txtOgrenciSoyad
         * - txtFotoYol
         * - txtSinav1, txtSinav2, txtSinav3
         * - txtNotID
         * - txtMinOrtalama, txtMaxOrtalama
         * 
         * ComboBox'lar:
         * - cmbKulup (Kulüp seçimi)
         * - cmbDers (Ders seçimi)
         * 
         * DataGridView'ler:
         * - dataGridView1 (Ana liste)
         * - dgvGecenler (Geçen öğrenciler)
         * - dgvKalanlar (Kalan öğrenciler)
         * 
         * CheckBox'lar:
         * - chkGecti
         * - chkKaldi
         * 
         * Label'lar:
         * - lblToplamDers
         * - lblGecenDers
         * - lblKalanDers
         * - lblGenelOrt
         * 
         * Button'lar:
         * - Her soru için ilgili buton
         */
    }

    // ================================================================
    // DTO Sınıfları (Soru 40 için)
    // ================================================================
    public class OgrenciNotOzetDTO
    {
        public int ToplamDers { get; set; }
        public int GecenDers { get; set; }
        public int KalanDers { get; set; }
        public decimal? GenelOrt { get; set; }
        public int? EnYuksekNot { get; set; }
        public int? EnDusukNot { get; set; }
    }
}
