using BepInEx.Configuration;

namespace WhipItUp;

/// <summary>
/// The tag BepInEx's Configuration Manager (F1) reads: <c>Order</c> sets where a setting sits inside its category, a HIGHER number is HIGHER in
/// the list (default 0). The KY hub settings tab (F6) lists a category in the order the entries are bound instead, so the same order is kept by
/// binding them in that order (user 2026-11-17: [General] Enable Mod is the first setting of every mod, in both menus).
/// </summary>
internal sealed class ConfigurationManagerAttributes
{
    public int? Order;
}

/// <summary>Descriptions that carry the F1 order tag. <see cref="First"/> = top of its category, <see cref="Last"/> = bottom of it.</summary>
internal static class ConfigOrder
{
    public const int FirstOrder = 1000, LastOrder = -1000;

    public static ConfigDescription First(string description)
    {
        return new ConfigDescription(description, null, new ConfigurationManagerAttributes { Order = FirstOrder });
    }

    public static ConfigDescription Last(string description)
    {
        return new ConfigDescription(description, null, new ConfigurationManagerAttributes { Order = LastOrder });
    }
}