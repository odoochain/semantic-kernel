﻿// Copyright (c) Microsoft. All rights reserved.

//using System.Collections;
//using System.Text;
//using Microsoft.SemanticKernel;

//namespace Step05;

///// <summary>
///// DEV HARNESS
///// </summary>
//public class Step05b_MapReduce(ITestOutputHelper output) : BaseTest(output, redirectSystemConsoleOutput: true)
//{
//    // Target Open AI Services
//    protected override bool ForceOpenAI => true;

//    private static readonly long[] s_seedInput = [1, 2, 3, 4, 5];

//    #region Reference

//    [Fact]
//    public async Task RunDiscreteReferenceAsync()
//    {
//        KernelProcess process = SetupDiscreteProcess(nameof(RunDiscreteReferenceAsync));
//        await RunProcessAsync(process, 123);
//    }

//    [Fact]
//    public async Task RunArrayReferenceAsync()
//    {
//        KernelProcess process = SetupArrayProcess(nameof(RunArrayReferenceAsync));
//        await RunProcessAsync(process, s_seedInput);
//    }

//    #endregion

//    [Fact]
//    public async Task RunMapReduceBasicAsync()
//    {
//        KernelProcess process = SetupBasicMapProcess(nameof(RunMapReduceBasicAsync));
//        await RunProcessAsync(process, s_seedInput);
//    }

//    [Fact]
//    public async Task RunMapReduceWithNoiseAsync()
//    {
//        KernelProcess process = SetupBasicMapProcess(nameof(RunMapReduceWithNoiseAsync));
//        await RunProcessAsync(process, s_seedInput);
//    }

//    [Fact]
//    public async Task RunMapReduceWithTransformAsync()
//    {
//        KernelProcess process = SetupTransformMapProcess(nameof(RunMapReduceWithTransformAsync));
//        await RunProcessAsync(process, s_seedInput);
//    }

//    [Fact]
//    public async Task RunMapReduceAsFirstStepAsync()
//    {
//        KernelProcess process = SetupProcessWithMapAsFirstStep(nameof(RunMapReduceAsFirstStepAsync));
//        await RunProcessAsync(process, s_seedInput, "Start");
//    }

//    [Fact]
//    public async Task RunMapReduceWithProcessAsync()
//    {
//        KernelProcess process = SetupMapProcessWithSubProcess(nameof(RunMapReduceWithProcessAsync));
//        await RunProcessAsync(process, s_seedInput);
//    }

//    [Fact]
//    public async Task RunMapReduceMultipleTargetsAsync()
//    {
//        KernelProcess process = SetupMultiTargetMapProcess(nameof(RunMapReduceMultipleTargetsAsync));
//        await RunProcessAsync(process, s_seedInput);
//    }

//    [Fact]
//    public async Task RunMapReducePartialTargetsAsync()
//    {
//        KernelProcess process = SetupPartialTargetMapProcess(nameof(RunMapReducePartialTargetsAsync));
//        await RunProcessAsync(process, s_seedInput);
//    }

//    [Fact]
//    public async Task RunMapBadInputAsync()
//    {
//        KernelProcess process = SetupBasicMapProcess(nameof(RunMapBadInputAsync));
//        await Assert.ThrowsAsync<KernelException>(() => RunProcessAsync(process, 25));
//    }

//    private async Task RunProcessAsync(KernelProcess process, object input, string? inputEvent = null)
//    {
//        this.WriteHorizontalRule();
//        using LocalKernelProcessContext localProcess =
//            await process.StartAsync(
//                new Kernel(),
//                new KernelProcessEvent
//                {
//                    Id = inputEvent ?? "Init",
//                    Data = input
//                });
//    }

//    private KernelProcess SetupBasicMapProcess(string processName)
//    {
//        ProcessBuilder process = new(processName);

//        var initStep = process.AddStepFromType<InitialStep>();
//        process
//            .OnInputEvent("Init")
//            .SendEventTo(new ProcessFunctionTargetBuilder(initStep));

//        var mapStep = process.AddMapFromType<BasicDiscreteStep>();

//        initStep
//            .OnEvent("Start")
//            .SendEventTo(mapStep);

//        var unionStep = process.AddStepFromType<UnionStep>();
//        mapStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(unionStep, "UnionCompute"));

//        var resultStep = process.AddStepFromType<ResultStep>();
//        unionStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(resultStep));

//        return process.Build();
//    }

//    private KernelProcess SetupMultiTargetMapProcess(string processName)
//    {
//        ProcessBuilder process = new(processName);

//        var initStep = process.AddStepFromType<InitialStep>();
//        process
//            .OnInputEvent("Init")
//            .SendEventTo(new ProcessFunctionTargetBuilder(initStep));

//        var mapStep =
//            process
//                .AddMapFromType<DiscreteStep>()
//                .ForTarget("DiscreteFork");

//        initStep
//            .OnEvent("Start")
//            .SendEventTo(mapStep);

//        var unionStep = process.AddStepFromType<UnionStep>();
//        mapStep
//            .OnEvent("CompleteSquare")
//            .SendEventTo(new ProcessFunctionTargetBuilder(unionStep, "UnionCompute"));

//        mapStep
//            .OnEvent("CompleteCube")
//            .SendEventTo(new ProcessFunctionTargetBuilder(unionStep, "UnionCompute"));

//        var resultStep = process.AddStepFromType<ResultStep>();
//        unionStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(resultStep));

//        return process.Build();
//    }

//    private KernelProcess SetupPartialTargetMapProcess(string processName)
//    {
//        ProcessBuilder process = new(processName);

//        var initStep = process.AddStepFromType<InitialStep>();
//        process
//            .OnInputEvent("Init")
//            .SendEventTo(new ProcessFunctionTargetBuilder(initStep));

//        var mapStep =
//            process
//                .AddMapFromType<DiscreteStep>()
//                .ForTarget("DiscreteFork");

//        initStep
//            .OnEvent("Start")
//            .SendEventTo(mapStep);

//        var unionStep = process.AddStepFromType<UnionStep>();
//        mapStep
//            .OnEvent("CompleteCube")
//            .SendEventTo(new ProcessFunctionTargetBuilder(unionStep, "UnionCompute"));

//        var resultStep = process.AddStepFromType<ResultStep>();
//        unionStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(resultStep));

//        return process.Build();
//    }

//    private KernelProcess SetupMapProcessWithSubProcess(string processName)
//    {
//        ProcessBuilder process = new(processName);

//        ProcessBuilder subProcess = new("MapSubprocess");

//        var initStep = process.AddStepFromType<InitialStep>();
//        process
//            .OnInputEvent("Init")
//            .SendEventTo(new ProcessFunctionTargetBuilder(initStep));

//        var discreteStep = subProcess.AddStepFromType<DiscreteStep>();
//        subProcess
//            .OnInputEvent("Start")
//            .SendEventTo(new ProcessFunctionTargetBuilder(discreteStep, "DiscreteSubprocess"));

//        var mapStep = process.AddMapFromProcess(subProcess, "Start");

//        initStep
//            .OnEvent("Start")
//            .SendEventTo(mapStep);

//        var unionStep = process.AddStepFromType<UnionStep>();
//        mapStep
//            .OnEvent("CompleteSquare")
//            .SendEventTo(new ProcessFunctionTargetBuilder(unionStep, "UnionCompute"));

//        var resultStep = process.AddStepFromType<ResultStep>();
//        unionStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(resultStep));

//        return process.Build();
//    }

//    private KernelProcess SetupProcessWithMapAsFirstStep(string processName)
//    {
//        ProcessBuilder process = new(processName);

//        var mapStep = process.AddMapFromType<BasicDiscreteStep>();

//        process
//            .OnInputEvent("Start")
//            .SendEventTo(mapStep);

//        var unionStep = process.AddStepFromType<UnionStep>();
//        mapStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(unionStep, "UnionCompute"));

//        var resultStep = process.AddStepFromType<ResultStep>();
//        unionStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(resultStep));

//        return process.Build();
//    }

//    private KernelProcess SetupNoisyMapProcess(string processName)
//    {
//        ProcessBuilder process = new(processName);

//        var initStep = process.AddStepFromType<InitialStep>();
//        process
//            .OnInputEvent("Init")
//            .SendEventTo(new ProcessFunctionTargetBuilder(initStep));

//        var mapStep =
//            process
//                .AddMapFromType<DiscreteStep>()
//                .ForTarget("DiscreteCompute");

//        initStep
//            .OnEvent("Start")
//            .SendEventTo(mapStep);

//        var unionStep = process.AddStepFromType<UnionStep>();
//        mapStep
//            .OnEvent("CompleteSquare")
//            .SendEventTo(new ProcessFunctionTargetBuilder(unionStep, "UnionCompute"));

//        var resultStep = process.AddStepFromType<ResultStep>();
//        unionStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(resultStep));

//        return process.Build();
//    }

//    private KernelProcess SetupTransformMapProcess(string processName)
//    {
//        ProcessBuilder process = new(processName);

//        var initStep = process.AddStepFromType<InitialStep>();
//        process
//            .OnInputEvent("Init")
//            .SendEventTo(new ProcessFunctionTargetBuilder(initStep));

//        var mapStep =
//            process
//                .AddMapFromType<DiscreteStep>()
//                .ForTarget("DiscreteTransform");

//        initStep
//            .OnEvent("Start")
//            .SendEventTo(mapStep);

//        var unionStep = process.AddStepFromType<UnionStep>();
//        mapStep
//            .OnEvent("CompleteFormat")
//            .SendEventTo(new ProcessFunctionTargetBuilder(unionStep, "UnionTransform"));

//        var resultStep = process.AddStepFromType<ResultStep>();
//        unionStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(resultStep));

//        return process.Build();
//    }

//    #region Reference

//    private KernelProcess SetupArrayProcess(string processName)
//    {
//        ProcessBuilder process = new(processName);

//        var initStep = process.AddStepFromType<InitialStep>();
//        process
//            .OnInputEvent("Init")
//            .SendEventTo(new ProcessFunctionTargetBuilder(initStep));

//        var innerStep = process.AddStepFromProcess(new ProcessBuilder($"Inner{processName}"));
//        var arrayStep = innerStep.AddStepFromType<ArrayStep>();
//        //var arrayStep = innerStep.AddStepFromType<MapStep>();
//        innerStep
//            .OnInputEvent("Start")
//            .SendEventTo(new ProcessFunctionTargetBuilder(arrayStep));
//        initStep
//            .OnEvent("Start")
//            .SendEventTo(innerStep.WhereInputEventIs("Start"));

//        var unionStep = process.AddStepFromType<UnionStep>();
//        innerStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(unionStep));

//        var resultStep = process.AddStepFromType<ResultStep>();
//        unionStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(resultStep));

//        return process.Build();
//    }

//    private KernelProcess SetupDiscreteProcess(string processName)
//    {
//        ProcessBuilder process = new(processName);

//        var initStep = process.AddStepFromType<InitialStep>();
//        process
//            .OnInputEvent("Init")
//            .SendEventTo(new ProcessFunctionTargetBuilder(initStep));

//        var innerStep = process.AddStepFromProcess(new ProcessBuilder($"Inner{processName}"));
//        var discreteStep = innerStep.AddStepFromType<DiscreteStep>();
//        innerStep
//            .OnInputEvent("Start")
//            .SendEventTo(new ProcessFunctionTargetBuilder(discreteStep, "DiscreteCompute"));
//        initStep
//            .OnEvent("Start")
//            .SendEventTo(innerStep.WhereInputEventIs("Start"));

//        var resultStep = process.AddStepFromType<ResultStep>();
//        innerStep
//            .OnEvent("Complete")
//            .SendEventTo(new ProcessFunctionTargetBuilder(resultStep));

//        return process.Build();
//    }

//    #endregion

//    private sealed class InitialStep : KernelProcessStep
//    {
//        [KernelFunction("ProcessInit")]
//        public async ValueTask InitAsync(KernelProcessStepContext context, object values)
//        {
//            System.Console.WriteLine($"PROCESS INPUT: {FormatList(", ", values)}");
//            await context.EmitEventAsync(new() { Id = "Start", Data = values });
//        }
//    }

//    private sealed class BasicDiscreteStep : KernelProcessStep
//    {
//        [KernelFunction("DiscreteCompute")]
//        public async ValueTask ComputeAsync(KernelProcessStepContext context, long value)
//        {
//            System.Console.WriteLine($"DISCRETE INPUT: {value}");
//            long square = value * value;
//            System.Console.WriteLine($"DISCRETE OUTPUT: {square}");
//            await context.EmitEventAsync(new() { Id = "Complete", Data = square });
//        }
//    }

//    private sealed class DiscreteStep : KernelProcessStep
//    {
//        [KernelFunction("DiscreteCompute")]
//        public async ValueTask ComputeAsync(KernelProcessStepContext context, long value)
//        {
//            System.Console.WriteLine($"DISCRETE INPUT: {value}");
//            long square = value * value;
//            System.Console.WriteLine($"DISCRETE OUTPUT: {square}");
//            await context.EmitEventAsync(new() { Id = "CompleteSquare", Data = square });
//        }

//        [KernelFunction("DiscreteFork")]
//        public async ValueTask DiscreteForkAsync(KernelProcessStepContext context, long value)
//        {
//            System.Console.WriteLine($"DISCRETE INPUT: {value}");
//            long square = value * value;
//            long cube = square * value;
//            System.Console.WriteLine($"DISCRETE OUTPUT: {square}");
//            System.Console.WriteLine($"DISCRETE OUTPUT: {cube}");
//            await context.EmitEventAsync(new() { Id = "CompleteSquare", Data = square });
//            await context.EmitEventAsync(new() { Id = "CompleteCube", Data = cube });
//        }

//        [KernelFunction("DiscreteTransform")]
//        public async ValueTask TransformAsync(KernelProcessStepContext context, long value)
//        {
//            System.Console.WriteLine($"DISCRETE INPUT: {value}");
//            string transform = $"#{value}";
//            System.Console.WriteLine($"DISCRETE OUTPUT: {transform}");
//            await context.EmitEventAsync(new() { Id = "CompleteFormat", Data = transform });
//        }

//        [KernelFunction("DiscreteSubprocess")]
//        public async ValueTask ComputeVisibleAsync(KernelProcessStepContext context, long value)
//        {
//            System.Console.WriteLine($"DISCRETE INPUT: {value}");
//            long square = value * value;
//            System.Console.WriteLine($"DISCRETE OUTPUT: {square}");
//            await context.EmitEventAsync(new() { Id = "CompleteSquare", Data = square, Visibility = KernelProcessEventVisibility.Public }); // %%% VALIDATE WITH SUBPROCESS
//        }

//        [KernelFunction("DiscreteNoise")]
//        public async ValueTask NoiseAsync(KernelProcessStepContext context, long value)
//        {
//            System.Console.WriteLine($"DISCRETE NOISE: {value}");
//        }
//    }

//    private sealed class ArrayStep : KernelProcessStep
//    {
//        [KernelFunction("DiscreteCompute")]
//        public async ValueTask ComputeAsync(KernelProcessStepContext context, IEnumerable values)
//        {
//            System.Console.WriteLine($"ARRAY INPUT: {values.GetType().Name}");

//            int count = 0;
//            foreach (var value in values)
//            {
//                ++count;
//            }

//            Type inputType = values.GetType();
//            if (!inputType.HasElementType)
//            {
//                throw new KernelException("%%%"); // MESSAGE
//            }
//            Type elementType = inputType.GetElementType()!;
//            Array results = Array.CreateInstance(elementType, count);

//            int index = 0;
//            foreach (var value in values)
//            {
//                long num = (long)value;
//                results.SetValue(num * num, index);
//                ++index;
//            }

//            System.Console.WriteLine($"ARRAY OUTPUT: {results}");

//            await context.EmitEventAsync(new() { Id = "Complete", Data = results, Visibility = KernelProcessEventVisibility.Public });
//        }
//    }

//    private sealed class UnionStep : KernelProcessStep
//    {
//        [KernelFunction("UnionCompute")]
//        public async ValueTask ComputeAsync(KernelProcessStepContext context, IList<long> values)
//        {
//            System.Console.WriteLine($"UNION INPUT: {string.Join(", ", values)}");
//            long sum = values.Sum();
//            System.Console.WriteLine($"UNION OUTPUT: {sum}");
//            await context.EmitEventAsync(new() { Id = "Complete", Data = sum });
//        }

//        [KernelFunction("UnionTransform")]
//        public async ValueTask TransformAsync(KernelProcessStepContext context, IList<string> values)
//        {
//            System.Console.WriteLine($"UNION INPUT: {string.Join(", ", values)}");
//            string list = string.Join("/", values);
//            System.Console.WriteLine($"UNION OUTPUT: {list}");
//            await context.EmitEventAsync(new() { Id = "Complete", Data = list });
//        }
//    }

//    private sealed class ResultStep : KernelProcessStep
//    {
//        [KernelFunction("Display")]
//        public async ValueTask DisplayAsync(KernelProcessStepContext context, object value)
//        {
//            System.Console.WriteLine($"RESULT INPUT: {value}");
//        }
//    }

//    private static string FormatList(string separator, object values)
//    {
//        if (values is not IEnumerable enumerable)
//        {
//            return $"{values}";
//        }

//        StringBuilder builder = new();
//        foreach (object value in enumerable)
//        {
//            builder.Append(value);
//            builder.Append(separator);
//        }
//        builder.Length -= separator.Length;

//        return builder.ToString();
//    }
//}
