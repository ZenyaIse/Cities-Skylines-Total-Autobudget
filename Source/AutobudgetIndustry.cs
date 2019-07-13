using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace Autobudget
{
    public class AutobudgetIndustry : AutobudgetBase
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetIndustry d = Singleton<AutobudgetManager>.instance.container.AutobudgetIndustry;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.BudgetMinValue);
                s.WriteInt32(d.BudgetMaxValue);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetIndustry d = Singleton<AutobudgetManager>.instance.container.AutobudgetIndustry;
                d.Enabled = s.ReadBool();
                d.BudgetMinValue = s.ReadInt32();
                d.BudgetMaxValue = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetIndustry data loaded.");
            }
        }

        public int BudgetMinValue = 50;
        public int BudgetMaxValue = 120;

        public AutobudgetIndustry()
        {
            refreshCount = 211;
        }

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "PlayerIndustry";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.PlayerIndustry;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override void setAutobudget()
        {
            int industryBudget = getBudgetForVehicles(typeof(WarehouseAI), 1, BudgetMinValue, BudgetMaxValue);

            setBudget(industryBudget);
        }
    }
}
