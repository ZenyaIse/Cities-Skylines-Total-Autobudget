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

                s.WriteInt32(d.TargetWaterStorageRatio);

                s.WriteBool(d.UseHeatingAutobudget);
                s.WriteInt32(d.HeatingBudgetMaxValue);
                s.WriteInt32(d.currentHeatingBudget);
            }

            public void Deserialize(DataSerializer s)
            {
                AutobudgetWater d = Singleton<AutobudgetManager>.instance.container.AutobudgetWater;
                if (s.version == 0)
                {
                    d.Enabled = s.ReadBool();
                    d.AutobudgetBuffer = s.ReadInt32();
                    d.BudgetMaxValue = s.ReadInt32();
                    d.PauseWhenBudgetTooHigh = s.ReadBool();
                    d.TargetWaterStorageRatio = s.ReadInt32();
                    d.UseHeatingAutobudget = s.ReadBool();
                    d.HeatingBudgetMaxValue = s.ReadInt32();
                    d.currentHeatingBudget = s.ReadInt32();

                    if (d.TargetWaterStorageRatio == 95)
                        d.TargetWaterStorageRatio = 50; // Change the default value
                }
                else if (s.version == 1)
                {
                    d.Enabled = s.ReadBool();
                    d.AutobudgetBuffer = s.ReadInt32();
                    d.BudgetMaxValue = s.ReadInt32();
                    d.PauseWhenBudgetTooHigh = s.ReadBool();

                    d.TargetWaterStorageRatio = s.ReadInt32();
                    bool tmp = s.ReadBool();

                    d.UseHeatingAutobudget = s.ReadBool();
                    d.HeatingBudgetMaxValue = s.ReadInt32();
                    d.currentHeatingBudget = s.ReadInt32();
                }
                else
                {
                    // Revert to the version 0
                    d.Enabled = s.ReadBool();
                    d.AutobudgetBuffer = s.ReadInt32();
                    d.BudgetMaxValue = s.ReadInt32();
                    d.PauseWhenBudgetTooHigh = s.ReadBool();

                    d.TargetWaterStorageRatio = s.ReadInt32();

                    d.UseHeatingAutobudget = s.ReadBool();
                    d.HeatingBudgetMaxValue = s.ReadInt32();
                    d.currentHeatingBudget = s.ReadInt32();
                }
            }

            public void AfterDeserialize(DataSerializer s)
            {
                Debug.Log(Mod.LogMsgPrefix + "AutobudgetWater data loaded.");
            }
        }

        private int currentHeatingBudget = 0;
        private int heatingCounter = 0;
        private int heatingRefreshCount = 1;

        public int AutobudgetBuffer = 3; // Percent of capacity
        public int BudgetMaxValue = 140;
        public bool PauseWhenBudgetTooHigh = true;
        public int TargetWaterStorageRatio = 50; // Percent of the water capacity
        public bool UseHeatingAutobudget = true;
        public int HeatingBudgetMaxValue = 120;

        public AutobudgetWater()
        {
            refreshCount = 151;
        }

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

            int waterStorageCapacity = dm.m_districts.m_buffer[0].GetWaterStorageCapacity();
            int waterStorageAmount = dm.m_districts.m_buffer[0].GetWaterStorageAmount();
            int waterStorageRatio = waterStorageCapacity == 0 ? 0 : waterStorageAmount * 100 / waterStorageCapacity;

            AutobudgetObjectsContainer o = Singleton<AutobudgetManager>.instance.container;
            EconomyManager em = Singleton<EconomyManager>.instance;
            SimulationManager sm = Singleton<SimulationManager>.instance;

            int budget = em.GetBudget(ItemClass.Service.Water, ItemClass.SubService.None, sm.m_isNightTime);

            float buffer = getBufferCoefficient(AutobudgetBuffer);
            int newWaterBudget = waterStorageRatio > TargetWaterStorageRatio ? 50 : calculateNewBudget(waterCapacity, waterConsumption, budget, buffer);
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

            // Heating autobudget
            if (UseHeatingAutobudget && newBudget < HeatingBudgetMaxValue)
            {
                if (heatingCounter-- <= 0)
                {
                    heatingCounter = heatingRefreshCount;

                    if (currentHeatingBudget < newBudget)
                    {
                        currentHeatingBudget = newBudget;
                    }

                    int heatingProblemCount = 0;
                    int allBldCount = 0;
                    BuildingManager bm = Singleton<BuildingManager>.instance;
                    for (int n = 0; n <= (255 + 1) * 192 - 1; n++)
                    {
                        Building.Flags flags = bm.m_buildings.m_buffer[n].m_flags;
                        if ((flags & Building.Flags.Created) != Building.Flags.None)
                        {
                            if (bm.m_buildings.m_buffer[n].m_heatingProblemTimer > 0)
                            {
                                heatingProblemCount++;
                            }
                            allBldCount++;
                        }
                    }

                    if (allBldCount > 0)
                    {
                        if (heatingProblemCount > 0)
                        {
                            currentHeatingBudget += 1 + heatingProblemCount * 20 / allBldCount;
                        }
                        else
                        {
                            currentHeatingBudget -= 1;
                        }
                    }
                }

                if (currentHeatingBudget > HeatingBudgetMaxValue)
                {
                    currentHeatingBudget = HeatingBudgetMaxValue;
                }

                if (currentHeatingBudget > newBudget)
                {
                    newBudget = currentHeatingBudget;
                }
            }

            setBudget(newBudget);
        }
    }
}
