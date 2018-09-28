using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using ICities;

namespace AutoBudget
{
    public static class BudgetControlsManager
    {
        private static bool budgetControlsAlreadyAdded = false;
        private static bool freezeUI = false;

        public static void AddBudgetControls()
        {
            if (budgetControlsAlreadyAdded)
            {
                return;
            }
            else
            {
                budgetControlsAlreadyAdded = true;

                EconomyPanel ep = ToolsModifierControl.economyPanel;
                UITabContainer economyContainer = ep.component.Find<UITabContainer>("EconomyContainer");

                //Taxes (ColossalFramework.UI.UIPanel)
                //Budget(ColossalFramework.UI.UIPanel)
                //Loans(ColossalFramework.UI.UIPanel)

                //System.Text.StringBuilder sb = new System.Text.StringBuilder("AutoBudget:\n");
                //foreach (UIComponent c in economyContainer.components)
                //{
                //    sb.AppendLine(c.ToString());
                //}
                //Debug.Log(sb.ToString());

                UIPanel budgetPanel = economyContainer.Find<UIPanel>("Budget");
                AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;

                float x1 = 44;
                float x2 = 558;
                float y = -8;
                float dy = 46;

                addCheckBox(budgetPanel, "Electricity", x1, y, c.AutobudgetElectricity.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetElectricity.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });

                addCheckBox(budgetPanel, "Road", x2, y, c.AutobudgetRoad.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetRoad.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });

                y -= dy;

                addCheckBox(budgetPanel, "Water", x1, y, c.AutobudgetWater.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetWater.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });

                y -= dy;

                addCheckBox(budgetPanel, "Garbage", x1, y, c.AutobudgetGarbage.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetGarbage.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });

                y -= dy;

                addCheckBox(budgetPanel, "Healthcare", x1, y, c.AutobudgetHealthcare.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetHealthcare.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });

                y -= dy;

                addCheckBox(budgetPanel, "Education", x1, y, c.AutobudgetEducation.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetEducation.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });

                y -= dy;

                addCheckBox(budgetPanel, "Police", x1, y, c.AutobudgetPolice.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetPolice.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });

                y -= dy;

                addCheckBox(budgetPanel, "Fire", x1, y, c.AutobudgetFire.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetFire.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });

                y -= dy * 3;

                addCheckBox(budgetPanel, "Taxi", x2, y, c.AutobudgetTaxi.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetTaxi.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });

                budgetPanel.eventVisibilityChanged += delegate (UIComponent component, bool value)
                {
                    UpdateUI();
                };
            }
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

        public static void UpdateUI()
        {
            EconomyPanel ep = ToolsModifierControl.economyPanel;
            UITabContainer economyContainer = ep.component.Find<UITabContainer>("EconomyContainer");
            UIPanel budgetPanel = economyContainer.Find<UIPanel>("Budget");
            AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;

            freezeUI = true;
            updateCheckBox(budgetPanel, "Electricity", c.AutobudgetElectricity.Enabled);
            updateCheckBox(budgetPanel, "Water", c.AutobudgetWater.Enabled);
            updateCheckBox(budgetPanel, "Garbage", c.AutobudgetGarbage.Enabled);
            updateCheckBox(budgetPanel, "Healthcare", c.AutobudgetHealthcare.Enabled);
            updateCheckBox(budgetPanel, "Education", c.AutobudgetEducation.Enabled);
            updateCheckBox(budgetPanel, "Police", c.AutobudgetPolice.Enabled);
            updateCheckBox(budgetPanel, "Fire", c.AutobudgetFire.Enabled);
            updateCheckBox(budgetPanel, "Road", c.AutobudgetRoad.Enabled);
            updateCheckBox(budgetPanel, "Taxi", c.AutobudgetTaxi.Enabled);
            freezeUI = false;
        }

        private static void updateCheckBox(UIPanel budgetPanel, string controlName, bool isChecked)
        {
            UICheckBox checkBox = budgetPanel.Find<UICheckBox>("checkBox" + controlName);
            if (checkBox != null)
            {
                checkBox.isChecked = isChecked;
            }
        }
    }
}
