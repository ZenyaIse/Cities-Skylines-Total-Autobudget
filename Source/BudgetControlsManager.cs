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
        private static bool isBudgetControlsCreated = false;

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
                            UpdateUI();
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
                            addControlsItem(budgetPanel, obj.GetEconomyPanelContainerName(), obj.GetBudgetItemName(), obj.Enabled, delegate (bool isChecked)
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

        private static void addControlsItem(UIPanel panel, string containerName, string budgetItemName, bool isChecked, OnCheckChanged eventCallback)
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
                        showOptions(budgetItemName);
                    });
                }
            }
        }

        private static void _createBudgetControlsIfNotCreated()
        {
            if (isBudgetControlsNotCreated())
            {
                EconomyPanel ep = ToolsModifierControl.economyPanel;
                UITabContainer economyContainer = ep.component.Find<UITabContainer>("EconomyContainer");

                //Taxes (ColossalFramework.UI.UIPanel)
                //Budget(ColossalFramework.UI.UIPanel)
                //Loans(ColossalFramework.UI.UIPanel)

                UIPanel budgetPanel = economyContainer.Find<UIPanel>("Budget");
                //SubServicesBudgetContainer (ColossalFramework.UI.UIPanel) (590.0, 37.0, 0.0), (590.0, -37.0, 0.0), (994.0, 131.0, 0.0)
                //ServicesBudgetContainer(ColossalFramework.UI.UIPanel)(74.0, -9.0, 0.0), (74.0, 9.0, 0.0), (478.0, 177.0, 0.0)

                //Electricity (ColossalFramework.UI.UIPanel) (0.0, 0.0, 0.0), (0.0, 0.0, 0.0), (478.0, 1076.0, 0.0)
                //WaterAndSewage(ColossalFramework.UI.UIPanel)(0.0, -46.0, 0.0), (0.0, 46.0, 0.0), (478.0, 1122.0, 0.0)
                //Garbage(ColossalFramework.UI.UIPanel)(0.0, -92.0, 0.0), (0.0, 92.0, 0.0), (478.0, 1168.0, 0.0)
                //Healthcare(ColossalFramework.UI.UIPanel)(0.0, -138.0, 0.0), (0.0, 138.0, 0.0), (478.0, 1214.0, 0.0)
                //FireDepartment(ColossalFramework.UI.UIPanel)(0.0, -184.0, 0.0), (0.0, 184.0, 0.0), (478.0, 1260.0, 0.0)
                //Police(ColossalFramework.UI.UIPanel)(0.0, -230.0, 0.0), (0.0, 230.0, 0.0), (478.0, 1306.0, 0.0)
                //Education(ColossalFramework.UI.UIPanel)(0.0, -276.0, 0.0), (0.0, 276.0, 0.0), (478.0, 1352.0, 0.0)
                //Beautification(ColossalFramework.UI.UIPanel)(0.0, -322.0, 0.0), (0.0, 322.0, 0.0), (478.0, 1398.0, 0.0)
                //Monuments(ColossalFramework.UI.UIPanel)(0.0, -368.0, 0.0), (0.0, 368.0, 0.0), (478.0, 1444.0, 0.0)
                //PlayerIndustry(ColossalFramework.UI.UIPanel)(0.0, -414.0, 0.0), (0.0, 414.0, 0.0), (478.0, 1490.0, 0.0)

                //System.Text.StringBuilder sb = new System.Text.StringBuilder("AutoBudget:\n");
                //foreach (UIComponent component in budgetPanel.Find("ServicesBudgetContainer").components)
                //{
                //    sb.AppendLine(component.ToString() + " " + component.position.ToString() + ", " + component.relativePosition.ToString() + ", " + component.absolutePosition.ToString());
                //}
                //Debug.Log(sb.ToString());

                AutobudgetObjectsContainer c = Singleton<AutobudgetManager>.instance.container;

                float x1 = 44;
                float x2 = 560;
                float dxBtn = 10;
                float y = 34;
                float dy = 46;
                float dyBtn = 20;

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
                    showOptions("Road");
                });

                y -= dy;

                // Electricity
                addCheckBox(budgetPanel, "Electricity", x1, y, c.AutobudgetElectricity.Enabled, delegate (bool isChecked)
                {
                    if (!freezeUI)
                    {
                        c.AutobudgetElectricity.Enabled = isChecked;
                        Mod.UpdateUI();
                    }
                });
                addButton(budgetPanel, "Electricity", x1 - dxBtn, y - dyBtn, delegate ()
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

                y -= dy * 2;

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
                        foreach (AutobudgetBase obj in c.AllAutobudgetObjects)
                        {
                            updateCheckBox(budgetPanel, obj.GetBudgetItemName(), obj.Enabled);
                        }
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
