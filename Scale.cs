﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Termie
{
    public partial class Scale : Form
    {
        Font origFont;
        Font monoFont;
        Color receivedColor = Color.Green;
        Color sentColor = Color.Blue;
        //private string m_old_value = "";
        private string m_Units = "KG";

        private int m_selected_ID_MH;
        public Scale()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            SqlHelper.Initialize();
            Settings.Read();
            // let form use multiple fonts
            origFont = Font;
            FontFamily ff = new FontFamily("Courier New");
            monoFont = new Font(ff, 8, FontStyle.Regular);
            Font = Settings.Option.MonoFont ? monoFont : origFont;
            CommPort com = CommPort.Instance;
            com.StatusChanged += OnStatusChanged;
            com.DataReceived += OnDataReceived;
            com.Open();


            //string a = Uri.EscapeDataString(m_com_name);            
            //MessageBox.Show(a);
            //string b = Uri.UnescapeDataString(a);
            //MessageBox.Show(b);

        }

        private void Scale_Load(object sender, EventArgs e)
        {
            SetSaveButtonIcon();
            SetPrintButtonIcon();

            //Get all available printers and add them to the combo box
            //foreach (String printer in PrinterSettings.InstalledPrinters)
            //{
            //    Settings.s_printer_name = printer.ToString();
            //    break;
            //}
            //MessageBox.Show(Settings.s_printer_name);


            if (File.Exists("personalized.ini"))
            {
                Settings.Read_personalized();
                this.lbl_App_Name.Text = Uri.UnescapeDataString(Settings.personalized.s_app_name);
                this.lbl_Scale_Com_NameA.Text = Uri.UnescapeDataString(Settings.personalized.s_com_name_A);
                this.lbl_Detail_COMA.Text = Uri.UnescapeDataString(Settings.personalized.s_com_detail_A) + Uri.UnescapeDataString(Settings.personalized.s_com_detail_A_2);
                this.lbl_Scale_Com_NameB.Text = Uri.UnescapeDataString(Settings.personalized.s_com_name_B);
                this.lbl_Detail_COMB.Text = Uri.UnescapeDataString(Settings.personalized.s_com_detail_B) + Uri.UnescapeDataString(Settings.personalized.s_com_detail_B_2); ;
            }
            else
            {
                this.lbl_App_Name.Text = "HỆ THỐNG CÂN KỸ THUẬT CƯỜNG THỊNH PHÁT";
                this.lbl_Scale_Com_NameA.Text = "CTY TNHH CÂN ĐIỆN TỬ CƯỜNG THỊNH PHÁT CTPS ELECTRONIC SCALE CO., LTD.";
                this.lbl_Detail_COMA.Text = "Đ/c: 603 QL13 - P.Hiệp Bình Chánh - Q.Thủ Đức - TP.HCM.\n" +
                                              "ĐT: (028) 66.544.865 – 0977.975.865 – 0903.665.012 //Kế Toán: 0167 951 5828.\n" +
                                              "Giờ làm việc: 8:00h – 17:00h.\n" +
                                              "Sau 17:00h Liên hệ: (028) 66.544.865.";
                this.lbl_Scale_Com_NameB.Text = "CTY TNHH CÂN ĐIỆN TỬ CƯỜNG THỊNH PHÁT CTPS ELECTRONIC SCALE CO., LTD.";
                this.lbl_Detail_COMB.Text = "Đ/c: 603 QL13 - P.Hiệp Bình Chánh - Q.Thủ Đức - TP.HCM.\n" +
                                              "ĐT: (028) 66.544.865 – 0977.975.865 – 0903.665.012 //Kế Toán: 0167 951 5828.\n" +
                                              "Giờ làm việc: 8:00h – 17:00h.\n" +
                                              "Sau 17:00h Liên hệ: (028) 66.544.865.";
                int subA = lbl_Detail_COMA.Text.Length / 2;
                int subB = lbl_Detail_COMB.Text.Length / 2;
                string detailA1 = lbl_Detail_COMA.Text.Substring(0, subA);
                string detailA2 = lbl_Detail_COMA.Text.Substring(subA, lbl_Detail_COMA.Text.Length - detailA1.Length);
                string detailB1 = lbl_Detail_COMB.Text.Substring(0, subB);
                string detailB2 = lbl_Detail_COMB.Text.Substring(subB, lbl_Detail_COMB.Text.Length - detailB1.Length);
                Settings.personalized.s_app_name = Uri.EscapeDataString(this.lbl_App_Name.Text);
                Settings.personalized.s_com_name_A = Uri.EscapeDataString(this.lbl_Scale_Com_NameA.Text);
                Settings.personalized.s_com_detail_A = Uri.EscapeDataString(detailA1);
                Settings.personalized.s_com_detail_A_2 = Uri.EscapeDataString(detailA2);
                Settings.personalized.s_com_name_B = Uri.EscapeDataString(this.lbl_Scale_Com_NameB.Text);
                Settings.personalized.s_com_detail_B = Uri.EscapeDataString(detailB1);
                Settings.personalized.s_com_detail_B_2 = Uri.EscapeDataString(detailB2);
                Settings.Write_personalized();
            }
            //string[] listformat = CultureInfo.CurrentUICulture.DateTimeFormat.GetAllDateTimePatterns();
            //string printt = "";
            //for (int i = 0; i < listformat.Length; i++)
            //{
            //    printt += "   ";
            //    printt += listformat[i];
            //}
            //MessageBox.Show(printt);
            SqlHelper.get10Values();
            this.dataGridView1.DataSource = SqlHelper.s_table_10Models;
            datagridview1_setHeaderName();

            this.logo.Image = Image.FromFile("./logo.png");
            //this.lbl_App_Name.Text = m_com_name;
            //this.lbl_Scale_Com_NameA.Text = "CTY TNHH CÂN ĐIỆN TỬ CƯỜNG THỊNH PHÁT CTPS ELECTRONIC SCALE CO., LTD.";
            //this.lbl_Detail_COMB.Text = "Đ/c: 603 QL13 - P.Hiệp Bình Chánh - Q.Thủ Đức - TP.HCM.";
            //this.lbl_contact1.Text = "ĐT: (028) 66.544.865 – 0977.975.865 – 0903.665.012 // Kế Toán: 0167 951 5828";
            //this.lbl_detail.Text = "Giờ làm việc: 8:00h – 17:00h.";
            //this.lbl_detail2.Text = "Sau 17:00h Liên hệ: (028) 66.544.865.";


            MatHangManager.init();
            this.dataGridView2.DataSource = MatHangManager.s_DanhSachMatHang;
            this.dataGridView2.Columns["Name"].HeaderText = "Tên";
            this.dataGridView2.Columns["Price"].HeaderText = "Giá";
            this.dataGridView2.Columns["HotKey"].HeaderText = "Phím tắt";
            this.dataGridView2.Columns["ID"].Visible = false;
            this.m_selected_ID_MH = int.Parse(dataGridView2.SelectedRows[0].Cells["ID"].Value.ToString());
            this.lbl_MatHang.Text = dataGridView2.SelectedRows[0].Cells["Name"].Value.ToString();


            //Tải lên danh sách mặt hàng
        }

        // delegate used for Invoke
        internal delegate void StringDelegate(string data);
        private void datagridview1_setHeaderName()
        {
            this.dataGridView1.Columns["ID"].Visible = false;
            this.dataGridView1.Columns["Name"].HeaderText = "Tên";
            this.dataGridView1.Columns["Value"].HeaderText = "Giá trị";
            this.dataGridView1.Columns["Unit"].HeaderText = "Đơn vị";
            this.dataGridView1.Columns["DateTime"].HeaderText = "Ngày lưu";
            this.dataGridView1.Columns["ThanhTien"].HeaderText = "Thành tiền";
        }
        /// <summary>
        /// Update the connection status
        /// </summary>
        public void OnStatusChanged(string status)
        {
            //Handle multi-threading
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnStatusChanged), new object[] { status });
                return;
            }

            this.txb_port.Text = status;
        }
        private bool m_isFullLine = false;
        /// <summary>
		/// Handle data received event from serial port.
		/// </summary>
		/// <param name="data">incoming data</param>
		public void OnDataReceived(string dataIn)
        {
            //Handle multi-threading
            if (InvokeRequired)
            {
                Invoke(new StringDelegate(OnDataReceived), new object[] { dataIn });
                return;
            }

            // if we detect a line terminator, add line to output
            int index;
            while (dataIn.Length > 0 &&
                ((index = dataIn.IndexOf("\r")) != -1 ||
                (index = dataIn.IndexOf("\n")) != -1))
            {
                String StringIn = dataIn.Substring(0, index);
                dataIn = dataIn.Remove(0, index + 1);

                m_isFullLine = true;
                AddData(StringIn);                
                //logFile_writeLine(StringIn);      
                
                partialLine = null;	// terminate partial line
            }
            // if we have data remaining, add a partial line
            if (dataIn.Length > 0)
            {
                partialLine = AddData(dataIn);
            }
            // if we have data remaining, add a partial line
            //if (dataIn.Length > 0)
            //{
            //    PrintData(dataIn);
            //}
        }

        /// <summary>
        /// Add data to the output.
        /// </summary>
        /// <param name="StringIn"></param>
        /// <returns></returns>
        private void PrintData(String StringIn)
        {
            //String StringOut = StringIn.Substring(1);
            //System.Threading.Thread.Sleep(20);
            if (m_isFullLine)
            {
                string t_value = StringIn.Split(',')[2];
                string value = t_value;
                foreach (char c in t_value)
                {
                    if (c == 32)
                    {
                        value = value.Remove(0, 1);
                    }
                    else
                    {
                        break;
                    }
                }
                if (value.Contains(' '))
                {
                    this.m_Units = value.Split(' ')[1];
                    if (this.m_Units.Length > 2)
                    {
                        this.m_Units = this.m_Units.Substring(0,2);
                    }
                    this.lbl_Values.Text = value.Split(' ')[0];
                    this.lbl_Values.ForeColor = receivedColor;
                }
                else
                {
                    if (value.Contains('t'))
                    {
                        this.m_Units = value.Substring(value.IndexOf('t'),4);
                        if (this.m_Units.Length > 2)
                        {
                            this.m_Units = this.m_Units.Substring(0, 2);
                        }
                        this.lbl_Values.Text = value.Substring(0,value.IndexOf('t'));
                        this.lbl_Values.ForeColor = receivedColor;
                    }
                }
                this.lbl_Unit.Text = this.m_Units;
                m_isFullLine = false;
            }

        }

        /// <summary>
        /// Prepare a string for output by converting non-printable characters.
        /// </summary>
        /// <param name="StringIn">input string to prepare.</param>
        /// <returns>output string.</returns>
        private String PrepareData(String StringIn)
        {
            // The names of the first 32 characters
            string[] charNames = { "NUL", "SOH", "STX", "ETX", "EOT",
                "ENQ", "ACK", "BEL", "BS", "TAB", "LF", "VT", "FF", "CR", "SO", "SI",
                "DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN", "ETB", "CAN", "EM", "SUB",
                "ESC", "FS", "GS", "RS", "US", "Space"};

            string StringOut = "";

            foreach (char c in StringIn)
            {
                if (Settings.Option.HexOutput)
                {
                    StringOut = StringOut + String.Format("{0:X2} ", (int)c);
                }
                else if (c < 32 && c != 9)
                {
                    StringOut = StringOut + "<" + charNames[c] + ">";

                    //Uglier "Termite" style
                    //StringOut = StringOut + String.Format("[{0:X2}]", (int)c);
                }
                else
                {
                    StringOut = StringOut + c;
                }
            }
            return StringOut;
        }
        private void btn_Setting_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Scale_FormClosing(object sender, FormClosingEventArgs e)
        {
            CommPort com = CommPort.Instance;
            if (com.IsOpen)
            {

                e.Cancel = true; //cancel the fom closing

                Thread CloseDown = new Thread(new ThreadStart(CloseSerialOnExit)); //close port in new thread to avoid hang

                CloseDown.Start(); //close port in new thread to avoid hang

            }
        }
        private void CloseSerialOnExit()
        {
            CommPort com = CommPort.Instance;
            try
            {
                com.Close(); //close the serial port
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); //catch any serial port closing error messages
            }
            this.Invoke(new EventHandler(NowClose)); //now close back in the main thread

        }

        private void NowClose(object sender, EventArgs e)
        {
            this.Close(); //now close the form
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //string value1;
            //NumberStyles style;
            //CultureInfo culture;
            //double number;
            //value1 = "1,345,978";
            //style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            //culture = CultureInfo.CreateSpecificCulture("en-US");
            //if (Double.TryParse(value1, style, culture, out number))
            //    Console.WriteLine("Converted '{0}' to {1}.", value1, number);
            //else
            //    Console.WriteLine("Unable to convert '{0}'.", value1);

            DataModel model = new DataModel();
            string value = this.lbl_Values.Text;           
            float fvalue;
            bool isValidValue = float.TryParse(value,
                             NumberStyles.Float | NumberStyles.AllowThousands,
                             CultureInfo.GetCultureInfo("en-US"),
                             out fvalue);
            //MessageBox.Show(temp.ToString());
            if (isValidValue)
            {
                model.m_name = this.lbl_MatHang.Text;
                model.m_unit = this.m_Units;
                model.m_value = value;
                //string a = this.dataGridView2.SelectedRows[0].Cells["Price"].Value.ToString();
                string price = this.dataGridView2.SelectedRows[0].Cells["Price"].Value.ToString();
                int ivalue;
                bool _isValidValue = int.TryParse(price,
                                    NumberStyles.Integer | NumberStyles.AllowThousands,
                                    CultureInfo.GetCultureInfo("en-US"),
                                    out ivalue);
                model.m_thanhTien = ivalue * fvalue;
                SqlHelper.InsertValue(model);
                SqlHelper.get10Values();
               
                MessageBox.Show(this.lbl_MatHang.Text + ": " + model.m_value.ToString() + " " + this.m_Units + "\nThành Tiền: " + model.m_thanhTien.ToString("#,0.###"));
                this.dataGridView1.DataSource = SqlHelper.s_table_10Models;
                datagridview1_setHeaderName();
            }
            else
            {
                MessageBox.Show("Giá trị cân không chính xác");
            }
        }

        private void btn_report_Click(object sender, EventArgs e)
        {
            form_Report report = new form_Report();
            report.FormClosed += report_Close;
            report.ShowDialog();
        }
        public void report_Close(object sender, FormClosedEventArgs e)
        {
            SqlHelper.get10Values();
            this.dataGridView1.DataSource = SqlHelper.s_table_10Models;
            datagridview1_setHeaderName();

        }
        public void form_personalized_closed(object sender, FormClosedEventArgs e)
        {
            Settings.Read_personalized();
            this.lbl_App_Name.Text = Uri.UnescapeDataString(Settings.personalized.s_app_name);
            this.lbl_Scale_Com_NameA.Text = Uri.UnescapeDataString(Settings.personalized.s_com_name_A);
            this.lbl_Detail_COMA.Text = Uri.UnescapeDataString(Settings.personalized.s_com_detail_A) + Uri.UnescapeDataString(Settings.personalized.s_com_detail_A_2);
            this.lbl_Scale_Com_NameB.Text = Uri.UnescapeDataString(Settings.personalized.s_com_name_B);
            this.lbl_Detail_COMB.Text = Uri.UnescapeDataString(Settings.personalized.s_com_detail_B) + Uri.UnescapeDataString(Settings.personalized.s_com_detail_B_2); ;

        }

        private class Line
        {
            public string Str;
            public Color ForeColor;

            public Line(string str, Color color)
            {
                Str = str;
                ForeColor = color;
            }
        };

        /// <summary>
		/// Partial line for AddData().
		/// </summary>
		private Line partialLine = null;

        /// <summary>
		/// Add data to the output.
		/// </summary>
		/// <param name="StringIn"></param>
		/// <returns></returns>
		private Line AddData(String StringIn)
        {
            String StringOut = PrepareData(StringIn);

            // if we have a partial line, add to it.
            if (partialLine != null)
            {
                // tack it on
                partialLine.Str = partialLine.Str + StringOut;
                
                PrintData(partialLine.Str);

                return partialLine;
            }
            m_isFullLine = false;
            return new Line(StringIn, receivedColor);
        }

        private void Scale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8 && e.Modifiers == Keys.Control)
            {
                form_personalized frm = new form_personalized();
                frm.FormClosed += form_personalized_closed;
                frm.ShowDialog();
                //MessageBox.Show("Ctrl + F8");
                //do stuff
            }
            if (e.Modifiers == Keys.None)
            {
               
                switch (e.KeyCode)
                {
                    case Keys.NumPad1:
                        //MessageBox.Show("11111");
                        for (int i = 0; i < this.dataGridView2.RowCount; i++)
                        {
                            if (int.Parse(this.dataGridView2.Rows[i].Cells["HotKey"].Value.ToString()) == 1)
                            {
                                this.lbl_MatHang.Text = this.dataGridView2.Rows[i].Cells["Name"].Value.ToString();
                                this.m_selected_ID_MH = int.Parse(this.dataGridView2.Rows[i].Cells["ID"].Value.ToString());
                                dataGridView2.ClearSelection();
                                this.dataGridView2.Rows[i].Selected = true;
                            }
                        }
                        break;
                    case Keys.NumPad2:
                        //MessageBox.Show("22222222");
                        for (int i = 0; i < this.dataGridView2.RowCount; i++)
                        {
                            if (int.Parse(this.dataGridView2.Rows[i].Cells["HotKey"].Value.ToString()) == 2)
                            {
                                this.lbl_MatHang.Text = this.dataGridView2.Rows[i].Cells["Name"].Value.ToString();
                                this.m_selected_ID_MH = int.Parse(this.dataGridView2.Rows[i].Cells["ID"].Value.ToString());
                                dataGridView2.ClearSelection();
                                this.dataGridView2.Rows[i].Selected = true;
                            }
                        }
                        break;
                    case Keys.NumPad3:
                        for (int i = 0; i < this.dataGridView2.RowCount; i++)
                        {
                            if (int.Parse(this.dataGridView2.Rows[i].Cells["HotKey"].Value.ToString()) == 3)
                            {
                                this.lbl_MatHang.Text = this.dataGridView2.Rows[i].Cells["Name"].Value.ToString();
                                this.m_selected_ID_MH = int.Parse(this.dataGridView2.Rows[i].Cells["ID"].Value.ToString());
                                dataGridView2.ClearSelection();
                                this.dataGridView2.Rows[i].Selected = true;
                            }
                        }
                        break;
                    case Keys.NumPad4:
                        for (int i = 0; i < this.dataGridView2.RowCount; i++)
                        {
                            if (int.Parse(this.dataGridView2.Rows[i].Cells["HotKey"].Value.ToString()) == 4)
                            {
                                this.lbl_MatHang.Text = this.dataGridView2.Rows[i].Cells["Name"].Value.ToString();
                                this.m_selected_ID_MH = int.Parse(this.dataGridView2.Rows[i].Cells["ID"].Value.ToString());
                                dataGridView2.ClearSelection();
                                this.dataGridView2.Rows[i].Selected = true;
                            }
                        }
                        break;
                    case Keys.NumPad5:
                        for (int i = 0; i < this.dataGridView2.RowCount; i++)
                        {
                            if (int.Parse(this.dataGridView2.Rows[i].Cells["HotKey"].Value.ToString()) == 5)
                            {
                                this.lbl_MatHang.Text = this.dataGridView2.Rows[i].Cells["Name"].Value.ToString();
                                this.m_selected_ID_MH = int.Parse(this.dataGridView2.Rows[i].Cells["ID"].Value.ToString());
                                dataGridView2.ClearSelection();
                                this.dataGridView2.Rows[i].Selected = true;
                            }
                        }
                        break;
                    case Keys.NumPad6:
                        for (int i = 0; i < this.dataGridView2.RowCount; i++)
                        {
                            if (int.Parse(this.dataGridView2.Rows[i].Cells["HotKey"].Value.ToString()) == 6)
                            {
                                this.lbl_MatHang.Text = this.dataGridView2.Rows[i].Cells["Name"].Value.ToString();
                                this.m_selected_ID_MH = int.Parse(this.dataGridView2.Rows[i].Cells["ID"].Value.ToString());
                                dataGridView2.ClearSelection();
                                this.dataGridView2.Rows[i].Selected = true;
                            }
                        }
                        break;
                    case Keys.NumPad7:
                        for (int i = 0; i < this.dataGridView2.RowCount; i++)
                        {
                            if (int.Parse(this.dataGridView2.Rows[i].Cells["HotKey"].Value.ToString()) == 7)
                            {
                                this.lbl_MatHang.Text = this.dataGridView2.Rows[i].Cells["Name"].Value.ToString();
                                this.m_selected_ID_MH = int.Parse(this.dataGridView2.Rows[i].Cells["ID"].Value.ToString());
                                dataGridView2.ClearSelection();
                                this.dataGridView2.Rows[i].Selected = true;
                            }
                        }
                        break;
                    case Keys.NumPad8:
                        for (int i = 0; i < this.dataGridView2.RowCount; i++)
                        {
                            if (int.Parse(this.dataGridView2.Rows[i].Cells["HotKey"].Value.ToString()) == 8)
                            {
                                this.lbl_MatHang.Text = this.dataGridView2.Rows[i].Cells["Name"].Value.ToString();
                                this.m_selected_ID_MH = int.Parse(this.dataGridView2.Rows[i].Cells["ID"].Value.ToString());
                                dataGridView2.ClearSelection();
                                this.dataGridView2.Rows[i].Selected = true;
                            }
                        }
                        break;
                    case Keys.NumPad9:
                        for (int i = 0; i < this.dataGridView2.RowCount; i++)
                        {
                            if (int.Parse(this.dataGridView2.Rows[i].Cells["HotKey"].Value.ToString()) == 9)
                            {
                                this.lbl_MatHang.Text = this.dataGridView2.Rows[i].Cells["Name"].Value.ToString();
                                this.m_selected_ID_MH = int.Parse(this.dataGridView2.Rows[i].Cells["ID"].Value.ToString());
                                dataGridView2.ClearSelection();
                                this.dataGridView2.Rows[i].Selected = true;
                            }
                        }
                        break;
                    case Keys.Space:
                        btn_Save_Click(sender, e);
                        break;
                }
               
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ThemXoaSuaMatHang frm_TXSMH = new ThemXoaSuaMatHang();
            frm_TXSMH.FormClosed += TXSMH_closed;
            frm_TXSMH.ShowDialog();
        }
        public void TXSMH_closed(object sender, FormClosedEventArgs e)
        {
            MatHangManager.refresh();
            this.dataGridView2.DataSource = MatHangManager.s_DanhSachMatHang;
            dataGridView2.ClearSelection();
            this.dataGridView2.Rows[0].Selected = true;
            this.m_selected_ID_MH = int.Parse(dataGridView2.SelectedRows[0].Cells["ID"].Value.ToString());
            this.lbl_MatHang.Text = dataGridView2.SelectedRows[0].Cells["Name"].Value.ToString();
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.m_selected_ID_MH = int.Parse(dataGridView2.SelectedRows[0].Cells["ID"].Value.ToString());
            this.lbl_MatHang.Text = dataGridView2.SelectedRows[0].Cells["Name"].Value.ToString();
        }

        private void btn_TimKiemMatHang_Click(object sender, EventArgs e)
        {
            if (txb_TimKiemMatHang.Text.Equals(""))
            {
                this.dataGridView2.DataSource = MatHangManager.s_DanhSachMatHang;
                dataGridView2.ClearSelection();
                this.dataGridView2.Rows[0].Selected = true;
                this.m_selected_ID_MH = int.Parse(dataGridView2.SelectedRows[0].Cells["ID"].Value.ToString());
                this.lbl_MatHang.Text = dataGridView2.SelectedRows[0].Cells["Name"].Value.ToString();
            }
            else
            {
                DataTable searchMatHang = MatHangManager.getMatHangByName(this.txb_TimKiemMatHang.Text);
                this.dataGridView2.DataSource = searchMatHang;
                dataGridView2.ClearSelection();
                if (searchMatHang.Rows.Count > 0)
                {                    
                    this.dataGridView2.Rows[0].Selected = true;
                    this.m_selected_ID_MH = int.Parse(dataGridView2.SelectedRows[0].Cells["ID"].Value.ToString());
                    this.lbl_MatHang.Text = dataGridView2.SelectedRows[0].Cells["Name"].Value.ToString();
                }
            }
        }

        private void txb_TimKiemMatHang_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {

                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        btn_TimKiemMatHang_Click(sender, e);
                        break;
                }
            }
        }

        private void btn_in_Click(object sender, EventArgs e)
        {

            if (Settings.s_printer_name.Equals(""))
            {
                MessageBox.Show("Bạn chưa chọn máy in.\nHãy chọn máy in trong mục cài đặt.");
                return;
            }
            //Create a PrintDocument object
            PrintDocument pd = new PrintDocument();

            //Set PrinterName as the selected printer in the printers list
            pd.PrinterSettings.PrinterName = Settings.s_printer_name;
            
            //Add PrintPage event handler
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);

            //Print the document
            pd.Print();
        }

        public void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {

            //Get the Graphics object
            Graphics g = ev.Graphics;

            //Create a font Arial with size 16
            Font font = new Font("Arial", 9);

            //Create a solid brush with black color
            SolidBrush brush = new SolidBrush(Color.Black);
            // Create point for upper-left corner of drawing.
            PointF drawPoint = new PointF(5.0F, 5.0F);

            //Draw "Hello Printer!";
            //g.DrawString("Hello Printer!",
            //font, brush,
            //new Rectangle(20, 20, 200, 100));
            g.DrawString(this.lbl_Scale_Com_NameA.Text + "\n************************\n" 
                + this.lbl_Detail_COMA.Text + "\n************************\n" 
                + this.lbl_MatHang.Text + " : " + this.lbl_Values.Text + " " + this.lbl_Unit.Text
                + "\n\n"
                + DateTime.Now.ToString() + ""
                + "\n\n************************\n Cảm ơn.",               
            font, brush,
            drawPoint);
            //MessageBox.Show(this.lbl_Scale_Com_NameA.Text + "\n************************\n"
            //    + this.lbl_Detail_COMA.Text + "\n************************\n"
            //    + this.lbl_MatHang.Text + " : " + this.lbl_Values.Text + " " + this.lbl_Unit.Text
            //    + "\n\n"
            //    + DateTime.Now.ToString() + ""
            //    + "\n\n************************\n Cảm ơn.");
        }

        private void SetSaveButtonIcon()
        {
            // Assign an image to the button.
            this.btn_Save.Image = Image.FromFile(Application.StartupPath + "\\save-icon.png");
            // Align the image and text on the button.
            this.btn_Save.ImageAlign = ContentAlignment.MiddleRight;
            //this.btn_Save.TextAlign = ContentAlignment.MiddleLeft;
            this.btn_Save.TextImageRelation = TextImageRelation.ImageBeforeText;
        }
        private void SetPrintButtonIcon()
        {
            // Assign an image to the button.
            this.btn_in.Image = Image.FromFile(Application.StartupPath + "\\print-icon.png");
            // Align the image and text on the button.
            this.btn_in.ImageAlign = ContentAlignment.MiddleRight;
            //this.btn_Save.TextAlign = ContentAlignment.MiddleLeft;
            this.btn_in.TextImageRelation = TextImageRelation.ImageBeforeText;
        }
    }
}
