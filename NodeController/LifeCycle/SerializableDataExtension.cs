namespace NodeController.LifeCycle
{
    using JetBrains.Annotations;
    using ICities;
    using KianCommons;
    using System;
    using NodeController.GUI;

    [Serializable]
    public class NCState {
        public static NCState Instance;

        public string Version = typeof(NCState).VersionOf().ToString(3);
        public byte[] NodeManagerData;
        public byte[] SegmentEndManagerData;
        public GameConfigT GameConfig;

        public static byte[] Serialize() {
            Instance = new NCState {
                NodeManagerData = NodeManager.Serialize(),
                SegmentEndManagerData = SegmentEndManager.Serialize(),
                GameConfig = Settings.GameConfig,
            };
            return SerializationUtil.Serialize(Instance);
        }

        public static void Deserialize(byte[] data) {
            if (data == null) {
                Log.Debug($"NCState.Deserialize(data=null)");
                Instance = new NCState();
            } else {
                Log.Debug($"NCState.Deserialize(data): data.Length={data?.Length}");
                Instance = SerializationUtil.Deserialize(data) as NCState;
                if (Instance?.Version != null) { //2.1.1 or above
                    SerializationUtil.DeserializationVersion = new Version(Instance.Version);
                } else {
                    // 2.0
                    SerializationUtil.DeserializationVersion = new System.Version(2, 0);
                    Instance.GameConfig = GameConfigT.LoadGameDefault; // for the sake of feature proofing.
                    Instance.GameConfig.UnviversalSlopeFixes = true; // in this version I do apply slope fixes.
                }
            }
            Settings.GameConfig = Instance.GameConfig;
            SegmentEndManager.Deserialize(Instance.SegmentEndManagerData);
            NodeManager.Deserialize(Instance.NodeManagerData);
        }

    }

    [UsedImplicitly]
    public class SerializableDataExtension
        : SerializableDataExtensionBase
    {
        private const string DATA_ID0 = "RoadTransitionManager_V1.0";
        private const string DATA_ID1 = "NodeController_V1.0";
        private const string DATA_ID = "NodeController_V2.0";

        public static int LoadingVersion;
        public override void OnLoadData()
        {
            byte[] data = serializableDataManager.LoadData(DATA_ID);
            if (data != null) {
                LoadingVersion = 2;
                NCState.Deserialize(data);
            } else {
                // convert to new version
                LoadingVersion = 1;
                data = serializableDataManager.LoadData(DATA_ID1)
                    ?? serializableDataManager.LoadData(DATA_ID0);
                NodeManager.Deserialize(data);
                SerializationUtil.DeserializationVersion = new Version(1,0);
            }
        }

        public override void OnSaveData()
        {
            byte[] data = NCState.Serialize();
            serializableDataManager.SaveData(DATA_ID, data);
        }
    }
}
