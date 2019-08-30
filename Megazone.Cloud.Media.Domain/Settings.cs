namespace Megazone.Cloud.Media.Domain
{
    public class Settings
    {
        public Settings(string projectId, GeneralSetting general, AssetSetting asset)
        {
            ProjectId = projectId;
            General = general;
            Asset = asset;
        }

        public string ProjectId { get; }
        public GeneralSetting General { get; }
        public AssetSetting Asset { get; }
    }

    public class GeneralSetting
    {
        public GeneralSetting(SettingProperty storagePrefix, SettingProperty storagePath)
        {
            StoragePrefix = storagePrefix;
            StoragePath = storagePath;
        }

        public SettingProperty StoragePrefix { get; }
        public SettingProperty StoragePath { get; }
    }

    public class AssetSetting
    {
        public AssetSetting(SettingProperty deleteAfterXDays, SettingProperty inputStoragePrefix,
            SettingProperty outputStoragePrefix, SettingProperty inputStoragePath, SettingProperty outputStoragePath)
        {
            DeleteAfterXDays = deleteAfterXDays;
            InputStoragePrefix = inputStoragePrefix;
            OutputStoragePrefix = outputStoragePrefix;
            InputStoragePath = inputStoragePath;
            OutputStoragePath = outputStoragePath;
        }

        public SettingProperty DeleteAfterXDays { get; }
        public SettingProperty InputStoragePrefix { get; }
        public SettingProperty OutputStoragePrefix { get; }
        public SettingProperty InputStoragePath { get; }
        public SettingProperty OutputStoragePath { get; }
    }


    public class SettingProperty
    {
        public SettingProperty(string id, string value, bool? isOverridable, int layer, string parentValue)
        {
            Id = id;
            Value = value;
            IsOverridable = isOverridable ?? false;
            Layer = layer;
            ParentValue = parentValue;
        }

        public string Id { get; }
        public string Value { get; }
        public bool IsOverridable { get; }
        public int Layer { get; }
        public string ParentValue { get; }
    }
}