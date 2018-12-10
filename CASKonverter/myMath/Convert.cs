using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CASKonverter.myMath
{
    public static class Convert
    {
        public enum Unit
        {
            Degrees,
            Grads,
            Radians
        }

        public static double RadiansTo(double Radians, Unit ToWhat)
        {
            if (ToWhat == Unit.Degrees)
            {
                return Radians * (180 / System.Math.PI);
            }
            else
            {
                return Radians * (200 / System.Math.PI);
            }
        }

        public static double Angle(double angle, Unit UnitFrom, Unit UnitTo)
        {
            switch (UnitFrom)
            {
                case Unit.Degrees:
                    angle = angle * (System.Math.PI / 180);
                    break;

                case Unit.Grads:
                    angle = angle * (System.Math.PI / 200);
                    break;
            }

            switch (UnitTo)
            {
                case Unit.Degrees:
                    angle = angle * (180 / System.Math.PI);
                    break;

                case Unit.Grads:
                    angle = angle * (200 / System.Math.PI);
                    break;

                case Unit.Radians:

                    break;
            }
            return angle;
        }


        /// <summary>
        /// verschieben der Kommastelle, sodass eine Integerzahl zurückgegeben werden kann
        /// </summary>
        /// <param name="Wert"></param>
        /// <param name="Precision"></param>
        /// <returns></returns>
        public static long shiftDelímeter(double Wert, double Precision)
        {
            if (Precision == 0)
                return 0;

            Wert /= Precision;
            Wert = Math.Round(Wert);
            long iWert = (long)Wert;

            return iWert;
        }
    }
}
