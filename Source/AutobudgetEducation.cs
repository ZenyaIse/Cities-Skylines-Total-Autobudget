using System;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace Autobudget
{
    public class AutobudgetEducation : AutobudgetBase
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetEducation d = Singleton<AutobudgetManager>.instance.container.AutobudgetEducation;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.ElementaryEducationTargetRate);
                s.WriteInt32(d.HighEducationTargetRate);
                s.WriteInt32(d.BudgetMaxValue);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetEducation d = Singleton<AutobudgetManager>.instance.container.AutobudgetEducation;
                if (s.version <= 2)
                {
                    d.Enabled = s.ReadBool();
                    d.ElementaryEducationTargetRate = s.ReadInt32();
                    d.HighEducationTargetRate = s.ReadInt32();
                    int tmp = s.ReadInt32();
                    d.BudgetMaxValue = s.ReadInt32();
                }
                else
                {
                    d.Enabled = s.ReadBool();
                    d.ElementaryEducationTargetRate = s.ReadInt32();
                    d.HighEducationTargetRate = s.ReadInt32();
                    d.BudgetMaxValue = s.ReadInt32();
                }
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetEducation data loaded.");
            }
        }

        public int BudgetMaxValue = 120;
        public int ElementaryEducationTargetRate = 90; // %
        public int HighEducationTargetRate = 90; // %

        public AutobudgetEducation()
        {
            refreshCount = 599;
        }

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "Education";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.Education;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override void setAutobudget()
        {
            District d = Singleton<DistrictManager>.instance.m_districts.m_buffer[0];

            int capacity1 = d.GetEducation1Capacity();
            int capacity2 = d.GetEducation2Capacity();

            // No education facilities
            if (capacity1 == 0 && capacity2 == 0) return;

            int need1 = d.GetEducation1Need();
            int need2 = d.GetEducation2Need();

            //Debug.Log(string.Format("Capacity: {0}, need: {1}", capacity1, need1));

            EconomyManager em = Singleton<EconomyManager>.instance;
            SimulationManager sm = Singleton<SimulationManager>.instance;

            int currentBudget = em.GetBudget(ItemClass.Service.Education, ItemClass.SubService.None, sm.m_isNightTime);
            float currentProductionRate = Helper.GetProductionRateFromBudget(currentBudget);

            // Convert to the normal capacity
            capacity1 = (int)(capacity1 / currentProductionRate + 0.5f);
            capacity2 = (int)(capacity2 / currentProductionRate + 0.5f);

            //Debug.Log(string.Format("currentBudget: {0}, currentProductionRate: {1}, normal capacity", currentBudget, currentProductionRate, capacity1));

            float targetProductionRate1 = 0.25f;
            if (capacity1 > 0 && need1 > 0)
            {
                targetProductionRate1 = need1 * (ElementaryEducationTargetRate * 0.01f) / capacity1;
            }

            float targetProductionRate2 = 0.25f;
            if (capacity2 > 0 && need2 > 0)
            {
                targetProductionRate2 = need2 * (HighEducationTargetRate * 0.01f) / capacity2;
            }

            float targetProductionRate = Math.Max(targetProductionRate1, targetProductionRate2);

            int newBudget = Helper.GetBudgetFromProductionRate(targetProductionRate);

            //Debug.Log(string.Format("targetProductionRate: {0}, newBudget: {1}", targetProductionRate, newBudget));

            newBudget = Math.Min(newBudget, BudgetMaxValue);

            setBudget(newBudget);
        }
    }
}
