using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_PJ
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        public string ID;

        public string user_name;

        public string password;

        public string fullname;

        public string phone_number;

        public string birth_date;

        public string home_address;

        public static UserInfo ToUserInfo(MySqlDataReader reader)
        {
            UserInfo info = new UserInfo();
            info.ID           = reader.GetValue(0).ToString();
            info.user_name    = reader.GetValue(1).ToString();
            info.password     = reader.GetValue(2).ToString();
            info.fullname     = reader.GetValue(3).ToString();
            info.phone_number = reader.GetValue(4).ToString();

            string birth_date = reader.GetValue(5).ToString();
            if (birth_date != "")
            {
                birth_date = DateTime.Parse(birth_date).ToString("D");
            }
            info.birth_date   = birth_date;

            info.home_address = reader.GetValue(6).ToString();
            return info;
        }

        public override string ToString()
        {
            string str = "";
            str += ID + "\r\t\r\t ";
            str += user_name + "\r\t\r\t ";
            str += fullname + "\r\t\r\t ";
            str += phone_number;

            return str;
        }
    }
}
