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
            //回车键：将当前商品添加刀商品列表中
            if (e.KeyValue == 13)
            {
                if (!VaildateInput()) return;

                //【2】判断商品是否以及存在商品列表
                Product product = productList.Where(o => o.ProductId == this.txtProductId.Text.Trim()).FirstOrDefault();
                //不存在添加新商品刀列表中
                if (product == null)
                {
                    AddNewProductToList();
                }

                //展示商品列表
                this.bs.DataSource = productList;//设置BindingSource的数据源
                this.dgvProdutList.DataSource = null;
                this.dgvProdutList.DataSource = this.bs; //BindingSource作为数据源


            }
        }
        //【1】销售人员输入信息的校验
        private bool VaildateInput()
        {
            if (this.txtProductId.Text.Trim().Length == 0) return false;
            //商品Id必须是数字（正则表达式）

            //商品单价。这块等

            return true;
        }

        //【3】在商品集合中添加商品
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
                    UnitPrice = Convert.ToDouble(this.txtUnitPrice.Text.Trim()),
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
            product.SubTotal = Convert.ToDouble(product.Quantity) * product.UnitPrice;
            //3.如果有则扣
            if (product.Discount != 0)
            {
                product.SubTotal *= Convert.ToDouble((10- product.Discount)/10);
            }
            //4添加商品编号
            product.Num = productList.Count + 1;
            //将商品对象添加刀商品列表中
            productList.Add(product);

            this.bs.MoveLast();//最后一行选中


        }

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

    }
}
