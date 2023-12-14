using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_PJ
{
    /// <summary>
    /// 销售信息
    /// </summary>
    public class SellingInfo
    {
        public string commodity_id;

        public string commodity_name;

        public string commodity_category;

        public string seller_id;

        public string seller_name;

        public string platform_id;

        public string platform_name;

        public string produce_date;

        public string shelf_life;

        public string produce_address;

        public string price;

        public string description;

        public string priceFloor;

        public bool isFavorite;

        public static SellingInfo ToSellingInfo(MySqlDataReader reader)
        {
            SellingInfo info = new SellingInfo();
            info.commodity_id       = reader.GetValue(0).ToString();
            info.commodity_name     = reader.GetValue(1).ToString();
            info.commodity_category = reader.GetValue(2).ToString();
            info.seller_id          = reader.GetValue(3).ToString();
            info.seller_name        = reader.GetValue(4).ToString();
            info.platform_id        = reader.GetValue(5).ToString();
            info.platform_name      = reader.GetValue(6).ToString();

            info.produce_date       = DateTime.Parse(reader.GetValue(7).ToString()).ToString("D");
            info.shelf_life         = reader.GetValue(8).ToString();
            info.produce_address    = reader.GetValue(9).ToString();
            info.price              = reader.GetValue(10).ToString();
            info.description        = reader.GetValue(11).ToString();

            return info;
        }

        public override string ToString()
        {
            string str = "";
            str += commodity_name + "\r\t\r\t ";
            str += seller_name + "\r\t\r\t ";
            str += platform_name + "\r\t\r\t ";
            str += price + "元";

            return str;
        }
    }
}
