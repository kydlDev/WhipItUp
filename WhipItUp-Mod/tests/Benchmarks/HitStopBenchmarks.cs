using BenchmarkDotNet.Attributes;
using WhipItUp.Weapons;

namespace WhipItUp.Benchmarks;

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
