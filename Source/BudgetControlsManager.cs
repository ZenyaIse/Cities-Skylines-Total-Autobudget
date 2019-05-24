using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using ICities;

namespace Autobudget
{
    public static class BudgetControlsManager
    {
        //System.Text.StringBuilder sb = new System.Text.StringBuilder("AutoBudget:\n");
        //foreach (UIComponent cmp in budgetPanel.Find("SubServicesBudgetContainer").components)
        //{
        //    sb.AppendLine(cmp.ToString());
        //}
        //Debug.Log(sb.ToString());

        //Roads(ColossalFramework.UI.UIPanel)
        //Bus(ColossalFramework.UI.UIPanel)
        //Tram(ColossalFramework.UI.UIPanel)
        //Metro(ColossalFramework.UI.UIPanel)
        //Train(ColossalFramework.UI.UIPanel)
        //Ship(ColossalFramework.UI.UIPanel)
        //Plane(ColossalFramework.UI.UIPanel)
        //Monorail(ColossalFramework.UI.UIPanel)
        //CableCar(ColossalFramework.UI.UIPanel)
        //Post(ColossalFramework.UI.UIPanel)
        //Taxi(ColossalFramework.UI.UIPanel)
        //Tours(ColossalFramework.UI.UIPanel)

        private class AutobudgetItemPanel : UIPanel
        {
            private UICheckBox checkBox;
            private UIButton btn;
            private string budgetItemName;

            public override void Awake()
            {
                base.Awake();

                UpdatePosition();
                this.size = new Vector2(30f, 40f);

                // Enable/disable checkbox
                checkBox = this.AddUIComponent<UICheckBox>();
                checkBox.position = new Vector3(6, 0);
                checkBox.size = new Vector2(30, 20);

                UISprite sprite = checkBox.AddUIComponent<UISprite>();
                sprite.spriteName = "ToggleBase";
                sprite.size = new Vector2(16f, 16f);
                sprite.relativePosition = new Vector3(2f, 2f);

                checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
                ((UISprite)checkBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
                checkBox.checkedBoxObject.size = new Vector2(16f, 16f);
                checkBox.checkedBoxObject.relativePosition = Vector3.zero;

                // Options button
                btn = this.AddUIComponent<UIButton>();
                btn.position = new Vector3(0, -20);
                btn.size = new Vector2(30, 16);
                btn.text = "...";
                btn.textColor = Color.white;
                btn.normalBgSprite = "ButtonMenu";
                btn.hoveredBgSprite = "ButtonMenuHovered";
                btn.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
                {
                    if (!string.IsNullOrEmpty(budgetItemName))
                    {
                        BudgetControlsManager.showOptionsPanel(budgetItemName);
                    }
                };
            }

            public void UpdatePosition()
            {
                this.position = new Vector3(45, -4);
            }

            public void SetName(string itemName)
            {
                budgetItemName = itemName;

                this.name = GetControlNameFromItemName(itemName);

                if (checkBox != null)
                {
                    checkBox.name = "autobudgetItemCheckBox" + itemName;
                }

                if (btn != null)
                {
                    btn.name = "autobudgetItemBtn" + itemName;
                }
            }

            public void SetCheckCallback(OnCheckChanged eventCallback)
            {
                if (checkBox != null)
                {
                    checkBox.eventCheckChanged += delegate (UIComponent component, bool value)
                    {
                        eventCallback(checkBox.isChecked);
                    };
                }
            }

            public bool isChecked
            {
                get
                {
                    if (checkBox != null)
                    {
                        return checkBox.isChecked;
                    }

                    return true;
                }

                set
                {
                    if (checkBox != null)
                    {
                        checkBox.isChecked = value;
                    }
                }
            }

            public static string GetControlNameFromItemName(string itemName)
            {
                return "autobudgetItemPanel" + itemName;
            }
        }

        private static bool freezeUI = false;
        private static bool isInitialized = false;


        #region Build controls

        public static void Init()
        {
            if (isInitialized) return;

            UITabContainer economyContainer = ToolsModifierControl.economyPanel.component.Find<UITabContainer>("EconomyContainer");
            if (economyContainer != null)
            {
                UIPanel budgetPanel = economyContainer.Find<UIPanel>("Budget");

                if (budgetPanel != null)
                {
                    budgetPanel.eventVisibilityChanged += BudgetPanel_eventVisibilityChanged;
                    isInitialized = true;
                }
            }
        }

        public static void ResetUI()
        {
            if (!isInitialized) return;

            UITabContainer economyContainer = ToolsModifierControl.economyPanel.component.Find<UITabContainer>("EconomyContainer");
            if (economyContainer != null)
            {
                UIPanel budgetPanel = economyContainer.Find<UIPanel>("Budget");

                if (budgetPanel != null)
                {
                    budgetPanel.eventVisibilityChanged -= BudgetPanel_eventVisibilityChanged;
                    isInitialized = false;
                }
            }

            removeControls();
        }

        private static void BudgetPanel_eventVisibilityChanged(UIComponent component, bool value)
        {
            if (value)
            {
                CheckControls();
                UpdateControls();
                UpdateSliders();
            }
        }

        private static void CheckControls()
        {
            if (isAutobudgetItemControlsCreated())
            {
                if (Singleton<AutobudgetManager>.instance.container.IsCreateControlsOnBudgetPanel)
                {
                    updateControlsPositions();
                }
                else
                {
                    removeControls();
                }
            }
            else
            {
                if (Singleton<AutobudgetManager>.instance.container.IsCreateControlsOnBudgetPanel)
                {
                    createControls();
                }
            }
        }

        private static void createControls()
        {
            UITabContainer economyContainer = ToolsModifierControl.economyPanel.component.Find<UITabContainer>("EconomyContainer");
            if (economyContainer != null)
            {
                UIPanel budgetPanel = economyContainer.Find<UIPanel>("Budget");
                if (budgetPanel != null)
                {
                    AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;

                    foreach (AutobudgetBase obj in c.AllAutobudgetObjects)
                    {
                        UIPanel container = budgetPanel.Find<UIPanel>(obj.GetEconomyPanelContainerName());
                        if (container != null)
                        {
                            UIPanel budgetItem = container.Find<UIPanel>(obj.GetBudgetItemName());
                            if (budgetItem != null)
                            {
                                AutobudgetItemPanel autobudgetPanel = budgetItem.AddUIComponent<AutobudgetItemPanel>();
                                autobudgetPanel.SetName(obj.GetBudgetItemName());
                                autobudgetPanel.isChecked = obj.Enabled;
                                autobudgetPanel.SetCheckCallback(delegate(bool isChecked)
                                {
                                    if (!freezeUI)
                                    {
                                        obj.Enabled = isChecked;
                                        Mod.UpdateUI();
                                    }
                                });
                            }
                        }
                    }
                }
            }
        }

        private static void updateControlsPositions()
        {
            UIPanel budgetPanel = getBudgetPanel();
            if (budgetPanel != null)
            {
                AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;

                foreach (AutobudgetBase obj in c.AllAutobudgetObjects)
                {
                    AutobudgetItemPanel autobudgetItem = budgetPanel.Find<AutobudgetItemPanel>(AutobudgetItemPanel.GetControlNameFromItemName(obj.GetBudgetItemName()));
                    if (autobudgetItem != null)
                    {
                        autobudgetItem.UpdatePosition();
                    }
                }
            }
        }

        private static void removeControls()
        {
            UITabContainer economyContainer = ToolsModifierControl.economyPanel.component.Find<UITabContainer>("EconomyContainer");
            if (economyContainer != null)
            {
                UIPanel budgetPanel = economyContainer.Find<UIPanel>("Budget");
                if (budgetPanel != null)
                {
                    AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;

                    foreach (AutobudgetBase obj in c.AllAutobudgetObjects)
                    {
                        UIPanel container = budgetPanel.Find<UIPanel>(obj.GetEconomyPanelContainerName());
                        if (container != null)
                        {
                            UIPanel budgetItem = container.Find<UIPanel>(obj.GetBudgetItemName());
                            if (budgetItem != null)
                            {
                                string controlName = AutobudgetItemPanel.GetControlNameFromItemName(obj.GetBudgetItemName());
                                AutobudgetItemPanel autobudgetItemControl = budgetItem.Find<AutobudgetItemPanel>(controlName);
                                if (autobudgetItemControl != null)
                                {
                                    budgetItem.RemoveUIComponent(autobudgetItemControl);
                                    Component.Destroy(autobudgetItemControl);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool isAutobudgetItemControlsCreated()
        {
            UITabContainer economyContainer = ToolsModifierControl.economyPanel.component.Find<UITabContainer>("EconomyContainer");
            if (economyContainer != null)
            {
                UIPanel budgetPanel = economyContainer.Find<UIPanel>("Budget");
                if (budgetPanel != null)
                {
                    AutobudgetBase obj = Singleton<AutobudgetManager>.instance.container.AllAutobudgetObjects[0];
                    string controlName = AutobudgetItemPanel.GetControlNameFromItemName(obj.GetBudgetItemName());
                    if (budgetPanel.Find<UIPanel>(controlName) != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void showOptionsPanel(string budgetName)
        {
            UIView.library.ShowModal("OptionsPanel");
            OptionsMainPanel optionsMainPanel = UIView.library.Get<OptionsMainPanel>("OptionsPanel");
            optionsMainPanel.SelectMod(Mod.ModNameEng);
            //Mod.ScrollTo(budgetName);
        }

        #endregion


        #region UI update

        public static void UpdateControls()
        {
            UIPanel budgetPanel = getBudgetPanel();
            if (budgetPanel != null)
            {
                AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;

                freezeUI = true;
                foreach (AutobudgetBase obj in c.AllAutobudgetObjects)
                {
                    updateCheckBox(budgetPanel, obj.GetBudgetItemName(), obj.Enabled);
                }
                freezeUI = false;
            }
        }

        private static void updateCheckBox(UIPanel budgetPanel, string controlName, bool isChecked)
        {
            AutobudgetItemPanel autobudgetItem = budgetPanel.Find<AutobudgetItemPanel>(AutobudgetItemPanel.GetControlNameFromItemName(controlName));
            if (autobudgetItem != null)
            {
                autobudgetItem.isChecked = isChecked;
            }
        }

        public static void UpdateSliders()
        {
            UIPanel budgetPanel = getBudgetPanel();
            if (budgetPanel != null)
            {
                SimulationManager sm = Singleton<SimulationManager>.instance;
                AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;

                freezeUI = true;
                foreach (AutobudgetBase obj in c.AllAutobudgetObjects)
                {
                    UIComponent budgetItem = budgetPanel.Find(obj.GetBudgetItemName());
                    if (budgetItem != null)
                    {
                        UISlider slider = budgetItem.Find<UISlider>(sm.m_isNightTime ? "NightSlider" : "DaySlider");
                        int updatedValue = Singleton<EconomyManager>.instance.GetBudget(obj.GetService(), obj.GetSubService(), sm.m_isNightTime);
                        if (slider.value != updatedValue)
                        {
                            slider.value = updatedValue;
                        }
                    }
                }
                freezeUI = false;
            }
        }

        #endregion


        #region Helpers

        public static UISlider GetBudgetSlider(string EconomyPanelContainerName, string budgetItemName, bool isNight)
        {
            UIPanel budgetPanel = getBudgetPanel();
            if (budgetPanel != null)
            {
                UIComponent container = budgetPanel.Find(EconomyPanelContainerName); // ServicesBudgetContainer, SubServicesBudgetContainer
                if (container != null)
                {
                    UIComponent budgetItem = container.Find(budgetItemName);
                    if (budgetItem != null)
                    {
                        // List of controls:
                        // SliderBackground (ColossalFramework.UI.UISlicedSprite)
                        // Icon (ColossalFramework.UI.UISprite)
                        // DaySlider (ColossalFramework.UI.UISlider)
                        // NightPercentage (ColossalFramework.UI.UILabel)
                        // DayPercentage (ColossalFramework.UI.UILabel)
                        // Total (ColossalFramework.UI.UILabel)
                        // NightSlider (ColossalFramework.UI.UISlider)
                        return budgetItem.Find<UISlider>(isNight ? "NightSlider" : "DaySlider");
                    }
                }
            }

            return null;
        }

        public static bool IsBudgetPanelVisible()
        {
            UIPanel budgetPanel = getBudgetPanel();
            if (budgetPanel != null)
            {
                return budgetPanel.isVisible;
            }

            return false;
        }

        private static UIPanel getBudgetPanel()
        {
            if (ToolsModifierControl.economyPanel != null)
            {
                UITabContainer economyContainer = ToolsModifierControl.economyPanel.component.Find<UITabContainer>("EconomyContainer");
                if (economyContainer != null)
                {
                    return economyContainer.Find<UIPanel>("Budget");
                }
            }

            return null;
        }

        #endregion
    }
}
