using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKS.Models
{
    public class DataProvider
    {
        private static DataProvider _ins;
        public static DataProvider Ins
        {
            get
            {
                if (_ins == null) _ins = new DataProvider();
                return _ins;
            }
            set => _ins = value;
        }

        public HotelManagementEntities DB { get; set; } // Khai báo thuộc tính DB để truy cập cơ sở dữ liệu

        private DataProvider()
        {
            DB = new HotelManagementEntities();
        }
    }
}
