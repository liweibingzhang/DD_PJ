using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using MySql.Data.MySqlClient; 

namespace DD_PJ
{
    /// <summary>
    /// 系统管理者
    /// </summary>
    public class Manager
    {
        private static Manager instance = new Manager();
        public static Manager Instance => instance;

        /// <summary>
        /// 和数据库的连接
        /// </summary>
        MySqlConnection conn;

        /// <summary>
        /// 数据库操作命令
        /// </summary>
        MySqlCommand cmd;

        private Manager()
        {
            //连入数据库
            conn = new MySqlConnection("server=localhost;username=root;password=210906;database=PCS;");
            conn.Open();
        }

        /// <summary>
        /// 获取某个关系模式中所有记录的名称
        /// </summary>
        /// <param name="type">关系模式</param>
        /// <returns></returns>
        public List<string> GetNames(E_RelationType type)
        {
            List<string> list = new List<string>();

            string relation = "";
            switch (type)
            {
                case E_RelationType.User:
                    relation = "user";
                    break;
                case E_RelationType.Seller:
                    relation = "seller";
                    break;
                case E_RelationType.Administrator:
                    relation = "administrator";
                    break;
                case E_RelationType.Commodity:
                    relation = "commodity";
                    break;
                case E_RelationType.Platform:
                    relation = "platform";
                    break;
                default:
                    break;
            }

            string cmdStr = string.Format("SELECT `{0}_name` FROM `{1}`", relation, relation);
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(reader.GetValue(0).ToString());
                }
            }
            return list;
        }

        /// <summary>
        /// 获取注册表
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetRegistry(E_RelationType type)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            string relation = "";
            switch (type)
            {
                case E_RelationType.User:
                    relation = "user";
                    break;
                case E_RelationType.Seller:
                    relation = "seller";
                    break;
                case E_RelationType.Administrator:
                    relation = "administrator";
                    break;
                default:
                    break;
            }

            string cmdStr = string.Format("SELECT `{0}_name`, `password` FROM `{1}`", relation, relation);
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader.GetValue(0).ToString();
                    string pwd = reader.GetValue(1).ToString();
                    dic.Add(name, pwd);
                }
            }
            return dic;
        }

        /// <summary>
        /// 得到一条记录
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="type">关系模式</param>
        /// <param name="selectBy">索引的类型(名称/ID)</param>
        /// <returns></returns>
        public object GetRecord(string index, E_RelationType type, string selectBy)
        {
            string cmdStr = "";
            switch (type)
            {
                case E_RelationType.User:
                    cmdStr = "SELECT * FROM `user` WHERE `user_name` = '" + index + "'";
                    if (selectBy == "id" || selectBy == "ID" || selectBy == "Id")
                        cmdStr = "SELECT * FROM `user` WHERE `ID` = " + index;
                    cmd = new MySqlCommand(cmdStr, conn);
                    UserInfo userInfo = null;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userInfo = UserInfo.ToUserInfo(reader);
                        }
                    }
                    return userInfo;
                case E_RelationType.Seller:
                    cmdStr = "SELECT * FROM `seller` WHERE `seller_name` = '" + index + "'";
                    if (selectBy == "id" || selectBy == "ID" || selectBy == "Id")
                        cmdStr = "SELECT * FROM `seller` WHERE `ID` = " + index;
                    cmd = new MySqlCommand(cmdStr, conn);
                    SellerInfo sellerInfo = null;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            sellerInfo = SellerInfo.ToSellerInfo(reader);
                        }
                    }
                    return sellerInfo;
                case E_RelationType.Administrator:
                    return "";
                case E_RelationType.Commodity:
                    cmdStr = "SELECT * FROM `commodity` WHERE `commodity_name` = '" + index + "'";
                    if (selectBy == "id" || selectBy == "ID" || selectBy == "Id")
                        cmdStr = "SELECT * FROM `commodity` WHERE `ID` = " + index;
                    cmd = new MySqlCommand(cmdStr, conn);
                    CommodityInfo commodityInfo = null;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            commodityInfo = CommodityInfo.ToCommodityInfo(reader);
                        }
                    }
                    return commodityInfo;
                case E_RelationType.Platform:
                    cmdStr = "SELECT * FROM `platform` WHERE `platform_name` = '" + index + "'";
                    if (selectBy == "id" || selectBy == "ID" || selectBy == "Id")
                        cmdStr = "SELECT * FROM `platform` WHERE `ID` = " + index;
                    cmd = new MySqlCommand(cmdStr, conn);
                    string platformID = null;
                    string platformName = null;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            platformID   = reader.GetValue(0).ToString();
                            platformName = reader.GetValue(1).ToString();
                        }
                    }
                    return (platformID, platformName);
                default:
                    break;
            }
            return null;
        }

        /// <summary>
        /// 在数据库删除一条记录
        /// </summary>
        /// <param name="id">记录的ID</param>
        /// <param name="type">关系模式</param>
        public void DeleteRecord(string id, E_RelationType type)
        {
            string cmdStr = "";
            switch (type)
            {
                case E_RelationType.User:
                    cmdStr = "DELETE FROM `user` WHERE `ID` = " + id;
                    break;
                case E_RelationType.Seller:
                    cmdStr = "DELETE FROM `seller` WHERE `ID` = " + id;
                    break;
                case E_RelationType.Platform:
                    cmdStr = "DELETE FROM `platform` WHERE `ID` = " + id;
                    break;
                case E_RelationType.Commodity:
                    cmdStr = "DELETE FROM `commodity` WHERE `ID` = " + id;
                    break;
                default:
                    return;
            }
            cmd = new MySqlCommand(cmdStr, conn);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 下架一件在售商品
        /// </summary>
        /// <param name="commodityID">商品id</param>
        /// <param name="sellerID">商家id</param>
        /// <param name="platformID">平台id</param>
        public void RemoveFromShelves(SellingInfo sellingInfo)
        {
            string cmdStr = string.Format("DELETE FROM `selling` WHERE `commodity_id` = {0} AND `seller_id` = {1} AND `platform_id` = {2}",
                                          sellingInfo.commodity_id, sellingInfo.seller_id, sellingInfo.platform_id);
            
            cmd = new MySqlCommand(cmdStr, conn);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userType">用户类型</param>
        /// <param name="name">用户名/商家名</param>
        /// <param name="password">密码</param>
        public void Register(string name, string password, E_RelationType userType)
        {
            string cmdStr = "";
            switch (userType)
            {
                case E_RelationType.User:
                    cmdStr = string.Format("INSERT INTO `user` VALUES(null, '{0}', '{1}', null, null, null, null)", name, password);
                    break;
                case E_RelationType.Seller:
                    cmdStr = string.Format("INSERT INTO `seller` VALUES(null, '{0}', '{1}', null, null, null)", name, password);
                    break;
                default:
                    break;
            }
            cmd = new MySqlCommand(cmdStr, conn);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 发布商品
        /// </summary>
        /// <param name="sellingInfo">要发布的商品信息</param>
        public void Publish(SellingInfo sellingInfo)
        {
            try
            {
                cmd = new MySqlCommand();
                cmd.Connection = conn;
                // 构造插入语句
                cmd.CommandText =
                    "INSERT INTO selling (commodity_id, seller_id, platform_id, produce_date, shelf_life, produce_address, price, description) " +
                    "VALUES (@commodity_id, @seller_id, @platform_id, @produce_date, @shelf_life, @produce_address, @price, @description)";

                // 添加参数
                cmd.Parameters.AddWithValue("@commodity_id", sellingInfo.commodity_id);
                cmd.Parameters.AddWithValue("@seller_id", sellingInfo.seller_id);
                cmd.Parameters.AddWithValue("@platform_id", sellingInfo.platform_id);
                cmd.Parameters.AddWithValue("@produce_date", DateTime.Parse(sellingInfo.produce_date));
                cmd.Parameters.AddWithValue("@shelf_life", int.Parse(sellingInfo.shelf_life));
                cmd.Parameters.AddWithValue("@produce_address", sellingInfo.produce_address);
                cmd.Parameters.AddWithValue("@price", decimal.Parse(sellingInfo.price));
                cmd.Parameters.AddWithValue("@description", sellingInfo.description);
                // 执行插入操作
                cmd.ExecuteNonQuery();

                MessageBox.Show("Publish successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 更新在售商品信息
        /// </summary>
        /// <param name="sellingInfo">新的在售商品信息</param>
        public void UpdateSellingInfo(SellingInfo sellingInfo)
        {
            try
            {
                cmd = new MySqlCommand();
                cmd.Connection = conn;
                // 构造更新语句
                cmd.CommandText = "UPDATE selling " +
                                      "SET produce_date = @produce_date, " +
                                      "    shelf_life = @shelf_life, " +
                                      "    produce_address = @produce_address, " +
                                      "    price = @price, " +
                                      "    description = @description " +
                                      "WHERE commodity_id = @commodity_id AND seller_id = @seller_id AND platform_id = @platform_id";

                // 添加参数
                cmd.Parameters.AddWithValue("@produce_date", DateTime.Parse(sellingInfo.produce_date));
                cmd.Parameters.AddWithValue("@shelf_life", int.Parse(sellingInfo.shelf_life));
                cmd.Parameters.AddWithValue("@produce_address", sellingInfo.produce_address);
                cmd.Parameters.AddWithValue("@price", decimal.Parse(sellingInfo.price));
                cmd.Parameters.AddWithValue("@description", sellingInfo.description);
                cmd.Parameters.AddWithValue("@commodity_id", sellingInfo.commodity_id);
                cmd.Parameters.AddWithValue("@seller_id", sellingInfo.seller_id);
                cmd.Parameters.AddWithValue("@platform_id", sellingInfo.platform_id);

                // 执行更新操作
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Update successful!");
                }
                else
                {
                    MessageBox.Show("No rows updated. SellingInfo not found in the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 获取指定商家在售商品信息
        /// </summary>
        /// <param name="sellerID">商家ID</param>
        /// <returns></returns>
        public List<SellingInfo> GetSellingInfo(string sellerID)
        {
            List<SellingInfo> list = new List<SellingInfo>();

            string cmdStr = "SELECT `commodity_id`, `commodity_name`, `category`, " +
                            "`seller_id`, `seller_name`, `platform_id`, `platform_name`, `produce_date`, " +
                            "`shelf_life`, `produce_address`, `price`, `selling`.description " +
                            "FROM `selling` JOIN `commodity` ON `selling`.commodity_id = `commodity`.ID " +
                            "JOIN `seller` ON `selling`.seller_id = `seller`.ID " +
                            "JOIN `platform` ON `selling`.platform_id = `platform`.ID " +
                            "WHERE `seller_id` = " + sellerID;
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(SellingInfo.ToSellingInfo(reader));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取所有在售商品信息
        /// </summary>
        /// <returns></returns>
        public List<SellingInfo> GetSellingInfo()
        {
            List<SellingInfo> list = new List<SellingInfo>();

            string cmdStr = "SELECT `commodity_id`, `commodity_name`, `category`, " +
                            "`seller_id`, `seller_name`, `platform_id`, `platform_name`, `produce_date`, " +
                            "`shelf_life`, `produce_address`, `price`, `selling`.description " +
                            "FROM `selling` JOIN `commodity` ON `selling`.commodity_id = `commodity`.ID " +
                            "JOIN `seller` ON `selling`.seller_id = `seller`.ID " +
                            "JOIN `platform` ON `selling`.platform_id = `platform`.ID";
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(SellingInfo.ToSellingInfo(reader));
                }
            }
            return list;
        }

        /// <summary>
        /// 更新商家信息
        /// </summary>
        /// <param name="sellerInfo">商家信息</param>
        public void UpdateSellerInfo(SellerInfo sellerInfo)
        {
            string cmdStr = string.Format("UPDATE `seller` " +
                                          "SET `fullname` = '{0}', `password` = '{1}', `address` = '{2}', `description` = '{3}', `phone_number` =  '{4}' " +
                                          "WHERE `ID` = {5}",
                                          sellerInfo.fullname, sellerInfo.password, sellerInfo.address, sellerInfo.description, sellerInfo.phone_number, sellerInfo.ID);
            cmd = new MySqlCommand(cmdStr, conn);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        public void UpdateUserInfo(UserInfo userInfo)
        {
            string cmdStr = string.Format("UPDATE `user` " +
                                          "SET `fullname` = '{0}', `password` = '{1}', `phone_number` = '{2}', `home_address` = '{3}', `birth_date` = '{4}' " +
                                          "WHERE `ID` = {5}",
                                          userInfo.fullname, userInfo.password, userInfo.phone_number, userInfo.home_address, userInfo.birth_date, userInfo.ID);
            cmd = new MySqlCommand(cmdStr, conn);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 用户收藏商品的方法
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="favorite">要收藏的商品的信息</param>
        public void AddToFavorites(string userID, SellingInfo favorite)
        {
            string cmdStr = string.Format("INSERT INTO `favorite` " +
                                          "VALUES({0}, {1}, {2}, {3}, {4})",
                                          userID, favorite.commodity_id, favorite.seller_id, favorite.platform_id, favorite.priceFloor);
            cmd = new MySqlCommand(cmdStr, conn);
            cmd.ExecuteNonQuery();
        }
        

        /// <summary>
        /// 用户取消收藏商品的方法
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="notFavorite">要收藏的商品的信息</param>
        public void RemoveFromFavorites(string userID, SellingInfo notFavorite)
        {
            string cmdStr = string.Format("DELETE FROM `favorite` " +
                                          "WHERE `user_id` = {0} and `commodity_id` = {1} and `seller_id` = {2} and `platform_id` = {3}",
                                          userID, notFavorite.commodity_id, notFavorite.seller_id, notFavorite.platform_id);
            cmd = new MySqlCommand(cmdStr, conn);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 将在售商品中用户收藏的商品添加到收藏夹
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="favorites">收藏夹</param>
        /// <param name="sellingInfoList">在售商品信息</param>
        public void GetFavorites(string userID, List<SellingInfo> favorites, List<SellingInfo> sellingInfoList)
        {
            string cmdStr = "SELECT `commodity_id`, `seller_id`, `platform_id`, `price_floor` FROM `favorite` WHERE `user_id` = " + userID;
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string commodity_id = reader.GetValue(0).ToString();
                    string seller_id    = reader.GetValue(1).ToString();
                    string platform_id  = reader.GetValue(2).ToString();
                    string price_floor  = reader.GetValue(3).ToString();
                    foreach (SellingInfo info in sellingInfoList)
                    {
                        if (info.commodity_id == commodity_id && info.seller_id == seller_id && info.platform_id == platform_id)
                        {
                            info.priceFloor = price_floor;
                            info.isFavorite = true;
                            favorites.Add(info);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取一件商品的历史价格
        /// </summary>
        /// <param name="info"></param>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public Dictionary<DateTime, float> GetHistoryPrices(SellingInfo info, int timespan)
        {
            Dictionary<DateTime, float> dict = new Dictionary<DateTime, float>();
            string cmdStr = string.Format("SELECT `price`, `date` FROM `history_price` " +
                                          "WHERE `commodity_id` = {0} and `seller_id` = {1} and `platform_id` = {2} and DATE_SUB(CURDATE(), INTERVAL {3} DAY) <= date",
                                          info.commodity_id, info.seller_id, info.platform_id, timespan);
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string price = reader.GetValue(0).ToString();
                    string date  = reader.GetValue(1).ToString();
                    if (price == "")
                        price = "0";
                    dict.Add(DateTime.Parse(date), float.Parse(price));
                }
            }
            return dict;
        }
        /// <summary>
        /// 获取同一类商品的收藏数量
        /// </summary>
        /// <param name="info"></param>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetCollectCountBySeller(int commodityId, int timespan)
        {
            Dictionary<string, int> collectCounts = new Dictionary<string, int>();
            string cmdStr = string.Format("SELECT `seller`.`seller_name`, COUNT(*) FROM `collects` " +
                                  "JOIN `seller` ON `collects`.`seller_id` = `seller`.`ID` " +
                                  "WHERE `collects`.`commodity_id` = {0} AND DATE_SUB(CURDATE(), INTERVAL {1} DAY) <= `collects`.`date` " +
                                  "GROUP BY `collects`.`seller_id`",
                commodityId, timespan);
            cmd = new MySqlCommand(cmdStr, conn);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string sellerName = reader[0].ToString();
                    int count = Convert.ToInt32(reader[1]);
                    collectCounts.Add(sellerName, count);
                }
            }
        return collectCounts;
        }

        public class FavoriteItem
        {
            public decimal? PriceFloor { get; set; }
        }
        /// <summary>
        /// 获取用户收藏商品的价格区间
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public Tuple<decimal, decimal> GetUserPriceRange(int userId)
        {
            string cmdStr = "SELECT `price_floor` FROM `collects` WHERE `user_id` = @userId";
            cmd = new MySqlCommand(cmdStr, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            List<decimal?> priceFloors = new List<decimal?>();

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        priceFloors.Add(reader.GetDecimal(0));
                    }
                }
            }
            if (priceFloors.Count == 0)
            {
                return null;
            }
            decimal minPrice = priceFloors.Min() ?? decimal.MaxValue;
            decimal maxPrice = priceFloors.Max() ?? decimal.MinValue;
            return Tuple.Create(minPrice, maxPrice);
}



        /// <summary>
        /// 获取某时间段之前该商品最近的价格
        /// </summary>
        /// <param name="info"></param>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public float GetHistoryPricesBefore(SellingInfo info, int timespan)
        {
            string price = "";
            string cmdStr = string.Format("SELECT `price`, `date` FROM `history_price` " +
                                          "WHERE `commodity_id` = {0} and `seller_id` = {1} and `platform_id` = {2} and DATE_SUB(CURDATE(), INTERVAL {3} DAY) > date " +
                                          "ORDER BY `date` DESC LIMIT 1",
                                          info.commodity_id, info.seller_id, info.platform_id, timespan);
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    price = reader.GetValue(0).ToString();
                }
            }
            if (price == "")
                price = "0";
            return float.Parse(price);
        }

        /// <summary>
        /// 关闭数据库的连接
        /// </summary>
        public void Close()
        {
            conn.Close();
        }

        /// <summary>
        /// 加载所有商品的信息
        /// </summary>
        /// <returns></returns>
        public List<CommodityInfo> GetCommoditiesInfo()
        {
            List<CommodityInfo> list = new List<CommodityInfo>();

            string cmdStr = "SELECT * FROM `commodity`";
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    CommodityInfo info = CommodityInfo.ToCommodityInfo(reader);
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>
        /// 加载所有商家的信息
        /// </summary>
        /// <returns></returns>
        public List<SellerInfo> GetSellersInfo()
        {
            List<SellerInfo> list = new List<SellerInfo>();

            string cmdStr = "SELECT * FROM `seller`";
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    SellerInfo info = SellerInfo.ToSellerInfo(reader);
                    list.Add(info);
                }
            }
            return list;
        }

        /// <summary>
        /// 加载所有平台的信息
        /// </summary>
        /// <returns></returns>
        public List<(string id, string name)> GetPlatformsInfo()
        {
            List<(string id, string name)> list = new List<(string id, string name)>();

            string cmdStr = "SELECT * FROM `platform`";
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string id = reader.GetValue(0).ToString();
                    string name = reader.GetValue(1).ToString();
                    list.Add((id, name));
                }
            }
            return list;
        }

        /// <summary>
        /// 加载所有用户的信息
        /// </summary>
        /// <returns></returns>
        public List<UserInfo> GetUsersInfo()
        {
            List<UserInfo> list = new List<UserInfo>();

            string cmdStr = "SELECT * FROM `user`";
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    UserInfo info = UserInfo.ToUserInfo(reader);
                    list.Add(info);
                }
            }
            return list;
        }


        // 根据枚举类型获取对应的数据库表字段名
        private string GetColumnName(E_RelationType xType)
        {
            switch (xType)
            {
                case E_RelationType.Commodity:
                    return "`commodity_id`";
                case E_RelationType.Seller:
                    return "`seller_id`";
                case E_RelationType.Platform:
                    return "`platform_id`";
                default:
                    return null;
            }
        }
        /// <summary>
        /// 统计所有被收藏的商品的分布情况
        /// </summary>
        /// <param name="xType">横坐标</param>
        /// <returns></returns>
        public Dictionary<string, int> GetFavoritesDistribution(E_RelationType xType)
        {
            string columnName = GetColumnName(xType);

            if (columnName == null)
            {
                throw new ArgumentException("Invalid E_RelationType");
            }

            string cmdStr = "SELECT {columnName}, COUNT(*) FROM `favorite` GROUP BY {columnName}";
            cmd = new MySqlCommand(cmdStr, conn);

            Dictionary<string, int> result = new Dictionary<string, int>();

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string key = reader[0].ToString();
                    int count = Convert.ToInt32(reader[1]);
                    result.Add(key, count);
                }
            }
            return result;
        }
    }
}
