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

        public int BudgetMinValue = 101;
        public int BudgetMaxValue = 120;

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 2 + 47;
            }
        }

        protected override void setAutobudget()
        {
            setBudgetForVehicles(
                ItemClass.Service.HealthCare,
                typeof(HospitalAI),
                1,
                BudgetMinValue,
                BudgetMaxValue);
        }
    }
}
