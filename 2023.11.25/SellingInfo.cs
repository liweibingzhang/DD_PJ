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

        public string commodity_catagory;

        public string seller_id;

        public string seller_name;

        public string platform_id;

        public string platform_name;

        public string produce_date;

        public string shelf_life;

        public string produce_address;

        public string price;

        public string description;

        public static SellingInfo ToSellingInfo(MySqlDataReader reader)
        {
            SellingInfo info = new SellingInfo();
            info.commodity_id    = reader.GetValue(0).ToString();
            info.seller_id       = reader.GetValue(1).ToString();
            info.platform_id     = reader.GetValue(2).ToString();

            DateTime product_date = Convert.ToDateTime(reader.GetValue(3).ToString());
            info.produce_date    = product_date.ToString("D");
            info.shelf_life      = reader.GetValue(4).ToString();
            info.produce_address = reader.GetValue(5).ToString();
            info.price           = reader.GetValue(6).ToString();
            info.description     = reader.GetValue(7).ToString();

            Dictionary<string, CommodityInfo> commodities = Manager.Instance.Commodities;
            Dictionary<string, SellerInfo> sellers = Manager.Instance.Sellers;
            Dictionary<string, string> platforms = Manager.Instance.Platforms;

            CommodityInfo commodity = commodities[info.commodity_id];
            info.commodity_name     = commodity.commodity_name;
            info.commodity_catagory = commodity.catagory;

            info.seller_name        = sellers[info.seller_id].seller_name;

            info.platform_name      = platforms[info.platform_id];

            return info;
        }

        public override string ToString()
        {
            string str = "";
            str += commodity_name + "\r\t             ";
            str += commodity_catagory + "\r\t         ";
            str += platform_name + "\r\t      ";
            str += price + "元";

            return str;
        }
    }
}
