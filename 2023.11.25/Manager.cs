using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        /// <summary>
        /// 所有商品的信息<商品id, 商品信息>
        /// </summary>
        public Dictionary<string, CommodityInfo> Commodities { get; private set; }

        /// <summary>
        /// 所有商家的信息<商家id, 商品信息>
        /// </summary>
        public Dictionary<string, SellerInfo> Sellers { get; private set; }

        /// <summary>
        /// 所有平台的信息<平台id, 平台信息>
        /// </summary>
        public Dictionary<string, string> Platforms { get; private set; }

        /// <summary>
        /// 所有用户的信息<用户id, 用户信息>
        /// </summary>
        public Dictionary<string, UserInfo> Users { get; private set; }

        private Manager()
        {
            //连入数据库
            conn = new MySqlConnection("server=localhost;username=root;password=210906;database=PCS;");
            conn.Open();
            //从数据库load信息
            Commodities = LoadCommoditiesInfo();
            Sellers = LoadSellersInfo();
            Platforms = LoadPlatformsInfo();
            Users = LoadUsersInfo();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userType">用户类型</param>
        /// <param name="name">用户名/商家名</param>
        /// <param name="password">密码</param>
        public void Register(string name, string password, E_UserType userType)
        {
            string cmdStr = "";
            switch (userType)
            {
                case E_UserType.User:
                    cmdStr = string.Format("INSERT INTO `user` VALUES(null, '{0}', '{1}', null, null, null)", name,
                        password);
                    break;
                case E_UserType.Seller:
                    cmdStr = string.Format("INSERT INTO `seller` VALUES(null, '{0}', '{1}', null, null, null)", name,
                        password);
                    break;
                default:
                    break;
            }

            cmd = new MySqlCommand(cmdStr, conn);
            cmd.ExecuteNonQuery();
            //更新注册表
            switch (userType)
            {
                case E_UserType.User:
                    Users = LoadUsersInfo();
                    break;
                case E_UserType.Seller:
                    Sellers = LoadSellersInfo();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 发布商品
        /// </summary>
        /// <param name="sellingInfo">要发布的商品信息</param>
        public void Publish(SellingInfo sellingInfo)
        {
            //情景：商家发布了此前尚未发布的商品，因此需要在数据库的表selling中 插入新的数据
            //Tips：
            // 1) 参数sellingInfo 是经过商家新发布的在售商品信息，你要做的是将这些信息插入表selling中
            // 2) 插入需要用到的 参数sellingInfo 里的字段 可以 确保不为空
            // 3）表selling 的定义见 我于2023.11.24日发送给你的压缩包 中的.sql文件
            // 4) 类SellingInfo的定义请见 我于2023.11.25日上传至github 的SellingInfo.cs文件
            // 5) 你在构建字符串时可能会用到string.Format()方法，请自行搜索其用法
            // 6) 你可以参照此文件中其他函数的写法
            // 7) 请注意类SellingInfo中的字段的类型 和 表selling中的字段的类型！！
            //    这关系到在写命令字符串时 一些字段需不需要用 单引号''包裹
            try
            {
                using (MySqlConnection connection =
                       new MySqlConnection("server=localhost;username=root;password=210906;database=PCS;"))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        // 构造插入语句
                        command.CommandText =
                            "INSERT INTO selling (commodity_id, seller_id, platform_id, produce_date, shelf_life, produce_address, price, description) " +
                            "VALUES (@commodity_id, @seller_id, @platform_id, @produce_date, @shelf_life, @produce_address, @price, @description)";

                        // 添加参数
                        command.Parameters.AddWithValue("@commodity_id", sellingInfo.commodity_id);
                        command.Parameters.AddWithValue("@seller_id", sellingInfo.seller_id);
                        command.Parameters.AddWithValue("@platform_id", sellingInfo.platform_id);
                        command.Parameters.AddWithValue("@produce_date", DateTime.Parse(sellingInfo.produce_date));
                        command.Parameters.AddWithValue("@shelf_life", int.Parse(sellingInfo.shelf_life));
                        command.Parameters.AddWithValue("@produce_address", sellingInfo.produce_address);
                        command.Parameters.AddWithValue("@price", decimal.Parse(sellingInfo.price));
                        command.Parameters.AddWithValue("@description", sellingInfo.description);
                        // 执行插入操作
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Publish successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 更新在售商品信息
        /// </summary>
        /// <param name="sellingInfo">新的在售商品信息</param>
        public void UpdateSellingInfo(SellingInfo sellingInfo)
        {
            //情景：商家对自己已发布的商品进行了修改，因此需要在数据库中实现对表selling的更新
            //Tips：
            // 1) 参数sellingInfo 是经过商家修改过后的在售商品信息，你要做的是用这些信息更新表selling中旧的信息
            // 2) 更新需要用到的 参数sellingInfo 里的字段 可以 确保不为空
            // 3）表selling 的定义见 我于2023.11.24日发送给你的压缩包 中的.sql文件
            // 4) 类SellingInfo的定义请见 我于2023.11.25日上传至github 的SellingInfo.cs文件
            // 5) 你在构建字符串时可能会用到string.Format()方法，请自行搜索其用法
            // 6) 你可以参照此文件中其他函数的写法
            // 7) 请注意类SellingInfo中的字段的类型 和 表selling中的字段的类型！！
            //    这关系到在写命令字符串时 一些字段需不需要用 单引号''包裹
            try
            {
                using (MySqlConnection connection =
                       new MySqlConnection("server=localhost;username=root;password=210906;database=PCS;"))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand())
                    {
                        command.Connection = connection;
                        // 构造更新语句
                        command.CommandText = "UPDATE selling " +
                                              "SET produce_date = @produce_date, " +
                                              "    shelf_life = @shelf_life, " +
                                              "    produce_address = @produce_address, " +
                                              "    price = @price, " +
                                              "    description = @description " +
                                              "WHERE commodity_id = @commodity_id AND seller_id = @seller_id AND platform_id = @platform_id";

                        // 添加参数
                        command.Parameters.AddWithValue("@produce_date", DateTime.Parse(sellingInfo.produce_date));
                        command.Parameters.AddWithValue("@shelf_life", int.Parse(sellingInfo.shelf_life));
                        command.Parameters.AddWithValue("@produce_address", sellingInfo.produce_address);
                        command.Parameters.AddWithValue("@price", decimal.Parse(sellingInfo.price));
                        command.Parameters.AddWithValue("@description", sellingInfo.description);
                        command.Parameters.AddWithValue("@commodity_id", sellingInfo.commodity_id);
                        command.Parameters.AddWithValue("@seller_id", sellingInfo.seller_id);
                        command.Parameters.AddWithValue("@platform_id", sellingInfo.platform_id);

                        // 执行更新操作
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Update successful!");
                        }
                        else
                        {
                            Console.WriteLine("No rows updated. SellingInfo not found in the database.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }


        /// <summary>
        /// 获取指定商家在售商品信息
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public List<SellingInfo> GetSellingInfo(string id)
        {
            List<SellingInfo> list = new List<SellingInfo>();

            string cmdStr = "SELECT * FROM `selling` WHERE `seller_id` = " + id;
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
                                          "SET `fullname` = '{0}', `password` = '{1}', `address` = '{2}', `description` = '{3}' " +
                                          "WHERE `ID` = {4}",
                sellerInfo.fullname, sellerInfo.password, sellerInfo.address, sellerInfo.description, sellerInfo.id);
            cmd = new MySqlCommand(cmdStr, conn);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 关闭数据库的连接
        /// </summary>
        public void Close()
        {
            conn.Close();
        }


        #region 从数据库加载信息的方法

        /// <summary>
        /// 加载所有商品的信息
        /// </summary>
        /// <returns></returns>
        Dictionary<string, CommodityInfo> LoadCommoditiesInfo()
        {
            Dictionary<string, CommodityInfo> dic = new Dictionary<string, CommodityInfo>();

            string cmdStr = "SELECT * FROM `commodity`";
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    CommodityInfo info = CommodityInfo.ToCommodityInfo(reader);
                    dic.Add(info.id, info);
                }
            }

            return dic;
        }

        /// <summary>
        /// 加载所有商家的信息
        /// </summary>
        /// <returns></returns>
        Dictionary<string, SellerInfo> LoadSellersInfo()
        {
            Dictionary<string, SellerInfo> dic = new Dictionary<string, SellerInfo>();

            string cmdStr = "SELECT * FROM `seller`";
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    SellerInfo info = SellerInfo.ToSellerInfo(reader);
                    dic.Add(info.id, info);
                }
            }

            return dic;
        }

        /// <summary>
        /// 加载所有平台的信息
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> LoadPlatformsInfo()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            string cmdStr = "SELECT * FROM `platform`";
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string id = reader.GetValue(0).ToString();
                    string name = reader.GetValue(1).ToString();
                    dic.Add(id, name);
                }
            }

            return dic;
        }

        /// <summary>
        /// 加载所有用户的信息
        /// </summary>
        /// <returns></returns>
        Dictionary<string, UserInfo> LoadUsersInfo()
        {
            Dictionary<string, UserInfo> dic = new Dictionary<string, UserInfo>();

            string cmdStr = "SELECT * FROM `user`";
            cmd = new MySqlCommand(cmdStr, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    UserInfo info = UserInfo.ToUserInfo(reader);
                    dic.Add(info.id, info);
                }
            }

            return dic;
        }

        #endregion
    }
}