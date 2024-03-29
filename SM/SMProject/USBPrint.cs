﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Models;

namespace SMProject
{
    class USBPrint
    {
        /// <summary>
        /// 打印购物小票
        /// </summary>
        /// <param name="e"></param>
        /// <param name="list">需要循环打印的商品列表</param>
        /// <param name="serialNumber">流水号</param>
        /// <param name="salesperson">售货员</param>
        public static void Print(System.Drawing.Printing.PrintPageEventArgs e, List<Product> list,
            string serialNumber, string salesperson)
        {
            //生成票据
            float left = 2; //打印区域的左边界
            float top = 10;//打印区域的上边界
            Font titlefont = new Font("楷体_gb2312", 12);//标题字体
            Font font = new Font("宋体", 8);//内容字体            
            e.Graphics.DrawString("    实惠到家超市", titlefont, Brushes.Blue, left + 10, top - 5, new StringFormat());//打印标题
            e.Graphics.DrawString("       购物凭证", titlefont, Brushes.Blue, left + 10, top + 15, new StringFormat());//打印标题
            //画一条分界线
            Pen pen = new Pen(Color.Green, 1);
            e.Graphics.DrawLine(pen, new Point((int)left - 2, (int)top + 35), new Point((int)left + (int)180, (int)top + 35));
            //打印内容标题
            decimal totalMoney = 0;//商品总计
            e.Graphics.DrawString("商品名称       单价  数量  金额", font, Brushes.Blue, left, top + titlefont.GetHeight(e.Graphics) + 12 + 12, new StringFormat());
            //循环打印商品清单
            for (int i = 1; i <= list.Count; i++)
            {
                totalMoney += list[i - 1].SubTotal;
                decimal unitPrice = list[i - 1].UnitPrice * Convert.ToDecimal(1.00);
                if (list[i - 1].Discount != 0)
                    unitPrice = unitPrice * list[i - 1].Discount / Convert.ToDecimal(10.00) * Convert.ToDecimal(1.00);
                //调整间距
                string sp1 = string.Empty;
                if (list[i - 1].ProductName.Length == 4)
                    sp1 = "        ";
                else if (list[i - 1].ProductName.Length == 3)
                    sp1 = "            ";
                else if (list[i - 1].ProductName.Length == 5)
                    sp1 = "    ";

                string sp2 = string.Empty;
                if (list[i - 1].UnitPrice.ToString().Length == 1)
                    sp2 = "         ";
                else if (list[i - 1].UnitPrice.ToString().Length == 2)
                    sp2 = "        ";
                else if (list[i - 1].UnitPrice.ToString().Length == 3)
                    sp2 = "       ";
                else if (list[i - 1].UnitPrice.ToString().Length == 4)
                    sp2 = "      ";
                else if (list[i - 1].UnitPrice.ToString().Length == 5)
                    sp2 = "     ";
                else if (list[i - 1].UnitPrice.ToString().Length == 6)
                    sp2 = "    ";

                string sp3 = string.Empty;
                if (list[i - 1].Quantity.ToString().Length == 1)
                    sp3 = "         ";
                else if (list[i - 1].Quantity.ToString().Length == 2)
                    sp3 = "       ";
                else if (list[i - 1].Quantity.ToString().Length == 3)
                    sp3 = "     ";
                e.Graphics.DrawString(list[i - 1].ProductName + sp1 + unitPrice + sp2 + list[i - 1].Quantity.ToString() + sp3 + list[i - 1].SubTotal.ToString(),
                    font, Brushes.Blue, left, top + titlefont.GetHeight(e.Graphics) + font.GetHeight(e.Graphics) * i + 12 + 12, new StringFormat());
            }

            for (int i = 1; i <= list.Count; i++)
            {

                totalMoney += list[i - 1].SubTotal;
                decimal unitPrice = list[i - 1].UnitPrice * Convert.ToDecimal(1.00);
                if (list[i - 1].Discount != 0)
                    unitPrice = unitPrice * list[i - 1].Discount / Convert.ToDecimal(10.00) * Convert.ToDecimal(1.00);
                float nextTop = top + titlefont.GetHeight(e.Graphics) + font.GetHeight(e.Graphics) * i + 25;
                e.Graphics.DrawString(list[i - 1].ProductName, font, Brushes.Blue, left, nextTop, new StringFormat());

            }


            //画一条分界线           
            e.Graphics.DrawLine(pen, new Point((int)left - 2, (int)top + 125), new Point((int)left + (int)180, (int)top + 125));
            //打印备注
            e.Graphics.DrawString("总计：  " + totalMoney * Convert.ToDecimal(1.00) + "  元", font, Brushes.Black, left, (int)top + 110 + 10 + 12, new StringFormat());
            e.Graphics.DrawString("流水号：" + serialNumber, font, Brushes.Black, left, (int)top + 110 + 10 + font.GetHeight(e.Graphics) * 2 + 12, new StringFormat());
            e.Graphics.DrawString("结算员：" + salesperson, font, Brushes.Black, left, (int)top + 110 + 10 + font.GetHeight(e.Graphics) * 3 + 12, new StringFormat());
            e.Graphics.DrawString("在7日内凭小票可开具购物发票", font, Brushes.Black, left, (int)top + 110 + 10 + font.GetHeight(e.Graphics) * 5 + 12, new StringFormat());
            e.Graphics.DrawString("            欢迎再次光临！", font, Brushes.Black, left, (int)top + 110 + 10 + font.GetHeight(e.Graphics) * 7 + 12, new StringFormat());

        }
    }
}
