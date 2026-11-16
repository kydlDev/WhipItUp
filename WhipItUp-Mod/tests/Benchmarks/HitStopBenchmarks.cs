using BenchmarkDotNet.Attributes;
using WhipItUp.Weapons;

namespace WhipItUp.Benchmarks
{
    /// <summary>The hit stop duration is computed for every whip hit (comparison axis: the three branches of the clamp).</summary>
    [MemoryDiagnoser]
    public class HitStopBenchmarks
    {
        float _vanilla, _below, _inside, _above;

        [GlobalSetup]
        public void Setup()
        {
            _vanilla = 0.12f;
            _below = -1f;
            _inside = 0.05f;
            _above = 0.5f;
        }

        [Benchmark(Baseline = true)]
        public float SettingInside() => HitStopMath.Duration(_vanilla, _inside);

        [Benchmark]
        public float SettingBelowZero() => HitStopMath.Duration(_vanilla, _below);

        [Benchmark]
        public float SettingAboveVanilla() => HitStopMath.Duration(_vanilla, _above);
    }
}

namespace WhipItUp.Benchmarks
{
    /// <summary>The keyed lash data: parsed once at game start (cold path), sampled every frame while a lash plays (hot path). Real shipped file.</summary>
    [MemoryDiagnoser]
    public class LashFramesBenchmarks
    {
        string _text, _clip;
        WhipLashFrames _frames;
        float _t;
        int _node;

        [GlobalSetup]
        public void Setup()
        {
            _text = System.IO.File.ReadAllText(System.IO.Path.Combine(System.AppContext.BaseDirectory, "whip_lash_frames.txt"));
            _frames = WhipLashFrames.Parse(_text, null);
            _clip = "W1_SideFlick";
            _t = 0.4137f;
            _node = 20;
        }

        [Benchmark]
        public int ParseShippedFile() => WhipLashFrames.Parse(_text, null).Count;

        [Benchmark]
        public float SampleOneNode()
        {
            _frames.Sample(_clip, _t, _node, out float x, out float y, out float z);
            return x + y + z;
        }
    }
}
