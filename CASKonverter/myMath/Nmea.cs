using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CASKonverter.Landesvermessung;

namespace CASKonverter.myMath
{
    public class Nmea
    {
        private List<GPS_Punkt> m_lsGPS = new List<GPS_Punkt>();
        //Konstruktor
        
        //Properties
        public List<GPS_Punkt> lsGPSPunkte
        {
            get { return m_lsGPS; }
        }

        //Methoden
        public List<GPS_Punkt> createGPS(string Text)
        {
            char[] seperator = { '\r', '\n' };
            string[] arText = Text.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            
            bool bTrigger = false;
            int index = 0;

            foreach(string Zeile in arText)
            {
                if (Zeile.Length > 10)
                {
                    if (Zeile.Substring(0, 10) == "#MARKTIMEA")
                    {
                        bTrigger = true;
                        index = 0;          //Positionen nach Trigger Ereignis
                    }

                    if (bTrigger && Zeile.Length >= 6 && Zeile.Substring(0, 6) == "$GPGGA")
                    {
                        char[] Seperator = { ',' };
                        string[] arZeile = Zeile.Split(Seperator);
                        GPS_Punkt objGPS = new GPS_Punkt(arZeile[1], arZeile[4], arZeile[2], arZeile[9], arZeile[6]);

                        m_lsGPS.Add(objGPS);
                        bTrigger = false;
                    }
                    index++;
                }
            }
            
            return m_lsGPS;
        }

        public class GPS_Punkt
        {
            private DateTime m_Time = new DateTime();
            private double[] m_coordEllOrg = new double[3];     //ell. Koordinaten Decimal im Original
            private double[] m_coordEllTrafo = new double[3];   //ell. Koordinaten transformiert
            private double[] m_coordGeoOrg = new double[3];     //geozentrische Koordinaten im Original
            private double[] m_coordGeoTrafo = new double[3];   //geozentrische Koordinaten transformiert
            private double[] m_coordGKOrg = new double[3];      //GK Koordinaten im Original
            private double[] m_coordGKTrafo = new double[3];    //GK Koordinaten transformiert
            private int m_Quality;                              //GPS Qualität

            //Konstruktor
            public GPS_Punkt(string Zeit, string L, string B, string Höhe, string Qualtiät)
            {
                double Time = double.Parse(Zeit, System.Globalization.CultureInfo.InvariantCulture);
                double Lgrad = double.Parse(L, System.Globalization.CultureInfo.InvariantCulture) / 100;
                double Bgrad = double.Parse(B, System.Globalization.CultureInfo.InvariantCulture) / 100;
                double H = double.Parse(Höhe, System.Globalization.CultureInfo.InvariantCulture);
                m_Quality = int.Parse(Qualtiät, System.Globalization.CultureInfo.InvariantCulture);

                m_Time = m_Time.AddHours((long) Math.Truncate(Time / 10000));
                Time -= m_Time.Hour * 10000;
                m_Time = m_Time.AddMinutes((long)Math.Truncate(Time / 100));
                Time -= m_Time.Minute * 100;
                m_Time = m_Time.AddSeconds((long)Math.Truncate(Time));
                Time -= m_Time.Second;
                m_Time = m_Time.AddMilliseconds((long)Math.Truncate(Time*1000));

                //MEZ
                m_Time= m_Time.AddHours(1.0);
                m_coordEllOrg[0] = convDecimal(Lgrad);
                m_coordEllOrg[1] = convDecimal(Bgrad);
                m_coordEllOrg[2] = H;

                calcGeo(m_coordEllOrg[0], m_coordEllOrg[1], m_coordEllOrg[2]);
                calcGK(m_coordEllOrg[0], m_coordEllOrg[1], m_coordEllOrg[2]);
            }

            //Properties
            public DateTime Time
            {
                get {return m_Time;}
            }

            //ellipsoidische Koordinaten
            public double[] coord_EllOrg
            {
                get { return m_coordEllOrg; }
            }

            public double[] coordEllTrafo
            {
                get { return m_coordEllTrafo; }
            }

            //geozentrische Koordinaten
            public double[] coord_GeoOrg
            {
                get { return m_coordGeoOrg; }
            }

            public double[] coord_GeoTrafo
            {
                get { return m_coordGeoTrafo; }
            }

            //GK Koordinaten
            public double[] coord_GKOrg
            {
                get { return m_coordGKOrg; }
            }

            public double[] coords_GKTrafo
            {
                get { return m_coordGKTrafo; }
            }

            public int quality
            {
                get { return m_Quality; }
            }

            //Methoden
            private double convDecimal(double Wert)
            {
                double DezimalGrad = Math.Truncate(Wert);
                DezimalGrad += (Wert - DezimalGrad) / 60 * 100;

                return DezimalGrad;
            }

            //Berechnung geozentrischer Koordinaten
            private void calcGeo(double L, double B, double H)
            {
                Punkt punkt = new Punkt(L, B, H);
                double[] coord = punkt.getCoord_Geo;

                m_coordGeoOrg = coord;
            }

            //Berechnung von GK Koordinaten
            private void calcGK(double L, double B, double H)
            {
                Punkt punkt = new Punkt(L, B, H);
                double[] coord = punkt.getCoord_GK;
                m_coordGKOrg = coord;
            }

            //Transformation
            public GPS_Punkt transform(Trafo Trafo)
            {
                if (Trafo != null)
                {
                    //Transformation
                    m_coordGeoTrafo = Trafo.transform(m_coordGeoOrg);

                    //Berechnung der transformierten ellipsoidischen Koordinaten
                    Ellipsoid ell_GRS80 = new Ellipsoid("Bessel");
                    m_coordEllTrafo = ell_GRS80.calc_CoordEll(m_coordGeoTrafo);

                    //Berechnung der transformierten ellipsoidischen Koordinaten
                    m_coordGKTrafo = ell_GRS80.calc_CoordGK(m_coordEllTrafo);
                }

                return this;
            }
        }
    }
}
