using System;
using System.IO;
using System.Xml.Serialization;

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
        //public AutobudgetFire AutobudgetFire;
        //public AutobudgetPolice AutobudgetPolice;
        //public AutobudgetTaxi AutobudgetTaxi;

        public void Save()
        {
            XmlSerializer ser = new XmlSerializer(typeof(AutobudgetObjectsContainer));
            TextWriter writer = new StreamWriter(getOptionsFilePath());
            ser.Serialize(writer, this);
            writer.Close();
        }

        public void CreateObjectsIfNotCreated()
        {
            if (AutobudgetElectricity == null) AutobudgetElectricity = new AutobudgetElectricity();
            if (AutobudgetWater == null) AutobudgetWater = new AutobudgetWater();
            if (AutobudgetGarbage == null) AutobudgetGarbage = new AutobudgetGarbage();
            if (AutobudgetHealthcare == null) AutobudgetHealthcare = new AutobudgetHealthcare();
            if (AutobudgetEducation == null) AutobudgetEducation = new AutobudgetEducation();
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
            //return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Colossal Order\\Cities_Skylines\\" + optionsFileName;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            path = Path.Combine(path, "Colossal Order");
            path = Path.Combine(path, "Cities_Skylines");
            path = Path.Combine(path, optionsFileName);
            return path;
        }
    }
}