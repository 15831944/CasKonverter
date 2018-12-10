using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CASKonverter.myMath;
using CSML;

namespace CASKonverter.Landesvermessung
{
    //Punkt
    public class Punkt
    {
        //private Ellipsoid Ellipsoid = new Ellipsoid();
        private double[] m_CoordEll = new double[3];      //ellipsoidische Koordinaten
        private double[] m_CoordGeo = new double[3];      //geozentrische Koordinaten
        private double[] m_CoordGK = new double[3];     //topographische Koordinaten

        //Konstruktor
        public Punkt(double P1, double P2, double P3)
        {
                    Ellipsoid ell_GRS80 = new Ellipsoid("GRS-80");
                    m_CoordEll = new double[] { P1, P2, P3 };
                    ell_GRS80.calcParameter(P1, P2);
                    m_CoordGeo = ell_GRS80.calc_CoordGeo(m_CoordEll);
                    m_CoordGK = ell_GRS80.calc_CoordGK(m_CoordEll);
        }

        //Properties
        public double[] coordEllOrg
        {
            get { return m_CoordEll; }
        }

        public double[] getCoord_Geo
        {
            get { return m_CoordGeo; }
        }

        public double[] getCoord_GK
        {
            get { return m_CoordGK; }
        }

        //Methoden
    }

//Bessel Ellipsoid
    public class Ellipsoid
    {
        private double[] m_Parameter;               //Parameter Rotationsellipsoid
        double m_a;                               //große Halbachse
        double m_b;                              //kleine Halbachse
        double m_c;           //Polkrümmungsradius
        double m_Epsilon;   //lineare Exzentrizität
        double m_e;         //1. numerische Exzentrizität
        double m_eStrich;         //2. numerische Exzentrizität
        double m_eta;
        double m_t;
        double m_W;
        double m_M;         //Meridiankrümmungsradius
        double m_N;         //Querkrümmungsradius
        double m_GB;        //Meridianbogenlänge

        //Konstruktor
        public Ellipsoid(string ellipsoid) 
        {
            switch (ellipsoid)
            {
                case "WGS84":
                    m_Parameter = new double[] {6378137.0, 6356752.31424};
                    break;

                case "GRS-80":
                    m_Parameter = new double[] {6378137.0, 6356752.31414};

                    break;

                case "Bessel":
                    m_Parameter = new double[] {6377397.155, 6356078.96282};

                    break;
            }

            m_a = m_Parameter[0];
            m_b = m_Parameter[1];
        }

        //Properties
        public double M
        {
            get { return m_M; }
        }

        public double N
        {
            get { return m_N; }
        }

        public double eStrich
        {
            get { return m_eStrich; }
        }
        public double e
        {
            get { return m_e; }
        }

        public double t
        {
            get { return m_t; }
        }

        public double eta
        {
            get { return m_eta; }
        }

        //Methoden

        public double calc_c()
        {
            return Math.Pow(m_a, 2) / m_b;
        }

        /// <summary>
        /// lineare Exzentrizität
        /// </summary>
        /// <returns></returns>
        private double calc_Epsilon()
        {
            return Math.Sqrt(Math.Pow(m_a, 2) - Math.Pow(m_b, 2));
        }

        /// <summary>
        /// 1. numerische Exzentrität
        /// </summary>
        /// <returns></returns>
        private double calc_e()
        {
            return m_Epsilon / m_a;
        }

        /// <summary>
        /// 2. numerische Exzentrizität
        /// </summary>
        /// <returns></returns>
        private double calc_eStrich()
        {
            return m_Epsilon / m_b;
        }

        private double calc_eta(double B)
        {
            double B_rad = myMath.Convert.Angle(B, myMath.Convert.Unit.Degrees, myMath.Convert.Unit.Radians);
            return m_eStrich * Math.Cos(B_rad);
        }

        private double calc_t(double B)
        {
            double B_rad = myMath.Convert.Angle(B, myMath.Convert.Unit.Degrees, myMath.Convert.Unit.Radians);
            return Math.Tan(B_rad);
        }

        private double calc_W(double B)
        {
            double B_rad = myMath.Convert.Angle(B, myMath.Convert.Unit.Degrees, myMath.Convert.Unit.Radians);
            return Math.Sqrt(1 - Math.Pow(m_e, 2) * Math.Pow(Math.Sin(B_rad), 2));
        }

        /// <summary>
        /// Meridiankrümmungsradius
        /// </summary>
        /// <returns></returns>
        private double calc_M(double B)
        {
            double B_rad = myMath.Convert.Angle(B, myMath.Convert.Unit.Degrees, myMath.Convert.Unit.Radians);
            return m_a / Math.Pow((1 - Math.Pow(m_e, 2) * Math.Pow(Math.Sin(B_rad), 2)), 1.5) * (1 - Math.Pow(m_e, 2));
        }

        /// <summary>
        /// Querkrümmungsradius
        /// </summary>
        /// <returns></returns>
        public double calc_N(double B)
        {
            double B_rad = B / 180 * Math.PI;
            return m_a / Math.Sqrt(1 - Math.Pow(m_e, 2) * Math.Pow(Math.Sin(B_rad), 2));
        }

        //Meridianbogenlänge
        private double calc_GB(double Breite)
        {
            double GB;
            double A, B, C, D, E, F;
            double A1, B1, C1, D1, E1, F1;
            double B_rad = myMath.Convert.Angle(Breite, myMath.Convert.Unit.Degrees, myMath.Convert.Unit.Radians);

            A = 1 - (double) 3 / 4 * Math.Pow(m_eStrich, 2) + (double) 45 / 64 * Math.Pow(m_eStrich, 4) - (double) 175 / 256 * Math.Pow(m_eStrich, 6) + (double) 11025 / 16384 * Math.Pow(m_eStrich, 8) - (double) 43659 / 65536 * Math.Pow(m_eStrich, 10);
            B = (double) 3 / 4 * Math.Pow(m_eStrich, 2) - (double) 15 / 16 * Math.Pow(m_eStrich, 4) + (double) 525 / 512 * Math.Pow(m_eStrich, 6) - (double) 2205 / 2048 * Math.Pow(m_eStrich, 8) + (double) 72765 / 65536 * Math.Pow(m_eStrich, 10);
            C = (double) 15 / 64 * Math.Pow(m_eStrich, 4) - (double) 105 / 256 * Math.Pow(m_eStrich, 6) + (double) 2205 / 4096 * Math.Pow(m_eStrich, 8) - (double) 10395 / 16384 * Math.Pow(m_eStrich, 10);
            D = (double) 35 / 512 * Math.Pow(m_eStrich, 6) - (double) 2205 / 4096 * Math.Pow(m_eStrich, 8) + (double) 31185 / 131072 * Math.Pow(m_eStrich, 10);
            E = (double) 315 / 16384 * Math.Pow(m_eStrich, 8) - (double) 3465 / 65536 * Math.Pow(m_eStrich, 10);
            F = (double) 693 / 131072 * Math.Pow(m_eStrich, 10);

            A1 = A * m_c;
            B1 = (double) 1/2 * B * m_c;
            C1 = (double) 1/4 * C * m_c;
            D1 = (double) 1/6 * D * m_c;
            E1 = (double) 1/8 * E * m_c;
            F1 = (double) 1/10 * F * m_c;

            GB = A1 * B_rad - B1 * Math.Sin(2 * B_rad) + C1 * Math.Sin(4 * B_rad) - D1 * Math.Sin(6 * B_rad) + E1 * Math.Sin(8 * B_rad) - F * Math.Sin(10 * B_rad);

            return GB;
        }

        /// <summary>
        /// Parameter berechnen, wenn Coord gesetzt ist
        /// </summary>
        public void calcParameter(double L, double B)
        {
            //diese Parameter können immer berechnet werden
            m_c = calc_c();
            m_Epsilon = calc_Epsilon();
            m_e = calc_e();
            m_eStrich = calc_eStrich();

            m_eta = calc_eta(B);
            m_t = calc_t(B);
            m_W = calc_W(B);
            m_M = calc_M(B);
            m_N = calc_N(B);
            m_GB = calc_GB(B);
        }

        /// <summary>
        /// Berechnung ellipsoidischer Koordinaten aus geozentrischen Koordinaten
        /// </summary>
        /// <param name="CoordGeo"></param>
        /// <returns></returns>
        public double[] calc_CoordEll(double[] CoordGeo)
        {
            Ellipsoid ell_Bessel = new Ellipsoid("Bessel");
            double[] coordEll = new double[3];
            double s = Math.Sqrt(Math.Pow(CoordGeo[0], 2) + Math.Pow(CoordGeo[1], 2));
            //Länge
            double L_rad = Math.Atan(CoordGeo[1] / CoordGeo[0]);
            double L = L_rad / Math.PI * 180;
            coordEll[0] = L;

            //Breite
            double B_rad= Math.Atan((CoordGeo[2]) / s);
            double B = B_rad / Math.PI * 180;

            for (int i = 0; i < 10000; i++ )
            {
                ell_Bessel.calcParameter(L, B);
                double E = ell_Bessel.e;
                double Ni = ell_Bessel.N;

                B_rad = Math.Atan((CoordGeo[2] + Math.Pow(E,2) * Ni * Math.Sin(B_rad)) / s ) ;
                B = B_rad / Math.PI * 180;
            }

                coordEll[1] = B_rad / Math.PI * 180;

            //Höhe
                double N = ell_Bessel.N;
                coordEll[2] = (CoordGeo[0] / (Math.Cos(B_rad) * Math.Cos(L_rad))) - N;

            return coordEll;
        }

        /// <summary>
        /// Berechnung geozentrischer Koordinaten aus ellipsoidischen Koordinaten
        /// </summary>
        /// <param name="CoordEll"></param>
        /// <returns></returns>
        public double[] calc_CoordGeo(double[] CoordEll)
        {
            Ellipsoid ellipsoid = new Ellipsoid("GRS-80");
            double[] coordGeo = null;
            double L = CoordEll[0];  double L_rad = myMath.Convert.Angle(L, myMath.Convert.Unit.Degrees, myMath.Convert.Unit.Radians);
            double B = CoordEll[1];  double B_rad = myMath.Convert.Angle(B, myMath.Convert.Unit.Degrees, myMath.Convert.Unit.Radians);

            ellipsoid.calcParameter( L, B);

            double x = (ellipsoid.N + CoordEll[2]) * Math.Cos(B_rad) * Math.Cos(L_rad);
            double y = (ellipsoid.N + CoordEll[2]) * Math.Cos(B_rad) * Math.Sin(L_rad);
            double z = (ellipsoid.N * (1 - Math.Pow(ellipsoid.e, 2)) + CoordEll[2]) * Math.Sin(B_rad);

            coordGeo = new double[] { x, y, z };
            return coordGeo;
        }

        /// <summary>
        /// Berechnung von Gauß-Krüger Koordinaten
        /// </summary>
        /// <param name="CoordEll"></param>
        /// <returns></returns>
        public double[] calc_CoordGK(double[] CoordEll)
        {
            const double dl = 13 + (double) 1/3;  //Länge bezogen auf Bezugsmeridian M31
            double[] coordGK = null;
            double B_rad = myMath.Convert.Angle(CoordEll[1], myMath.Convert.Unit.Degrees, myMath.Convert.Unit.Radians);
            double a1, a2, a3, a4, a5, a6, a7, a8;
            double Rechtswert, Hochwert;
            double l = CoordEll[0] - dl;
            double l_rad = myMath.Convert.Angle(l, myMath.Convert.Unit.Degrees, myMath.Convert.Unit.Radians);
            Ellipsoid ell_Bessel = new Ellipsoid("Bessel");
            ell_Bessel.calcParameter(CoordEll[0], CoordEll[1]);
            double N = ell_Bessel.N;
            double GB = ell_Bessel.m_GB;
            double t = ell_Bessel.t;
            double eta = ell_Bessel.m_eta;


            a1 = N * Math.Cos(B_rad);
            a2 = (double) 1/2 * t * N * Math.Pow(Math.Cos(B_rad),2);
            a3 = (double) 1/6 * N * Math.Pow(Math.Cos(B_rad*(1 - Math.Pow(t,2) + Math.Pow(eta,2))),3);
            a4 = (double) 1/24 * N * Math.Sin(B_rad)*Math.Pow(Math.Cos(B_rad*(5-Math.Pow(t,2)+9*Math.Pow(eta,2)+4*Math.Pow(eta,4))),3);
            a5 = (double) 1/120 * N * Math.Pow(Math.Cos(B_rad*(5-18*Math.Pow(t,2)+Math.Pow(t,4)+14*Math.Pow(eta,2)
                 -58*Math.Pow(eta,2) * Math.Pow(t,2)+13*Math.Pow(eta,4)-64*Math.Pow(eta,4)*Math.Pow(t,2))),5);
            a6 = (double) 1/720 * N * Math.Sin(B_rad)*Math.Pow(Math.Cos(B_rad*(61-58*Math.Pow(t,2)+Math.Pow(t,4)
                +270*Math.Pow(eta,2)-330*Math.Pow(eta,4)*Math.Pow(t,2))),5);
            a7 = (double) 1/5040 * N *Math.Pow(Math.Cos(B_rad*(61-479*Math.Pow(t,2)+179*Math.Pow(t,4)-Math.Pow(t,6))),7);
            a8 = (double) 1/40320 * N *Math.Sin(B_rad)*Math.Pow(Math.Cos(B_rad*(1385-3111*Math.Pow(t,2)+543*Math.Pow(t,4)-Math.Pow(t,6))),7);

            Rechtswert = a1*l_rad + a3 * Math.Pow(l_rad,3) + a5 * Math.Pow(l_rad,5) +a7 * Math.Pow(l_rad,7);
            
            Hochwert = GB +a2 * Math.Pow(l_rad,2) + a4 * Math.Pow(l_rad,4) + a6 * Math.Pow(l_rad,6) + a8 * Math.Pow(l_rad,8);
            Hochwert -= 5000000;

            coordGK = new double[] {Rechtswert, Hochwert, CoordEll[2]};
            return coordGK;
        }
    }

    /// <summary>
    /// 7 Parameter Transformation
    /// </summary>
    public class Trafo
    {
        private string m_Name;                              //Name der Transformation
        private double[] m_Drehpunkt= new double[3];        //Drehpunkt im alten System
        private double[] m_Translation = new double[3];     //Translation
        private double[] m_Rotation = new double[3];        //Rotation
        private double m_M;                                 //Maßstab
        List<string> lsZeilen = new List<string>();

        //Konstruktor
        public Trafo(string Text)
        {
            //Kommentare weglassen
            string[] arZeilen = Text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string zeile in arZeilen)
            {
                if (zeile[0] !='\'')
                    lsZeilen.Add(zeile);
            }

            string[] arText = lsZeilen[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            m_Name = arText[0].Substring(0, arText[0].IndexOf('|'));
            m_Drehpunkt = new double[] { double.Parse(arText[9], System.Globalization.CultureInfo.InvariantCulture),
                                         double.Parse(arText[8], System.Globalization.CultureInfo.InvariantCulture),
                                         double.Parse(arText[10], System.Globalization.CultureInfo.InvariantCulture) };

            m_Translation = new double[] { double.Parse(arText[2], System.Globalization.CultureInfo.InvariantCulture),
                                           double.Parse(arText[1], System.Globalization.CultureInfo.InvariantCulture),
                                           double.Parse(arText[3], System.Globalization.CultureInfo.InvariantCulture) };

            m_Rotation = new double[] { double.Parse(arText[5], System.Globalization.CultureInfo.InvariantCulture),
                                        double.Parse(arText[4], System.Globalization.CultureInfo.InvariantCulture),                         
                                        double.Parse(arText[6], System.Globalization.CultureInfo.InvariantCulture) };

            m_M = double.Parse(arText[7],System.Globalization.CultureInfo.InvariantCulture);
        }



        #region Properties
        public string Name
        {
            get { return m_Name; }
        }

        public double[] Drehpunkt
        {
            get { return m_Drehpunkt; }
        }

        public double[] Translation
        {
            get { return m_Translation; }
        }
        
        public double[] Rotation
        {
            get { return m_Rotation; }
        }

        public double M
        {
            get { return m_M; }
        }
        #endregion

        #region Methoden
        /// <summary>
        /// Transformation der geozentrischen Koordinaten
        /// </summary>
        /// <param name="coordGeoOrg"></param>
        /// <returns></returns>
        public double[] transform(double[] coordGeoOrg)
        {
            double[] coordGeoTrafo = new double[3];

            //Rotation
            Matrix M = new Matrix(3, 3);
            Matrix P = new Matrix(1, 3);

            P = new Matrix(new double[,] {{coordGeoOrg[0] - m_Drehpunkt[0],
                                           coordGeoOrg[1] - m_Drehpunkt[1],
                                           coordGeoOrg[2] - m_Drehpunkt[2]}});

            //x-Achse
            double rX_rad = -m_Rotation[1] / 200 * Math.PI;
            M = new Matrix(new double[,] {{Math.Cos(rX_rad), 0, -Math.Sin(rX_rad)},
                                          {0,1,0},
                                          {Math.Sin(rX_rad), 0, Math.Cos(rX_rad)}});

            P = P * M;

            //y-Achse
            double rY_rad = -m_Rotation[0] / 200 * Math.PI;
            M = new Matrix(new double[,] {{1,0,0},
                                          {0, Math.Cos(rY_rad), -Math.Sin(rY_rad)},
                                          {0, Math.Sin(rY_rad), Math.Cos(rY_rad)}});

            P = P * M;

            //z-Achse
            double rZ_rad = -m_Rotation[2] / 200 * Math.PI;
            M = new Matrix(new double[,] {{Math.Cos(rZ_rad), -Math.Sin(rZ_rad), 0},
                                          {Math.Sin(rZ_rad), Math.Cos(rZ_rad), 0},
                                          {0,0,1}});

            P = P * M;

            coordGeoTrafo[0] = m_Drehpunkt[0] + P.Column(1)[1].Re;
            coordGeoTrafo[1] = m_Drehpunkt[1] + P.Column(2)[1].Re;
            coordGeoTrafo[2] = m_Drehpunkt[2] + P.Column(3)[1].Re;

            //Translation
            coordGeoTrafo[0] = coordGeoTrafo[0] + m_Translation[0];
            coordGeoTrafo[1] = coordGeoTrafo[1] + m_Translation[1];
            coordGeoTrafo[2] = coordGeoTrafo[2] + m_Translation[2];

            //Maßstab
            coordGeoTrafo[0] *= m_M;
            coordGeoTrafo[1] *= m_M;
            coordGeoTrafo[2] *= m_M;

            return coordGeoTrafo;
        }
        #endregion
    }
}

