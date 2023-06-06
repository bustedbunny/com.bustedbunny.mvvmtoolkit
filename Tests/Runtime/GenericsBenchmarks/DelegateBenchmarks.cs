using System;
using MVVMToolkit.Binding.Generics;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace MVVMToolkit.RuntimeTests
{
    public static class DelegateBenchmarks
    {
        private class Source
        {
            public float Value { get; set; }
        }

        private class Target
        {
            public float Value { get; set; }
        }

        private const float SampleValue = 49.5f;
        private const string PropertyName = nameof(Source.Value);

        private const int Warmup = 10;
        private const int MeasurementCount = 5;
        private const int IterationCount = 1000;

        [Test, Performance]
        public static void DirectProperty()
        {
            var source = new Source();
            var target = new Target { Value = SampleValue };

            Measure.Method(() => { source.Value = target.Value; }).WarmupCount(Warmup)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationCount).GC().Run();
        }

        [Test, Performance]
        public static void BoxedDirectProperty()
        {
            var source = new Source();
            var target = new Target { Value = SampleValue };
            Measure.Method(() =>
                {
                    var value = (object)target.Value;
                    source.Value = (float)value;
                }).WarmupCount(Warmup).MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationCount).GC().Run();
        }


        [Test, Performance]
        public static void InstancedFallback()
        {
            var source = new Source();
            var target = new Target { Value = SampleValue };

            var getProp = source.GetType().GetProperty(PropertyName);
            var setProp = target.GetType().GetProperty(PropertyName);

            var solver = new SingleFallback<Source, Target, float>();

            var action = solver.Solve(getProp!.GetGetMethod(), source, setProp!.GetSetMethod(), target);

            Measure.Method(() => action()).WarmupCount(Warmup).MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationCount).GC().Run();
        }

        [Test, Performance]
        public static void ParameterizedFallback()
        {
            var source = new Source();
            var target = new Target { Value = SampleValue };

            var getProp = source.GetType().GetProperty(PropertyName);
            var setProp = target.GetType().GetProperty(PropertyName);

            var solver = new SingleFallback<Source, Target, float>();

            var solution = solver.Solve(getProp!.GetGetMethod(), setProp!.GetSetMethod());
            var action = new Action(() => solution(source, target));

            Measure.Method(() => action()).WarmupCount(Warmup).MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationCount).GC().Run();
        }

        [Test, Performance]
        public static void InstancedBinding()
        {
            var source = new Source();
            var target = new Target { Value = SampleValue };

            var getProp = source.GetType().GetProperty(PropertyName);
            var setProp = target.GetType().GetProperty(PropertyName);

            var solver = new FloatSolver();

            var action = solver.Solve(getProp!.GetGetMethod(), source, setProp!.GetSetMethod(), target);

            Measure.Method(() => action()).WarmupCount(Warmup).MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationCount).GC().Run();
        }

        [Test, Performance]
        public static void Reflection()
        {
            var source = new Source();
            var target = new Target { Value = SampleValue };

            var getProp = source.GetType().GetProperty(PropertyName);
            var setProp = target.GetType().GetProperty(PropertyName);


            Measure.Method(() => setProp!.SetValue(target, getProp!.GetValue(source))).WarmupCount(Warmup)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationCount).GC().Run();
        }
    }
}