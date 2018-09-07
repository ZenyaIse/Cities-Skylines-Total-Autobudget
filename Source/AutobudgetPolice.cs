using ColossalFramework;

namespace AutoBudget
{
    public class AutobudgetPolice : AutobudgetVehicles
    {
        public int BudgetMinValue = 50;
        public int BudgetMaxValue = 120;

        protected override int refreshCount
        {
            get
            {
                return oneDayFrames / 2 + 3;
            }
        }

        protected override void setAutobudget()
        {
            setBudgetForVehicles(
                ItemClass.Service.PoliceDepartment,
                typeof(PoliceStationAI),
                1,
                BudgetMinValue,
                BudgetMaxValue);
        }

        //protected void setAutobudget_old()
        //{
        //    if (!Singleton<BuildingManager>.exists) return;

        //    BuildingManager bm = Singleton<BuildingManager>.instance;

        //    FastList<ushort> serviceBuildings = bm.GetServiceBuildings(ItemClass.Service.PoliceDepartment);
        //    if (serviceBuildings == null || serviceBuildings.m_buffer == null) return;

        //    AutobudgetOptionsSerializable o = Singleton<AutobudgetOptionsManager>.instance.values;

        //    int budget = Singleton<EconomyManager>.instance.GetBudget(ItemClass.Service.PoliceDepartment, ItemClass.SubService.None, Singleton<SimulationManager>.instance.m_isNightTime);
        //    int productionRate = PlayerBuildingAI.GetProductionRate(100, budget);

        //    int newBudget = budget;
        //    int normalVehicleCapacity_Min = 9999;
        //    int needToEncreaseCapacity = 0;
        //    int canDecreaseCapacity = 0;
        //    int perfectVehiclesCount = 0;

        //    for (int i = 0; i < serviceBuildings.m_size; i++)
        //    {
        //        ushort n = serviceBuildings.m_buffer[i];
        //        if (n == 0) continue;

        //        Building bld = bm.m_buildings.m_buffer[(int)n];
        //        if ((bld.m_flags & Building.Flags.Active) == 0) continue;

        //        if (bld.Info.m_buildingAI is PoliceStationAI)
        //        {
        //            int normalVehicleCapacity = (bld.Info.m_buildingAI as PoliceStationAI).m_policeCarCount;
        //            int vehicleCapacity = (productionRate * normalVehicleCapacity + 99) / 100;
        //            int vehicleCount = countVehiclesInUse(ref bld, TransferManager.TransferReason.Crime);

        //            if (vehicleCount >= vehicleCapacity)
        //            {
        //                needToEncreaseCapacity++;
        //                normalVehicleCapacity_Min = Math.Min(normalVehicleCapacity_Min, normalVehicleCapacity);
        //            }
        //            else if (vehicleCapacity > vehicleCount + 1)
        //            {
        //                canDecreaseCapacity++;
        //                normalVehicleCapacity_Min = Math.Min(normalVehicleCapacity_Min, normalVehicleCapacity);
        //            }
        //            else
        //            {
        //                // Case of (vehicleCapacity == vehicleCount + 1)
        //                perfectVehiclesCount++;
        //            }
        //        }
        //    }

        //    if (needToEncreaseCapacity > 0)
        //    {
        //        // Add 1 vehicle
        //        newBudget = getBudgetNeededToGetOneMoreVehicle((productionRate * normalVehicleCapacity_Min + 99) / 100, normalVehicleCapacity_Min, budget, 130);
        //    }
        //    else if (canDecreaseCapacity > 0 && perfectVehiclesCount == 0)
        //    {
        //        newBudget = getBudgetNeededToGetOneLessVehicle((productionRate * normalVehicleCapacity_Min + 99) / 100, normalVehicleCapacity_Min, budget, 50);
        //    }

        //    if (newBudget != budget)
        //    {
        //        Singleton<EconomyManager>.instance.SetBudget(
        //            ItemClass.Service.PoliceDepartment,
        //            ItemClass.SubService.None,
        //            newBudget,
        //            Singleton<SimulationManager>.instance.m_isNightTime
        //            );
        //    }
        //}
    }
}
