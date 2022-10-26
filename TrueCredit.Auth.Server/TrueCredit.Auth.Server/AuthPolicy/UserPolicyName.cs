using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueCredit.Auth.Server
{
    public static class UserPolicyName
    {
        //=======================================业务权限======================
        /// <summary>
        /// 能调用应用一般监测功能查询各种信息的权限
        /// </summary>
        public const string User = "user";
        /// <summary>
        /// 管理员可以管理一些一般用户
        /// </summary>
        public const string Administrator = "administrator";
        /// <summary>
        /// 可以管理包括管理员在内的用户
        /// </summary>
        public const string MasterAdministrator = "masterAministrator";
    }
}
