using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.DbApi.Benchmark.Tests;

public class MyClassBenchmark
{
    private readonly MyClass subject = new MyClass();
    private readonly int _smallNumber;
    private readonly int _largeNumber;

    public MyClassBenchmark()
    {
        _smallNumber = Random.Shared.Next(1, 100);
        _largeNumber = Random.Shared.Next(1000000, 10000000);
    }

    [Benchmark]
    public int SmallMultiply() => subject.Multiply(_smallNumber, _smallNumber);

    [Benchmark]
    public int LargeMultiply() => subject.Multiply(_largeNumber, _largeNumber);
}