using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;

namespace SMProject
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FrmLogin frmLogin = new FrmLogin();
            DialogResult dialogResult = frmLogin.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                Application.Run(new FrmSaleManage());
            }
            else
            {
                Application.Exit();
            }
        }

        public static SalesPerson currentSalesPerson = null;
    }
}
