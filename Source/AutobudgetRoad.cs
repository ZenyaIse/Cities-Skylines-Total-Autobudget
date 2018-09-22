using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetRoad : AutobudgetVehicles
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetRoad d = Singleton<AutobudgetManager>.instance.container.AutobudgetRoad;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.BudgetMaxValue);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetRoad d = Singleton<AutobudgetManager>.instance.container.AutobudgetRoad;
                d.Enabled = s.ReadBool();
                d.BudgetMaxValue = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetRoad data loaded.");
            }
        }

        public int BudgetMaxValue = 110;

        public override string GetEconomyPanelContainerName()
        {
            return "SubServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "Roads";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.Road;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 2 + 37;
            }
        }

        protected override void setAutobudget()
        {
            setBudgetForVehicles(typeof(SnowDumpAI), 1, 50, BudgetMaxValue);
        }
    }
}
