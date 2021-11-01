const { src, dest, parallel, task } = require('gulp');
const replace = require('gulp-replace');
const filter = require('gulp-filter');

class Tasks {
    xunit2nunitTransformer(source) {
        return source
            .pipe(replace('using Xunit;', 'using NUnit.Framework;\r\nusing Rx.Unity.Tests.Helper;'))
            .pipe(replace('Assert.Equal(', 'XunitAssert.Equal('))
            .pipe(replace('Assert.NotEqual(', 'XunitAssert.NotEqual('))
            .pipe(replace('Assert.IsType<', 'XunitAssert.IsType<'))
            .pipe(replace('Assert.IsType(', 'XunitAssert.IsType('))
            .pipe(replace('Assert.Same(', 'Assert.AreSame('))
            .pipe(replace('Assert.NotSame(', 'Assert.AreNotSame('))
            .pipe(replace('Assert.Empty(', 'CollectionAssert.IsEmpty('))
            .pipe(replace(/\[Fact([^\]]+)?\]/g, '[Test]'))
            .pipe(replace(/\[Trait\([^\(]*\)\]/g, '// $&'))
            .pipe(replace(/\[Test[^.\w]([^\(]+(?<=\sasync\s)[^\(]+\s{1,}(\w+)\s{0,}\()/gm, '[Test]\r\n        public void $2_Sync() => $2().Wait();\r\n$1'));
    }

    testCode() {
        const threadPoolSchedulerReplacementFilter = filter([
            "**",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Concurrency/SchedulerTest.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Concurrency/ThreadPoolSchedulerTest.cs",
        ], {restore: true});
        return this.xunit2nunitTransformer(src([
            "../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/**/*.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Internal/HalfSerializerTest.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/LicenseHeaderTest.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/ArgumentValidationTest.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Aliases.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/AliasesTest.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/TaskLikeSupportTest.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/ObservableRemotingTest.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/ObservableSafetyTest.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/QbservableTest.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/QbservableExTest.cs",
            "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/obj/**/*.cs",
        ]))
        .pipe(threadPoolSchedulerReplacementFilter)
        .pipe(replace('ThreadPoolScheduler.Instance', 'Rx.Unity.Concurrency.ThreadPoolOnlyScheduler.Instance'))
        .pipe(threadPoolSchedulerReplacementFilter.restore)
        .pipe(replace('public class ', 'public partial class '))
        .pipe(dest("../../Assets/Tests/Tests.System.Reactive"));
    }

    testingLibCode() {
        return this.xunit2nunitTransformer(src([
            "../../../reactiveFork/Rx.NET/Source/src/Microsoft.Reactive.Testing/**/*.cs",
            "!../../../reactiveFork/Rx.NET/Source/src/Microsoft.Reactive.Testing/obj/**/*.cs",
            "!../../../reactiveFork/Rx.NET/Source/src/Microsoft.Reactive.Testing/Properties/AssemblyInfo.cs"
        ])).pipe(dest("../../Assets/Tests/Microsoft.Reactive.Testing"));
    }
}

const tasks = new Tasks();
exports.default = parallel(() => tasks.testCode(), () => tasks.testingLibCode());
