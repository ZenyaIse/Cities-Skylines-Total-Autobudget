using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;
using ICities;

namespace AutoBudget
{
    public static class BudgetControlsManager
    {
        private static bool freezeUI = false;
        private static bool isInitialized = false;

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
                            UpdateUI();
                        }
                    };

                    isInitialized = true;
                }
            }
        }

        private static void createBudgetControlsIfNotCreated()
        {
            if (isBudgetControlsNotCreated())
            {
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
                float x2 = 560;
                float dxBtn = 10;
                float y = -8;
                float dy = 46;
                float dyBtn = 20;

                // Electricity
                addCheckBox(budgetPanel, "Electricity", x1, y, c.AutobudgetElectricity.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetElectricity.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });
                //UIPanel electricityOptionsPanel = addOptionsPanel(budgetPanel, "Electricity", x1 + 2 * dxBtn, y - 2 * dyBtn);
                addButton(budgetPanel, "Electricity", x1 - dxBtn, y - dyBtn, delegate ()
                {
                    UIView.library.ShowModal("OptionsPanel");
                    //UICustomControl optionsMainPanel = (UICustomControl)UIView.Find("OptionsMainPanel");
                    
                });

                // Road
                addCheckBox(budgetPanel, "Road", x2, y, c.AutobudgetRoad.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetRoad.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });
                addButton(budgetPanel, "Road", x2 - dxBtn, y - dyBtn, delegate ()
                {
                    showOptions("Electricity");
                });

                y -= dy;

                // Water
                addCheckBox(budgetPanel, "Water", x1, y, c.AutobudgetWater.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetWater.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });
                addButton(budgetPanel, "Water", x1 - dxBtn, y - dyBtn, delegate ()
                {
                    showOptions("Water");
                });

                y -= dy;

                // Garbage
                addCheckBox(budgetPanel, "Garbage", x1, y, c.AutobudgetGarbage.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetGarbage.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });
                addButton(budgetPanel, "Garbage", x1 - dxBtn, y - dyBtn, delegate ()
                {
                    showOptions("Garbage");
                });

                y -= dy;

                // Healthcare and deathcare
                addCheckBox(budgetPanel, "Healthcare", x1, y, c.AutobudgetHealthcare.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetHealthcare.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });
                addButton(budgetPanel, "Healthcare", x1 - dxBtn, y - dyBtn, delegate ()
                {
                    showOptions("Healthcare");
                });

                y -= dy;

                // Fire service
                addCheckBox(budgetPanel, "Fire", x1, y, c.AutobudgetFire.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetFire.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });
                addButton(budgetPanel, "Fire", x1 - dxBtn, y - dyBtn, delegate ()
                {
                    showOptions("Fire");
                });

                y -= dy;

                // Police
                addCheckBox(budgetPanel, "Police", x1, y, c.AutobudgetPolice.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetPolice.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });
                addButton(budgetPanel, "Police", x1 - dxBtn, y - dyBtn, delegate ()
                {
                    showOptions("Police");
                });

                y -= dy;

                // Education
                addCheckBox(budgetPanel, "Education", x1, y, c.AutobudgetEducation.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetEducation.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });
                addButton(budgetPanel, "Education", x1 - dxBtn, y - dyBtn, delegate ()
                {
                    showOptions("Education");
                });

                y -= dy * 3;

                // Taxi
                addCheckBox(budgetPanel, "Taxi", x2, y, c.AutobudgetTaxi.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetTaxi.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });
                addButton(budgetPanel, "Taxi", x2 - dxBtn, y - dyBtn, delegate ()
                {
                    showOptions("Taxi");
                });
            }
        }

        private static void showOptions(string budgetName)
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

        private static UIPanel addOptionsPanel(UIPanel panel, string controlName, float x, float y)
        {
            UIPanel optionsPanel = panel.AddUIComponent<UIPanel>();
            optionsPanel.name = "optionsPanel" + controlName;
            optionsPanel.position = new Vector3(x, y);
            optionsPanel.size = new Vector2(200, 200);
            optionsPanel.backgroundSprite = "ButtonMenu";
            optionsPanel.canFocus = true;
            optionsPanel.isVisible = false;
            optionsPanel.eventLeaveFocus += delegate (UIComponent component, UIFocusEventParameter eventParam)
            {
                optionsPanel.isVisible = false;
            };

            return optionsPanel;
        }

        public static void UpdateUI()
        {
            createBudgetControlsIfNotCreated();

            EconomyPanel ep = ToolsModifierControl.economyPanel;
            if (ep != null && ep.component != null)
            {
                UITabContainer economyContainer = ep.component.Find<UITabContainer>("EconomyContainer");
                if (economyContainer != null)
                {
                    UIPanel budgetPanel = economyContainer.Find<UIPanel>("Budget");
                    if (budgetPanel != null)
                    {
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
                }
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

        private static bool isBudgetControlsNotCreated()
        {
            EconomyPanel ep = ToolsModifierControl.economyPanel;
            if (ep != null && ep.component != null)
            {
                UITabContainer economyContainer = ep.component.Find<UITabContainer>("EconomyContainer");
                if (economyContainer != null)
                {
                    UIPanel budgetPanel = economyContainer.Find<UIPanel>("Budget");
                    if (budgetPanel != null)
                    {
                        if (budgetPanel.Find<UICheckBox>("checkBoxElectricity") == null)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
