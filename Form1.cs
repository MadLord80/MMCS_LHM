using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace MMCS_LHM
{
    public partial class Form1 : Form
    {
        private help_functions func = new help_functions();
        private HeadersInfo hi = new HeadersInfo();

        private int maxNumberOfBlinks = 5;
        private int blinkCount = 0;

        private int current_length = 0;
        private int ver_changed_id = -1;
        bool adv_mode = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fill_models();
        }

        private void fill_models()
        {
            foreach (Object[] model in this.hi.models)
            {
                ListViewItem item1 = new ListViewItem((string)model.GetValue(0));
                item1.SubItems.Add((string)model.GetValue(1));
                item1.SubItems.Add("");
                this.listView2.Items.Add(item1);
            }
            this.listView2.Items[0].Selected = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string cur_mode = this.toolStripButton1.Text;
            this.adv_mode = (cur_mode == "Simple mode") ? true : false;
            this.toolStripButton1.Text = (cur_mode == "Simple mode") ? "Advanced mode" : "Simple mode";

            this.button2.Visible = (this.adv_mode == true) ? true : false;
            this.listView2.Columns[0].Text = (this.adv_mode == true) ? "№" : "Model";
            this.listView2.Items.Clear();
            if (this.adv_mode == false) fill_models();

            if (this.hi.hnew.Length != 0) getHeaderInfo(this.hi.hnew, this.listView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                getHeader(this.openFileDialog1, this.listView1, "current");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                getHeader(this.openFileDialog2, this.listView2, "donor");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection mdonor = this.listView2.SelectedItems;
            ListView.SelectedListViewItemCollection mcurrent = this.listView1.SelectedItems;

            if (mcurrent.Count == 0 || mdonor.Count == 0)
            {
                Info("Error: please, select unit for current and donor!");
                return;
            }

            int current_id = Convert.ToInt32(mcurrent[0].SubItems[0].Text) - 1;
            if (this.adv_mode == true)
            {
                int donor_id = Convert.ToInt32(mdonor[0].SubItems[0].Text) - 1;
                this.hi.change_MII(current_id, donor_id);
            }
            else
            {
                string donor_name = mdonor[0].SubItems[1].Text;
                this.hi.change_MIIName(current_id, donor_name);
            }
            
            if (this.hi.changed_id.Length != 0) this.button8.Enabled = true;
            getHeaderInfo(this.hi.hnew, this.listView1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection mdonor = this.listView2.SelectedItems;
            ListView.SelectedListViewItemCollection mcurrent = this.listView1.SelectedItems;

            if (mcurrent.Count == 0 || mdonor.Count == 0)
            {
                Info("Error: please, select unit for current and donor!");
                return;
            }

            int current_id = Convert.ToInt32(mcurrent[0].SubItems[0].Text) - 1;
            if (this.adv_mode == true)
            {
                int donor_id = Convert.ToInt32(mdonor[0].SubItems[0].Text) - 1;
                this.hi.add_MII(current_id, donor_id);
            }
            else
            {
                string donor_name = mdonor[0].SubItems[1].Text;
                this.hi.add_MIIName(current_id, donor_name);
            }

            if (this.hi.changed_id.Length != 0) this.button8.Enabled = true;
            getHeaderInfo(this.hi.hnew, this.listView1);
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem sel_item = this.listView1.GetItemAt(e.X, e.Y);
            this.ver_changed_id = Convert.ToInt32(sel_item.SubItems[0].Text) - 1;
            ListViewItem.ListViewSubItem sel_subitem = sel_item.GetSubItemAt(e.X, e.Y);
            int subitem_id = sel_item.SubItems.IndexOf(sel_subitem);

            if (subitem_id == 1 || subitem_id == 2)
            {
                int eleft = sel_subitem.Bounds.Left + 2;
                int ewidth = sel_subitem.Bounds.Width;
                TextBox hedit = (subitem_id == 1) ? this.name_edit : this.version_edit;
                hedit.Bounds = new Rectangle(eleft + this.listView1.Left, sel_subitem.Bounds.Top + this.listView1.Top,
                    ewidth, sel_subitem.Bounds.Height);
                hedit.Text = sel_subitem.Text;
                hedit.Show();
                hedit.Focus();
            }
        }

        private void name_edit_KeyPress(object sender, KeyPressEventArgs e)
        {
            char pressed = e.KeyChar;
            if (pressed == (char)Keys.Return)
            {
                e.Handled = true;
                this.name_edit.Hide();
                if (this.ver_changed_id != -1)
                {
                    this.hi.change_MIIName(this.ver_changed_id, this.name_edit.Text);
                    getHeaderInfo(this.hi.hnew, this.listView1);
                }
            }
            else if (pressed == (char)Keys.Escape)
            {
                e.Handled = true;
                this.name_edit.Hide();
            }
        }

        private void name_edit_Leave(object sender, EventArgs e)
        {
            this.name_edit.Hide();
        }

        private void version_edit_KeyPress(object sender, KeyPressEventArgs e)
        {
            char pressed = e.KeyChar;
            if (pressed == (char)Keys.Return)
            {
                e.Handled = true;
                this.version_edit.Hide();
                if (this.ver_changed_id != -1)
                {
                    this.hi.change_MIIVersion(this.ver_changed_id, this.version_edit.Text);
                    getHeaderInfo(this.hi.hnew, this.listView1);
                }
            }
            else if (pressed == (char)Keys.Escape)
            {
                e.Handled = true;
                this.version_edit.Hide();
            }
        }

        private void version_edit_Leave(object sender, EventArgs e)
        {
            this.version_edit.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = "KWI files|*.kwi|All files (*.*)|*.*";
            this.saveFileDialog1.FilterIndex = 1;
            this.saveFileDialog1.RestoreDirectory = true;

            int blocks = this.current_length / this.hi.BLOCK_SIZE;
            byte[] block = new byte[this.hi.BLOCK_SIZE];

            this.label1.Visible = true;
            this.progressBar1.Visible = true;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = blocks;
            this.progressBar1.Value = 0;
            this.progressBar1.Step = 1;

            if (this.saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (FileStream fs = new FileStream(this.saveFileDialog1.FileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(this.hi.hnew, 0, this.hi.hnew.Length);
                    using (FileStream fs2 = new FileStream(this.openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                    {
                        for (int i = 1; i <= blocks; i++)
                        {
                            fs2.Read(block, 0, this.hi.BLOCK_SIZE);
                            if (i * this.hi.BLOCK_SIZE <= this.hi.hcurrent.Length) continue;
                            fs.Write(block, 0, this.hi.BLOCK_SIZE);
                            this.progressBar1.PerformStep();
                        }
                        fs2.Close();
                    }
                    fs.Close();
                    Info("New file created: " + Path.GetFileName(this.saveFileDialog1.FileName));
                    this.label1.Visible = false;
                    this.progressBar1.Visible = false;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HelpPage hp = new HelpPage();
            hp.Show();
            e.Cancel = true;
        }

        private void getHeader(OpenFileDialog file, ListView list, string cheader)
        {
            byte[] header = ReadBytesFromFile(file, this.hi.NUMBER_OF_MODULES_OFFSET);
            byte[] temp = new byte[this.hi.MMI_SIZE];
            if (header.Length == 0) 
            {
                Info("Error: Empty data!");
                return;
            }
            else if (header[this.hi.NUMBER_OF_MODULES_OFFSET - 1] == 0x00) 
            {
                Info("Error: No modules!");
                return;
            }

            //Number of Accommodated Modules
            int nam = header[this.hi.NUMBER_OF_MODULES_OFFSET - 1];
            
            //check is loading.kwi
            this.func.spliceByteArray(header, ref temp, 2, 2);
            int reserv = BitConverter.ToInt16(temp,0);
            if (reserv != 0 || nam > 20)
            {
                Info("Error: file does not look like loading.kwi!\r\nRESERV: " + reserv + "\tNAM: " + nam);
                return;
            }

            //calculate header size
            int header_length = 20 + nam * this.hi.MII_SIZE + nam * this.hi.MMI_SIZE;
            int num_of_blocks = header_length / this.hi.BLOCK_SIZE;
            int offset_block = header_length % this.hi.BLOCK_SIZE;
            if (offset_block > 0) {
                header_length = (num_of_blocks + 1) * this.hi.BLOCK_SIZE;
            }
            else if (header_length > 1) {
                header_length = num_of_blocks * this.hi.BLOCK_SIZE;
            }
            else {
                header_length = this.hi.BLOCK_SIZE;
            }

            header = ReadBytesFromFile(file, header_length);
            if (cheader == "current")
            {
                this.hi.set_hcurrent(header);
                using (FileStream fs = new FileStream(file.FileName, FileMode.Open, FileAccess.Read))
                {
                    this.current_length = (int)fs.Length;
                    fs.Close();
                }
            }
            else
            {
                this.hi.set_hdonor(header);
            }
            getHeaderInfo(header, list);

            if (this.hi.hnew.Length != 0)
            {
                this.button3.Enabled = true;
                this.button4.Enabled = true;
            }
        }

        private void getHeaderInfo(byte[] header, ListView view)
        {
            view.Items.Clear();
            byte[] temp = new byte[this.hi.MMI_SIZE];
            int nam = header[this.hi.NUMBER_OF_MODULES_OFFSET - 1];
            //MODULES
            for (int i = 0; i < nam; i++)
            {
                //Module Identification Information
                int mii_offset = 20 + i * this.hi.MII_SIZE;

                this.func.spliceByteArray(header, ref temp, mii_offset + 4, 52);
                string mname = this.func.ByteArrayToString(temp);
                
                if (this.adv_mode == false && mname.Contains("MLD") == false) continue;

                this.func.spliceByteArray(header, ref temp, mii_offset + 56, 8);
                string mver = this.func.ByteArrayToString(temp);

                //Module Management Information
                int mstart = 0; int mlength = 0;
                if (view.Name == "listView1")
                {
                    int mmi_offset = 20 + nam * this.hi.MII_SIZE + i * this.hi.MMI_SIZE;

                    this.func.spliceByteArray(header, ref temp, mmi_offset + 68, 4);
                    mstart = this.func.ByteArrayLEToInt(temp);
                    mstart += mstart;
                    this.func.spliceByteArray(header, ref temp, mmi_offset + 72, 4);
                    mlength = this.func.ByteArrayLEToInt(temp);
                }

                ListViewItem item1 = new ListViewItem((i + 1).ToString());
                if (view.Name == "listView1" && Array.IndexOf(this.hi.changed_id, i) != -1) item1.ForeColor = Color.Red;
                item1.SubItems.Add(mname);
                item1.SubItems.Add(mver);
                if (view.Name == "listView1")
                {
                    item1.SubItems.Add(String.Format("{0:X}", mstart));
                    item1.SubItems.Add(mlength.ToString());
                }
                view.Items.Add(item1);
            }
            view.Items[0].Selected = true;
        }

        private byte[] ReadBytesFromFile(OpenFileDialog file, int length)
        {
            byte[] outarr = new byte[length];
            using (FileStream fs = new FileStream(file.FileName, FileMode.Open, FileAccess.Read))
            {
                fs.Read(outarr, 0, length);
                fs.Close();
            }
            return outarr;
        }

        private void Info(String text)
        {
            this.textBox1.Text = text;

            this.timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.blinkCount++;
            int odd = this.blinkCount % 2;
            this.textBox1.Font = (odd == 0) ? new Font(this.textBox1.Font, FontStyle.Bold) : new Font(this.textBox1.Font, FontStyle.Regular);
            if (this.blinkCount == this.maxNumberOfBlinks)
            {
                this.blinkCount = 0;
                this.timer1.Stop();
            }
        }
    }
}
