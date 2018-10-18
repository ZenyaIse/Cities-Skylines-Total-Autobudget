# Total Autobudget (mod for Cities Skylines)

This mod enables budget autotuning for most of your city’s services.

The goal of this mod is to optimize the city budget, prevent blackouts or water outages due to production fluctuations, set target values for some services (such as education), and reduce the player’s burden of budget micromanagement. Also it is very helpful if you are playing at hard difficulty settings, such as vanilla hard mod or zenya’s difficulty tuning mod.

Autobudget target services:
- Electricity
- Water, Sewage, and Heating
- Garbage
- Healthcare and Deathcare
- Education
- Police
- Fire service
- Road maintenance and snow dumps
- Taxi

How to enable or disable autobudget?
- In the mod options.
- In the budget tab of the economy panel. There are checkboxes near every autotunable items.
- In case you manually move a budget item slider, the autobudget for this item will be automatically disabled.

All settings are stored in your game save file, so you can use different settings for each game.

## Detailed autobudget options

### Electricity

**Buffer**

Set how much electricity production should exceed electricity consumption (in percentage). 3% or more is usually enough to avoid blackouts. 1%-2% is good to decrease electricity expenses but this may cause short-time blackouts in some parts of your city.

**Maximum budget**

Set the maximum value of electricity budget. 130-140% is good only for very small towns, 120-125% is recommended if you have two or more power stations.

**Autopause when budget is too high**

When the electricity autobudget raises up to the maximum value, the game will automatically pause and switch to the electricity info view mode. You can unpause game and continue playing ignoring lack of electricity - the game will not autopause any more until the autobudget drops and raises again.

*Note*

Electricity autobudget will not work properly if you have two or more separate areas in your city. Connect the separated areas with power lines or disable electricity autobudget in this case.


### Water, Sewage, and Heating

**Buffer**

Set how much production should exceed consumption (in percentage). 3% or more is usually enough to avoid outages. 1%-2% is good to decrease expenses but may cause short-time outages in some parts of your city.
Water buffer (not sewage buffer) will automatically set to 0% (internally, not seen in the options) if there is enough water in your water tanks (see the next option).

**Target water storage**

Autobudget will try not to allow water tanks be filled less than this value.

**Maximum budget**

Set the maximum value of water and sewage budget. 130-140% is good only for very small towns, 120-125% is recommended if you have two or more pump stations.

**Autopause when budget is too high**

When the water / sewage budget raises up to the maximum value, the game will automatically pause and switch to the water and sewage info view mode. You can unpause game and continue playing ignoring lack of water or sewage capacity - the game will not autopause any more until the budget drops and raises again.

**Increase budget if not enough heating**

If some of your buildings have heating problems, the budget increases until the heating problems disappear or the budget reaches the maximum value (see the next option).

**Max heating budget**

Budget rising due to heating problems will never exceed this value.

*Note*
Water / sewage / heating autobudget will not work properly if you have two or more separate areas in your city. Connect the separated areas with pipes or disable water and sewage autobudget in this case.


### Garbage

**Maximum budget**

Set the maximum value of garbage budget. 115-120% is recommended.

**Max garbage amount (% of capacity)**

If you have productive garbage facilities (recycling center or incineration plant) and at least one of them is piled with garbage more than this value, the garbage budget will be raised to the maximum to speed up the garbage processing rate. 70-90% is recommended.

*Note*
The mod will try to set the budget so that at least one garbage track is waiting in each of the garbage facilities.


### Healthcare and Deathcare

**Minimum budget**

Set the minimum value of healthcare and deathcare budget.

**Maximum budget**

Set the maximum value of healthcare and deathcare budget.

*Note 1*

Setting the minimum budget too low may cause the low land value issue.

*Note 2*

Mod is trying to set the budget value so that there is at least one ambulance waiting in each of the hospitals and at least one hearse waiting in each of the deathcare facilities.


### Education

**Elementary / High school / University education**

Set the target education rate for each type of education. For example, if you set 70%, the mod will try to set the budget to educate 70% of people and leave 30% uneducated. Because budget cannot be set for each type of education individually, the maximum value of budget is chosen. That means, for example, if you set 70%, you will actually have 70% or more (but not less) of your people educated.

**Maximum budget**

Set the maximum value of education budget.


### Police

**Minimum budget**

Set the minimum value of police budget.

**Maximum budget**

Set the maximum value of police budget.

*Note*

Mod is trying to set the budget value so that there is at least one patrol car waiting in each of the police stations.


### Fire service

**Minimum budget**

Set the minimum value of fire service budget.

**Maximum budget**

Set the maximum value of fire service budget.

**Minimum tracks waiting**

Set the minimum number of tracks waiting in each of the fire stations.

*Note*

Disaster support is planned in future versions.


### Road maintenance and snow dumps

**Minimum budget**

Set the minimum value of road maintenance and snow dump budget.

**Maximum budget**

Set the maximum value of road maintenance and snow dump budget.

*Note*

Setting the minimum budget too low may cause lack of citizens inflow into the city.


### Taxi

**Maximum budget**

Set the maximum value of taxi budget.

**Taxis waiting in depots**

**Taxis waiting at stands**

Set the target number of taxis waiting in depots and at taxi stands. These are averaged target numbers. The actual numbers is very dependant on the depots and taxi stands placement.
