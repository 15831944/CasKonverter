using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Xml;

namespace CASKonverter.myUtilities
{
    public sealed class mySettings
    {
        private static volatile mySettings instance;
        private static object syncRoot = new object();

        private XmlDocument doc = new XmlDocument();
        private XmlNode myRoot, myNode;
        private XmlNodeList myNodeList;
        private string m_xmlName;

        private int m_Nachkommastellen;

        //Konstruktor
        public mySettings()
        {
            Assembly assem = typeof(CASKonverter).Assembly;
            m_xmlName = Path.Combine(Path.GetDirectoryName(assem.Location), "settings.xml");

            try { doc.Load(m_xmlName); }
            catch (System.IO.FileNotFoundException)
            {
                ////Defaulteinstellungen
                //Knoten
                myRoot = doc.CreateElement(assem.GetName().Name.ToString() + "_" + assem.GetName().Version.ToString());
                doc.AppendChild(myRoot);

                //Nachkommastellen
                myNode = doc.CreateElement("Nachkommastellen");
                myNode.InnerText = "4";

                myRoot.AppendChild(myNode);

                doc.Save(m_xmlName);
            }

            //Werte auslesen
            myNodeList = doc.GetElementsByTagName("Nachkommastellen");
            myNode = myNodeList[0];
            m_Nachkommastellen = Convert.ToInt32(myNode.InnerText.ToString());
        }

        //Properties
        public int Nachkommastellen
        {
            get { return m_Nachkommastellen; }
        }

        //Methoden
        public static mySettings Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new mySettings();
                    }
                }
                return instance;
            }
        }
        public void chgNachkommatellen(int digits)
        {
            myNodeList = doc.GetElementsByTagName("Nachkommastellen");
            myNode = myNodeList[0];
            myNode.InnerText = digits.ToString();
            m_Nachkommastellen = digits;

            doc.Save(m_xmlName);
        }
    }
}
