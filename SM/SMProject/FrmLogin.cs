using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL;
using Models;

using System.Net;


namespace SMProject
{
    public partial class FrmLogin : Form
    {
        private SalesPersonService salesPersonService = new SalesPersonService();
        public FrmLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (this.txtAccount.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入账号！","提示信息！");
                this.txtAccount.SelectAll();
                this.txtAccount.Focus();
                return;
            }

            if (this.txtPassword.Text.Trim().Length == 0)
            {
                MessageBox.Show("请输入密码！", "提示信息！");
                this.txtPassword.SelectAll();
                this.txtPassword.Focus();
                return;
            }
            //封装对象
            SalesPerson salesPerson = new SalesPerson
            {
                SPName = this.txtAccount.Text.Trim(),
                LoginPwd = this.txtPassword.Text.Trim()
            };

            //调用后台
            try
            {
                salesPerson = salesPersonService.UserLogin(salesPerson);
                if (salesPerson != null)
                {
                    //保存用户信息
                    Program.currentSalesPerson = salesPerson;
                    //记录日志
                    Program.currentSalesPerson.LoginLogId = salesPersonService.WriteLoginlog(new LoginLogs
                    {
                        LoginId = salesPerson.SalesPersonId,
                        SPName = salesPerson.SPName,
                        ServerName = Dns.GetHostName()
                    });

                    //设置窗体返回值
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("用户名或密码错误！", "提示信息！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "操作异常");
            }
        }

        private void BtnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
