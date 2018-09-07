using ColossalFramework;

namespace AutoBudget
{
    public class AutobudgetPolice : AutobudgetVehicles
    {
        public int BudgetMinValue = 50;
        public int BudgetMaxValue = 120;

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 2 + 3;
            }
        }

        protected override void setAutobudget()
        {
            setBudgetForVehicles(
                ItemClass.Service.PoliceDepartment,
                typeof(PoliceStationAI),
                1,
                BudgetMinValue,
                BudgetMaxValue);
        }
    }
}
