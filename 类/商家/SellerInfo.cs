using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_PJ
{
    /// <summary>
    /// 商家信息
    /// </summary>
    public class SellerInfo
    {
        public string ID;

        public string seller_name;

        public string password;

        public string fullname;

        public string address;

        public string description;

        public string phone_number;

        public static SellerInfo ToSellerInfo(MySqlDataReader reader)
        {
            SellerInfo info = new SellerInfo();
            info.ID           = reader.GetValue(0).ToString();
            info.seller_name  = reader.GetValue(1).ToString();
            info.password     = reader.GetValue(2).ToString();
            info.fullname     = reader.GetValue(3).ToString();
            info.address      = reader.GetValue(4).ToString();
            info.description  = reader.GetValue(5).ToString();
            info.phone_number = reader.GetValue(6).ToString();
            return info;
        }

        public override string ToString()
        {
            string str = "";
            str += ID + "\r\t\r\t ";
            str += seller_name + "\r\t\r\t ";
            str += address + "\r\t\r\t ";
            str += phone_number;

            return str;
        }
    }
}
