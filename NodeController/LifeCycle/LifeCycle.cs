namespace NodeController.LifeCycle
{
    using NodeController.Tool;
    using NodeController.Util;
    using KianCommons;
    using ICities;
    using NodeController.GUI;



    public static class LifeCycle
    {
        public static void Load(LoadMode mode = LoadMode.NewGame)
        {
            HelpersExtensions.VERBOSE = false;
            Log.Info("LifeCycle.Load() called");
            CSURUtil.Init();
            HarmonyExtension.InstallHarmony();
            NodeControllerTool.Create();
            if (Settings.GameConfig == null) {
                switch (mode) {
                    case LoadMode.NewGameFromScenario:
                    case LoadMode.LoadScenario:
                    case LoadMode.LoadMap:
                        // no NC or old NC
                        Settings.GameConfig = GameConfigT.LoadGameDefault;
                        break;
                    default:
                        Settings.GameConfig = GameConfigT.NewGameDefault;
                        break;
                }
            }

            NodeManager.Instance.OnLoad();
            SegmentEndManager.Instance.OnLoad();
        }

        public static void UnLoad()
        {
            Log.Info("LifeCycle.Release() called");
            Settings.GameConfig = null;
            HarmonyExtension.UninstallHarmony();
            NodeControllerTool.Remove();
        }
    }
}
