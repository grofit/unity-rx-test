const { src, dest } = require('gulp');
const replace = require('gulp-replace');
const filter = require('gulp-filter');

exports.default = () => {
    const threadPoolSchedulerReplacementFilter = filter([
        "**",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Concurrency/SchedulerTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Concurrency/ThreadPoolSchedulerTest.cs",
    ], {restore: true});
    return src([
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
    ])
    .pipe(replace('[Fact]', '[Test]'))
    .pipe(replace('using Xunit;', 'using NUnit.Framework;\r\nusing Rx.Unity.Tests.Helper;'))
    .pipe(replace('Assert.Equal(', 'XunitAssert.Equal('))
    .pipe(replace('Assert.NotEqual(', 'XunitAssert.NotEqual('))
    .pipe(replace('Assert.IsType<', 'XunitAssert.IsType<'))
    .pipe(replace('Assert.IsType(', 'XunitAssert.IsType('))
    .pipe(replace('Assert.Same(', 'Assert.AreSame('))
    .pipe(replace('Assert.NotSame(', 'Assert.AreNotSame('))
    .pipe(replace('Assert.Empty(', 'CollectionAssert.IsEmpty('))
    .pipe(replace('[Fact(Skip = "")]', '//[Test, Explicit]'))
    .pipe(replace(/\[Trait\([^\(]*\)\]/g, '// $&'))
    .pipe(threadPoolSchedulerReplacementFilter)
    .pipe(replace('ThreadPoolScheduler.Instance', 'Rx.Unity.Concurrency.ThreadPoolOnlyScheduler.Instance'))
    .pipe(threadPoolSchedulerReplacementFilter.restore)
    .pipe(replace('public class ', 'public partial class '))    
    .pipe(dest("../../Assets/Tests/Tests.System.Reactive"));
};
