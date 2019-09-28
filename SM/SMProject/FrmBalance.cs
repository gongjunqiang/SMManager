using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMProject
{
    public partial class FrmBalance : Form
    {
        public FrmBalance(string totalMoney)
        {
            InitializeComponent();
            this.lblTotalMoney.Text = totalMoney;
            this.txtRecieveMoney.Text = totalMoney;
            this.txtRecieveMoney.SelectAll();
            this.txtRecieveMoney.Focus();

        }

        /// <summary>
        /// 会员卡号触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)//回车键表示正常结算
            {
                if (this.txtMemberNo.Text.Trim().Length == 0)//没有会员卡
                {
                    this.Tag = this.txtRecieveMoney.Text;
                }
                else//有会员卡号：将实际收款与卡号同返回
                {
                    //需要判断判断会员卡号是否存在
                    this.Tag = this.txtRecieveMoney.Text.Trim() + "|" + this.txtMemberNo.Text.Trim();
                }
                this.DialogResult = DialogResult.OK;
                this.Close();

            }
            else if (e.KeyValue == 115)//放弃购买F4
            {
                this.Tag = "F4";
                this.Close();
            }
            else if (e.KeyValue == 13)//需要删除部分商品F5
            {
                this.Tag = "F5";
                this.Close();
            }
           
        }
    }
}
