namespace DuAn.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Security.Cryptography;

    public partial class Model1 : DbContext
    {
        public static class AccountDAO
        {
            public static string MaHoaMatKhau(String password)
            {
                //Tạo MD5 
                MD5 mh = MD5.Create();
                //Chuyển kiểu chuổi thành kiểu byte
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
                //mã hóa chuỗi đã chuyển
                byte[] hash = mh.ComputeHash(inputBytes);
                //tạo đối tượng StringBuilder (làm việc với kiểu dữ liệu lớn)
                String sb = "";
                for (int i = 0; i < hash.Length; i++)
                {
                    sb += hash[i].ToString("x");
                }
                return sb;
            }

            private static string RandomSaltHash()
            {
                string rs = "";
                Random rd = new Random();
                for (int i = 0; i < 20; i++)
                {
                    rs += Convert.ToString((Char)rd.Next(65, 90));
                }
                return rs;
            }
            static Model1 db = new Model1();
            public static Account CheckLogin(string username, string password)
            {
                try
                {
                    var rs = db.Accounts.SingleOrDefault(x => x.Username == username);
                    if (rs != null && rs.Password == MaHoaMatKhau(rs.SaltPassword + password))
                    {
                        return rs;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return null;
            }
            public static void AddAccount(Account acc)
            {
                acc.SaltPassword = RandomSaltHash();
                // ma hoa mat khau
                acc.Password = MaHoaMatKhau(acc.SaltPassword + acc.Password);
                db.Accounts.Add(acc);
                db.SaveChanges();
            }
            public static bool CheckUsername(string username)
            {
                foreach (Account c in db.Accounts)
                {
                    if (c.Username == username)
                    {
                        return false;
                    }
                }
                return true;
            }

        }

        public Model1()
            : base("name=Model12")
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<CongThucTongSanLuong> CongThucTongSanLuongs { get; set; }
        public virtual DbSet<CongTo> CongToes { get; set; }
        public virtual DbSet<CongTy> CongTies { get; set; }
        public virtual DbSet<DiemDo> DiemDoes { get; set; }
        public virtual DbSet<DiemDo_CongTo> DiemDo_CongTo { get; set; }
        public virtual DbSet<Kenh> Kenhs { get; set; }
        public virtual DbSet<LoaiSanLuong> LoaiSanLuongs { get; set; }
        public virtual DbSet<LogCongTy> LogCongTies { get; set; }
        public virtual DbSet<LogDiemDo> LogDiemDoes { get; set; }
        public virtual DbSet<LogKenh> LogKenhs { get; set; }
        public virtual DbSet<LogNhaMay> LogNhaMays { get; set; }
        public virtual DbSet<LogTinhChatDiemDo> LogTinhChatDiemDoes { get; set; }
        public virtual DbSet<NhaMay> NhaMays { get; set; }
        public virtual DbSet<RoleAccount> RoleAccounts { get; set; }
        public virtual DbSet<SanLuong> SanLuongs { get; set; }
        public virtual DbSet<SanLuongDuKien> SanLuongDuKiens { get; set; }
        public virtual DbSet<SanLuongThucTe> SanLuongThucTes { get; set; }
        public virtual DbSet<TinhChatDiemDo> TinhChatDiemDoes { get; set; }
        public virtual DbSet<TongSanLuong_Ngay> TongSanLuong_Ngay { get; set; }
        public virtual DbSet<TongSanLuong_ThangNam> TongSanLuong_ThangNam { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.SaltPassword)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.IdentifyCode)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<CongThucTongSanLuong>()
                .HasMany(e => e.TongSanLuong_Ngay)
                .WithRequired(e => e.CongThucTongSanLuong)
                .HasForeignKey(e => e.CongThucID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CongTo>()
                .Property(e => e.Serial)
                .IsFixedLength();

            modelBuilder.Entity<CongTo>()
                .HasMany(e => e.DiemDo_CongTo)
                .WithRequired(e => e.CongTo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CongTy>()
                .Property(e => e.TenVietTat)
                .IsFixedLength();

            modelBuilder.Entity<CongTy>()
                .HasMany(e => e.LogCongTies)
                .WithRequired(e => e.CongTy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CongTy>()
                .HasMany(e => e.NhaMays)
                .WithRequired(e => e.CongTy)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DiemDo>()
                .Property(e => e.TenDiemDo)
                .IsFixedLength();

            modelBuilder.Entity<DiemDo>()
                .HasMany(e => e.DiemDo_CongTo)
                .WithRequired(e => e.DiemDo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DiemDo>()
                .HasMany(e => e.LogDiemDoes)
                .WithRequired(e => e.DiemDo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DiemDo>()
                .HasMany(e => e.SanLuongs)
                .WithRequired(e => e.DiemDo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DiemDo>()
                .HasMany(e => e.SanLuongThucTes)
                .WithRequired(e => e.DiemDo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Kenh>()
                .Property(e => e.Ten)
                .IsUnicode(false);

            modelBuilder.Entity<Kenh>()
                .HasMany(e => e.LogKenhs)
                .WithRequired(e => e.Kenh)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Kenh>()
                .HasMany(e => e.SanLuongs)
                .WithRequired(e => e.Kenh)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Kenh>()
                .HasMany(e => e.SanLuongThucTes)
                .WithRequired(e => e.Kenh)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LoaiSanLuong>()
                .Property(e => e.Loai)
                .IsUnicode(false);

            modelBuilder.Entity<LoaiSanLuong>()
                .HasMany(e => e.SanLuongDuKiens)
                .WithRequired(e => e.LoaiSanLuong)
                .HasForeignKey(e => e.LoaiID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LoaiSanLuong>()
                .HasMany(e => e.TongSanLuong_ThangNam)
                .WithRequired(e => e.LoaiSanLuong)
                .HasForeignKey(e => e.LoaiID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LogCongTy>()
                .Property(e => e.TenVietTat)
                .IsFixedLength();

            modelBuilder.Entity<LogDiemDo>()
                .Property(e => e.TenDiemDo)
                .IsFixedLength();

            modelBuilder.Entity<LogDiemDo>()
                .Property(e => e.NhaMayID)
                .IsFixedLength();

            modelBuilder.Entity<LogKenh>()
                .Property(e => e.Ten)
                .IsUnicode(false);

            modelBuilder.Entity<LogNhaMay>()
                .Property(e => e.TenVietTat)
                .IsFixedLength();

            modelBuilder.Entity<NhaMay>()
                .Property(e => e.TenVietTat)
                .IsFixedLength();

            modelBuilder.Entity<NhaMay>()
                .HasMany(e => e.DiemDoes)
                .WithRequired(e => e.NhaMay)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NhaMay>()
                .HasMany(e => e.LogNhaMays)
                .WithRequired(e => e.NhaMay)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RoleAccount>()
                .HasMany(e => e.Accounts)
                .WithRequired(e => e.RoleAccount)
                .HasForeignKey(e => e.RoleID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TinhChatDiemDo>()
                .HasMany(e => e.DiemDoes)
                .WithRequired(e => e.TinhChatDiemDo)
                .HasForeignKey(e => e.TinhChatID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TinhChatDiemDo>()
                .HasMany(e => e.LogTinhChatDiemDoes)
                .WithRequired(e => e.TinhChatDiemDo)
                .WillCascadeOnDelete(false);
        }
    }
}
