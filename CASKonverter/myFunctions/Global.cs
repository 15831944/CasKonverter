using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace CASKonverter.myFormats
{
    public static class Global
    {
        public static string Format0 = "0";
        public static string Format1 = "0.0";
        public static string Format2 = "0.00";
        public static string Format3 = "0.000";
        public static string Format4 = "0.0000";
        public static string Format5 = "0.00000";

        public static double Format(int iKomma)
        {
            double dFormat = 1;

            for (int i = 0; i < iKomma; i++)
                dFormat /= 10;

            if (dFormat == 1)
                dFormat = 0;

            return dFormat;
        }

        public static NumberFormatInfo Provider0
        {
            get
            {
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                provider.NumberDecimalDigits = 0;
                provider.NumberGroupSizes = new int[] { 3 };

                return provider;
            }
        }
        public static NumberFormatInfo Provider3
        {
            get
            {
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                provider.NumberDecimalDigits = 3;
                provider.NumberGroupSizes = new int[] { 3 };

                return provider;
            }
        }

        public static NumberFormatInfo Provider4
        {
            get {
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                provider.NumberDecimalDigits = 4;

                return provider;
            }
        }

        public static NumberFormatInfo Provider5
        {
            get
            {
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                provider.NumberDecimalDigits = 5;
               
                return provider;
            }
        }
        
        public static int StringDigits(string String)
        {
            int PosKomma = String.LastIndexOf('.');
            if (PosKomma == -1)
                return 0;

            int Kommastellen = String.Length - PosKomma -1;

            return Kommastellen;   
        }

        public static string Formatstring(int Kommastellen)
        {
            string Formatstring = String.Empty;

            return Formatstring;
        }
    }
}
 