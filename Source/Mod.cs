using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Plugins;
using UnityEngine;
using System.Reflection;

namespace AutoBudget
{
    public class Mod : IUserMod
    {
        public static string ModNameEng = "Total Autobudget";
        public static string LogMsgPrefix = ">>> " + ModNameEng + ": ";
        public static string Version = "2018/11/18";

        private bool freezeUI = false;

        private UICheckBox UI_Electricity_Enabled;
        private UISlider UI_Electricity_Buffer;
        private UISlider UI_Electricity_MaxBudget;
        private UICheckBox UI_Electricity_AutoPause;

        private UICheckBox UI_Water_Enabled;
        private UISlider UI_Water_Buffer;
        private UISlider UI_Water_MaxBudget;
        private UICheckBox UI_Water_AutoPause;
        private UISlider UI_Water_StorageAmount;
        private UICheckBox UI_Water_UseHeating;
        private UISlider UI_Water_MaxHeatingBudget;

        private UICheckBox UI_Garbage_Enabled;
        private UISlider UI_Garbage_MaxBudget;
        private UISlider UI_Garbage_MaxAmount;

        private UICheckBox UI_Healthcare_Enabled;
        private UISlider UI_Healthcare_MinBudget;
        private UISlider UI_Healthcare_MaxBudget;

        private UICheckBox UI_Education_Enabled;
        private UISlider UI_Education_ElementaryRate;
        private UISlider UI_Education_HighRate;
        private UISlider UI_Education_MaxBudget;

        private UICheckBox UI_Police_Enabled;
        private UISlider UI_Police_MinBudget;
        private UISlider UI_Police_MaxBudget;

        private UICheckBox UI_Fire_Enabled;
        private UISlider UI_Fire_MinBudget;
        private UISlider UI_Fire_MaxBudget;
        private UISlider UI_Fire_TrucksExcessNum;

        private UICheckBox UI_Road_Enabled;
        private UISlider UI_Road_MinBudget;
        private UISlider UI_Road_MaxBudget;

        private UICheckBox UI_Post_Enabled;
        private UISlider UI_Post_MaxBudget;

        private UICheckBox UI_Taxi_Enabled;
        private UISlider UI_Taxi_MaxBudget;
        private UISlider UI_Taxi_DepotVehiclesExcessNum;
        private UISlider UI_Taxi_StandVehiclesExcessNum;

        public string Name
        {
            get { return ModNameEng; }
        }

        public string Description
        {
            get { return "Autobudget for almost everything with extended options (ver. " + Version + ")"; }
        }

        #region Options UI

        public static void UpdateUI()
        {
            foreach (PluginManager.PluginInfo current in Singleton<PluginManager>.instance.GetPluginsInfo())
            {
                if (current.isEnabled)
                {
                    IUserMod[] instances = current.GetInstances<IUserMod>();
                    MethodInfo method = instances[0].GetType().GetMethod("TotalAutobudgetOptionsUpdateUI", BindingFlags.Instance | BindingFlags.Public);
                    if (method != null)
                    {
                        method.Invoke(instances[0], new object[] { } );
                    }
                }
            }
        }

        public void TotalAutobudgetOptionsUpdateUI()
        {
            if (UI_Electricity_Enabled == null) return;

            AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;

            freezeUI = true;

            UI_Electricity_Enabled.isChecked = c.AutobudgetElectricity.Enabled;
            UI_Electricity_Buffer.value = c.AutobudgetElectricity.AutobudgetBuffer;
            UI_Electricity_MaxBudget.value = c.AutobudgetElectricity.BudgetMaxValue;
            UI_Electricity_AutoPause.isChecked = c.AutobudgetElectricity.PauseWhenBudgetTooHigh;

            UI_Water_Enabled.isChecked = c.AutobudgetWater.Enabled;
            UI_Water_Buffer.value = c.AutobudgetWater.AutobudgetBuffer;
            UI_Water_MaxBudget.value = c.AutobudgetWater.BudgetMaxValue;
            UI_Water_AutoPause.isChecked = c.AutobudgetWater.PauseWhenBudgetTooHigh;
            UI_Water_StorageAmount.value = c.AutobudgetWater.TargetWaterStorageRatio;
            UI_Water_UseHeating.isChecked = c.AutobudgetWater.UseHeatingAutobudget;
            UI_Water_MaxHeatingBudget.value = c.AutobudgetWater.HeatingBudgetMaxValue;

            UI_Garbage_Enabled.isChecked = c.AutobudgetGarbage.Enabled;
            UI_Garbage_MaxBudget.value = c.AutobudgetGarbage.BudgetMaxValue;
            UI_Garbage_MaxAmount.value = c.AutobudgetGarbage.MaximumGarbageAmount;

            UI_Healthcare_Enabled.isChecked = c.AutobudgetHealthcare.Enabled;
            UI_Healthcare_MinBudget.value = c.AutobudgetHealthcare.BudgetMinValue;
            UI_Healthcare_MaxBudget.value = c.AutobudgetHealthcare.BudgetMaxValue;

            UI_Education_Enabled.isChecked = c.AutobudgetEducation.Enabled;
            UI_Education_ElementaryRate.value = c.AutobudgetEducation.ElementaryEducationTargetRate;
            UI_Education_HighRate.value = c.AutobudgetEducation.HighEducationTargetRate;
            UI_Education_MaxBudget.value = c.AutobudgetEducation.BudgetMaxValue;

            UI_Police_Enabled.isChecked = c.AutobudgetPolice.Enabled;
            UI_Police_MinBudget.value = c.AutobudgetPolice.BudgetMinValue;
            UI_Police_MaxBudget.value = c.AutobudgetPolice.BudgetMaxValue;

            UI_Fire_Enabled.isChecked = c.AutobudgetFire.Enabled;
            UI_Fire_MinBudget.value = c.AutobudgetFire.BudgetMinValue;
            UI_Fire_MaxBudget.value = c.AutobudgetFire.BudgetMaxValue;
            UI_Fire_TrucksExcessNum.value = c.AutobudgetFire.FireTrucksExcessNum;

            UI_Road_Enabled.isChecked = c.AutobudgetRoad.Enabled;
            UI_Road_MinBudget.value = c.AutobudgetRoad.BudgetMinValue;
            UI_Road_MaxBudget.value = c.AutobudgetRoad.BudgetMaxValue;

            UI_Post_Enabled.isChecked = c.AutobudgetPost.Enabled;
            UI_Post_MaxBudget.value = c.AutobudgetPost.BudgetMaxValue;

            UI_Taxi_Enabled.isChecked = c.AutobudgetTaxi.Enabled;
            UI_Taxi_MaxBudget.value = c.AutobudgetTaxi.BudgetMaxValue;
            UI_Taxi_DepotVehiclesExcessNum.value = c.AutobudgetTaxi.TargetNumberOfVehiclesWaitingAtDepot;
            UI_Taxi_StandVehiclesExcessNum.value = c.AutobudgetTaxi.TargetNumberOfVehiclesWaitingAtStand;

            freezeUI = false;
        }

        private void addLabelToSlider(object obj)
        {
            addLabelToSlider(obj, "");
        }

        private void addLabelToSlider(object obj, string postfix)
        {
            UISlider uISlider = obj as UISlider;
            if (uISlider == null) return;

            UILabel label = uISlider.parent.AddUIComponent<UILabel>();
            label.text = uISlider.value.ToString() + postfix;
            label.textScale = 1f;
            (uISlider.parent as UIPanel).autoLayout = false;
            label.position = new Vector3(uISlider.position.x + uISlider.width + 15, uISlider.position.y);

            UILabel titleLabel = (uISlider.parent as UIPanel).Find<UILabel>("Label");
            titleLabel.anchor = UIAnchorStyle.None;
            titleLabel.position = new Vector3(titleLabel.position.x, titleLabel.position.y + 3);

            uISlider.eventValueChanged += new PropertyChangedEventHandler<float>(delegate (UIComponent component, float value)
            {
                label.text = uISlider.value.ToString() + postfix;
            });
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            AutobudgetManager am = Singleton<AutobudgetManager>.instance;

            #region Electricity

            UIHelperBase electricityGroup = helper.AddGroup("Electricity");

            UI_Electricity_Enabled = (UICheckBox)electricityGroup.AddCheckbox("Enable", am.container.AutobudgetElectricity.Enabled, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.AutobudgetElectricity.Enabled = isChecked;
                BudgetControlsManager.UpdateControls();
            });

            addLabelToSlider(UI_Electricity_Buffer = (UISlider)electricityGroup.AddSlider("Buffer", 0, 5, 1, am.container.AutobudgetElectricity.AutobudgetBuffer, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetElectricity.AutobudgetBuffer = (int)val;
            }), "%");
            UI_Electricity_Buffer.tooltip = "Set how much production should exceed consumption";

            addLabelToSlider(UI_Electricity_MaxBudget = (UISlider)electricityGroup.AddSlider("Maximum budget", 50, 150, 1, am.container.AutobudgetElectricity.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetElectricity.BudgetMaxValue = (int)val;
            }), "%");
            UI_Electricity_MaxBudget.tooltip = "Budget will not be raised higher then this value";

            UI_Electricity_AutoPause = (UICheckBox)electricityGroup.AddCheckbox("Autopause when budget is too high", am.container.AutobudgetElectricity.PauseWhenBudgetTooHigh, delegate (bool isChecked)
            {
                if (!freezeUI) am.container.AutobudgetElectricity.PauseWhenBudgetTooHigh = isChecked;
            });
            UI_Electricity_AutoPause.tooltip = "Pause and switch to the electricity info view mode when the autobudget raises up to the maximum value";

            helper.AddSpace(20);

            #endregion


            #region Water, sewage, and heating

            UIHelperBase waterGroup = helper.AddGroup("Water, sewage, and heating");

            UI_Water_Enabled = (UICheckBox)waterGroup.AddCheckbox("Enable", am.container.AutobudgetWater.Enabled, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.AutobudgetWater.Enabled = isChecked;
                BudgetControlsManager.UpdateControls();
            });

            addLabelToSlider(UI_Water_Buffer = (UISlider)waterGroup.AddSlider("Buffer", 0, 5, 1, am.container.AutobudgetWater.AutobudgetBuffer, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetWater.AutobudgetBuffer = (int)val;
            }), "%");
            UI_Water_Buffer.tooltip = "Set how much production should exceed consumption";

            addLabelToSlider(UI_Water_MaxBudget = (UISlider)waterGroup.AddSlider("Maximum budget", 50, 150, 1, am.container.AutobudgetWater.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetWater.BudgetMaxValue = (int)val;
            }), "%");
            UI_Water_MaxBudget.tooltip = "Budget will not be raised higher then this value";

            UI_Water_AutoPause = (UICheckBox)waterGroup.AddCheckbox("Autopause when budget is too high", am.container.AutobudgetWater.PauseWhenBudgetTooHigh, delegate (bool isChecked)
            {
                if (!freezeUI) am.container.AutobudgetWater.PauseWhenBudgetTooHigh = isChecked;
            });
            UI_Water_AutoPause.tooltip = "Pause and switch to the water and sewage info view mode when the autobudget raises up to the maximum value";

            addLabelToSlider(UI_Water_StorageAmount = (UISlider)waterGroup.AddSlider("Target water storage", 0, 100, 1, am.container.AutobudgetWater.TargetWaterStorageRatio, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetWater.TargetWaterStorageRatio = (int)val;
            }), "%");
            UI_Water_StorageAmount.tooltip = "When water storage tanks are filled more than this value, water consumption will not influence the budget";

            UI_Water_UseHeating = (UICheckBox)waterGroup.AddCheckbox("Increase budget if not enough heating", am.container.AutobudgetWater.UseHeatingAutobudget, delegate (bool isChecked)
            {
                if (!freezeUI) am.container.AutobudgetWater.UseHeatingAutobudget = isChecked;
            });
            UI_Water_UseHeating.tooltip = "The budget increases when some of your buildings have heating problems";

            addLabelToSlider(UI_Water_MaxHeatingBudget = (UISlider)waterGroup.AddSlider("Max heating budget", 50, 150, 1, am.container.AutobudgetWater.HeatingBudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetWater.HeatingBudgetMaxValue = (int)val;
            }), "%");
            UI_Water_MaxHeatingBudget.tooltip = "Budget rising due to heating problems will never exceed this value";

            helper.AddSpace(20);

            #endregion


            #region Garbage disposal

            UIHelperBase garbageGroup = helper.AddGroup("Garbage disposal");

            UI_Garbage_Enabled = (UICheckBox)garbageGroup.AddCheckbox("Enable", am.container.AutobudgetGarbage.Enabled, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.AutobudgetGarbage.Enabled = isChecked;
                BudgetControlsManager.UpdateControls();
            });

            addLabelToSlider(UI_Garbage_MaxBudget = (UISlider)garbageGroup.AddSlider("Maximum budget", 50, 150, 1, am.container.AutobudgetGarbage.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetGarbage.BudgetMaxValue = (int)val;
            }), "%");
            UI_Garbage_MaxBudget.tooltip = "Budget will not be raised higher then this value";

            addLabelToSlider(UI_Garbage_MaxAmount = (UISlider)garbageGroup.AddSlider("Max garbage amount", 0, 100, 1, am.container.AutobudgetGarbage.MaximumGarbageAmount, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetGarbage.MaximumGarbageAmount = (int)val;
            }), "%");
            UI_Garbage_MaxAmount.tooltip = "When at least one of the recycling centers or incineration plants is piled with garbage more than this value, the budget will be raised to the maximum";

            helper.AddSpace(20);

            #endregion


            #region Healthcare

            UIHelperBase healthcareGroup = helper.AddGroup("Healthcare and Deathcare");

            UI_Healthcare_Enabled = (UICheckBox)healthcareGroup.AddCheckbox("Enable", am.container.AutobudgetHealthcare.Enabled, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.AutobudgetHealthcare.Enabled = isChecked;
                BudgetControlsManager.UpdateControls();
            });

            addLabelToSlider(UI_Healthcare_MinBudget = (UISlider)healthcareGroup.AddSlider("Minimum budget", 50, 150, 1, am.container.AutobudgetHealthcare.BudgetMinValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetHealthcare.BudgetMinValue = (int)val;
            }), "%");
            UI_Healthcare_MinBudget.tooltip = "Budget will not be lowered below this value";

            addLabelToSlider(UI_Healthcare_MaxBudget = (UISlider)healthcareGroup.AddSlider("Maximum budget", 50, 150, 1, am.container.AutobudgetHealthcare.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetHealthcare.BudgetMaxValue = (int)val;
            }), "%");
            UI_Healthcare_MaxBudget.tooltip = "Budget will not be raised higher then this value";

            helper.AddSpace(20);
            
            #endregion


            #region Education

            UIHelperBase educationGroup = helper.AddGroup("Education");

            UI_Education_Enabled = (UICheckBox)educationGroup.AddCheckbox("Enable", am.container.AutobudgetEducation.Enabled, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.AutobudgetEducation.Enabled = isChecked;
                BudgetControlsManager.UpdateControls();
            });

            UI_Education_ElementaryRate = (UISlider)educationGroup.AddSlider("Elementary education", 10, 100, 5, am.container.AutobudgetEducation.ElementaryEducationTargetRate, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetEducation.ElementaryEducationTargetRate = (int)val;
            });
            addLabelToSlider(UI_Education_ElementaryRate, "%");
            UI_Education_ElementaryRate.tooltip = "Target elementary education rate";

            UI_Education_HighRate = (UISlider)educationGroup.AddSlider("High school education", 10, 100, 5, am.container.AutobudgetEducation.HighEducationTargetRate, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetEducation.HighEducationTargetRate = (int)val;
            });
            addLabelToSlider(UI_Education_HighRate, "%");
            UI_Education_HighRate.tooltip = "Target high school education rate";

            UI_Education_MaxBudget = (UISlider)educationGroup.AddSlider("Maximum budget", 50, 150, 1, am.container.AutobudgetEducation.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetEducation.BudgetMaxValue = (int)val;
            });
            addLabelToSlider(UI_Education_MaxBudget, "%");
            UI_Education_MaxBudget.tooltip = "Budget will not be raised higher then this value";

            helper.AddSpace(20);

            #endregion


            #region Police

            UIHelperBase policeGroup = helper.AddGroup("Police");

            UI_Police_Enabled = (UICheckBox)policeGroup.AddCheckbox("Enable", am.container.AutobudgetPolice.Enabled, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.AutobudgetPolice.Enabled = isChecked;
                BudgetControlsManager.UpdateControls();
            });

            addLabelToSlider(UI_Police_MinBudget = (UISlider)policeGroup.AddSlider("Minimum budget", 50, 150, 1, am.container.AutobudgetPolice.BudgetMinValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetPolice.BudgetMinValue = (int)val;
            }), "%");
            UI_Police_MinBudget.tooltip = "Budget will not be lowered below this value";

            addLabelToSlider(UI_Police_MaxBudget = (UISlider)policeGroup.AddSlider("Maximum budget", 50, 150, 1, am.container.AutobudgetPolice.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetPolice.BudgetMaxValue = (int)val;
            }), "%");
            UI_Police_MaxBudget.tooltip = "Budget will not be raised higher then this value";

            helper.AddSpace(20);

            #endregion


            #region Fire

            UIHelperBase fireGroup = helper.AddGroup("Fire service");

            UI_Fire_Enabled = (UICheckBox)fireGroup.AddCheckbox("Enable", am.container.AutobudgetFire.Enabled, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.AutobudgetFire.Enabled = isChecked;
                BudgetControlsManager.UpdateControls();
            });

            addLabelToSlider(UI_Fire_MinBudget = (UISlider)fireGroup.AddSlider("Minimum budget", 50, 150, 1, am.container.AutobudgetFire.BudgetMinValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetFire.BudgetMinValue = (int)val;
            }), "%");
            UI_Fire_MinBudget.tooltip = "Budget will not be lowered below this value";

            addLabelToSlider(UI_Fire_MaxBudget = (UISlider)fireGroup.AddSlider("Maximum budget", 50, 150, 1, am.container.AutobudgetFire.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetFire.BudgetMaxValue = (int)val;
            }), "%");
            UI_Fire_MaxBudget.tooltip = "Budget will not be raised higher then this value";

            addLabelToSlider(UI_Fire_TrucksExcessNum = (UISlider)fireGroup.AddSlider("Minimum trucks waiting", 1, 5, 1, am.container.AutobudgetFire.FireTrucksExcessNum, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetFire.FireTrucksExcessNum = (int)val;
            }), " trucks");
            UI_Fire_TrucksExcessNum.tooltip = "Minimum number of trucks waiting in each of the fire stations";

            helper.AddSpace(20);

            #endregion


            #region Roads

            UIHelperBase roadGroup = helper.AddGroup("Road maintenance and snow dumps");

            UI_Road_Enabled = (UICheckBox)roadGroup.AddCheckbox("Enable", am.container.AutobudgetRoad.Enabled, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.AutobudgetRoad.Enabled = isChecked;
                BudgetControlsManager.UpdateControls();
            });

            addLabelToSlider(UI_Road_MinBudget = (UISlider)roadGroup.AddSlider("Minimum budget", 50, 150, 1, am.container.AutobudgetRoad.BudgetMinValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetRoad.BudgetMinValue = (int)val;
            }), "%");
            UI_Road_MinBudget.tooltip = "Budget will not be lowered below this value";

            addLabelToSlider(UI_Road_MaxBudget = (UISlider)roadGroup.AddSlider("Maximum budget", 50, 150, 1, am.container.AutobudgetRoad.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetRoad.BudgetMaxValue = (int)val;
            }), "%");
            UI_Road_MaxBudget.tooltip = "Budget will not be raised higher then this value";

            helper.AddSpace(20);

            #endregion


            #region Post

            UIHelperBase postGroup = helper.AddGroup("Post offices");

            UI_Post_Enabled = (UICheckBox)postGroup.AddCheckbox("Enable", am.container.AutobudgetPost.Enabled, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.AutobudgetPost.Enabled = isChecked;
                BudgetControlsManager.UpdateControls();
            });

            addLabelToSlider(UI_Post_MaxBudget = (UISlider)postGroup.AddSlider("Maximum budget", 50, 150, 1, am.container.AutobudgetPost.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetPost.BudgetMaxValue = (int)val;
            }), "%");
            UI_Post_MaxBudget.tooltip = "Budget will not be raised higher then this value";

            helper.AddSpace(20);

            #endregion


            #region Taxi

            UIHelperBase taxiGroup = helper.AddGroup("Taxi");

            UI_Taxi_Enabled = (UICheckBox)taxiGroup.AddCheckbox("Enable", am.container.AutobudgetTaxi.Enabled, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.AutobudgetTaxi.Enabled = isChecked;
                BudgetControlsManager.UpdateControls();
            });

            addLabelToSlider(UI_Taxi_MaxBudget = (UISlider)taxiGroup.AddSlider("Maximum budget", 50, 150, 1, am.container.AutobudgetTaxi.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetTaxi.BudgetMaxValue = (int)val;
            }), "%");
            UI_Taxi_MaxBudget.tooltip = "Budget will not be raised higher then this value";

            addLabelToSlider(UI_Taxi_DepotVehiclesExcessNum = (UISlider)taxiGroup.AddSlider("Taxis waiting at depots", 1, 5, 1, am.container.AutobudgetTaxi.TargetNumberOfVehiclesWaitingAtDepot, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetTaxi.TargetNumberOfVehiclesWaitingAtDepot = (int)val;
            }), " taxis");
            UI_Taxi_DepotVehiclesExcessNum.tooltip = "Target number of taxis waiting in depots";

            addLabelToSlider(UI_Taxi_StandVehiclesExcessNum = (UISlider)taxiGroup.AddSlider("Taxis waiting at stands", 1, 5, 1, am.container.AutobudgetTaxi.TargetNumberOfVehiclesWaitingAtStand, delegate (float val)
            {
                if (!freezeUI) am.container.AutobudgetTaxi.TargetNumberOfVehiclesWaitingAtStand = (int)val;
            }), " taxis");
            UI_Taxi_StandVehiclesExcessNum.tooltip = "Target number of taxis waiting at taxi stands";

            helper.AddSpace(20);

            #endregion


            helper.AddCheckbox("Create controls on the budget panel", am.container.IsCreateControlsOnBudgetPanel, delegate (bool isChecked)
            {
                if (freezeUI) return;
                am.container.IsCreateControlsOnBudgetPanel = isChecked;
            });
            helper.AddSpace(20);


            // Save buttons
            helper.AddButton("Save as default for new games", delegate ()
            {
                am.container.Save();
            });
            helper.AddButton("Reset to the last saved values", delegate ()
            {
                am.ReadValuesFromFile();
                TotalAutobudgetOptionsUpdateUI();
            });
            helper.AddButton("Reset to the mod default values", delegate ()
            {
                am.ResetToDefaultValues();
                TotalAutobudgetOptionsUpdateUI();
            });
            helper.AddSpace(20);
        }

        #endregion
    }
}
