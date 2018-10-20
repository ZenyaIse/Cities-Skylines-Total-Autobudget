using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetFire : AutobudgetVehicles
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetFire d = Singleton<AutobudgetManager>.instance.container.AutobudgetFire;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.BudgetMinValue);
                s.WriteInt32(d.BudgetMaxValue);
                s.WriteInt32(d.FireTracksExcessNum);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetFire d = Singleton<AutobudgetManager>.instance.container.AutobudgetFire;
                d.Enabled = s.ReadBool();
                d.BudgetMinValue = s.ReadInt32();
                d.BudgetMaxValue = s.ReadInt32();
                d.FireTracksExcessNum = s.ReadInt32();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetFire data loaded.");
            }
        }

        public int FireTracksExcessNum = 2;
        public int BudgetMinValue = 50;
        public int BudgetMaxValue = 120;

        public AutobudgetFire()
        {
            refreshCount = 149;
        }

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "FireDepartment";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.FireDepartment;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override void setAutobudget()
        {
            int fireBudget = getBudgetForVehicles(typeof(FireStationAI), FireTracksExcessNum, BudgetMinValue, BudgetMaxValue);
            int disasterBudget = getBudgetForVehicles(typeof(DisasterResponseBuildingAI), 1, BudgetMinValue, BudgetMaxValue);

            setBudget(Mathf.Max(fireBudget, disasterBudget));
        }
    }
}
