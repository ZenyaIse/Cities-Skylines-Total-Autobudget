using ColossalFramework;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetFire : AutobudgetVehicles
    {
        public int FireTracksExcessNum = 2;
        public int BudgetMinValue = 50;
        public int BudgetMaxValue = 120;

        public static string[] StrategyNames = new string[] { "Less tracks", "Normal", "More tracks" };
        public static int TracksExcessNumToStrategyIndex(int tracksNum)
        {
            return Mathf.Clamp(tracksNum - 1, 0, 2);
        }
        public static int StrategyIndexToTracksExcessNum(int index)
        {
            return index + 1;
        }

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 4;
            }
        }

        protected override void setAutobudget()
        {
            setBudgetForVehicles(
                ItemClass.Service.FireDepartment,
                typeof(PoliceStationAI),
                FireTracksExcessNum,
                BudgetMinValue,
                BudgetMaxValue);
        }
    }
}
