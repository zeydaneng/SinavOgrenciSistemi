-- ================================================================
-- ENTITY FRAMEWORK ÖDEV - VERİTABANI OLUŞTURMA SCRİPTİ
-- Veritabanı: dbSinavOgrenci
-- ================================================================

-- Veritabanını Oluşturma
CREATE DATABASE dbSinavOgrenci;
GO

USE dbSinavOgrenci;
GO

-- ================================================================
-- TABLOLARIN OLUŞTURULMASI
-- ================================================================

-- 1. TBLKULUPLER - Kulüp Tablosu
CREATE TABLE TBLKULUPLER (
    KULUPID   INT IDENTITY(1,1) PRIMARY KEY,
    KULUPAD   NVARCHAR(50) NOT NULL
);
GO

-- 2. TBLDERSLER - Ders Tablosu
CREATE TABLE TBLDERSLER (
    DERSID   INT IDENTITY(1,1) PRIMARY KEY,
    DERSAD   NVARCHAR(50) NOT NULL
);
GO

-- 3. TBLOGRENCI - Öğrenci Tablosu
CREATE TABLE TBLOGRENCI (
    OgrenciID    INT IDENTITY(1,1) PRIMARY KEY,
    OgrenciAd    NVARCHAR(50) NOT NULL,
    OgrenciSoyad NVARCHAR(50) NOT NULL,
    OgrenciFoto  NVARCHAR(250) NULL,
    KulupID      INT NULL,

    CONSTRAINT FK_TBLOGRENCI_KULUP
        FOREIGN KEY (KulupID) REFERENCES TBLKULUPLER(KULUPID)
);
GO

-- 4. TBLNOTLAR - Not Tablosu
CREATE TABLE TBLNOTLAR (
    NOTID     INT IDENTITY(1,1) PRIMARY KEY,
    DERS      INT NOT NULL,
    OGRENCI   INT NOT NULL,
    SINAV1    INT NULL,
    SINAV2    INT NULL,
    SINAV3    INT NULL,
    ORTALAMA  DECIMAL(5,2) NULL,
    DURUM     BIT NULL,

    CONSTRAINT FK_TBLNOTLAR_DERS
        FOREIGN KEY (DERS) REFERENCES TBLDERSLER(DERSID),

    CONSTRAINT FK_TBLNOTLAR_OGRENCI
        FOREIGN KEY (OGRENCI) REFERENCES TBLOGRENCI(OgrenciID)
);
GO

-- ================================================================
-- ÖRNEK VERİLER
-- ================================================================

-- Kulüpler
INSERT INTO TBLKULUPLER (KULUPAD) VALUES
(N'Bilgisayar Topluluğu'),
(N'Robotik Kulübü'),
(N'Siber Güvenlik Kulübü'),
(N'Yazılım Geliştirme Kulübü'),
(N'Yapay Zeka Topluluğu');
GO

-- Dersler
INSERT INTO TBLDERSLER (DERSAD) VALUES
(N'Veri Tabanı'),
(N'Programlama'),
(N'Ağ Teknolojileri'),
(N'Web Tasarım'),
(N'Mobil Uygulama');
GO

-- Öğrenciler
INSERT INTO TBLOGRENCI (OgrenciAd, OgrenciSoyad, OgrenciFoto, KulupID) VALUES
(N'Ali', N'Yılmaz', NULL, 1),
(N'Ayşe', N'Kaya', NULL, 2),
(N'Mehmet', N'Demir', NULL, 3),
(N'Fatma', N'Çelik', NULL, 1),
(N'Ahmet', N'Şahin', NULL, 2),
(N'Zeynep', N'Arslan', NULL, 4),
(N'Mustafa', N'Özkan', NULL, 5),
(N'Elif', N'Yıldırım', NULL, 3),
(N'Hasan', N'Aydın', NULL, 4),
(N'Merve', N'Koç', NULL, 5);
GO

-- Notlar
INSERT INTO TBLNOTLAR (DERS, OGRENCI, SINAV1, SINAV2, SINAV3, ORTALAMA, DURUM) VALUES
(1, 1, 70, 80, 90, 80.00, 1),
(1, 2, 40, 50, 45, 45.00, 0),
(2, 3, 85, 90, 88, 87.67, 1),
(1, 3, 75, 80, 85, 80.00, 1),
(2, 1, 60, 55, 65, 60.00, 1),
(3, 2, 35, 40, 38, 37.67, 0),
(2, 4, 90, 95, 92, 92.33, 1),
(3, 5, 55, 60, 58, 57.67, 1),
(4, 6, 80, 85, 82, 82.33, 1),
(5, 7, 45, 40, 42, 42.33, 0),
(1, 8, 70, 75, 72, 72.33, 1),
(2, 9, 65, 70, 68, 67.67, 1),
(3, 10, 50, 45, 48, 47.67, 0),
(4, 1, 88, 92, 90, 90.00, 1),
(5, 2, 30, 35, 32, 32.33, 0);
GO

-- ================================================================
-- STORED PROCEDURES
-- ================================================================

-- SORU 30: sp_NotEkleKontrollu - Tekrar kayıt kontrolü ile not ekleme
CREATE PROCEDURE sp_NotEkleKontrollu
    @OgrenciID INT,
    @DersID INT,
    @Sinav1 INT,
    @Sinav2 INT,
    @Sinav3 INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Aynı öğrenci-ders kaydı var mı kontrol et
    IF EXISTS (SELECT 1 FROM TBLNOTLAR WHERE OGRENCI = @OgrenciID AND DERS = @DersID)
    BEGIN
        RAISERROR('Bu öğrenci bu derse zaten kayıtlı!', 16, 1);
        RETURN;
    END
    
    -- Ortalama ve durum hesapla
    DECLARE @Ortalama DECIMAL(5,2);
    DECLARE @Durum BIT;
    
    SET @Ortalama = (@Sinav1 + @Sinav2 + @Sinav3) / 3.0;
    SET @Durum = CASE WHEN @Ortalama >= 50 THEN 1 ELSE 0 END;
    
    -- Kayıt ekle
    INSERT INTO TBLNOTLAR (DERS, OGRENCI, SINAV1, SINAV2, SINAV3, ORTALAMA, DURUM)
    VALUES (@DersID, @OgrenciID, @Sinav1, @Sinav2, @Sinav3, @Ortalama, @Durum);
    
    SELECT 'Kayıt başarıyla eklendi.' AS Mesaj;
END
GO

-- SORU 31: sp_OgrenciDetayGetir - Öğrenci detay raporu
CREATE PROCEDURE sp_OgrenciDetayGetir
    @OgrenciID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        o.OgrenciAd + ' ' + o.OgrenciSoyad AS OgrenciAdSoyad,
        k.KULUPAD AS KulupAdi,
        d.DERSAD AS DersAdi,
        n.SINAV1,
        n.SINAV2,
        n.SINAV3,
        n.ORTALAMA,
        CASE WHEN n.DURUM = 1 THEN 'GEÇTİ' ELSE 'KALDI' END AS Durum
    FROM TBLOGRENCI o
    LEFT JOIN TBLKULUPLER k ON o.KulupID = k.KULUPID
    LEFT JOIN TBLNOTLAR n ON o.OgrenciID = n.OGRENCI
    LEFT JOIN TBLDERSLER d ON n.DERS = d.DERSID
    WHERE o.OgrenciID = @OgrenciID;
END
GO

-- SORU 32: sp_DersBasariRaporu - Ders bazında başarı analizi
CREATE PROCEDURE sp_DersBasariRaporu
    @DersID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ToplamOgrenci INT;
    DECLARE @GecenSayisi INT;
    DECLARE @KalanSayisi INT;
    DECLARE @BasariYuzdesi DECIMAL(5,2);
    DECLARE @EnYuksekOrtalama DECIMAL(5,2);
    DECLARE @EnBasariliOgrenci NVARCHAR(100);
    
    -- Toplam öğrenci sayısı
    SELECT @ToplamOgrenci = COUNT(*) FROM TBLNOTLAR WHERE DERS = @DersID;
    
    -- Geçen ve kalan sayıları
    SELECT @GecenSayisi = COUNT(*) FROM TBLNOTLAR WHERE DERS = @DersID AND DURUM = 1;
    SELECT @KalanSayisi = COUNT(*) FROM TBLNOTLAR WHERE DERS = @DersID AND DURUM = 0;
    
    -- Başarı yüzdesi
    SET @BasariYuzdesi = CASE WHEN @ToplamOgrenci > 0 
        THEN CAST(@GecenSayisi AS DECIMAL(5,2)) / @ToplamOgrenci * 100 
        ELSE 0 END;
    
    -- En yüksek ortalama
    SELECT @EnYuksekOrtalama = MAX(ORTALAMA) FROM TBLNOTLAR WHERE DERS = @DersID;
    
    -- En başarılı öğrenci
    SELECT TOP 1 @EnBasariliOgrenci = o.OgrenciAd + ' ' + o.OgrenciSoyad
    FROM TBLNOTLAR n
    INNER JOIN TBLOGRENCI o ON n.OGRENCI = o.OgrenciID
    WHERE n.DERS = @DersID AND n.ORTALAMA = @EnYuksekOrtalama;
    
    SELECT 
        @ToplamOgrenci AS ToplamOgrenci,
        @GecenSayisi AS GecenSayisi,
        @KalanSayisi AS KalanSayisi,
        @BasariYuzdesi AS BasariYuzdesi,
        @EnYuksekOrtalama AS EnYuksekOrtalama,
        @EnBasariliOgrenci AS EnBasariliOgrenci;
END
GO

-- SORU 33: sp_NotGuncelle - Not güncelleme
CREATE PROCEDURE sp_NotGuncelle
    @NotID INT,
    @S1 INT,
    @S2 INT,
    @S3 INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Ortalama DECIMAL(5,2);
    DECLARE @Durum BIT;
    
    SET @Ortalama = (@S1 + @S2 + @S3) / 3.0;
    SET @Durum = CASE WHEN @Ortalama >= 50 THEN 1 ELSE 0 END;
    
    UPDATE TBLNOTLAR
    SET SINAV1 = @S1,
        SINAV2 = @S2,
        SINAV3 = @S3,
        ORTALAMA = @Ortalama,
        DURUM = @Durum
    WHERE NOTID = @NotID;
    
    SELECT * FROM TBLNOTLAR WHERE NOTID = @NotID;
END
GO

-- SORU 34: sp_TopluSil - Kulüp bazlı toplu silme
CREATE PROCEDURE sp_TopluSil
    @KulupID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SilinenKayit INT = 0;
    
    -- Kulüpteki öğrencilerin notlarını sil
    DELETE n
    FROM TBLNOTLAR n
    INNER JOIN TBLOGRENCI o ON n.OGRENCI = o.OgrenciID
    WHERE o.KulupID = @KulupID;
    
    SET @SilinenKayit = @@ROWCOUNT;
    
    SELECT @SilinenKayit AS SilinenKayitSayisi;
END
GO

-- SORU 44: sp_OgrenciTamSil - Transaction ile öğrenci silme
CREATE PROCEDURE sp_OgrenciTamSil
    @OgrenciID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Öğrencinin kulübünü al
        DECLARE @KulupID INT;
        SELECT @KulupID = KulupID FROM TBLOGRENCI WHERE OgrenciID = @OgrenciID;
        
        -- Öğrencinin notlarını sil
        DELETE FROM TBLNOTLAR WHERE OGRENCI = @OgrenciID;
        
        -- Öğrenciyi sil
        DELETE FROM TBLOGRENCI WHERE OgrenciID = @OgrenciID;
        
        COMMIT TRANSACTION;
        SELECT 'İşlem başarılı' AS Sonuc;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ================================================================
-- SCALAR FUNCTIONS
-- ================================================================

-- SORU 35: fn_GenelBasariPuani - Öğrencinin genel başarı puanı
CREATE FUNCTION fn_GenelBasariPuani(@OgrenciID INT)
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @GenelOrtalama DECIMAL(5,2);
    
    SELECT @GenelOrtalama = AVG(ORTALAMA)
    FROM TBLNOTLAR
    WHERE OGRENCI = @OgrenciID;
    
    RETURN ISNULL(@GenelOrtalama, 0);
END
GO

-- ================================================================
-- TABLE-VALUED FUNCTIONS
-- ================================================================

-- SORU 36: fn_DersNotListesi - Ders bazlı not listesi
CREATE FUNCTION fn_DersNotListesi(@DersID INT)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        o.OgrenciAd + ' ' + o.OgrenciSoyad AS OgrenciAdSoyad,
        n.SINAV1,
        n.SINAV2,
        n.SINAV3,
        n.ORTALAMA,
        CASE WHEN n.DURUM = 1 THEN 'GEÇTİ' ELSE 'KALDI' END AS Durum
    FROM TBLNOTLAR n
    INNER JOIN TBLOGRENCI o ON n.OGRENCI = o.OgrenciID
    WHERE n.DERS = @DersID
);
GO

-- SORU 37: fn_NotFiltrele - Çok parametreli TVF
CREATE FUNCTION fn_NotFiltrele(
    @MinOrtalama DECIMAL(5,2),
    @MaxOrtalama DECIMAL(5,2),
    @DersID INT = NULL,
    @Durum BIT = NULL
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        o.OgrenciAd + ' ' + o.OgrenciSoyad AS OgrenciAdSoyad,
        d.DERSAD AS DersAdi,
        n.ORTALAMA,
        CASE WHEN n.DURUM = 1 THEN 'GEÇTİ' ELSE 'KALDI' END AS Durum
    FROM TBLNOTLAR n
    INNER JOIN TBLOGRENCI o ON n.OGRENCI = o.OgrenciID
    INNER JOIN TBLDERSLER d ON n.DERS = d.DERSID
    WHERE n.ORTALAMA BETWEEN @MinOrtalama AND @MaxOrtalama
        AND (@DersID IS NULL OR n.DERS = @DersID)
        AND (@Durum IS NULL OR n.DURUM = @Durum)
);
GO

-- SORU 38: fn_KulupBasariOrani - Kulüp başarı analizi
CREATE FUNCTION fn_KulupBasariOrani()
RETURNS TABLE
AS
RETURN
(
    SELECT 
        k.KULUPAD AS KulupAdi,
        COUNT(DISTINCT o.OgrenciID) AS UyeSayisi,
        AVG(n.ORTALAMA) AS OrtalamaBasari,
        CAST(SUM(CASE WHEN n.DURUM = 1 THEN 1.0 ELSE 0 END) / 
             NULLIF(COUNT(*), 0) * 100 AS DECIMAL(5,2)) AS BasariOrani
    FROM TBLKULUPLER k
    LEFT JOIN TBLOGRENCI o ON k.KULUPID = o.KulupID
    LEFT JOIN TBLNOTLAR n ON o.OgrenciID = n.OGRENCI
    GROUP BY k.KULUPID, k.KULUPAD
);
GO

-- SORU 39: fn_ZayifOgrenciler - Zayıf başarı gösteren öğrenciler (Inline TVF)
CREATE FUNCTION fn_ZayifOgrenciler()
RETURNS TABLE
AS
RETURN
(
    SELECT 
        o.OgrenciAd + ' ' + o.OgrenciSoyad AS OgrenciAdSoyad,
        n.SINAV1,
        (SELECT AVG(CAST(SINAV1 AS DECIMAL(5,2))) FROM TBLNOTLAR) AS GenelOrtalama,
        (SELECT AVG(CAST(SINAV1 AS DECIMAL(5,2))) FROM TBLNOTLAR) * 0.8 AS EsikDeger
    FROM TBLNOTLAR n
    INNER JOIN TBLOGRENCI o ON n.OGRENCI = o.OgrenciID
    WHERE n.SINAV1 < (SELECT AVG(CAST(SINAV1 AS DECIMAL(5,2))) FROM TBLNOTLAR) * 0.8
);
GO

-- SORU 40: fn_OgrenciNotOzet - Öğrenci not özeti
CREATE FUNCTION fn_OgrenciNotOzet(@OgrenciID INT)
RETURNS @Result TABLE (
    ToplamDers INT,
    GecenDers INT,
    KalanDers INT,
    GenelOrt DECIMAL(5,2),
    EnYuksekNot INT,
    EnDusukNot INT
)
AS
BEGIN
    INSERT INTO @Result
    SELECT 
        COUNT(*) AS ToplamDers,
        SUM(CASE WHEN DURUM = 1 THEN 1 ELSE 0 END) AS GecenDers,
        SUM(CASE WHEN DURUM = 0 THEN 1 ELSE 0 END) AS KalanDers,
        AVG(ORTALAMA) AS GenelOrt,
        MAX(GREATEST(SINAV1, SINAV2, SINAV3)) AS EnYuksekNot,
        MIN(LEAST(SINAV1, SINAV2, SINAV3)) AS EnDusukNot
    FROM TBLNOTLAR
    WHERE OGRENCI = @OgrenciID;
    
    RETURN;
END
GO

PRINT 'Veritabanı ve tüm nesneler başarıyla oluşturuldu!';
GO
