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

            container.CreateObjectsIfNotCreated();
        }

        public void SetAutobudgetAll()
        {
            container.AutobudgetElectricity.SetAutobudget();
            container.AutobudgetWater.SetAutobudget();
            container.AutobudgetGarbage.SetAutobudget();
            container.AutobudgetHealthcare.SetAutobudget();
            container.AutobudgetEducation.SetAutobudget();
            //container.AutobudgetFire.SetAutobudget();
            //container.AutobudgetPolice.SetAutobudget();
            //container.AutobudgetTaxi.SetAutobudget();
        }
    }
}
