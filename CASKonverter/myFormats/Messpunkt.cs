using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CASKonverter.myFormats
{
    public class Messpunkt
    {
        private string m_PNum;
        private double m_Rechtswert;
        private double m_Hochwert;
        private double? m_Höhe = null;
        private double m_zH;
        private double m_AddK;
        private string m_Code = String.Empty;
        private DateTime m_Datum;
        private int m_PrecisionX;
        private int m_PrecisionY;
        private int m_PrecisionZ;
        private List<string> m_lsDat = new List<string>();
        private List<string> m_lsXZ = new List<string>();
        private List<string> m_lsYZ = new List<string>();

        //Konstruktor
        public Messpunkt(string Nr, double Rechtswert, double Hochwert, double? Höhe, double zH, double AddK, string Code, DateTime Datum, int PrecX, int PrecY, int PrecZ)
        {
            m_PNum = Nr;
            m_Rechtswert = Rechtswert;
            m_Hochwert = Hochwert;
            m_Höhe = Höhe;
            m_zH = zH;
            m_AddK = AddK;
            m_Code = Code;
            m_Datum = Datum;
            m_PrecisionX = PrecX;
            m_PrecisionY = PrecY;
            m_PrecisionZ = PrecZ;
        }

        //Properties
        public string NR
        {
            get { return m_PNum; }
        }

        public double Rechtswert
        {
            get { return m_Rechtswert; }
            set { m_Rechtswert = value; }
        }

        public double Hochwert
        {
            get { return m_Hochwert; }
            set { m_Hochwert = value; }
        }

        public double? Höhe
        {
            get { return m_Höhe; }
            set { m_Höhe = value; }
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

        public DateTime? Datum
        {
            get 
            {
                if (m_Datum.Year < 1900)
                    return null;
                else
                    return m_Datum; 
            }
        }

        public int PrecisionX
        {
            get { return m_PrecisionX; }
            set { m_PrecisionX = value; }
        }

        public int PrecisionY
        {
            get { return m_PrecisionY; }
            set { m_PrecisionY = value; }
        }

        public int PrecisionZ
        {
            get { return m_PrecisionZ; }
            set { m_PrecisionZ = value; }
        }
    }
}
