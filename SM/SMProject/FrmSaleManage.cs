﻿using System;
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
    public partial class FrmSaleManage : Form
    {
        public FrmSaleManage()
        {
            InitializeComponent();
            //展示当前销售人员信息
            this.lblSalePerson.Text = Program.currentSalesPerson.SPName;


        }
    }
}
