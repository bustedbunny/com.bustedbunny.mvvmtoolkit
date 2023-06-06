using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.Collections.LowLevel.Unsafe;
using Unity.PerformanceTesting;

namespace MVVMToolkit.RuntimeTests
{
    public static class FieldPointerTests
    {
        private class TargetClass
        {
            public float additionalField;

            // Field is put specifically between others
            public string stringField;
            public int additionalField1;
        }

        private class SourceClass
        {
            public int additionalField;

            public int additionalField1;

            // Field is put specifically between others
            public string stringField;
            public float additionalField2;
        }


        [Test]
        public static unsafe void PinnedTest()
        {
            var dstFieldInfo = typeof(TargetClass).GetField(nameof(TargetClass.stringField));
            var dstOffset = UnsafeUtility.GetFieldOffset(dstFieldInfo);

            var srcFieldInfo = typeof(SourceClass).GetField(nameof(SourceClass.stringField));
            var srcOffset = UnsafeUtility.GetFieldOffset(srcFieldInfo);

            const string testString = "HelloWorld";
            var dst = new TargetClass();
            var src = new SourceClass() { stringField = testString };


            var dstAddr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(dst, out var dstHandle);
            var srcAddr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(src, out var srcHandle);

            ref var dstField = ref UnsafeUtility.As<byte, string>(ref *(dstAddr + dstOffset));
            ref var srcField = ref UnsafeUtility.As<byte, string>(ref *(srcAddr + srcOffset));


            dstField = srcField;

            UnsafeUtility.ReleaseGCObject(dstHandle);
            UnsafeUtility.ReleaseGCObject(srcHandle);

            Assert.IsTrue(dst.stringField == src.stringField);
        }

        private const int Warmup = 10;
        private const int MeasurementCount = 5;
        private const int IterationCount = 1000;

        [Test, Performance]
        public static unsafe void PinnedBenchmark()
        {
            var dstFieldInfo = typeof(TargetClass).GetField(nameof(TargetClass.stringField));
            var dstOffset = UnsafeUtility.GetFieldOffset(dstFieldInfo);

            var srcFieldInfo = typeof(SourceClass).GetField(nameof(SourceClass.stringField));
            var srcOffset = UnsafeUtility.GetFieldOffset(srcFieldInfo);

            const string testString = "HelloWorld";
            var dst = new TargetClass();
            var src = new SourceClass() { stringField = testString };

            Measure.Method(() =>
                {
                    var dstAddr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(dst, out var dstHandle);
                    var srcAddr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(src, out var srcHandle);

                    ref var dstField = ref UnsafeUtility.As<byte, string>(ref *(dstAddr + dstOffset));
                    ref var srcField = ref UnsafeUtility.As<byte, string>(ref *(srcAddr + srcOffset));


                    dstField = srcField;

                    UnsafeUtility.ReleaseGCObject(dstHandle);
                    UnsafeUtility.ReleaseGCObject(srcHandle);
                }).WarmupCount(Warmup).MeasurementCount(MeasurementCount).IterationsPerMeasurement(IterationCount).GC()
                .Run();
        }


        private static readonly int HeaderSize = (int)GetHeaderSize(new());

        private static unsafe nint GetHeaderSize(StrongBox<byte> o)
        {
            fixed (byte* _ = &o.Value)
            {
                ref var r0 = ref Unsafe.As<StrongBox<byte>, nint>(ref o);
                return Unsafe.ByteOffset(ref *(byte*)r0, ref o.Value);
            }
        }

        public static ref TField GetField<T, TField>(ref T owner, int offset)
        {
            var ofs = (nint)offset;
            if (typeof(T).IsValueType)
                return ref Unsafe.AddByteOffset(ref Unsafe.As<T, TField>(ref owner), ofs);

            ref var r0 = ref Unsafe.As<StrongBox<TField>>(owner)!.Value;
            ref var r1 = ref Unsafe.AddByteOffset(ref r0, ofs - HeaderSize);
            return ref r1!;
        }


        [Test]
        public static void UtilityTest()
        {
            var dstFieldInfo = typeof(TargetClass).GetField(nameof(TargetClass.stringField));
            var dstOffset = UnsafeUtility.GetFieldOffset(dstFieldInfo);

            var srcFieldInfo = typeof(SourceClass).GetField(nameof(SourceClass.stringField));
            var srcOffset = UnsafeUtility.GetFieldOffset(srcFieldInfo);

            const string testString = "HelloWorld";
            var dst = new TargetClass();
            var src = new SourceClass { stringField = testString };

            ref var dstField = ref GetField<TargetClass, string>(ref dst, dstOffset);
            ref var srcField = ref GetField<SourceClass, string>(ref src, srcOffset);

            dstField = srcField;

            Assert.IsTrue(dst.stringField == src.stringField);
        }

        [Test, Performance]
        public static void UtilityBenchmark()
        {
            var dstFieldInfo = typeof(TargetClass).GetField(nameof(TargetClass.stringField));
            var dstOffset = UnsafeUtility.GetFieldOffset(dstFieldInfo);

            var srcFieldInfo = typeof(SourceClass).GetField(nameof(SourceClass.stringField));
            var srcOffset = UnsafeUtility.GetFieldOffset(srcFieldInfo);

            const string testString = "HelloWorld";
            var dst = new TargetClass();
            var src = new SourceClass { stringField = testString };


            Measure.Method(() =>
                {
                    ref var dstField = ref GetField<TargetClass, string>(ref dst, dstOffset);
                    ref var srcField = ref GetField<SourceClass, string>(ref src, srcOffset);

                    dstField = srcField;
                }).WarmupCount(Warmup).MeasurementCount(MeasurementCount).IterationsPerMeasurement(IterationCount).GC()
                .Run();
        }
    }
}