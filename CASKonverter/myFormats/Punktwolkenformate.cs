using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using CASKonverter.myMath;
using CASKonverter.Landesvermessung;
using System.Data;

namespace CASKonverter.myFormats
{
    public class GSI
    {
        private string m_GSI_Text = String.Empty;
        private List<Messpunkt> m_vMP = new List<Messpunkt>();
        private List<string> m_lsGSI = new List<string>();

        //Konstruktor
        public GSI(string sGSI)
        {
            m_GSI_Text = sGSI;
            create_vMP();
        }

        public GSI(List<Messpunkt> vMP)
        {
            m_vMP = vMP;
            create_GSI();
        }

        //Properties
        public string GSI_Text
        {
            get { return m_GSI_Text; }
        }

        public List<Messpunkt> vMP
        {
            get { return m_vMP; }
        }

        public List<string> lsGSI
        {
            get { return m_lsGSI; }
        }


        //Methoden
        public void create_vMP()
        {
            char[] seperator = { '\r', '\n' };
            string[] arText = m_GSI_Text.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

            foreach (string Zeile in arText)
            {
                GsiZeile objGsiZeile = new GsiZeile(Zeile);

                if (objGsiZeile.Nr != null && objGsiZeile.X != null && objGsiZeile.Y != null)
                {
                    Messpunkt objMP = new Messpunkt(objGsiZeile.Nr, objGsiZeile.X.Value, objGsiZeile.Y.Value, objGsiZeile.Z, objGsiZeile.zH, objGsiZeile.AddKonstante,
                                                    objGsiZeile.Code, objGsiZeile.Datum, objGsiZeile.Precision, objGsiZeile.Precision, objGsiZeile.Precision);
                    m_vMP.Add(objMP);
                }
            }
        }

        //Datentyp für GSI Zeile
        private class GsiZeile
        {
            private string m_Nr;
            private double? m_x, m_y, m_z;
            private double m_AddK, m_zH;
            private int m_Precision;
            private string m_Code;
            private DateTime m_Datum;
            private int m_Jahr;
            private int m_Monat;
            private int m_Tag;
            private int m_Stunde;
            private int m_Minute;
            private int m_Sekunde;
            string[] m_Wort;

            //Konstruktor
            public GsiZeile(string Zeile)
            {
                Wort objWort;
                char[] parameter = { ' ' };
                this.m_Wort = Zeile.Split(parameter, StringSplitOptions.RemoveEmptyEntries);

                foreach (string sWort in m_Wort)
                {
                    objWort = new Wort(sWort);

                    //Komponenten zuweisen
                    switch (objWort.Wortidentifikation)
                    {
                        //Punktnummer
                        case 11:
                            m_Nr = objWort.Wert;
                            break;

                        //Datum Jahr
                        case 18:
                            //Jahr
                            string Jahr = objWort.Wert.Substring(0, 2);
                            m_Jahr = 2000 + System.Convert.ToInt32(Jahr);

                            //Sekunde
                            string Sekunde = objWort.Wert.Substring(2, 2);
                            m_Sekunde = System.Convert.ToInt32(Sekunde);

                            break;

                        //Datum Monat+Tag
                        case 19:
                            //Monat
                            string Datum;
                            if (objWort.Wert.Length < 8)
                                Datum = "0" + objWort.Wert;
                            else
                                Datum = objWort.Wert;

                            string Monat = Datum.Substring(0, 2);
                            m_Monat = System.Convert.ToInt32(Monat);

                            //Tag
                            string Tag = Datum.Substring(2, 2);
                            m_Tag = System.Convert.ToInt32(Tag);

                            //Stunde
                            string Stunde = Datum.Substring(4, 2);
                            m_Stunde = System.Convert.ToInt32(Stunde);

                            //Minute
                            string Minute = Datum.Substring(6, 2);
                            m_Minute = System.Convert.ToInt32(Minute);

                            try
                            {
                                m_Datum = new DateTime(m_Jahr, m_Monat, m_Tag, m_Stunde, m_Minute, m_Sekunde);
                            }
                            catch { }

                            break;

                         //Additionskonstante
                        //case 51:
                        //    m_AddK = System.Convert.ToDouble(objWort.Wert, Global.Provider4);

                        //    break;

                        //Messcode
                        case 71:
                            m_Code = objWort.Wert;

                            break;

                        //Rechtswert
                        case 81:
                            m_y = System.Convert.ToDouble(objWort.Wert, Global.Provider4);
                            if (objWort.Vorzeichen == '-')
                                m_y *= -1;

                            //Anzahl Nachkommastellen
                            m_Precision = objWort.Wert.Substring(objWort.Wert.LastIndexOf('.') + 1).Length;

                            break;

                        //Hochwert
                        case 82:
                            m_x = System.Convert.ToDouble(objWort.Wert, Global.Provider4);
                            if (objWort.Vorzeichen == '-')
                                m_x *= -1;
                            break;

                        //Höhe
                        case 83:
                            m_z = System.Convert.ToDouble(objWort.Wert, Global.Provider4);
                            if (objWort.Vorzeichen == '-')
                                m_z *= -1;
                            break;

                        //STP Rechtswert
                        case 84:
                            m_y = System.Convert.ToDouble(objWort.Wert, Global.Provider4);

                            //Anzahl Nachkommastellen
                            m_Precision = objWort.Wert.Substring(objWort.Wert.LastIndexOf('.') + 1).Length;

                            break;

                        //STP Hochwert
                        case 85:
                            m_x = System.Convert.ToDouble(objWort.Wert, Global.Provider4);
                            break;

                        //STP Höhe
                        case 86:
                            m_z = System.Convert.ToDouble(objWort.Wert, Global.Provider4);
                            break;

                        //Zielhöhe
                        case 87:
                            m_zH = System.Convert.ToDouble(objWort.Wert, Global.Provider4);
                            break;
                    }
                }
            }

            //Properties
            public string Nr
            {
                get { return this.m_Nr; }
                set { m_Nr = value; }
            }

            public double? Y
            {
                get
                {
                    if (this.m_x.HasValue)
                        return this.m_x.Value;
                    else
                        return null;
                }
                set { m_x = value; }
            }

            public double? X
            {
                get
                {
                    if (this.m_y.HasValue)
                        return this.m_y.Value;
                    else
                        return null;
                }
                set { m_y = value; }
            }

            public double? Z
            {
                get
                {
                    if (this.m_z.HasValue)
                        return this.m_z.Value;
                    else
                        return null;
                }
                set { m_z = value; }
            }

            public double zH
            {
                get { return m_zH; }
            }

            public double AddKonstante
            {
                get { return m_AddK; }
            }

            public string Code
            {
                get { return m_Code; }
                set { m_Code = value; }
            }

            public DateTime Datum
            { get { return m_Datum; } }

            public int Precision
            {
                get { return m_Precision; }
                set { m_Precision = value; }
            }
        }

        //Datentyp für Wort
        private class Wort
        {
            private int m_Wortidentifikation;
            private char m_Vorzeichen;
            private int m_PosVorzeichen;
            private string m_Wert;
            private char m_Einheit;

            //Konstruktor
            public Wort(string Wort)
            {
                //Vorzeichen
                if (Wort.Contains("+"))
                    m_Vorzeichen = '+';
                else
                    if (Wort.Contains("-"))
                        m_Vorzeichen = '-';
                    else
                        m_Vorzeichen = '0';

                //PosVorzeichen
                if (m_Vorzeichen != '0')
                    m_PosVorzeichen = Wort.IndexOf(m_Vorzeichen);

                //Wortidentifiktation
                string Wortidentifikation = String.Empty;
                if (m_Vorzeichen != '0')
                    Wortidentifikation = Wort.Substring(m_PosVorzeichen - 6, 2);
                else
                    Wortidentifikation = Wort.Substring(0, 2);

                m_Wortidentifikation = System.Convert.ToInt32(Wortidentifikation);

                //Wert
                if (m_Vorzeichen != '0')
                {
                    m_Wert = Wort.Substring(m_PosVorzeichen + 1);
                    m_Wert = m_Wert.TrimStart(new char[] { '0' });
                }

                //Einheit
                // 0 - Meter (letzte Stelle == mm)
                // 1 - Feet
                // 2 - 400 gon
                // 3 - 360 grad dezimal
                // 4 - 360 grad sexagesimal
                // 5 - 6400 mil
                // 6 - Meter (letzte Stelle == 1/10 mm)
                // 7 - Feet (letzte Stelle == 1/10000 feet)
                // 8 - Meter (letzte Stelle == 1/100 mm)
                if (m_Vorzeichen != '0')
                    m_Einheit = Wort[m_PosVorzeichen - 1];

                //Wertkorrektur entsprechend Einheiten
                if (m_Wortidentifikation != 11 && m_Wortidentifikation != 71)
                {
                    if (m_Wert.Contains('.'))
                        m_Wert = m_Wert.Substring(0, m_Wert.IndexOf('.'));

                    switch (m_Einheit)
                    {
                        case '0':
                            if (Wert != "")
                            {
                                m_Wert = m_Wert.PadLeft(3, '0');
                                m_Wert = Wert.Insert(Wert.Length - 3, ".");
                            }
                            else
                                m_Wert = "0";
                            break;

                        case '6':
                            if (Wert != "")
                            {
                                m_Wert = m_Wert.PadLeft(4, '0');
                                m_Wert = Wert.Insert(Wert.Length - 4, ".");
                            }
                            else
                                m_Wert = "0";

                            break;

                        case '8':
                            if (Wert != "")
                            {
                                m_Wert = m_Wert.PadLeft(5, '0');
                                m_Wert = Wert.Insert(Wert.Length - 5, ".");
                            }
                            else
                                m_Wert = "0";

                            break;
                    }
                }
            }

            //Properties
            public int Wortidentifikation
            {
                get { return m_Wortidentifikation; }
            }

            public string Wert
            {
                get { return m_Wert; }
            }

            public char Vorzeichen
            {
                get { return m_Vorzeichen; }
            }

            public int Einheit
            {
                get { return m_Einheit; }
            }
        }

        private void create_GSI()
        {
            int iZähler = 1;
            char Einheit;

            foreach (Messpunkt objMP in m_vMP)
            {
                int Faktor = 1;
                //Einheit festlegen
                switch (objMP.PrecisionX)
                {
                    case 0:
                        Einheit = '0';
                        Faktor = 1000;
                        break;
                    case 1:
                        Einheit = '0';
                        Faktor = 100;
                        break;
                    case 2:
                        Einheit = '0';
                        Faktor = 10;
                        break;
                    case 3:
                        Einheit = '0';
                        break;

                    case 4:
                        Einheit = '6';
                        break;

                    case 5:
                        Einheit = '8';
                        break;

                    default:
                        Einheit = '0';
                        break;
                }

                string sZeile = String.Empty;

                //Zähler
                string sZähler = "*11" + iZähler.ToString("0000") + "+";
                iZähler++;
                sZeile += sZähler;

                //Punktnummer
                string sPNum = objMP.NR.PadLeft(16, '0');
                sZeile += sPNum;

                //Rechtswert
                string sRechtswert = " 81..1";
                sRechtswert += Einheit;

                if (Math.Sign(objMP.Rechtswert) == 1)
                    sRechtswert += "+";
                else
                    sRechtswert += "-";

                long iRechtswert = Math.Abs(myMath.Convert.shiftDelímeter(objMP.Rechtswert, Global.Format(objMP.PrecisionX))) * Faktor;
                sRechtswert += iRechtswert.ToString().PadLeft(16, '0');
                sZeile += sRechtswert;

                //Hochwert
                string sHochwert = " 82..1";
                sHochwert += Einheit;

                if (Math.Sign(objMP.Hochwert) == 1)
                    sHochwert += "+";
                else
                    sHochwert += "-";

                long iHochwert = Math.Abs(myMath.Convert.shiftDelímeter(objMP.Hochwert, Global.Format(objMP.PrecisionX))) * Faktor;
                sHochwert += iHochwert.ToString().PadLeft(16, '0');
                sZeile += sHochwert;

                //Höhe
                double Höhe;
                if (objMP.Höhe.HasValue)
                    Höhe = objMP.Höhe.Value;
                else
                    Höhe = 0;

                string sHöhe = " 83..1";
                sHöhe += Einheit;

                if (Math.Sign(Höhe) >= 0)
                    sHöhe += "+";
                else
                    sHöhe += "-";

                long iHöhe = Math.Abs(myMath.Convert.shiftDelímeter(Höhe, Global.Format(objMP.PrecisionX))) * Faktor;
                sHöhe += iHöhe.ToString(Global.Provider4).PadLeft(16, '0') + " ";
                sZeile += sHöhe;

                //Code
                if (objMP.Code != null)
                {
                    string Code = " 71..1";
                    Code += objMP.Code.PadLeft(16, '0');

                    sZeile += Code;
                }

                m_GSI_Text += sZeile + "\r\n";
            }
        }
    }

    public class DAT
    {
        private string m_DAT_Text = String.Empty;
        private string m_DAT_Text_mm = String.Empty;
        private string m_DATxz_Text = String.Empty;
        private string m_DATxz_Text_mm = String.Empty;
        private string m_DATyz_Text = String.Empty;
        private string m_DATyz_Text_mm = String.Empty;
        private List<Messpunkt> m_vMP = new List<Messpunkt>();
        private List<Messpunkt> m_vMP_mm = new List<Messpunkt>();
        private myUtilities.MyConfig _config = new myUtilities.MyConfig();

        //Konstruktor
        public DAT(string sDat, bool isMM)
        {
            m_DAT_Text = sDat.Replace(',', '.');
            create_vMP(isMM);

            m_DAT_Text_mm = create_DAT(m_vMP_mm);

            m_DATxz_Text = create_DATxz(m_vMP);
            m_DATxz_Text_mm = create_DATxz(m_vMP_mm);

            m_DATyz_Text = create_DATyz(m_vMP);
            m_DATyz_Text_mm = create_DATyz(m_vMP_mm);
        }

        public DAT(List<Messpunkt> vMP)
        {
            m_vMP = vMP;
            create_vMP(false);

            m_DAT_Text = create_DAT(m_vMP);
            m_DAT_Text_mm = create_DAT(m_vMP_mm);

            m_DATxz_Text = create_DATxz(m_vMP);
            m_DATxz_Text_mm = create_DATxz(m_vMP_mm);

            m_DATyz_Text = create_DATyz(m_vMP);
            m_DATyz_Text_mm = create_DATyz(m_vMP_mm);
        }

        //Properties
        public string DAT_Text
        {
            get { return m_DAT_Text; }
        }

        public string DAT_Text_mm
        {
            get { return m_DAT_Text_mm; }
        }

        public List<Messpunkt> vMP
        {
            get { return m_vMP; }
        }

        public string DATxz_Text
        {
            get { return m_DATxz_Text; }
        }

        public string DATxz_Text_mm
        {
            get { return m_DATxz_Text_mm; }
        }

        public string DATyz_Text
        {
            get { return m_DATyz_Text; }
        }

        public string DATyz_Text_mm
        {
            get { return m_DATyz_Text_mm; }
        }

        //Methoden
        private void create_vMP(bool isMM)
        {
            bool bFehler = false;

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalDigits = _config.GetAppSettingInt("decimals");

            string[] arText = m_DAT_Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string Zeile in arText)
            {
                try
                {
                    string[] Datensatz = Zeile.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                   // NumFormat.NumberDecimalDigits = Global.StringDigits(Datensatz[1]);

                    string PNum = Datensatz[0];
                    double Rechtswert = System.Convert.ToDouble(Datensatz[1], nfi);
                    double Hochwert = System.Convert.ToDouble(Datensatz[2], nfi);
                    double? Höhe = null;
                    int PresicionX = Global.StringDigits(Datensatz[1]);
                    int PresicionY = Global.StringDigits(Datensatz[2]);
                    int PresicionZ = Global.StringDigits(Datensatz[3]);

                    try
                    {
                        if (Datensatz[3] != null)
                            Höhe = System.Convert.ToDouble(Datensatz[3], nfi);
                    }
                    catch { }

                    //Einlesen von Meter oder Millimeter?
                    if (isMM)
                    {
                        Rechtswert /= 1000;
                        Hochwert /= 1000;
                        Höhe /= 1000;

                        PresicionX += 3;
                        PresicionY += 3;
                        PresicionZ += 3;
                    }

                    //Code
                    string Code = String.Empty;
                    try
                    {
                        if (Datensatz[6] != null)
                            Code = Datensatz[6];
                    }
                    catch { }

                    Messpunkt objMP = new Messpunkt(PNum, Rechtswert, Hochwert, Höhe, 0,0, Code, new DateTime(), PresicionX, PresicionY, PresicionZ);
                    m_vMP.Add(objMP);
                }
                catch { bFehler = true; }
            }

            if (bFehler)
                System.Windows.Forms.MessageBox.Show("Fehler bei der Konvertierung festgestellt!!!");

            create_vMP_mm();
        }

        private string create_DAT(List<Messpunkt> lsMP)
        {
            string Text = String.Empty;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalDigits = _config.GetAppSettingInt("decimals");

            for (int i = 0; i < lsMP.Count; i++)
            {
                Messpunkt MP = lsMP[i];

                string Zeile = MP.NR.PadLeft(9);

                Zeile += MP.Rechtswert.ToString("F", nfi).PadLeft(13);
                Zeile += MP.Hochwert.ToString("F", nfi).PadLeft(13);

                if (MP.Höhe.HasValue)
                {
                    Zeile += MP.Höhe.Value.ToString("F", nfi).PadLeft(11);
                }

                //Code
                if (MP.Code != null)
                    Zeile += MP.Code.PadLeft(13);
                else
                    Zeile += String.Empty.PadLeft(13);

                //Datum
                if (MP.Datum != null)
                    Zeile += MP.Datum.Value.ToString("dd.MM.yyy").PadLeft(13);

                Zeile = Zeile.Replace(',', '.');
                Zeile += "\r\n";

                Text += Zeile;
            }
            return Text;
        }

        private string create_DATxz(List<Messpunkt> vMP)
        {
            string Text = String.Empty;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalDigits = _config.GetAppSettingInt("decimals");

            for (int i = 0; i < vMP.Count; i++)
            {
                Messpunkt objMP = vMP[i];
                string Zeile = objMP.NR.PadLeft(9);

                Zeile += objMP.Rechtswert.ToString("N", nfi).PadLeft(13);

                if (objMP.Höhe.HasValue)
                {
                    Zeile += objMP.Höhe.Value.ToString("N", nfi).PadLeft(13);
                }

                Zeile += objMP.Hochwert.ToString("N", nfi).PadLeft(13);

                Zeile = Zeile.Replace(',', '.');
                Zeile += "\r\n";

                Text += Zeile;
            }
            return Text;
        }

        private string create_DATyz(List<Messpunkt> vMP)
        {
            string Text = String.Empty;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalDigits = _config.GetAppSettingInt("decimals");

            for (int i = 0; i < vMP.Count; i++)
            {
                Messpunkt objMP = vMP[i];
 
                string Zeile = objMP.NR.PadLeft(9);

                Zeile += objMP.Hochwert.ToString("N", nfi).PadLeft(13);
                Zeile.Replace(",", ".");

                if (objMP.Höhe.HasValue)
                {
                    Zeile += objMP.Höhe.Value.ToString("N", nfi).PadLeft(13);
                }

                Zeile += objMP.Rechtswert.ToString("N", nfi).PadLeft(13);
                Zeile.Replace(",", ".");

                Zeile = Zeile.Replace(',', '.');
                Zeile += "\r\n";

                Text += Zeile;
            }
            return Text;
        }

        protected void create_vMP_mm()
        {
            foreach (Messpunkt MP in m_vMP)
            {
                double Rechtswert = MP.Rechtswert * 1000;
                double Hochwert = MP.Hochwert * 1000;

                double? Höhe = null;
                if (MP.Höhe.HasValue)
                    Höhe = MP.Höhe.Value * 1000;

                int Precision = MP.PrecisionX - 3;

                if (Precision < 0)
                    Precision = 0;

                Messpunkt objMP = new Messpunkt(MP.NR, Rechtswert, Hochwert, Höhe, 0,0, null, new DateTime(), Precision, Precision, Precision);
                m_vMP_mm.Add(objMP);
            }
        }
    }

    public class CSV
    {
        private string m_CSV_Text = String.Empty;
        private string m_CSV_Text_mm = String.Empty;
        private List<Messpunkt> m_vMP = new List<Messpunkt>();
        private List<Messpunkt> m_vMP_mm = new List<Messpunkt>();
        private DataTable m_Table = new DataTable();
        private myUtilities.MyConfig _config = new myUtilities.MyConfig();

        //Konstruktor
        public CSV(string sCSV)
        {
            initTable();
            m_CSV_Text = sCSV;
            create_vMP();

            m_CSV_Text_mm = create_CSV(m_vMP_mm);
        }

        public CSV(List<Messpunkt> vMP)
        {
            initTable();
            m_vMP = vMP;
            create_vMP_mm();
            m_CSV_Text = create_CSV(vMP);
            m_CSV_Text_mm = create_CSV(m_vMP_mm);
        }

        //Properties
        public string CSV_Text
        {
            get { return m_CSV_Text; }
        }

        public DataTable dataTable
        {
            get { return m_Table; }
        }

        public string CSV_Text_mm
        {
            get { return m_CSV_Text_mm; }
        }

        public List<Messpunkt> vMP
        {
            get { return m_vMP; }
        }

        //Methoden
        public void initTable()
        {
            m_Table.Columns.Add("Pnr", typeof(string));
            m_Table.Columns.Add("Rechtswert", typeof(double));
            m_Table.Columns.Add("Hochwert", typeof(double));
            m_Table.Columns.Add("Höhe", typeof(double));
        }

        private void create_vMP()
        {
            m_vMP.Clear();
            bool bFehler = false;

            int decimals = _config.GetAppSettingInt("decimals");
            NumberFormatInfo NumFormatXYZ = new NumberFormatInfo();

            int Xdigits, Ydigits, Zdigits;
            int max;
            NumberFormatInfo NumFormatXY = new NumberFormatInfo();
            NumberFormatInfo NumFormatZ = new NumberFormatInfo();

            string[] arText = m_CSV_Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string row in arText)
            {
                try
                {
                    string Zeile = row.Replace(',', '.');

                    string[] Datensatz = Zeile.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    Xdigits = (Global.StringDigits(Datensatz[1]) > 4) ? 4 : Global.StringDigits(Datensatz[1]);
                    Ydigits = (Global.StringDigits(Datensatz[2]) > 4) ? 4 : Global.StringDigits(Datensatz[2]);
                    Zdigits = (Global.StringDigits(Datensatz[3]) > 4) ? 4 : Global.StringDigits(Datensatz[3]);

                    max = (Xdigits >= Ydigits) ? Xdigits : Ydigits;
                    max = (max >= Zdigits) ? max : Zdigits;

                    NumFormatXY.NumberDecimalDigits = max;
                    NumFormatZ.NumberDecimalDigits = max;

                    string PNum = Datensatz[0];
                    double Rechtswert = Math.Round(System.Convert.ToDouble(Datensatz[1], NumFormatXY), NumFormatXY.NumberDecimalDigits);
                    double Hochwert = Math.Round(System.Convert.ToDouble(Datensatz[2], NumFormatXY), NumFormatXY.NumberDecimalDigits);
                    double? Höhe = null;

                    try
                    {
                        if (Datensatz[3] != null)
                            Höhe = Math.Round(System.Convert.ToDouble(Datensatz[3], NumFormatZ), NumFormatZ.NumberDecimalDigits);
                    }
                    catch { }

                    string Code = String.Empty;
                    try
                    {
                        if (Datensatz[4] != null)
                            Code = Datensatz[4];
                    }
                    catch { }

                    Messpunkt objMP = new Messpunkt(PNum, Rechtswert, Hochwert, Höhe, 0,0, Code, new DateTime(), NumFormatXY.NumberDecimalDigits,
                                                                                                            NumFormatXY.NumberDecimalDigits,
                                                                                                            NumFormatZ.NumberDecimalDigits);
                    m_vMP.Add(objMP);
                }

                catch { bFehler = true; }
            }

            if (bFehler)
                System.Windows.Forms.MessageBox.Show("Fehler bei der Konvertierung festgestellt!!!");

            create_vMP_mm();
        }

        public class NMEA
        {
            private List<Nmea.GPS_Punkt> lsGPS = new List<Nmea.GPS_Punkt>();
            private Dictionary<string, DateTime> m_dicKameras = new Dictionary<string, DateTime>();
            private Landesvermessung.Trafo trafo = null;
            private const string m_TrafoNameDef = "keine Trafo!";
            bool m_btTrafoPressed = false;


        }

        private void create_vMP_mm()
        {
            foreach (Messpunkt MP in m_vMP)
            {
                double Rechtswert = MP.Rechtswert * 1000;
                double Hochwert = MP.Hochwert * 1000;

                double? Höhe = null;
                if (MP.Höhe.HasValue)
                    Höhe = MP.Höhe.Value * 1000;

                int Precision = MP.PrecisionX - 3;

                if (Precision < 0)
                    Precision = 0;

                Messpunkt objMP = new Messpunkt(MP.NR, Rechtswert, Hochwert, Höhe, 0,0, MP.Code, new DateTime(), Precision, Precision, Precision);
                m_vMP_mm.Add(objMP);
            }
        }

        private string create_CSV(List<Messpunkt> vMP)
        {
            string Text = String.Empty;
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalDigits = _config.GetAppSettingInt("decimals");

            for (int i = 0; i < vMP.Count; i++)
            {
                Messpunkt objMP = vMP[i];

                string Zeile = objMP.NR;
                m_Table.Rows.Add(objMP.NR);

                Zeile += ";";

                Zeile += objMP.Rechtswert.ToString("N", nfi) + ";";
                Zeile.Replace(",", ".");

                Zeile += objMP.Hochwert.ToString("N", nfi) + ";";
                Zeile.Replace(",", ".");

                if (objMP.Höhe.HasValue)
                {
                    Zeile += objMP.Höhe.Value.ToString("N", nfi) + ";";
                }

                Zeile += ";";

                //Datum
                if (objMP.Datum != null)
                {
                    Zeile += objMP.Datum.Value.ToString("dd.MM.yyy");
                }

                //Zeile = Zeile.Replace(',', '.');
                Zeile += "\r\n";

                Text += Zeile;
            }
            return Text;
        }
    }
}

