using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace Autobudget
{
    public class AutobudgetPost : AutobudgetBase
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetPost d = Singleton<AutobudgetManager>.instance.container.AutobudgetPost;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.BudgetMaxValue);
            }

            public void Deserialize(DataSerializer s)
            {
                if (s.version >= 4)
                {
                    AutobudgetPost d = Singleton<AutobudgetManager>.instance.container.AutobudgetPost;
                    d.Enabled = s.ReadBool();
                    d.BudgetMaxValue = s.ReadInt32();
                }
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetPost data loaded.");
            }
        }

        public int BudgetMaxValue = 120;

        public AutobudgetPost()
        {
            refreshCount = 397;
        }

        public override string GetEconomyPanelContainerName()
        {
            return "SubServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "Post";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.PublicTransport;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.PublicTransportPost;
        }

        protected override void setAutobudget()
        {
            setBudget(getBudgetForVehicles(typeof(PostOfficeAI), 1, 50, BudgetMaxValue));
        }
    }
}
