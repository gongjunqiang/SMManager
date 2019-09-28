
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// 会员实体类
    /// </summary>
    [Serializable]
    public class SMMembers
    {
        public string MemeberId { get; set; }

        public string MemeberName { get; set; }

        public int Points { get; set; }

        public string PhoneNmaber { get; set; }

        public string MemberAddress { get; set; }

        public DateTime OpenTime { get; set; }

        public int MemberStatus { get; set; }
    }
}
