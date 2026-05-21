using QLKS.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QLKS.Services
{
    public static class DbService
    {
        public static string ConnectionString
        {
            get
            {
                return @"Data Source=.;Initial Catalog=HotelManagement;Integrated Security=True;TrustServerCertificate=True";
            }
        }

        public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);

        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static DataTable GetDataTable(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            using (var adapter = new SqlDataAdapter(cmd))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                var table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }
    }

    public static class SessionService
    {
        public static CurrentUser CurrentUser { get; private set; }
        public static bool IsLoggedIn => CurrentUser != null;

        public static bool Login(string username, string password, out string error)
        {
            error = null;
            const string sql = @"
SELECT TOP 1 MaNV, HoTen, Username, VaiTro
FROM NhanVien
WHERE Username = @Username AND PasswordHash = @Password AND TrangThai = 1";
            var table = DbService.GetDataTable(sql,
                new SqlParameter("@Username", username ?? ""),
                new SqlParameter("@Password", password ?? ""));

            if (table.Rows.Count == 0)
            {
                error = "Sai tài khoản/mật khẩu hoặc nhân viên đã nghỉ.";
                return false;
            }

            var row = table.Rows[0];
            CurrentUser = new CurrentUser
            {
                MaNV = Convert.ToInt32(row["MaNV"]),
                HoTen = row["HoTen"].ToString(),
                Username = row["Username"].ToString(),
                VaiTro = row["VaiTro"].ToString()
            };
            return true;
        }

        public static void Logout() => CurrentUser = null;

        public static bool HasRole(params string[] roles)
        {
            if (CurrentUser == null) return false;
            foreach (var role in roles)
                if (string.Equals(CurrentUser.VaiTro, role, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}
