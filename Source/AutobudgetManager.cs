using ColossalFramework;

namespace AutoBudget
{
    public class AutobudgetManager : Singleton<AutobudgetManager>
    {
        public AutobudgetObjectsContainer container;

        private AutobudgetManager()
        {
            container = AutobudgetObjectsContainer.CreateFromFile();
            if (container == null)
            {
                container = new AutobudgetObjectsContainer();
            }

            container.InitObjects();
        }

        public void SetAutobudgetAll()
        {
            foreach (AutobudgetBase obj in container.AllAutobudgetObjects)
            {
                obj.SetAutobudget();
            }
        }
    }
}
