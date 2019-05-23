using ColossalFramework;

namespace Autobudget
{
    public class AutobudgetManager : Singleton<AutobudgetManager>
    {
        public AutobudgetObjectsContainer container;

        private AutobudgetManager()
        {
            ReadValuesFromFile();
        }

        public void SetAutobudgetAll()
        {
            foreach (AutobudgetBase obj in container.AllAutobudgetObjects)
            {
                obj.SetAutobudget();
            }
        }

        public void ResetToDefaultValues()
        {
            container = new AutobudgetObjectsContainer();
            container.InitObjects();
        }

        public void ReadValuesFromFile()
        {
            container = AutobudgetObjectsContainer.CreateFromFile();
            if (container == null)
            {
                container = new AutobudgetObjectsContainer();
            }

            container.InitObjects();
        }
    }
}
