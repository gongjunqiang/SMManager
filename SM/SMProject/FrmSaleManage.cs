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

namespace SMProject
{
    public partial class FrmSaleManage : Form
    {
        private SalesPersonService salesPersonService = new SalesPersonService();
        private ProductService productService = new ProductService();
        //定义商品集合用于保存商品信息
        private List<Product> productList = new List<Product>();
        //定义数据绑定对象：在list与dgv中使用
        private BindingSource bs = new BindingSource();


        public FrmSaleManage()
        {
            InitializeComponent();
            //展示当前销售人员信息
            this.lblSalePerson.Text = Program.currentSalesPerson.SPName;
            //生成流水号
            this.lblSerialNum.Text = CreateSeriaINum();
            this.dgvProdutList.AutoGenerateColumns = false;
        }

        #region  窗体拖动、关闭【实际项目中不用】

        private Point mouseOff;//鼠标移动位置变量
        private bool leftFlag;//标签是否为左键
        private void FrmMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }
        private void FrmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }
        private void FrmMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }

        #endregion

        #region 扫描商品条码、上下移动选择商品、删除商品、商品计算、系统退出的时间的“入口”
        private void TxtProductId_KeyDown(object sender, KeyEventArgs e)
        {
            this.Text = e.KeyValue.ToString();


            //回车键：将当前商品添加刀商品列表中
            if (e.KeyValue == 13)
            {
                #region 添加商品并跟新商品数量及价格
                if (!VaildateInput()) return;
                //【2】判断商品是否以及存在商品列表
                Product product = productList.Where(o => o.ProductId == this.txtProductId.Text.Trim()).FirstOrDefault();
                //不存在添加新商品刀列表中
                if (product == null)
                {
                    AddNewProductToList();
                }
                else//【3】商品已经存在商品列表，跟新商品列表中对应商品的信息
                {
                    product.Quantity += Convert.ToInt32(this.txtQuantity.Text.Trim());
                    product.SubTotal = product.Quantity * product.UnitPrice;
                    if (product.Discount != 0)
                    {
                        product.SubTotal *= Convert.ToDecimal(Convert.ToDecimal(product.Discount) / 10);
                    }
                }
                //【4】展示商品列表
                //展示商品列表
                this.bs.DataSource = productList;//设置BindingSource的数据源
                this.dgvProdutList.DataSource = null;
                this.dgvProdutList.DataSource = this.bs; //BindingSource作为数据源

                //【5】小计金额更新
                this.lblTotalMoney.Text = productList.Select(o => o.SubTotal).Sum().ToString();
                //【6】清空对应的信息
                this.txtProductId.Text = "";
                this.txtQuantity.Text = "1";
                this.txtUnitPrice.Text = "0.00";
                this.txtDiscount.Text = "0";
                this.lblReceivedMoney.Text = "0.00";
                this.lblReturnMoney.Text = "0.00";
                this.txtProductId.Focus();
                #endregion
            }
            else if (e.KeyValue == 38)
            {
                #region 向上移动
                if (this.dgvProdutList.RowCount <= 1)
                {
                    return;
                }
                this.bs.MovePrevious();
                #endregion
            }
            else if (e.KeyValue == 40)
            {
                #region 向下移动
                if (this.dgvProdutList.RowCount <= 1)
                {
                    return;
                }
                this.bs.MoveNext();
                #endregion
            }
            else if (e.KeyValue == 46)
            {
                #region 删除当前行
                if (this.dgvProdutList.RowCount == 0) return;
                //1.从bs中移除当前行
                this.bs.RemoveCurrent();
                //2.这里需要重新设置dgvProdutList的数据源
                this.dgvProdutList.DataSource = null;
                this.dgvProdutList.DataSource = this.bs;
                // this.Text = this.productList.Count.ToString();//测试结果：从BindingSource删除数时，他的数据源中对应的数据也会被删除
                #endregion
            }
            else if (e.KeyValue == 112)
            {
                #region F1商品结算  
                if (this.dgvProdutList.RowCount == 0) return;
                Balance();


                #endregion
            }
            else if (e.KeyValue == 121)//F10关闭
            {
                this.Close();
            }

        }

        #region 商品结算
        private void Balance()
        {
            //【1】打开UI界面:考虑取消或者修改的情况
            FrmBalance frmBalance = new FrmBalance(this.lblTotalMoney.Text);
            DialogResult dialogResult = frmBalance.ShowDialog();
            if (dialogResult == DialogResult.Cancel)
            {
                if (frmBalance.Tag.ToString() == "F4")//放弃购买:重新生成流水号，等待下一个客户
                {
                    RestForm();
                }
                else if (frmBalance.Tag.ToString() == "F5")//放弃部分商品
                {
                    this.txtProductId.Focus();
                }
            }
            else
            {
                //【2】正式结算
                SMMembers members = null;
                if (frmBalance.Tag.ToString().Contains("|"))//判断是否有会员卡
                {
                    string[] info = frmBalance.Tag.ToString().Split('|');
                    this.lblReceivedMoney.Text = info[0];
                    members = new SMMembers { MemeberId = info[1], Points = (int)Convert.ToDouble(this.lblTotalMoney.Text.Trim()) / 10 };
                }
                else//不包含会员卡
                {
                    this.lblReceivedMoney.Text = frmBalance.Tag.ToString();
                }
                //显示找零
                this.lblReturnMoney.Text = (Convert.ToDecimal(this.lblReceivedMoney.Text.Trim()) - Convert.ToDecimal(this.lblTotalMoney.Text.Trim())).ToString();
                //【3】封装主表对象
                SalesListMain salesListMain = new SalesListMain
                {
                    SeriaINum = this.lblSerialNum.Text,
                    TotalMoney = Convert.ToDecimal(this.lblTotalMoney.Text.Trim()),
                    RealRecieve = Convert.ToDecimal(this.lblReceivedMoney.Text.Trim()),
                    ReturnMoney = Convert.ToDecimal(this.lblReturnMoney.Text.Trim()),
                    SalesPersonId = Program.currentSalesPerson.SalesPersonId,
                };
                //【4】封装明细表对象
                foreach (Product product in productList)
                {
                    salesListMain.ListDetail.Add(new SalesListDetail
                    {
                        SeriaINum = this.lblSerialNum.Text,
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Quantity = product.Quantity,
                        UnitPrice = product.UnitPrice,
                        Discount = product.Discount,
                        SubTotalMoney = product.SubTotal,
                    });
                }

                //【5】调用后台
                try
                {
                    productService.SaveSaleInfo(salesListMain, members);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存数据发生异常"+ex.Message);
                }

                //【6】打印小票
                this.printDocument.Print();
                //【7】生成新的流水号
                RestForm();
            }
        }

        private void RestForm()
        {
            this.lblSerialNum.Text = CreateSeriaINum();
            this.dgvProdutList.DataSource = null;
            this.productList.Clear();
            this.lblTotalMoney.Text = "0.00";
            this.lblReturnMoney.Text = "0.00";
            this.txtProductId.Focus();
        }
        #endregion

        #region 添加新商品及添加商品前的判断
        //【1】销售人员输入信息的校验
        private bool VaildateInput()
        {
            if (this.txtProductId.Text.Trim().Length == 0) return false;
            //商品Id必须是数字（正则表达式）

            //商品单价。这块等

            return true;
        }

        //在商品集合中添加新商品
        private void AddNewProductToList()
        {
            //1.根据商品编号查询商品信息
            Product product = productService.GetProductById(this.txtProductId.Text.Trim());
            //如果数据库中无该商品编码，则认为此商品未临时商品或输错编号
            if (product == null)
            {
                //询问是否是临时商品，如果不是，则直接通过DialogResult关闭
                //此处当作是临时商品
                product = new Product
                {
                    ProductId = this.txtProductId.Text.Trim(),
                    ProductName = "暂时未提供名称",
                    UnitPrice = Convert.ToDecimal(this.txtUnitPrice.Text.Trim()),
                    Discount = Convert.ToInt32(this.txtDiscount.Text.Trim())//目前只允许输入整数
                };
            }
            else
            {
                //展示商品信息
                this.txtUnitPrice.Text = product.UnitPrice.ToString();
                this.txtDiscount.Text = product.Discount.ToString();
            }
            //2.计算小计金额;数量*单价
            product.Quantity = Convert.ToInt32(this.txtQuantity.Text.Trim());
            product.SubTotal = Convert.ToDecimal(product.Quantity) * product.UnitPrice;
            //3.如果有则扣
            if (product.Discount != 0)
            {
                product.SubTotal *= Convert.ToDecimal(Convert.ToDecimal(product.Discount)/10);
            }
            //4添加商品编号
            product.Num = productList.Count + 1;
            //将商品对象添加刀商品列表中
            productList.Add(product);
            this.bs.MoveLast();//最后一行选中
        }
        #endregion

        #region 从列表中删除商品后更新总计金额与序号
        /// <summary>
        /// dgv行数发生变化时：跟新总计金额
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvProdutList_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            //更新总计金额
            this.lblTotalMoney.Text = productList.Select(o => o.SubTotal).Sum().ToString();
            //更新序号：
            for (int i = 0; i < this.productList.Count; i++)
            {
                this.productList[i].Num = i+1;
            }

        }
        #endregion

        #region 数量、单价、折扣回车键商品编号获取焦点


        /// <summary>
        /// 数量、单价、折扣回车键商品编号获取焦点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtOther_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                this.txtProductId.Focus();
            }
        }
        #endregion
        #endregion

        #region 退出日志记录
        /// <summary>
        /// 窗体关闭写入日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmSaleManage_FormClosing(object sender, FormClosingEventArgs e)
        {
            DateTime dt = salesPersonService.GetDBServerTime();
            DialogResult dialogResult = MessageBox.Show("是否退出？","提示信息！",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                try
                {
                    salesPersonService.WriteExitLog(Program.currentSalesPerson.LoginLogId, dt);
                }   
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        /// <summary>
        /// 生成流水号
        /// </summary>
        /// <returns></returns>
        private string CreateSeriaINum()
        {
            string seriaINum = salesPersonService.GetDBServerTime().ToString("yyyyMMddHHmmssms");
            Random random = new Random();
            seriaINum += random.Next(10, 99).ToString();
            return seriaINum;
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            USBPrint.Print(e, this.productList, this.lblSerialNum.Text, this.lblSalePerson.Text);
        }
    }
}
