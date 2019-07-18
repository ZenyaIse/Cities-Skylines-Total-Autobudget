using System;
using ColossalFramework;
using UnityEngine;

namespace Autobudget
{
    class VehiclesHelper
    {
        public static int CountVehiclesInUse(ref Building bld, bool isSecondary = false)
        {
            if (bld.Info.m_buildingAI is HospitalAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.Sick);
            }
            if (bld.Info.m_buildingAI is CemeteryAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.Dead);
            }
            if (bld.Info.m_buildingAI is LandfillSiteAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.Garbage);
            }
            if (bld.Info.m_buildingAI is FireStationAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.Fire);
            }
            if (bld.Info.m_buildingAI is PoliceStationAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.Crime);
            }
            if (bld.Info.m_buildingAI is SnowDumpAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.Snow);
            }
            if (bld.Info.m_buildingAI is MaintenanceDepotAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.RoadMaintenance);
            }
            if (bld.Info.m_buildingAI is DepotAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.Taxi);
            }
            if (bld.Info.m_buildingAI is DisasterResponseBuildingAI)
            {
                return isSecondary ? CountVehiclesInUse(ref bld, TransferManager.TransferReason.Collapsed2) : CountVehiclesInUse(ref bld, TransferManager.TransferReason.Collapsed);
            }
            if (bld.Info.m_buildingAI is PostOfficeAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.Mail);
            }
            if (bld.Info.m_buildingAI is WarehouseAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.None);
            }
            if (bld.Info.m_buildingAI is WaterFacilityAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.None);
            }
            if (bld.Info.m_buildingAI is HelicopterDepotAI)
            {
                return isSecondary ? 0 : CountVehiclesInUse(ref bld, TransferManager.TransferReason.None);
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

        public static int GetNormalVehicleCapacity(ref Building bld, bool isSecondary = false)
        {
            if (bld.Info.m_buildingAI is HospitalAI)
            {
                return isSecondary ? 0 : (bld.Info.m_buildingAI as HospitalAI).m_ambulanceCount;
            }
            if (bld.Info.m_buildingAI is CemeteryAI)
            {
                return isSecondary ? 0 : (bld.Info.m_buildingAI as CemeteryAI).m_hearseCount;
            }
            if (bld.Info.m_buildingAI is LandfillSiteAI)
            {
                return isSecondary ? 0 : (bld.Info.m_buildingAI as LandfillSiteAI).m_garbageTruckCount;
            }
            if (bld.Info.m_buildingAI is FireStationAI)
            {
                return isSecondary ? 0 : (bld.Info.m_buildingAI as FireStationAI).m_fireTruckCount;
            }
            if (bld.Info.m_buildingAI is PoliceStationAI)
            {
                return isSecondary ? 0 : (bld.Info.m_buildingAI as PoliceStationAI).m_policeCarCount;
            }
            if (bld.Info.m_buildingAI is SnowDumpAI)
            {
                return isSecondary ? 0 : (bld.Info.m_buildingAI as SnowDumpAI).m_snowTruckCount;
            }
            if (bld.Info.m_buildingAI is MaintenanceDepotAI)
            {
                return isSecondary ? 0 : (bld.Info.m_buildingAI as MaintenanceDepotAI).m_maintenanceTruckCount;
            }
            if (bld.Info.m_buildingAI is DepotAI)
            {
                return isSecondary ? (bld.Info.m_buildingAI as DepotAI).m_maxVehicleCount2 : (bld.Info.m_buildingAI as DepotAI).m_maxVehicleCount;
            }
            if (bld.Info.m_buildingAI is DisasterResponseBuildingAI)
            {
                return isSecondary ? (bld.Info.m_buildingAI as DisasterResponseBuildingAI).m_helicopterCount : (bld.Info.m_buildingAI as DisasterResponseBuildingAI).m_vehicleCount;
            }
            if (bld.Info.m_buildingAI is PostOfficeAI)
            {
                return isSecondary ? (bld.Info.m_buildingAI as PostOfficeAI).m_postTruckCount : (bld.Info.m_buildingAI as PostOfficeAI).m_postVanCount;
            }
            if (bld.Info.m_buildingAI is WarehouseAI)
            {
                return isSecondary ? 0 : (bld.Info.m_buildingAI as WarehouseAI).m_truckCount;
            }
            if (bld.Info.m_buildingAI is WaterFacilityAI)
            {
                return isSecondary ? 0 : (bld.Info.m_buildingAI as WaterFacilityAI).m_pumpingVehicles;
            }
            if (bld.Info.m_buildingAI is HelicopterDepotAI)
            {
                return isSecondary ? 0 : (bld.Info.m_buildingAI as HelicopterDepotAI).m_helicopterCount;
            }

            throw new Exception("getNormalVehicleCapacity from " + bld.Info.name + " is not implemented.");
        }

        public static int GetMinimumBudgetToGetVehicles(int normalVehicleCapacity, int requiredVehiclesCount, int maxBudget)
        {
            if (normalVehicleCapacity <= 0)
            {
                return 50;
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
