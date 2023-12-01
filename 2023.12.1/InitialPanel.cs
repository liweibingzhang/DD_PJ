using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DD_PJ
{
    /// <summary>
    /// 关系模式
    /// </summary>
    public enum E_RelationType
    {
        /// <summary>
        /// 用户
        /// </summary>
        User,
        /// <summary>
        /// 商家
        /// </summary>
        Seller,
        /// <summary>
        /// 管理员
        /// </summary>
        Administrator,
        /// <summary>
        /// 商品
        /// </summary>
        Commodity,
        /// <summary>
        /// 平台
        /// </summary>
        Platform,
    }

    /// <summary>
    /// 初始面板
    /// </summary>
    public partial class InitialPanel : Form
    {
        public InitialPanel()
        {
            InitializeComponent();
        }

        private void btn_asUser_Click(object sender, EventArgs e)
        {
            new LoginPanel(E_RelationType.User, this).Show();
            Hide();
        }

        private void btn_asSeller_Click(object sender, EventArgs e)
        {
            new LoginPanel(E_RelationType.Seller, this).Show();
            Hide();
        }

        private void btn_asManager_Click(object sender, EventArgs e)
        {
            new LoginPanel(E_RelationType.Administrator, this).Show();
            Hide();
        }

        private void InitialPanel_FormClosed(object sender, FormClosedEventArgs e)
        {
            //关闭数据库的连接
            Manager.Instance.Close();
        }

        private void btn_return_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
