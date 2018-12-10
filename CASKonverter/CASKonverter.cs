using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CASKonverter.myMath;

using CASKonverter.myFormats;

namespace CASKonverter
{
    public partial class CASKonverter : Form
    {
        private string m_FilenameFull;
        private string m_Filename;
        private string m_Extention;
        private string m_FilePath;
        private string m_CommandLineArg;
        private List<Nmea.GPS_Punkt> m_lsGPS = new List<Nmea.GPS_Punkt>();

        GSI objGSI;
        DAT objDAT;
        CSV objCSV;
        Nmea objNmea = new Nmea();

        //Konstruktor
        public CASKonverter()
        {
            InitializeComponent();

            try { m_CommandLineArg = Environment.GetCommandLineArgs()[1]; }
            catch { }

            if (m_CommandLineArg != null)
                DateiÖffnen(m_CommandLineArg);

            //Settings
            myUtilities.mySettings objSettings = myUtilities.mySettings.Instance;
        }

        //Properties
        public bool isMM
        {
            get
            {
                if (bt_Einheit.Text == "Millimeter")
                    return true;
                else
                    return false;
            }
        }

        //Methoden
        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void öffnenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateiÖffnen(null);
        }

        private void DateiÖffnen(string Filename)
        {
            System.Windows.Forms.DialogResult bFileOK = System.Windows.Forms.DialogResult.No;

            //Tabellen löschen
            tb_gsi.Text = String.Empty;
            tb_dat.Text = String.Empty;
            tb_xz.Text = String.Empty;
            tb_yz.Text = String.Empty;
            tb_CSV.Text = String.Empty;
            
            m_FilePath = String.Empty;

            if (Filename == null)
            {
                openFileDialog1.Filter = "Punktdaten|*.gsi;*.dat;*.csv;*.log";
                openFileDialog1.FileName = "";
                bFileOK = openFileDialog1.ShowDialog();
                m_FilenameFull = openFileDialog1.FileName;
            }
            else
            {
                m_FilenameFull = Filename;
                bFileOK = System.Windows.Forms.DialogResult.OK;
            }

            if (bFileOK == System.Windows.Forms.DialogResult.OK)
            {
                m_Filename = m_FilenameFull.Substring(m_FilenameFull.LastIndexOf('\\') + 1);
                m_Filename = m_Filename.Remove(m_Filename.LastIndexOf('.'));
                m_FilePath = m_FilenameFull.Substring(0, m_FilenameFull.LastIndexOf('\\'));
                m_Extention = m_FilenameFull.Substring(m_FilenameFull.LastIndexOf('.') + 1).ToLower();

                StreamReader sr = new StreamReader(m_FilenameFull);
                string sText = sr.ReadToEnd();
                sr.Close();

                switch (m_Extention)
                {
                    case "gsi":
                        tabPage.SelectedTab = tabPage.TabPages[0];
                    
                        objGSI = new GSI(sText);
                        refreshDisplay(objGSI.vMP);

                        break;

                    case "dat":
                        tabPage.SelectedTab = tabPage.TabPages[1];

                        objDAT = new DAT(sText, isMM);
                        refreshDisplay(objDAT.vMP);

                        break;

                    case "csv":
                        tabPage.SelectedTab = tabPage.TabPages[4];

                        objCSV = new CSV(sText);
                        refreshDisplay(objCSV.vMP);

                        break;

                    case "log":
                        tabPage.SelectedTab = tabPage.TabPages[6];
                        objNmea.createGPS(sText);
                        refreshDisplay(objNmea.lsGPSPunkte, false);

                        break;

                    default:
                        MessageBox.Show("nicht unterstützes Dateiformat!");
                        break;
                }
                contextMenuStrip.Enabled = true;
                contextMenuStrip.Items[0].Enabled = true;
                dataGridView1.DataSource = objCSV.dataTable;
            }
        }

        //Anzeige aktualisieren
        public void refreshDisplay(List<Messpunkt> vMP)
        {
            objGSI = new GSI(vMP);
            objDAT = new DAT(vMP);
            objCSV = new CSV(vMP);

            if (bt_Einheit.Text == "Meter")
            {
                tb_gsi.Text = objGSI.GSI_Text;
                tb_dat.Text = objDAT.DAT_Text;
                tb_xz.Text = objDAT.DATxz_Text;
                tb_yz.Text = objDAT.DATyz_Text;
                tb_CSV.Text = objCSV.CSV_Text;
            }
            else
            {
                tb_gsi.Text = objGSI.GSI_Text;
                tb_dat.Text = objDAT.DAT_Text_mm;
                tb_xz.Text = objDAT.DATxz_Text_mm;
                tb_yz.Text = objDAT.DATyz_Text_mm;
                tb_CSV.Text = objCSV.CSV_Text_mm;
            }
        }
        public void refreshDisplay(List<Nmea.GPS_Punkt> lsGPS, bool bTrafo)
        {
            m_lsGPS = lsGPS;
            double[] coordsEll = new double[3];
            double[] coordsGeo = new double[3];
            double[] coordsGK = new double[3];

            tB_ell.Clear();
            //tb_geo.Clear();
            //tb_gk.Clear();

            //Ausgabe  Koordinaten
            foreach (Nmea.GPS_Punkt GPS in lsGPS)
            {
                if (bTrafo)
                {
                    coordsEll = GPS.coordEllTrafo;
                    coordsGeo = GPS.coord_GeoTrafo;
                    coordsGK = GPS.coords_GKTrafo;
                }
                else
                {
                    coordsEll = GPS.coord_EllOrg;
                    coordsGeo = GPS.coord_GeoOrg;
                    coordsGK = GPS.coord_GKOrg;
                }

                tB_ell.Text += GPS.Time.ToLongTimeString() + "  " + "L=" + coordsEll[0].ToString("0.000000") + "  B=" + coordsEll[1].ToString("0.000000") + "  H=" + coordsEll[2].ToString("0.000") + "   " + GPS.quality.ToString() + System.Environment.NewLine;
                //tb_geo.Text += GPS.Time.ToLongTimeString() + "  " + "X=" + coordsGeo[0].ToString("0000000.000") + "  Y=" + coordsGeo[1].ToString("0000000.000") + "  Z=" + coordsGeo[2].ToString("0000000.000") + System.Environment.NewLine;
                //tb_gk.Text += GPS.Time.ToLongTimeString() + "  " + "Y=" + coordsGK[0].ToString("0.000") + "  X=" + coordsGK[1].ToString("0.000") + "  Z=" + coordsGK[2].ToString("0.000") + System.Environment.NewLine;
            }
        }
        private void DateiSpeichern()
        {
            saveFileDialog1.FileName = m_Filename;
            saveFileDialog1.InitialDirectory = m_FilePath;

            if (m_FilePath != "")
            {
                switch (tabPage.SelectedTab.Text)
                {
                    //Dat File speichern
                    case "dat":
                        saveFileDialog1.DefaultExt = "dat";
                        saveFileDialog1.Filter = "Dat File|*.dat";

                        if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                            sw.Write(tb_dat.Text);
                            sw.Close();
                        }

                        break;

                    //Gsi File speichern
                    case "gsi":
                        saveFileDialog1.DefaultExt = "gsi";
                        saveFileDialog1.Filter = "Gsi File|*.gsi";

                        if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                            sw.Write(tb_gsi.Text);
                            sw.Close();
                        }

                        break;

                    case "xz":
                        saveFileDialog1.DefaultExt = "dat";
                        saveFileDialog1.Filter = "Dat File|*.dat";
                        saveFileDialog1.FileName = m_Filename + "- xz";

                        if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                            sw.Write(tb_xz.Text);
                            sw.Close();
                        }

                        break;

                    case "yz":
                        saveFileDialog1.DefaultExt = "dat";
                        saveFileDialog1.Filter = "Dat File|*.dat";
                        saveFileDialog1.FileName = m_Filename + "- yz";

                        if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                            sw.Write(tb_yz.Text);
                            sw.Close();
                        }

                        break;

                    case "csv":
                        saveFileDialog1.DefaultExt = "csv";
                        saveFileDialog1.Filter = "csv File|*.csv";
                        saveFileDialog1.FileName = m_Filename;

                        if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                            sw.Write(tb_CSV.Text);
                            sw.Close();
                        }

                        break;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DateiÖffnen(null);
        }

        private void beendenToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void speichernToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DateiSpeichern();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            DateiÖffnen(null);
        }

        private void tsMI_öffnen(object sender, EventArgs e)
        {
            DateiÖffnen(null);
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            DateiSpeichern();
        }

        private void tsB_Help_Click(object sender, EventArgs e)
        {
            AboutBox objAboutBox = new AboutBox();
            objAboutBox.ShowDialog();
        }

        private void tb_dat_refreshRowsColumns(object sender, EventArgs e)
        {
            int index = tb_dat.SelectionStart;
            tsL_Row.Text = (tb_dat.GetLineFromCharIndex(index) + 1).ToString();
            tsL_Column.Text = (index - tb_dat.GetFirstCharIndexOfCurrentLine() + 1).ToString();
        }

        private void tb_dat_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void tb_gsi_refreshRowsColumns(object sender, EventArgs e)
        {
            int index = tb_gsi.SelectionStart;
            tsGsi_Row.Text = (tb_gsi.GetLineFromCharIndex(index) + 1).ToString();
            tsGsi_Column.Text = (index - tb_gsi.GetFirstCharIndexOfCurrentLine() + 1).ToString();
        }

        private void tb_gsi_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void bt_Einheit_Click(object sender, EventArgs e)
        {
            switch (bt_Einheit.Text)
            {
                case "Meter":
                    bt_Einheit.Text = "Millimeter";
                    bt_Einheit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;

                    if (objDAT != null)
                    {
                        tb_dat.Text = objDAT.DAT_Text_mm;
                        tb_xz.Text = objDAT.DATxz_Text_mm;
                        tb_yz.Text = objDAT.DATyz_Text_mm;
                        tb_CSV.Text = objCSV.CSV_Text_mm;
                    }

                    break; 

                case "Millimeter":
                    bt_Einheit.Text = "Meter";
                    bt_Einheit.FlatStyle = FlatStyle.System;

                    if (objDAT != null)
                    {
                        tb_dat.Text = objDAT.DAT_Text;
                        tb_xz.Text = objDAT.DATxz_Text;
                        tb_yz.Text = objDAT.DATyz_Text;
                        tb_CSV.Text = objCSV.CSV_Text;
                    }

                    break;
            }
        }

        private void tb_DATxz_refreshRowsColumns(object sender, EventArgs e)
        {
            int index = tb_xz.SelectionStart;
            tsDATxz_Row.Text = (tb_xz.GetLineFromCharIndex(index) + 1).ToString();
            tsDATxz_Column.Text = (index - tb_xz.GetFirstCharIndexOfCurrentLine() + 1).ToString();
        }

        private void tb_DATyz_refreshRowsColumns(object sender, EventArgs e)
        {
            int index = tb_yz.SelectionStart;
            tsDATyz_Row.Text = (tb_yz.GetLineFromCharIndex(index) + 1).ToString();
            tsDATyz_Column.Text = (index - tb_yz.GetFirstCharIndexOfCurrentLine() + 1).ToString();
        }

        private void tb_CSV_refreshRowsColumns(object sender, EventArgs e)
        {
            int index = tb_CSV.SelectionStart;
            tsCSV_Row.Text = (tb_CSV.GetLineFromCharIndex(index) + 1).ToString();
            tsCSV_Column.Text = (index - tb_CSV.GetFirstCharIndexOfCurrentLine() + 1).ToString();
        }

        private void tb_DATxz_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void heightOffsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myFunctions.Height_Offset objHeightOffset = new myFunctions.Height_Offset();
            objHeightOffset.ShowDialog();

            refreshDisplay(vMP_Offset(objGSI.vMP, objHeightOffset.Offset));
        }

        private List<Messpunkt> vMP_Offset(List<Messpunkt> vMP, double Offset)
        {
            foreach (Messpunkt MP in vMP)
            {
                MP.Höhe += Offset;
            }

            return vMP;
        }

        private void tsB_Settings_Click(object sender, EventArgs e)
        {
            myFunctions.diaSettings objSettings = new myFunctions.diaSettings();
            objSettings.ShowDialog();
        }

        private void exportGoogleEarthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog_ell.DefaultExt = "kml";
            saveFileDialog_ell.Filter = "GoogleEarth File|*.kml";

            if (saveFileDialog_ell.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog_ell.FileName);
                //Header
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\">");
                sw.WriteLine("    <Document id=\"feat_1\">");

                //Placemarks
                int id = 1;
                double[] coord_Ell = new double[3];

                foreach (Nmea.GPS_Punkt gps in m_lsGPS)
                {
                    //if (m_btTrafoPressed)
                    //    coord_Ell = gps.coordEllTrafo;
                    //else
                        coord_Ell = gps.coord_EllOrg;

                    sw.WriteLine("        <Placemark id=\"feat_{0}\">", id.ToString());
                    sw.WriteLine("            <name>{0}</name>", gps.Time.ToShortTimeString());
                    sw.WriteLine("            <Point id=\"{0}\">", gps.Time.ToLongTimeString());
                    sw.WriteLine("                <coordinates>{0},{1},0.0</coordinates>", coord_Ell[0].ToString().Replace(',', '.'), coord_Ell[1].ToString().Replace(',', '.'));
                    sw.WriteLine("            </Point>");
                    sw.WriteLine("        </Placemark>");
                    id++;
                }

                //Foot
                sw.WriteLine("    </Document>");
                sw.WriteLine("</kml>");

                sw.Close();
            }
        }
    }
}