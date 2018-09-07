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

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 2 + 31;
            }
        }

        protected override void setAutobudget()
        {
            setBudgetForVehicles(
                ItemClass.Service.Garbage,
                typeof(LandfillSiteAI),
                1,
                50,
                BudgetMaxValue);
        }
    }
}
