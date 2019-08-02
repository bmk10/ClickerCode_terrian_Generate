using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Clicker
{
    public partial class EditWin : Form
    {
        #region Fields
        private ActionEntry action;
        #endregion

        #region Construction
        public EditWin(ActionEntry action)
        {
            InitializeComponent();
            this.action = action;
        }
        #endregion

        #region Private Methods
        private void EditWin_Load(object sender, EventArgs e)
        {
            if (action != null)
            {
                txbX.Text = action.X.ToString();
                txbY.Text = action.Y.ToString();
                txbText.Text = action.Text.ToString();
                nWait.Value = action.Interval;
                cbType.SelectedIndex = (int)action.Type;
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            action.X = Int32.Parse(txbX.Text);
            action.Y =  Int32.Parse(txbY.Text);
            action.Text = txbText.Enabled ? txbText.Text : string.Empty;
            action.Interval = (int)nWait.Value;
            action.Type = (ClickType)cbType.SelectedIndex;
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region Public methods
        public ActionEntry Actionentry
        {
            set { action = value; }
            get { return action; }
        }
        #endregion

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txbText.Enabled = cbType.SelectedIndex.Equals(3);
        }

        

        

        
    }
}
