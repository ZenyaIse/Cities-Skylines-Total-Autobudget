using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetHealthcare : AutobudgetVehicles
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetHealthcare d = Singleton<AutobudgetManager>.instance.container.AutobudgetHealthcare;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.BudgetMinValue);
                s.WriteInt32(d.BudgetMaxValue);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetHealthcare d = Singleton<AutobudgetManager>.instance.container.AutobudgetHealthcare;
                d.Enabled = s.ReadBool();
                d.BudgetMinValue = s.ReadInt32();
                d.BudgetMaxValue = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetHealthcare data loaded.");
            }
        }

        public int BudgetMinValue = 100;
        public int BudgetMaxValue = 120;

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "Healthcare";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.HealthCare;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 2 + 47;
            }
        }

        protected override void setAutobudget()
        {
            int healthcareBudget = getBudgetForVehicles(typeof(HospitalAI), 1, BudgetMinValue, BudgetMaxValue);
            int deathcareBudget = getBudgetForVehicles(typeof(CemeteryAI), 1, BudgetMinValue, BudgetMaxValue);

            setBudget(Mathf.Max(healthcareBudget, deathcareBudget));
        }
    }
}
