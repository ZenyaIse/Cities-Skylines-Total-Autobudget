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

        private bool freezeUI = false;

        private UICheckBox UI_Electricity_Enabled;
        private UISlider UI_Electricity_Buffer;
        private UISlider UI_Electricity_MaxBudget;
        private UICheckBox UI_Electricity_AutoPause;

        private UICheckBox UI_Water_Enabled;
        private UISlider UI_Water_Buffer;
        private UISlider UI_Water_MaxBudget;
        private UICheckBox UI_Water_AutoPause;

        private UICheckBox UI_Garbage_Enabled;
        private UISlider UI_Garbage_MaxBudget;

        private UICheckBox UI_Healthcare_Enabled;
        private UISlider UI_Healthcare_MinBudget;
        private UISlider UI_Healthcare_MaxBudget;

        private UICheckBox UI_Education_Enabled;
        private UISlider UI_Education_ElementaryRate;
        private UISlider UI_Education_HighRate;
        private UISlider UI_Education_UnivRate;
        private UISlider UI_Education_MaxBudget;

        private UICheckBox UI_Police_Enabled;
        private UISlider UI_Police_MinBudget;
        private UISlider UI_Police_MaxBudget;

        private UICheckBox UI_Fire_Enabled;
        private UISlider UI_Fire_MinBudget;
        private UISlider UI_Fire_MaxBudget;
        private UISlider UI_Fire_TracksExcessNum;

        public string Name
        {
            get { return ModNameEng; }
        }

        public string Description
        {
            get { return "Autobudget for almost everything with extended options (ver. 2018/8/23)"; }
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

            UI_Garbage_Enabled.isChecked = c.AutobudgetGarbage.Enabled;
            UI_Garbage_MaxBudget.value = c.AutobudgetGarbage.BudgetMaxValue;

            UI_Healthcare_Enabled.isChecked = c.AutobudgetHealthcare.Enabled;
            UI_Healthcare_MinBudget.value = c.AutobudgetHealthcare.BudgetMinValue;
            UI_Healthcare_MaxBudget.value = c.AutobudgetHealthcare.BudgetMaxValue;

            UI_Education_Enabled.isChecked = c.AutobudgetEducation.Enabled;
            UI_Education_ElementaryRate.value = c.AutobudgetEducation.ElementaryEducationTargetRate;
            UI_Education_HighRate.value = c.AutobudgetEducation.HighEducationTargetRate;
            UI_Education_UnivRate.value = c.AutobudgetEducation.UnivEducationTargetRate;
            UI_Education_MaxBudget.value = c.AutobudgetEducation.BudgetMaxValue;

            UI_Police_Enabled.isChecked = c.AutobudgetPolice.Enabled;
            UI_Police_MinBudget.value = c.AutobudgetPolice.BudgetMinValue;
            UI_Police_MaxBudget.value = c.AutobudgetPolice.BudgetMaxValue;

            UI_Fire_Enabled.isChecked = c.AutobudgetFire.Enabled;
            UI_Fire_MinBudget.value = c.AutobudgetFire.BudgetMinValue;
            UI_Fire_MaxBudget.value = c.AutobudgetFire.BudgetMaxValue;
            UI_Fire_TracksExcessNum.value = c.AutobudgetFire.FireTracksExcessNum;

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
            AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;

            #region Electricity
            UIHelperBase electricityGroup = helper.AddGroup("Electricity");
            UI_Electricity_Enabled = (UICheckBox)electricityGroup.AddCheckbox("Enable electricity usage autobudget", c.AutobudgetElectricity.Enabled, delegate (bool isChecked)
            {
                if (!freezeUI) c.AutobudgetElectricity.Enabled = isChecked;
            });
            addLabelToSlider(UI_Electricity_Buffer = (UISlider)electricityGroup.AddSlider("Buffer", 0, 5, 1, c.AutobudgetElectricity.AutobudgetBuffer, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetElectricity.AutobudgetBuffer = (int)val;
            }), "%");
            addLabelToSlider(UI_Electricity_MaxBudget = (UISlider)electricityGroup.AddSlider("Maximum budget", 50, 150, 1, c.AutobudgetElectricity.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetElectricity.BudgetMaxValue = (int)val;
            }), "%");
            UI_Electricity_AutoPause = (UICheckBox)electricityGroup.AddCheckbox("Autopause when the budget is too high", c.AutobudgetElectricity.PauseWhenBudgetTooHigh, delegate (bool isChecked)
            {
                if (!freezeUI) c.AutobudgetElectricity.PauseWhenBudgetTooHigh = isChecked;
            });

            helper.AddSpace(20);
            #endregion

            #region Water and sewage
            UIHelperBase waterGroup = helper.AddGroup("Water and sewage");
            UI_Water_Enabled = (UICheckBox)waterGroup.AddCheckbox("Enable water and sewage autobudget", c.AutobudgetWater.Enabled, delegate (bool isChecked)
            {
                if (!freezeUI) c.AutobudgetWater.Enabled = isChecked;
            });
            addLabelToSlider(UI_Water_Buffer = (UISlider)waterGroup.AddSlider("Buffer", 0, 5, 1, c.AutobudgetWater.AutobudgetBuffer, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetWater.AutobudgetBuffer = (int)val;
            }), "%");
            addLabelToSlider(UI_Water_MaxBudget = (UISlider)waterGroup.AddSlider("Maximum budget", 50, 150, 1, c.AutobudgetWater.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetWater.BudgetMaxValue = (int)val;
            }), "%");
            UI_Water_AutoPause = (UICheckBox)waterGroup.AddCheckbox("Autopause when the budget is too high", c.AutobudgetWater.PauseWhenBudgetTooHigh, delegate (bool isChecked)
            {
                if (!freezeUI) c.AutobudgetWater.PauseWhenBudgetTooHigh = isChecked;
            });

            helper.AddSpace(20);
            #endregion

            #region Garbage
            UIHelperBase garbageGroup = helper.AddGroup("Garbage");
            UI_Garbage_Enabled = (UICheckBox)garbageGroup.AddCheckbox("Enable garbage autobudget", c.AutobudgetGarbage.Enabled, delegate (bool isChecked)
            {
                if (!freezeUI) c.AutobudgetGarbage.Enabled = isChecked;
            });
            addLabelToSlider(UI_Garbage_MaxBudget = (UISlider)garbageGroup.AddSlider("Maximum budget", 50, 150, 1, c.AutobudgetGarbage.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetGarbage.BudgetMaxValue = (int)val;
            }), "%");

            helper.AddSpace(20);
            #endregion

            #region Healthcare
            UIHelperBase healthcareGroup = helper.AddGroup("Healthcare and Deathcare");
            UI_Healthcare_Enabled = (UICheckBox)healthcareGroup.AddCheckbox("Enable healthcare autobudget", c.AutobudgetHealthcare.Enabled, delegate (bool isChecked)
            {
                if (!freezeUI) c.AutobudgetHealthcare.Enabled = isChecked;
            });
            addLabelToSlider(UI_Healthcare_MinBudget = (UISlider)healthcareGroup.AddSlider("Minimum budget", 50, 150, 1, c.AutobudgetHealthcare.BudgetMinValue, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetHealthcare.BudgetMinValue = (int)val;
            }), "%");
            addLabelToSlider(UI_Healthcare_MaxBudget = (UISlider)healthcareGroup.AddSlider("Maximum budget", 50, 150, 1, c.AutobudgetHealthcare.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetHealthcare.BudgetMaxValue = (int)val;
            }), "%");

            helper.AddSpace(20);
            #endregion

            #region Education

            UIHelperBase educationGroup = helper.AddGroup("Education");
            UI_Education_Enabled = (UICheckBox)educationGroup.AddCheckbox("Enable education autobudget", c.AutobudgetEducation.Enabled, delegate (bool isChecked)
            {
                if (!freezeUI) c.AutobudgetEducation.Enabled = isChecked;
            });

            UI_Education_ElementaryRate = (UISlider)educationGroup.AddSlider("Elementary education", 10, 100, 5, c.AutobudgetEducation.ElementaryEducationTargetRate, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetEducation.ElementaryEducationTargetRate = (int)val;
            });
            addLabelToSlider(UI_Education_ElementaryRate, "%");

            UI_Education_HighRate = (UISlider)educationGroup.AddSlider("High school education", 10, 100, 5, c.AutobudgetEducation.HighEducationTargetRate, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetEducation.HighEducationTargetRate = (int)val;
            });
            addLabelToSlider(UI_Education_HighRate, "%");

            UI_Education_UnivRate = (UISlider)educationGroup.AddSlider("University education", 10, 100, 5, c.AutobudgetEducation.UnivEducationTargetRate, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetEducation.UnivEducationTargetRate = (int)val;
            });
            addLabelToSlider(UI_Education_UnivRate, "%");

            UI_Education_MaxBudget = (UISlider)educationGroup.AddSlider("Maximum budget", 50, 150, 1, c.AutobudgetEducation.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetEducation.BudgetMaxValue = (int)val;
            });
            addLabelToSlider(UI_Education_MaxBudget, "%");

            helper.AddSpace(20);

            #endregion

            #region Police
            UIHelperBase policeGroup = helper.AddGroup("Police");
            UI_Police_Enabled = (UICheckBox)policeGroup.AddCheckbox("Enable police autobudget", c.AutobudgetPolice.Enabled, delegate (bool isChecked)
            {
                if (!freezeUI) c.AutobudgetPolice.Enabled = isChecked;
            });
            addLabelToSlider(UI_Police_MinBudget = (UISlider)policeGroup.AddSlider("Minimum budget", 50, 150, 1, c.AutobudgetPolice.BudgetMinValue, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetPolice.BudgetMinValue = (int)val;
            }), "%");
            addLabelToSlider(UI_Police_MaxBudget = (UISlider)policeGroup.AddSlider("Maximum budget", 50, 150, 1, c.AutobudgetPolice.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetPolice.BudgetMaxValue = (int)val;
            }), "%");

            helper.AddSpace(20);
            #endregion

            #region Fire
            UIHelperBase fireGroup = helper.AddGroup("Fire service");
            UI_Fire_Enabled = (UICheckBox)fireGroup.AddCheckbox("Enable fire service autobudget", c.AutobudgetFire.Enabled, delegate (bool isChecked)
            {
                if (!freezeUI) c.AutobudgetFire.Enabled = isChecked;
            });
            addLabelToSlider(UI_Fire_MinBudget = (UISlider)fireGroup.AddSlider("Minimum budget", 50, 150, 1, c.AutobudgetFire.BudgetMinValue, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetFire.BudgetMinValue = (int)val;
            }), "%");
            addLabelToSlider(UI_Fire_MaxBudget = (UISlider)fireGroup.AddSlider("Maximum budget", 50, 150, 1, c.AutobudgetFire.BudgetMaxValue, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetFire.BudgetMaxValue = (int)val;
            }), "%");
            addLabelToSlider(UI_Fire_TracksExcessNum = (UISlider)fireGroup.AddSlider("Minimum fire tracks waiting", 1, 5, 1, c.AutobudgetFire.FireTracksExcessNum, delegate (float val)
            {
                if (!freezeUI) c.AutobudgetFire.FireTracksExcessNum = (int)val;
            }), " tracks");

            helper.AddSpace(20);
            #endregion

            #region Taxi
            //UIHelperBase taxiGroup = helper.AddGroup("Taxi");
            //taxiGroup.AddCheckbox("Enable taxi autobudget", c.EnableTaxiAutobudget, delegate (bool isChecked)
            //{
            //    c.EnableTaxiAutobudget = isChecked;
            //});
            #endregion

            // Save button
            helper.AddButton("Save", delegate ()
            {
                c.Save();
            });
        }

        #endregion
    }
}
