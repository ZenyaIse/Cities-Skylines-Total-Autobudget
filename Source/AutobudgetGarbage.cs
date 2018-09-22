using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetGarbage : AutobudgetVehicles
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetGarbage d = Singleton<AutobudgetManager>.instance.container.AutobudgetGarbage;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.BudgetMaxValue);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetGarbage d = Singleton<AutobudgetManager>.instance.container.AutobudgetGarbage;
                d.Enabled = s.ReadBool();
                d.BudgetMaxValue = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetGarbage data loaded.");
            }
        }

        public int BudgetMaxValue = 110;

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "Garbage";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.Garbage;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 2 + 31;
            }
        }

        protected override void setAutobudget()
        {
            setBudgetForVehicles(typeof(LandfillSiteAI), 1, 50, BudgetMaxValue);
        }
    }
}
