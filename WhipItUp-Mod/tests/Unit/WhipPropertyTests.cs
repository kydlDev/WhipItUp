using System;
using FsCheck;
using WhipItUp.Weapons;

namespace WhipItUp.Tests.Unit;

/// <summary>Property tests (FsCheck): the hit stop never exceeds the game's pause or goes negative, and the lash parser survives any text.</summary>
[TestFixture]
public class WhipPropertyTests
{
    static readonly Gen<float> Seconds = Props.GameFloat(5f);

    [Test]
    public void HitStop_IsCappedByTheGamePause_AndNeverNegative()
    {
        Props.Check(Prop.ForAll(Seconds.ToArbitrary(), Seconds.ToArbitrary(), (vanilla, setting) =>
        {
            float d = HitStopMath.Duration(vanilla, setting);
            return vanilla <= 0f ? d == vanilla : d >= 0f && d <= vanilla + 1e-6f;
        }));
    }

    [Test]
    public void HitStop_ASettingInsideTheRangeIsKept()
    {
        Props.Check(Prop.ForAll(Gen.Choose(1, 5000).ToArbitrary(), Gen.Choose(0, 100).ToArbitrary(), (v, pct) =>
        {
            float vanilla = v / 1000f, setting = vanilla * pct / 100f;
            return Math.Abs(HitStopMath.Duration(vanilla, setting) - setting) < 1e-6f;
        }));
    }

    [Test]
    public void LashParser_NeverThrows_OnAnyText()
    {
        Props.Check(Prop.ForAll(Arb.Default.String(), text =>
        {
            var frames = WhipLashFrames.Parse(text, null);
            return frames != null && frames.Count >= 0;
        }));
    }

    [Test]
    public void LashSample_ClampsTime_AndRefusesUnknownNodes()
    {
        var frames = WhipLashFrames.Parse("clip C 3 2\ncrack 1\n0 0 0 1 1 1\n0 0 0 2 2 2\n0 0 0 3 3 3\n", null);
        Props.Check(Prop.ForAll(Props.GameFloat(10f).ToArbitrary(), Gen.Choose(-3, 5).ToArbitrary(), (t, node) =>
        {
            float x, y, z;
            bool ok = frames.Sample("C", t, node, out x, out y, out z);
            if (node < 0 || node >= 2) return !ok;
            return ok && x >= 0f && x <= 3.0001f;
        }));
    }
}
