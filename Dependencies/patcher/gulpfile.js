const { src, dest, parallel, task } = require("gulp");
const replace = require("gulp-replace");
const filter = require("gulp-filter");

class Tasks {
    patchRxMainCsProj() {
        return src("../reactive/Rx.NET/Source/src/System.Reactive/System.Reactive.csproj")
        .pipe(replace("<TargetFrameworks>netcoreapp3.1;netstandard2.0;net472;uap10.0.16299;net5.0;net5.0-windows10.0.19041</TargetFrameworks>", "<TargetFramework>netstandard2.0</TargetFramework><LangVersion>9</LangVersion><DefineConstants>NO_NULLABLE_ATTRIBUTES</DefineConstants>"))
        .pipe(replace("<SignAssembly>true</SignAssembly>", "<SignAssembly>false</SignAssembly>"))
        .pipe(dest((file) => file.base));
    }
    
    patchRxRemoveUnsupportedFeatures() {
        return src("../reactive/Rx.NET/Source/src/System.Reactive/TaskObservable.cs")
        .pipe(replace("[AsyncMethodBuilder(typeof(TaskObservableMethodBuilder<>))]", ""))
        .pipe(replace("<SignAssembly>true</SignAssembly>", "<SignAssembly>false</SignAssembly>"))
        .pipe(dest((file) => file.base));
    }

    testCode() {
        const threadPoolSchedulerReplacementFilter = filter([
            "**",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Concurrency/SchedulerTest.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Concurrency/ThreadPoolSchedulerTest.cs",
        ], {restore: true});
        return this.xunit2nunitTransformer(src([
            "../reactive/Rx.NET/Source/tests/Tests.System.Reactive/**/*.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Internal/HalfSerializerTest.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/LicenseHeaderTest.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/ArgumentValidationTest.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Aliases.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/AliasesTest.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/TaskLikeSupportTest.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/ObservableRemotingTest.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/ObservableSafetyTest.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/QbservableTest.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/QbservableExTest.cs",
            "!../reactive/Rx.NET/Source/tests/Tests.System.Reactive/obj/**/*.cs",
        ]))
        .pipe(threadPoolSchedulerReplacementFilter)
        .pipe(replace("ThreadPoolScheduler.Instance", "Rx.Unity.Concurrency.ThreadPoolOnlyScheduler.Instance"))
        .pipe(threadPoolSchedulerReplacementFilter.restore)
        .pipe(replace("public class ", "public partial class "))
        .pipe(dest("../Tests/Tests.System.Reactive"));
    }

    testingLibCode() {
        return this.xunit2nunitTransformer(src([
            "../reactive/Rx.NET/Source/src/Microsoft.Reactive.Testing/**/*.cs",
            "!../reactive/Rx.NET/Source/src/Microsoft.Reactive.Testing/obj/**/*.cs",
            "!../reactive/Rx.NET/Source/src/Microsoft.Reactive.Testing/Properties/AssemblyInfo.cs"
        ])).pipe(dest("../Tests/Microsoft.Reactive.Testing"));
    }

    rxDataTestCode() {
        return src([
            "../Rx.Data/src/Rx.Data.Tests/**/*.cs",
            "!../Rx.Data/src/Rx.Data.Tests/obj/**/*.cs",
        ]).pipe(dest("../Tests/Rx.Data.Tests"));
    }

    xunit2nunitTransformer(source) {
        return source
            .pipe(replace("using Xunit;", "using NUnit.Framework;\r\nusing Rx.Unity.Tests.Helper;"))
            .pipe(replace("Assert.Equal(", "XunitAssert.Equal("))
            .pipe(replace("Assert.NotEqual(", "XunitAssert.NotEqual("))
            .pipe(replace("Assert.IsType<", "XunitAssert.IsType<"))
            .pipe(replace("Assert.IsType(", "XunitAssert.IsType("))
            .pipe(replace("Assert.Same(", "Assert.AreSame("))
            .pipe(replace("Assert.NotSame(", "Assert.AreNotSame("))
            .pipe(replace("Assert.Empty(", "CollectionAssert.IsEmpty("))
            .pipe(replace(/\[Fact([^\]]+)?\]/g, "[Test]"))
            .pipe(replace(/\[Trait\([^\(]*\)\]/g, "// $&"))
            .pipe(replace(/\[Test[^.\w]([^\(]+(?<=\sasync\s)[^\(]+\s{1,}(\w+)\s{0,}\()/gm, "[Test]\r\n        public void $2_Sync() => $2().Wait();\r\n$1"));
    }
}

const tasks = new Tasks();
exports.generateRxProj = parallel(() => tasks.patchRxMainCsProj(), () => tasks.patchRxRemoveUnsupportedFeatures());
exports.generateTests = parallel(() => tasks.testCode(), () => tasks.testingLibCode(), () => tasks.rxDataTestCode());
