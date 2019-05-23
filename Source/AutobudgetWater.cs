using System;
using ColossalFramework;
using ColossalFramework.IO;
using UnityEngine;

namespace Autobudget
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
                }
                else
                {
                    // Revert to the version 0
                    d.Enabled = s.ReadBool();
                    d.AutobudgetBuffer = s.ReadInt32();
                    d.BudgetMaxValue = s.ReadInt32();
                    d.PauseWhenBudgetTooHigh = s.ReadBool();

                    d.TargetWaterStorageRatio = s.ReadInt32();
                }
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
        private int bufferDecreaseDueToWaterStorage_prev = 0;

        public int AutobudgetBuffer = 3; // Percent of capacity
        public int BudgetMaxValue = 140;
        public bool PauseWhenBudgetTooHigh = true;
        public int TargetWaterStorageRatio = 50; // Percent of the water capacity

        public AutobudgetWater()
        {
            refreshCount = 109;
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

        // Return -1 if no need to change
        private int getWaterBudget()
        {
            DistrictManager dm = Singleton<DistrictManager>.instance;

            // Water
            int waterCapacity = dm.m_districts.m_buffer[0].GetWaterCapacity();
            int waterConsumption = dm.m_districts.m_buffer[0].GetWaterConsumption();
            // Sewage
            int sewageCapacity = dm.m_districts.m_buffer[0].GetSewageCapacity();
            int sewageAccumulation = dm.m_districts.m_buffer[0].GetSewageAccumulation();

            // No water and no sewage
            if (waterCapacity <= 0 && sewageCapacity <= 0)
            {
                return -1;
            }


            int waterStorageCapacity = dm.m_districts.m_buffer[0].GetWaterStorageCapacity();
            int waterStorageAmount = dm.m_districts.m_buffer[0].GetWaterStorageAmount();
            int waterStorageRatio = waterStorageCapacity == 0 ? 0 : waterStorageAmount * 100 / waterStorageCapacity;
            int bufferDecreaseDueToWaterStorage = (waterStorageRatio < TargetWaterStorageRatio) ? 0 : ((waterStorageRatio - 40) / 10);

            // Check changes from the previous time
            if (waterCapacity == waterCapacity_prev && waterConsumption == waterConsumption_prev &&
                sewageCapacity == sewageCapacity_prev && sewageAccumulation == sewageAccumulation_prev &&
                bufferDecreaseDueToWaterStorage == bufferDecreaseDueToWaterStorage_prev)
            {
                return -1;
            }

            waterCapacity_prev = waterCapacity;
            waterConsumption_prev = waterConsumption;
            sewageCapacity_prev = sewageCapacity;
            sewageAccumulation_prev = sewageAccumulation;
            bufferDecreaseDueToWaterStorage_prev = bufferDecreaseDueToWaterStorage;


            int budget = Singleton<EconomyManager>.instance.GetBudget(ItemClass.Service.Water, ItemClass.SubService.None, Singleton<SimulationManager>.instance.m_isNightTime);

            int newWaterBudget = calculateNewBudget(waterCapacity, waterConsumption, budget, getBufferCoefficient(AutobudgetBuffer - bufferDecreaseDueToWaterStorage));
            int newSewageBudget = calculateNewBudget(sewageCapacity, sewageAccumulation, budget, getBufferCoefficient(AutobudgetBuffer));
            int newBudget = Math.Max(newWaterBudget, newSewageBudget);

            if (newBudget > BudgetMaxValue)
            {
                newBudget = BudgetMaxValue;
            }

            return newBudget;
        }

        // Not used
        //private void updateHeatingBudget()
        //{
        //    int heatingProblemCount = 0;
        //    int allBldCount = 0;
        //    BuildingManager bm = Singleton<BuildingManager>.instance;
        //    for (int n = 0; n <= (255 + 1) * 192 - 1; n++)
        //    {
        //        Building.Flags flags = bm.m_buildings.m_buffer[n].m_flags;
        //        if ((flags & Building.Flags.Created) != Building.Flags.None)
        //        {
        //            if (bm.m_buildings.m_buffer[n].m_heatingProblemTimer > 0)
        //            {
        //                heatingProblemCount++;
        //            }
        //            allBldCount++;
        //        }
        //    }

        //    if (allBldCount > 0)
        //    {
        //        if (heatingProblemCount > 0)
        //        {
        //            currentHeatingBudget += 1 + heatingProblemCount * 20 / allBldCount;
        //        }
        //        else
        //        {
        //            currentHeatingBudget -= 1;
        //        }
        //    }

        //    if (currentHeatingBudget > HeatingBudgetMaxValue)
        //    {
        //        currentHeatingBudget = HeatingBudgetMaxValue;
        //    }
        //}

        protected override void setAutobudget()
        {
            int newBudget = getWaterBudget();

            if (newBudget == -1) return;

            // Pause if the water budget is too high
            if (newBudget >= BudgetMaxValue && PauseWhenBudgetTooHigh && !isPausedRecently)
            {
                SetPause();
                isPausedRecently = true;
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.Water, InfoManager.SubInfoMode.Default);
            }

            // If the water budget drops, reset the "resently paused" flag
            if (newBudget < BudgetMaxValue)
            {
                isPausedRecently = false;
            }

            setBudget(newBudget);
        }
    }
}
