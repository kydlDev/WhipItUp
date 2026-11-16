using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace WhipItUp.Tests.Integration;

/// <summary>
/// User, 2026-11-16: "a mod should absolutely NEVER have those settings set, it should always expect them to be set by Core." The text
/// size, the text colour and the font of every menu are KydlCore's. A mod's view code names the ROLE of a text (UiKit.TextLabel / Style /
/// Tint / Size / TabText / ColourChip with a TextRole) and never sets a size, a colour or a font itself.
/// </summary>
[TestFixture]
public class UiTextRulesTests
{
    // a font size, a text colour or a font set by hand, the kit's size constants, and the kit's label that takes a size and a colour
    static readonly Regex Forbidden = new Regex(@"style\.fontSize|style\.color\b|unityFont|UiKit\.Label\(|UiKit\.(Title|Section|Font|Button|Small|Name)Size\b");

    // not the menus KydlCore shows: nothing is excused in this mod
    static readonly string[] Excused = {  };

    static IEnumerable<string> ViewFiles()
    {
        var dirs = new List<string> { TestPaths.Src };
        string unity = Path.GetFullPath(Path.Combine(TestPaths.ModDir, "..", "WhipItUp-Unity", "Assets"));
        if (Directory.Exists(unity))
            foreach (string ui in Directory.GetDirectories(unity, "*_UI"))
                if (Directory.Exists(Path.Combine(ui, "Runtime"))) dirs.Add(Path.Combine(ui, "Runtime"));
        foreach (string dir in dirs)
            foreach (string file in Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories))
            {
                string path = file.Replace('/', '\\');
                bool excused = path.Contains("\\Testing\\");
                foreach (string part in Excused) excused |= path.Contains(part);
                if (!excused) yield return file;
            }
    }

    [Test]
    public void ViewCode_NeverSetsATextSize_ATextColour_OrAFont()
    {
        var offenders = new List<string>();
        int read = 0;
        foreach (string file in ViewFiles())
        {
            string text = File.ReadAllText(file);
            if (!text.Contains("UnityEngine.UIElements") && !text.Contains("KydlCore.Ui")) continue;
            read++;
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string code = Regex.Replace(lines[i], @"//.*$", "");
                if (Forbidden.IsMatch(code)) offenders.Add(Path.GetFileName(file) + ":" + (i + 1) + " " + code.Trim());
            }
        }

        Assert.That(offenders, Is.Empty, "text sizes, text colours and fonts are set by KydlCore: use UiKit.TextLabel / Style / Tint / Size / TabText / ColourChip with a TextRole (" + read + " view file(s) read)");
    }
}
