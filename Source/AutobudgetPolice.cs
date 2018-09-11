using ColossalFramework;

namespace AutoBudget
{
    public class AutobudgetPolice : AutobudgetVehicles
    {
        public int BudgetMinValue = 50;
        public int BudgetMaxValue = 120;

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "Police";
        }

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 2 + 3;
            }
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.PoliceDepartment;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override void setAutobudget()
        {
            setBudgetForVehicles(
                typeof(PoliceStationAI),
                1,
                BudgetMinValue,
                BudgetMaxValue);
        }
    }
}
