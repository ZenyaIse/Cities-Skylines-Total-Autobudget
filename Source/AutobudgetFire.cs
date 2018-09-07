using ColossalFramework;
using UnityEngine;

namespace AutoBudget
{
    public class AutobudgetFire : AutobudgetVehicles
    {
        public int FireTracksExcessNum = 2;
        public int BudgetMinValue = 50;
        public int BudgetMaxValue = 120;

        public static string[] StrategyNames = new string[] { "Less tracks", "Normal", "More tracks" };
        public static int TracksExcessNumToStrategyIndex(int tracksNum)
        {
            return Mathf.Clamp(tracksNum - 1, 0, 2);
        }
        public static int StrategyIndexToTracksExcessNum(int index)
        {
            return index + 1;
        }

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 4;
            }
        }

        protected override void setAutobudget()
        {
            setBudgetForVehicles(
                ItemClass.Service.FireDepartment,
                typeof(PoliceStationAI),
                FireTracksExcessNum,
                BudgetMinValue,
                BudgetMaxValue);
        }

        //protected void setAutobudget_old()
        //{
        //    if (!Singleton<BuildingManager>.exists) return;

        //    BuildingManager bm = Singleton<BuildingManager>.instance;

        //    FastList<ushort> serviceBuildings = bm.GetServiceBuildings(ItemClass.Service.FireDepartment);
        //    if (serviceBuildings == null || serviceBuildings.m_buffer == null) return;

        //    AutobudgetOptionsSerializable o = Singleton<AutobudgetOptionsManager>.instance.values;

        //    int budget = Singleton<EconomyManager>.instance.GetBudget(ItemClass.Service.FireDepartment, ItemClass.SubService.None, Singleton<SimulationManager>.instance.m_isNightTime);
        //    int productionRate = PlayerBuildingAI.GetProductionRate(100, budget);

        //    int newBudget = budget;
        //    int normalVehicleCapacity_Min = 9999;
        //    int needToEncreaseCapacityCount = 0;
        //    int canDecreaseCapacityCount = 0;
        //    int perfectVehiclesCount = 0;

        //    for (int i = 0; i < serviceBuildings.m_size; i++)
        //    {
        //        ushort n = serviceBuildings.m_buffer[i];
        //        if (n == 0) continue;

        //        Building bld = bm.m_buildings.m_buffer[(int)n];
        //        if ((bld.m_flags & Building.Flags.Active) == 0) continue;

        //        if (bld.Info.m_buildingAI is FireStationAI)
        //        {
        //            int normalVehicleCapacity = (bld.Info.m_buildingAI as FireStationAI).m_fireTruckCount;
        //            int vehicleCapacity = (productionRate * normalVehicleCapacity + 99) / 100;
        //            int vehicleCount = countVehiclesInUse(ref bld, TransferManager.TransferReason.Fire);
        //            //Debug.Log(string.Format("normalVehicleCapacity: {0}, vehicleCapacity: {1}, vehicleCount: {2}", normalVehicleCapacity, vehicleCapacity, vehicleCount));

        //            if (vehicleCount >= vehicleCapacity)
        //            {
        //                needToEncreaseCapacityCount++;
        //                normalVehicleCapacity_Min = Math.Min(normalVehicleCapacity_Min, normalVehicleCapacity);
        //            }
        //            else if (vehicleCapacity > vehicleCount + o.FireTracksExcessNum)
        //            {
        //                canDecreaseCapacityCount++;
        //                normalVehicleCapacity_Min = Math.Min(normalVehicleCapacity_Min, normalVehicleCapacity);
        //            }
        //            else
        //            {
        //                perfectVehiclesCount++;
        //            }
        //        }
        //    }

        //    if (needToEncreaseCapacityCount > 0)
        //    {
        //        // Add 1 vehicle
        //        newBudget = getBudgetNeededToGetOneMoreVehicle((productionRate * normalVehicleCapacity_Min + 99) / 100, normalVehicleCapacity_Min, budget, o.FireBudgetMaxValue);
        //    }
        //    else if (canDecreaseCapacityCount > 0 && perfectVehiclesCount == 0)
        //    {
        //        newBudget = getBudgetNeededToGetOneLessVehicle((productionRate * normalVehicleCapacity_Min + 99) / 100, normalVehicleCapacity_Min, budget, o.FireBudgetMinValue);
        //    }

        //    if (newBudget != budget)
        //    {
        //        Singleton<EconomyManager>.instance.SetBudget(
        //            ItemClass.Service.FireDepartment,
        //            ItemClass.SubService.None,
        //            newBudget,
        //            Singleton<SimulationManager>.instance.m_isNightTime
        //            );
        //    }
        //}
    }
}
