using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using ICities;

namespace AutoBudget
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

        private static bool freezeUI = false;
        private static bool isInitialized = false;
        private static bool isBudgetControlsCreated = false;


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
                    budgetPanel.eventVisibilityChanged += delegate (UIComponent component, bool value)
                    {
                        if (value)
                        {
                            createBudgetControlsIfNotCreated();
                            UpdateControls();
                            UpdateSliders();
                        }
                    };

                    isInitialized = true;
                }
            }
        }

        private static void createBudgetControlsIfNotCreated()
        {
            if (isBudgetControlsCreated) return;

            if (ToolsModifierControl.economyPanel != null)
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
                            addControlItem(budgetPanel, obj.GetEconomyPanelContainerName(), obj.GetBudgetItemName(), obj.Enabled, delegate (bool isChecked)
                            {
                                if (!freezeUI)
                                {
                                    obj.Enabled = isChecked;
                                    Mod.UpdateUI();
                                }
                            });
                        }

                        isBudgetControlsCreated = true;
                    }
                }
            }
        }

        private static void addControlItem(UIPanel panel, string containerName, string budgetItemName, bool isChecked, OnCheckChanged eventCallback)
        {
            UIPanel container = panel.Find<UIPanel>(containerName);
            if (container != null)
            {
                UIPanel budgetItem = container.Find<UIPanel>(budgetItemName);
                if (budgetItem != null)
                {
                    float x = 50;
                    float y = -4;

                    addCheckBox(budgetItem, budgetItemName, x, y, isChecked, eventCallback);
                    addButton(budgetItem, budgetItemName, x - 10, y - 20, delegate ()
                    {
                        showOptionsPanel(budgetItemName);
                    });
                }
            }
        }

        private static void showOptionsPanel(string budgetName)
        {
            UIView.library.ShowModal("OptionsPanel");
            OptionsMainPanel optionsMainPanel = UIView.library.Get<OptionsMainPanel>("OptionsPanel");
            optionsMainPanel.SelectMod(Mod.ModNameEng);
            //Mod.ScrollTo(budgetName);
        }

        private static void addCheckBox(UIPanel panel, string controlName, float x, float y, bool isChecked, OnCheckChanged eventCallback)
        {
            UICheckBox checkBox = panel.AddUIComponent<UICheckBox>();
            checkBox.name = "checkBox" + controlName;
            checkBox.position = new Vector3(x, y);
            checkBox.size = new Vector2(30, 20);

            UISprite sprite = checkBox.AddUIComponent<UISprite>();
            sprite.spriteName = "ToggleBase";
            sprite.size = new Vector2(16f, 16f);
            sprite.relativePosition = new Vector3(2f, 2f);

            checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
            checkBox.checkedBoxObject.size = new Vector2(16f, 16f);
            checkBox.checkedBoxObject.relativePosition = Vector3.zero;

            checkBox.eventCheckChanged += delegate (UIComponent component, bool value)
            {
                eventCallback(checkBox.isChecked);
            };
        }

        private static void addButton(UIPanel panel, string controlName, float x, float y, OnButtonClicked eventCallback)
        {
            UIButton btn = panel.AddUIComponent<UIButton>();
            btn.name = "btn" + controlName;
            btn.position = new Vector3(x, y);
            btn.size = new Vector2(30, 16);
            btn.text = "...";
            btn.textColor = Color.white;
            btn.normalBgSprite = "ButtonMenu";
            btn.hoveredBgSprite = "ButtonMenuHovered";
            btn.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                eventCallback();
            };
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
            UICheckBox checkBox = budgetPanel.Find<UICheckBox>("checkBox" + controlName);
            if (checkBox != null)
            {
                checkBox.isChecked = isChecked;
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
