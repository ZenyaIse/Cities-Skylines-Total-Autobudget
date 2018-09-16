using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace AutoBudget   
{
    public class AutobudgetObjectsContainer
    {
        private const string optionsFileName = "TotalAutobudgetOptions.xml";

        public AutobudgetElectricity AutobudgetElectricity;
        public AutobudgetWater AutobudgetWater;
        public AutobudgetGarbage AutobudgetGarbage;
        public AutobudgetHealthcare AutobudgetHealthcare;
        public AutobudgetEducation AutobudgetEducation;
        public AutobudgetPolice AutobudgetPolice;
        public AutobudgetFire AutobudgetFire;
        public AutobudgetRoad AutobudgetRoad;
        public AutobudgetTaxi AutobudgetTaxi;

        [XmlIgnore]
        public List<AutobudgetBase> AllAutobudgetObjects = new List<AutobudgetBase>();

        public void Save()
        {
            XmlSerializer ser = new XmlSerializer(typeof(AutobudgetObjectsContainer));
            TextWriter writer = new StreamWriter(getOptionsFilePath());
            ser.Serialize(writer, this);
            writer.Close();
        }

        public void InitObjects()
        {
            if (AutobudgetElectricity == null) AutobudgetElectricity = new AutobudgetElectricity();
            if (AutobudgetWater == null) AutobudgetWater = new AutobudgetWater();
            if (AutobudgetGarbage == null) AutobudgetGarbage = new AutobudgetGarbage();
            if (AutobudgetHealthcare == null) AutobudgetHealthcare = new AutobudgetHealthcare();
            if (AutobudgetEducation == null) AutobudgetEducation = new AutobudgetEducation();
            if (AutobudgetPolice == null) AutobudgetPolice = new AutobudgetPolice();
            if (AutobudgetFire == null) AutobudgetFire = new AutobudgetFire();
            if (AutobudgetRoad == null) AutobudgetRoad = new AutobudgetRoad();
            if (AutobudgetTaxi == null) AutobudgetTaxi = new AutobudgetTaxi();

            AllAutobudgetObjects.Clear();
            AllAutobudgetObjects.Add(AutobudgetElectricity);
            AllAutobudgetObjects.Add(AutobudgetWater);
            AllAutobudgetObjects.Add(AutobudgetGarbage);
            AllAutobudgetObjects.Add(AutobudgetHealthcare);
            AllAutobudgetObjects.Add(AutobudgetEducation);
            AllAutobudgetObjects.Add(AutobudgetPolice);
            AllAutobudgetObjects.Add(AutobudgetFire);
            AllAutobudgetObjects.Add(AutobudgetRoad);
            AllAutobudgetObjects.Add(AutobudgetTaxi);
        }

        public static AutobudgetObjectsContainer CreateFromFile()
        {
            string path = getOptionsFilePath();

            if (!File.Exists(path)) return null;

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(AutobudgetObjectsContainer));
                TextReader reader = new StreamReader(path);
                AutobudgetObjectsContainer instance = (AutobudgetObjectsContainer)ser.Deserialize(reader);
                reader.Close();

                return instance;
            }
            catch
            {
                return null;
            }
        }

        private static string getOptionsFilePath()
        {
            //return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Colossal Order", "Cities_Skylines", optionsFileName);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Combine(path, "Colossal Order");
            path = Path.Combine(path, "Cities_Skylines");
            path = Path.Combine(path, optionsFileName);
            return path;
        }
    }
}