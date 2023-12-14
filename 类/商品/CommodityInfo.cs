using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DD_PJ
{
    public class CommodityInfo
    {
        public string ID;

        public string commodity_name;

        public string catagory;

        public static CommodityInfo ToCommodityInfo(MySqlDataReader reader)
        {
            CommodityInfo info = new CommodityInfo();
            info.ID             = reader.GetValue(0).ToString();
            info.commodity_name = reader.GetValue(1).ToString();
            info.catagory       = reader.GetValue(2).ToString();
            return info;
        }
    }
}
