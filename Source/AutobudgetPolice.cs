using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetPolice : AutobudgetVehicles
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetPolice d = Singleton<AutobudgetManager>.instance.container.AutobudgetPolice;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.BudgetMinValue);
                s.WriteInt32(d.BudgetMaxValue);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetPolice d = Singleton<AutobudgetManager>.instance.container.AutobudgetPolice;
                d.Enabled = s.ReadBool();
                d.BudgetMinValue = s.ReadInt32();
                d.BudgetMaxValue = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetPolice data loaded.");
            }
        }

        public int BudgetMinValue = 50;
        public int BudgetMaxValue = 110;

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "Police";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.PoliceDepartment;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 2 + 13;
            }
        }

        protected override void setAutobudget()
        {
            setBudgetForVehicles(typeof(PoliceStationAI), 1, BudgetMinValue, BudgetMaxValue);
        }
    }
}
