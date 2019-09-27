using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 商品实体类
    /// </summary>
    [Serializable]
    public class Product
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }

        public double UnitPrice { get; set; }

        public int Discount { get; set; }

        public int CategoryId { get; set; }


        //商品分类名称
        public string CategoryName { get; set; }


        //扩展属性扫描商品时使用
        //商品序号
        public int Num { get; set; }
        //商品数量
        public int Quantity { get; set; }
        //小计金额
        public double SubTotal { get; set; }



    }
}
