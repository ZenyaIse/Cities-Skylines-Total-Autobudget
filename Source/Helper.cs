using System;
using ColossalFramework;
using UnityEngine;

namespace Autobudget
{
    public static class Helper
    {
        public static void SetPause()
        {
            if (Singleton<SimulationManager>.exists)
            {
                Singleton<SimulationManager>.instance.SimulationPaused = true;
            }
        }

        public static int CountVehiclesInUse(ref Building bld)
        {
            if (bld.Info.m_buildingAI is HospitalAI)
            {
                return CountVehiclesInUse(ref bld, TransferManager.TransferReason.Sick);
            }
            if (bld.Info.m_buildingAI is CemeteryAI)
            {
                return CountVehiclesInUse(ref bld, TransferManager.TransferReason.Dead);
            }
            if (bld.Info.m_buildingAI is LandfillSiteAI)
            {
                return CountVehiclesInUse(ref bld, TransferManager.TransferReason.Garbage);
            }
            if (bld.Info.m_buildingAI is FireStationAI)
            {
                return CountVehiclesInUse(ref bld, TransferManager.TransferReason.Fire);
            }
            if (bld.Info.m_buildingAI is PoliceStationAI)
            {
                return CountVehiclesInUse(ref bld, TransferManager.TransferReason.Crime);
            }
            if (bld.Info.m_buildingAI is SnowDumpAI)
            {
                return CountVehiclesInUse(ref bld, TransferManager.TransferReason.Snow);
            }
            if (bld.Info.m_buildingAI is MaintenanceDepotAI)
            {
                return CountVehiclesInUse(ref bld, TransferManager.TransferReason.RoadMaintenance);
            }
            if (bld.Info.m_buildingAI is DepotAI)
            {
                return CountVehiclesInUse(ref bld, TransferManager.TransferReason.Taxi);
            }
            if (bld.Info.m_buildingAI is DisasterResponseBuildingAI)
            {
                return CountVehiclesInUse(ref bld, TransferManager.TransferReason.Collapsed);
            }
            if (bld.Info.m_buildingAI is PostOfficeAI)
            {
                return CountVehiclesInUse(ref bld, TransferManager.TransferReason.Mail);
            }
            if (bld.Info.m_buildingAI is WarehouseAI)
            {
                //int t = countVehiclesInUse(ref bld, (bld.Info.m_buildingAI as WarehouseAI).m_storageType);
                int t = CountVehiclesInUse(ref bld, TransferManager.TransferReason.None);
                //Debug.Log("m_storageType: " + (bld.Info.m_buildingAI as WarehouseAI).m_storageType.ToString() + ", countVehiclesInUse: " + t.ToString());
                return t;
            }

            return 0;
        }

        public static int CountVehiclesInUse(ref Building bld, TransferManager.TransferReason reason)
        {
            int vehiclesCount = 0;

            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort n = bld.m_ownVehicles;
            int counter = 0;
            while (n != 0)
            {
                //if (reason == TransferManager.TransferReason.None)
                //    Debug.Log("m_transferType: " + ((TransferManager.TransferReason)instance.m_vehicles.m_buffer[(int)n].m_transferType).ToString());

                // If reason is None, count all vehicles
                if (reason == TransferManager.TransferReason.None || (TransferManager.TransferReason)instance.m_vehicles.m_buffer[(int)n].m_transferType == reason)
                {
                    vehiclesCount++;
                }
                n = instance.m_vehicles.m_buffer[(int)n].m_nextOwnVehicle;
                if (++counter > 16384)
                {
                    //CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }

            return vehiclesCount;
        }

        public static int GetNormalVehicleCapacity(ref Building bld)
        {
            if (bld.Info.m_buildingAI is HospitalAI)
            {
                return (bld.Info.m_buildingAI as HospitalAI).m_ambulanceCount;
            }
            if (bld.Info.m_buildingAI is CemeteryAI)
            {
                return (bld.Info.m_buildingAI as CemeteryAI).m_hearseCount;
            }
            if (bld.Info.m_buildingAI is LandfillSiteAI)
            {
                return (bld.Info.m_buildingAI as LandfillSiteAI).m_garbageTruckCount;
            }
            if (bld.Info.m_buildingAI is FireStationAI)
            {
                return (bld.Info.m_buildingAI as FireStationAI).m_fireTruckCount;
            }
            if (bld.Info.m_buildingAI is PoliceStationAI)
            {
                return (bld.Info.m_buildingAI as PoliceStationAI).m_policeCarCount;
            }
            if (bld.Info.m_buildingAI is SnowDumpAI)
            {
                return (bld.Info.m_buildingAI as SnowDumpAI).m_snowTruckCount;
            }
            if (bld.Info.m_buildingAI is MaintenanceDepotAI)
            {
                return (bld.Info.m_buildingAI as MaintenanceDepotAI).m_maintenanceTruckCount;
            }
            if (bld.Info.m_buildingAI is DepotAI)
            {
                return (bld.Info.m_buildingAI as DepotAI).m_maxVehicleCount;
            }
            if (bld.Info.m_buildingAI is DisasterResponseBuildingAI)
            {
                return (bld.Info.m_buildingAI as DisasterResponseBuildingAI).m_vehicleCount;
            }
            if (bld.Info.m_buildingAI is PostOfficeAI)
            {
                return (bld.Info.m_buildingAI as PostOfficeAI).m_postVanCount;
            }
            if (bld.Info.m_buildingAI is WarehouseAI)
            {
                return (bld.Info.m_buildingAI as WarehouseAI).m_truckCount;
            }

            throw new Exception("getNormalVehicleCapacity from " + bld.Info.name + " is not implemented.");
        }

        public static int GetMinimumBudgetToGetVehicles(int normalVehicleCapacity, int requiredVehiclesCount, int maxBudget)
        {
            // Should not be, but just in case...
            if (normalVehicleCapacity <= 0)
            {
                return 100;
            }

            int productionRate;
            int productionRateMax = PlayerBuildingAI.GetProductionRate(100, maxBudget);
            do
            {
                requiredVehiclesCount--;
                productionRate = requiredVehiclesCount * 100 / normalVehicleCapacity;
                if (productionRate < 25) return 50;
            } while (productionRate >= productionRateMax);

            int budget;
            if (productionRate < 100)
            {
                budget = (int)Mathf.Sqrt(productionRate * 100);
            }
            else if (productionRate > 100)
            {
                budget = (int)(150f - 50f * Mathf.Sqrt(9f - 4f * (1f + productionRate / 100f)));
            }
            else
            {
                budget = 100;
            }
            budget += 1;

            return budget;
        }
    }
}
