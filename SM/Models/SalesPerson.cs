using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 销售员实体类
    /// </summary>
    [Serializable]
    public class SalesPerson
    {
        /// <summary>
        /// 登录ID
        /// </summary>
        public int SalesPersonId { get; set; }
        /// <summary>
        /// 销售员姓名
        /// </summary>
        public string SPName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPwd { get; set; }

        /// <summary>
        /// 扩展属性：用户登录记录的日志Id
        /// </summary>
        public int LoginLogId { get; set; }
    }
}
