using System;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetElectricity : AutobudgetNoVehicles
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetElectricity d = Singleton<AutobudgetManager>.instance.container.AutobudgetElectricity;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.AutobudgetBuffer);
                s.WriteInt32(d.BudgetMaxValue);
                s.WriteBool(d.PauseWhenBudgetTooHigh);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetElectricity d = Singleton<AutobudgetManager>.instance.container.AutobudgetElectricity;
                d.Enabled = s.ReadBool();
                d.AutobudgetBuffer = s.ReadInt32();
                d.BudgetMaxValue = s.ReadInt32();
                d.PauseWhenBudgetTooHigh = s.ReadBool();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetElectricity data loaded.");
            }
        }

        private int capacity_prev = 0;
        private int consumption_prev = 0;

        public int AutobudgetBuffer = 3; // Percent of capacity
        public int BudgetMaxValue = 140;
        public bool PauseWhenBudgetTooHigh = true;

        protected override int refreshCount
        {
            get
            {
                return 7;
            }
        }

        protected override void setAutobudget()
        {
            DistrictManager dm = Singleton<DistrictManager>.instance;

            int capacity = dm.m_districts.m_buffer[0].GetElectricityCapacity();

            // No electricity
            if (capacity <= 0) return;

            int consumption = dm.m_districts.m_buffer[0].GetElectricityConsumption();

            // No changes from the previous state
            if (capacity == capacity_prev && consumption == consumption_prev) return;

            capacity_prev = capacity;
            consumption_prev = consumption;

            AutobudgetObjectsContainer o = Singleton<AutobudgetManager>.instance.container;
            EconomyManager em = Singleton<EconomyManager>.instance;
            SimulationManager sm = Singleton<SimulationManager>.instance;

            int budget = em.GetBudget(ItemClass.Service.Electricity, ItemClass.SubService.None, sm.m_isNightTime);
            int newBudget = calculateNewBudget(capacity, consumption, budget, getBufferCoefficient(AutobudgetBuffer));

            newBudget = Math.Min(newBudget, BudgetMaxValue);

            if (newBudget == BudgetMaxValue && PauseWhenBudgetTooHigh && !isPausedRecently)
            {
                SetPause();
                isPausedRecently = true;
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Electricity, InfoManager.SubInfoMode.Default);
            }

            if (newBudget < BudgetMaxValue)
            {
                isPausedRecently = false;
            }

            //Debug.Log("Electricity: " + budget.ToString() + " -> " + newBudget.ToString());
            setBudget(ItemClass.Service.Electricity, ItemClass.SubService.None, newBudget);
        }
    }
}
