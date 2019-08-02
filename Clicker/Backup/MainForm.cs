using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Collections;

namespace Clicker
{
    //[Flags]
    public enum ClickType
    {
        click = 0,
        rightClick = 1 ,
        doubleClick = 2 ,
        SendKeys = 3
    }
    public partial class MainForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);
        
        #region Fields
        private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
        private const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040; /* middle button up */
        private const int MOUSEEVENTF_XDOWN = 0x0080; /* x button down */
        private const int MOUSEEVENTF_XUP = 0x0100; /* x button down */
        private const int MOUSEEVENTF_WHEEL = 0x0800; /* wheel button rolled */
        private const int MOUSEEVENTF_VIRTUALDESK = 0x4000; /* map to entire virtual desktop */
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000; /* absolute move */

        private SynchronizationContext context = null;
        private DateTime start, end;
        private bool first = true;
        private List<ActionEntry> actions;
        private Thread runActionThread;
        private bool byTextEntry;
        private Hashtable schedualeList;
        #endregion

        #region Construction
        public MainForm()
        {
            InitializeComponent();
            context = SynchronizationContext.Current;
            actions = new List<ActionEntry>();
            schedualeList = new Hashtable();
        }
        #endregion

        #region Private Methods

        private void RunAction()
        {
            foreach (ActionEntry action in actions)
            {
                if (action.Type.Equals(ClickType.SendKeys))
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(WorkSendKeys), action);
                }
                else// if (entry is ClickEntry)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(WorkClick), action);
                }

                int tmpIntervl = action.Interval.Equals(0) ? 0 : action.Interval * 1000 - 100;
                Thread.Sleep(tmpIntervl);
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkEnableButtons), null);
        }
        private void WorkSendKeys(object state)
        {
            this.context.Send(new SendOrPostCallback(delegate(object _state)
            {
                ActionEntry action = state as ActionEntry;
                SendKeys.Send(action.Text);
            }), state);
        }
        private void WorkClick(object state)
        {
            this.context.Send(new SendOrPostCallback(delegate(object _state)
            {
                ActionEntry action = state as ActionEntry;
                SetCursorPos(action.X, action.Y);
                Thread.Sleep(100);
                if (action.Type.Equals(ClickType.click))
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                else if (action.Type.Equals(ClickType.doubleClick))
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                    Thread.Sleep(100);
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }
                else //if (action.Type.Equals(ClickType.rightClick))
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                }
            }), state);
        }
        private void WorkEnableButtons(object state)
        {
            this.context.Send(new SendOrPostCallback(delegate(object _state)
            {
                enableButtons(true);
            }), state);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (byTextEntry) return;

            if (e.KeyChar.Equals('c') || e.KeyChar.Equals('d')
                || e.KeyChar.Equals('r') || e.KeyChar.Equals('t'))
            {
                end = DateTime.Now;
                if (first)
                {
                    start = end;
                    first = false;
                }

                ClickType ct = ClickType.click;
                if (e.KeyChar.Equals('c'))
                {
                    //cl = ClickType.click;
                }
                else if (e.KeyChar.Equals('d'))
                {
                    ct = ClickType.doubleClick;
                }
                else if (e.KeyChar.Equals('r'))
                {
                    ct = ClickType.rightClick;
                }
                else //if (e.KeyChar.Equals('t'))
                {
                    ct = ClickType.SendKeys;
                }

                int x = Cursor.Position.X;
                int y = Cursor.Position.Y;
                TimeSpan ts = end - start;
                double sec = 0;
                if (nWait.Value.Equals(0))
                {
                    sec = ts.TotalSeconds;
                    sec = Math.Round(sec, 1);
                }
                else
                {
                    sec = (double)nWait.Value;
                }
                start = end;
                string point = x.ToString() + "," + y.ToString();

                string text = ct.Equals(ClickType.SendKeys) ? txbEntry.Text : string.Empty;
                ListViewItem lvi = new ListViewItem(new string[] { point, ct.ToString(), "0", text });
                ActionEntry acion = new ActionEntry(x, y, text, 0, ct);
                lvi.Tag = acion;
                lvActions.Items.Add(lvi);
                int index = lvActions.Items.Count;
                if (index > 1)
                {
                    lvActions.Items[index - 2].SubItems[2].Text = sec.ToString();
                    (lvActions.Items[index - 2].Tag as ActionEntry).Interval = (int)sec;
                }
            }
            if (e.KeyChar.Equals('S'))
            {
                btnStart.PerformClick();
            }
            else if (e.KeyChar.Equals((char)Keys.Escape))//Esc
            {
                btnCancel.PerformClick();
                this.Focus();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lvActions.Items.Clear();
            first = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            enableButtons(false);

            if (runActionThread == null || !runActionThread.IsAlive)
            {
                actions.Clear();
                foreach (ListViewItem lvi in lvActions.Items)
                {
                    actions.Add(lvi.Tag as ActionEntry);
                }
                runActionThread = new Thread(RunAction);
                runActionThread.Start();
            }

        }
        private void enableButtons(bool enabel)
        {
            btnClear.Enabled = enabel;
            btnOpen.Enabled = enabel;
            btnSave.Enabled = enabel;
            lvActions.Enabled = enabel;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (runActionThread != null && runActionThread.IsAlive)
            {
                runActionThread.Abort();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (runActionThread != null && runActionThread.IsAlive)
            {
                runActionThread.Abort();
                enableButtons(true);
            }
        }

        private void txbEntry_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((Char)Keys.Escape))//Esc
            {
                nWait.Focus();
            }
        }

        private void txbEntry_Enter(object sender, EventArgs e)
        {
            byTextEntry = true;
        }

        private void txbEntry_Leave(object sender, EventArgs e)
        {
            byTextEntry = false;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int cout = lvActions.Items.Count;
            int coutselect = lvActions.SelectedItems.Count;
            if (cout.Equals(coutselect))
            {
                btnClear.PerformClick();
            }
            else
            {
                for (int i = coutselect - 1; i >= 0; --i)
                {
                    int index = lvActions.SelectedItems[i].Index;
                    lvActions.Items[index].Remove();
                }
            }
        }

        private void lvActions_MouseDown(object sender, MouseEventArgs e)
        {
            int coutselect = lvActions.SelectedItems.Count;
            deleteToolStripMenuItem.Available = coutselect > 0;
            editToolStripMenuItem.Available = coutselect == 1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog();
            file.Filter = "XML File |*.xml";
            if (file.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer ser = new XmlSerializer(typeof(ActionsEntry));
                ActionsEntry tmpAction = new ActionsEntry();
                List<ActionsEntryAction> tmpActionsEntryActions = new List<ActionsEntryAction>();
                foreach (ListViewItem lvi in lvActions.Items)
                {
                    ActionEntry tmpActionEntry = lvi.Tag as ActionEntry;
                    ActionsEntryAction tmpActionsEntryAction = new ActionsEntryAction();
                    tmpActionsEntryAction.X = tmpActionEntry.X;
                    tmpActionsEntryAction.Y = tmpActionEntry.Y;
                    tmpActionsEntryAction.Text = tmpActionEntry.Text;
                    tmpActionsEntryAction.interval = tmpActionEntry.Interval;
                    tmpActionsEntryAction.Type = (int)tmpActionEntry.Type;
                    tmpActionsEntryActions.Add(tmpActionsEntryAction);
                }
                tmpAction.Action = tmpActionsEntryActions.ToArray();

                using (XmlWriter writer = XmlWriter.Create(file.FileName))
                {
                    ser.Serialize(writer, tmpAction);
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            bool runIt = false;
            if (MessageBox.Show("After openning configuration, are you want to run it?", "Clicker", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                 == DialogResult.Yes)
            {
                runIt = true;
            }
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "XML File |*.xml";
            file.Multiselect = false;
            if (file.ShowDialog() == DialogResult.OK)
            {
                OpenFileXml(runIt, file.FileName);
                string name = file.SafeFileName;
                this.Text = "Clicer - " + name.Substring(0, name.Length - 4);
            }
        }

        private void OpenFileXml(bool runIt, string file)
        {
            //Get data from XML file
            XmlSerializer ser = new XmlSerializer(typeof(ActionsEntry));
            using (FileStream fs = System.IO.File.Open(file, FileMode.Open))
            {
                try
                {
                    ActionsEntry entry = (ActionsEntry)ser.Deserialize(fs);
                    lvActions.Items.Clear();
                    foreach (ActionsEntryAction ae in entry.Action)
                    {
                        string point = ae.X.ToString() + "," + ae.Y.ToString();
                        string interval = (ae.interval).ToString();
                        ListViewItem lvi = new ListViewItem(new string[] { point, ((ClickType)(ae.Type)).ToString(), interval, ae.Text });
                        ActionEntry acion = new ActionEntry(ae.X, ae.Y, ae.Text, ae.interval, (ClickType)(ae.Type));
                        lvi.Tag = acion;
                        lvActions.Items.Add(lvi);
                    }
                    
                    if (runIt)
                    {
                        btnStart.PerformClick();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Clicer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
       
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActionEntry action = lvActions.SelectedItems[0].Tag as ActionEntry;
            EditWin frm = new EditWin(action);
            frm.Actionentry = action;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                action = frm.Actionentry;
                lvActions.SelectedItems[0].Tag = action;
                lvActions.SelectedItems[0].Text = action.X + "," + action.Y;
                lvActions.SelectedItems[0].SubItems[1].Text = action.Type.ToString();
                lvActions.SelectedItems[0].SubItems[2].Text = action.Interval.ToString();
                lvActions.SelectedItems[0].SubItems[3].Text = action.Text;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.linkLabel.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.aspx");

        }

        private void lvActions_DoubleClick(object sender, EventArgs e)
        {
            if (editToolStripMenuItem.Available)
            {
                editToolStripMenuItem.PerformClick();
            }
        }
        #endregion
    }
}
