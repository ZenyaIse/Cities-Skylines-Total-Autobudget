using System;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetWater : AutobudgetNoVehicles
    {
        public class Data : IDataContainer
        {
            public void Serialize(DataSerializer s)
            {
                AutobudgetWater d = Singleton<AutobudgetManager>.instance.container.AutobudgetWater;
                s.WriteBool(d.Enabled);
                s.WriteInt32(d.AutobudgetBuffer);
                s.WriteInt32(d.BudgetMaxValue);
                s.WriteBool(d.PauseWhenBudgetTooHigh);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetWater d = Singleton<AutobudgetManager>.instance.container.AutobudgetWater;
                d.Enabled = s.ReadBool();
                d.AutobudgetBuffer = s.ReadInt32();
                d.BudgetMaxValue = s.ReadInt32();
                d.PauseWhenBudgetTooHigh = s.ReadBool();
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetWater data loaded.");
            }
        }

        private int waterCapacity_prev = 0;
        private int waterConsumption_prev = 0;
        private int sewageCapacity_prev = 0;
        private int sewageAccumulation_prev = 0;

        public int AutobudgetBuffer = 3; // Percent of capacity
        public int BudgetMaxValue = 140;
        public bool PauseWhenBudgetTooHigh = true;

        public override string GetEconomyPanelContainerName()
        {
            return "ServicesBudgetContainer";
        }

        public override string GetBudgetItemName()
        {
            return "WaterAndSewage";
        }

        public override ItemClass.Service GetService()
        {
            return ItemClass.Service.Water;
        }

        public override ItemClass.SubService GetSubService()
        {
            return ItemClass.SubService.None;
        }

        protected override int refreshCount
        {
            get
            {
                return 113;
            }
        }

        protected override void setAutobudget()
        {
            DistrictManager dm = Singleton<DistrictManager>.instance;

            // Water
            int waterCapacity = dm.m_districts.m_buffer[0].GetWaterCapacity();
            int waterConsumption = dm.m_districts.m_buffer[0].GetWaterConsumption();
            // Sewage
            int sewageCapacity = dm.m_districts.m_buffer[0].GetSewageCapacity();
            int sewageAccumulation = dm.m_districts.m_buffer[0].GetSewageAccumulation();

            // No water and no sewage
            if (waterCapacity <= 0 && sewageCapacity <= 0) return;

            // No changes from the previous state
            if (waterCapacity == waterCapacity_prev && waterConsumption == waterConsumption_prev
                && sewageCapacity == sewageCapacity_prev && sewageAccumulation == sewageAccumulation_prev) return;

            waterCapacity_prev = waterCapacity;
            waterConsumption_prev = waterConsumption;
            sewageCapacity_prev = sewageCapacity;
            sewageAccumulation_prev = sewageAccumulation;

            AutobudgetObjectsContainer o = Singleton<AutobudgetManager>.instance.container;
            EconomyManager em = Singleton<EconomyManager>.instance;
            SimulationManager sm = Singleton<SimulationManager>.instance;

            int budget = em.GetBudget(ItemClass.Service.Water, ItemClass.SubService.None, sm.m_isNightTime);

            float buffer = getBufferCoefficient(AutobudgetBuffer);
            int newWaterBudget = calculateNewBudget(waterCapacity, waterConsumption, budget, buffer);
            int newSewageBudget = calculateNewBudget(sewageCapacity, sewageAccumulation, budget, buffer);
            int newBudget = Math.Max(newWaterBudget, newSewageBudget);

            newBudget = Math.Min(newBudget, BudgetMaxValue);

            if (newBudget == BudgetMaxValue && PauseWhenBudgetTooHigh && !isPausedRecently)
            {
                SetPause();
                isPausedRecently = true;
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Water, InfoManager.SubInfoMode.Default);
            }

            if (newBudget < BudgetMaxValue)
            {
                isPausedRecently = false;
            }

            setBudget(newBudget);
        }
    }
}
