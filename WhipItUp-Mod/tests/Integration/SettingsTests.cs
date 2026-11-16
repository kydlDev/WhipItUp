using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WhipItUp.Tests.Integration;

/// <summary>
/// User rule: the player config holds only what a player should change, everything else is a Tunable (dev-only, shown by KydlTools).
/// KydlTools finds Tunables by reflection (type name "Tunable`1", properties Section / Name / Description / Default / Value / External):
/// these tests keep that contract from breaking.
/// </summary>
[TestFixture]
public class SettingsTests
{
    const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    static List<object> Tunables()
    {
        var list = new List<object>();
        foreach (var f in ModAssembly.Type("WhipItUp.ModSettings").GetFields(Static))
            if (f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition().Name == "Tunable`1") list.Add(f.GetValue(null));
        return list;
    }

    [Test]
    public void Tunables_HaveTheMembersKydlToolsReads()
    {
        foreach (var t in Tunables())
            foreach (var p in new[] { "Section", "Name", "Description", "Default", "Value", "External" })
                Assert.That(t.GetType().GetProperty(p), Is.Not.Null, "Tunable property " + p + " missing (KydlTools reads it)");
    }

    [Test]
    public void Tunables_StartAtTheirFixedValue()
    {
        foreach (var t in Tunables())
        {
            var ty = t.GetType();
            Assert.That(ty.GetProperty("Value").GetValue(t, null), Is.EqualTo(ty.GetProperty("Default").GetValue(t, null)), t.ToString());
        }
    }

    [Test]
    public void PlayerConfig_HasDebugLogSwitch()
    {
        var src = File.ReadAllText(Path.Combine(TestPaths.Src, "Configuration", "ModSettings.cs"));

        Assert.That(Regex.IsMatch(src, @"Bind\(\s*""DebugLog""\s*,\s*""Enable""\s*,\s*false"), Is.True, "[DebugLog] Enable (off by default) is required in every mod");
    }

    [Test]
    public void PlayerConfig_HasEnableMod()
    {
        var src = File.ReadAllText(Path.Combine(TestPaths.Src, "Configuration", "ModSettings.cs"));

        Assert.That(Regex.IsMatch(src, @"Bind\(\s*""General""\s*,\s*""Enable mod""\s*,"), Is.True, "[General] Enable mod (on by default) is the global switch of every mod");
    }

    [Test]
    public void PlayerConfig_CategorySwitchesAreInGeneral()
    {
        // PROJECT_RULES 11 (user, 2026-11-16): the switch of every category is [General] "Enable <Category>"; the only "Enable" that stays in its own section is [DebugLog] Enable
        var src = File.ReadAllText(Path.Combine(TestPaths.Src, "Configuration", "ModSettings.cs"));

        foreach (Match m in Regex.Matches(src, @"\.Bind\(\s*""(?<s>[^""]+)""\s*,\s*""(?<k>[^""]+)"""))
        {
            string section = m.Groups["s"].Value;
            string key = m.Groups["k"].Value;
            if (key == "Enable")
            {
                Assert.That(section, Is.EqualTo("DebugLog"), "[" + section + "] Enable: the switch of a category lives in [General] as 'Enable <Category>' (PROJECT_RULES 11)");
            }

            if (section == "General" && key.StartsWith("Enable ") && key != "Enable mod")
            {
                Assert.That(Regex.IsMatch(key, @"^Enable [A-Z]"), Is.True, "[General] " + key + ": the category name after 'Enable ' starts with a capital letter");
            }
        }
    }

    [Test]
    public void PlayerConfig_NamesSayAppearanceNotLook()
    {
        // user, 2026-11-16: config sections and keys are called "Appearance", never "Look"
        var src = File.ReadAllText(Path.Combine(TestPaths.Src, "Configuration", "ModSettings.cs"));

        foreach (Match m in Regex.Matches(src, @"\.Bind\(\s*""(?<s>[^""]+)""\s*,\s*""(?<k>[^""]+)"""))
        {
            Assert.That(Regex.IsMatch(m.Groups["s"].Value + " " + m.Groups["k"].Value, @"\bLook\b", RegexOptions.IgnoreCase), Is.False, "config name '" + m.Groups["s"].Value + " / " + m.Groups["k"].Value + "': use 'Appearance', not 'Look'");
        }
    }

    [Test]
    public void PlayerConfig_HasNoMenuKey()
    {
        var src = File.ReadAllText(Path.Combine(TestPaths.Src, "Configuration", "ModSettings.cs"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Regex.IsMatch(src, @"Bind\(\s*""[^""]*""\s*,\s*""MenuKey"""), Is.False, "no mod has its own menu key: the menu is tabs in the KY hub (F6, KydlCore)");
            Assert.That(Regex.IsMatch(src, @"ConfigEntry<KeyCode>\s+MenuKey"), Is.False, "no MenuKey entry (KydlCore owns the hub key)");
        }
    }

    [Test]
    public void ConfigDescriptions_DoNotTalkAboutOldVersions()
    {
        var src = File.ReadAllText(Path.Combine(TestPaths.Src, "Configuration", "ModSettings.cs"));

        foreach (Match m in Regex.Matches(src, @"Bind\([^;]*?""(?<d>[^""]{20,})"""))
            Assert.That(Regex.IsMatch(m.Groups["d"].Value, @"\b(previous|formerly|renamed|old value|used to|before version)\b", RegexOptions.IgnoreCase), Is.False, m.Groups["d"].Value);
    }
}
