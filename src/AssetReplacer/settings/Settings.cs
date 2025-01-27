using BepInEx.Configuration;

namespace AssetReplacer.Settings;

public class AssetReplacerSettings(ConfigFile config)
{
    public ConfigEntry<bool> MySettingsBool = config.Bind<bool>("SectionName", "MySettingsBool", true, "This is an example boolean setting!");
}
