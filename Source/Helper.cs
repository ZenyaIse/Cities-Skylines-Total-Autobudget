using ColossalFramework;

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
    }
}
