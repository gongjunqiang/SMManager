using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    /// <summary>
    /// 商品数据访问类
    /// </summary>
    public class ProductService
    {
        /// <summary>
        /// 根据商品Id获取商品信息
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public Product GetProductById(string productId)
        {
            string sql = "select ProductId,ProductName,UnitPrice,Discount from Products where ProductId=@ProductId";
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter("@ProductId",productId),
            };
            Product product = null;
            try
            {
                SqlDataReader reader = SQLHelper.ExecuteReader(sql, sqlParameters);
                if (reader.Read())
                {
                    product = new Product
                    {
                        ProductId = reader["ProductId"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        UnitPrice = Convert.ToDouble(reader["UnitPrice"]),
                        Discount = Convert.ToInt32(reader["Discount"])
                    };
                }
                reader.Close();
                return product;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
