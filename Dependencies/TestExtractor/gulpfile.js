const { src, dest } = require('gulp');
const replace = require('gulp-replace');

exports.default = () =>
    src([
        "../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/**/*.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Internal/HalfSerializerTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/LicenseHeaderTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/ArgumentValidationTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Aliases.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/AliasesTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/ObservableRemotingTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/ObservableSafetyTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/QbservableTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/Linq/QbservableExTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/obj/**/*.cs",
    ])
    .pipe(replace('[Fact]', '[Test]'))
    .pipe(replace('using Xunit;', 'using NUnit.Framework;'))
    .pipe(replace('Assert.Equal(', 'Assert.AreEqual('))
    .pipe(replace('Assert.NotEqual(', 'Assert.AreNotEqual('))
    .pipe(replace('Assert.Same(', 'Assert.AreSame('))
    .pipe(replace('Assert.NotSame(', 'Assert.AreNotSame('))
    .pipe(replace('Assert.Empty(', 'CollectionAssert.IsEmpty('))
    .pipe(replace('[Fact(Skip = "")]', '//[Test, Explicit]'))
    .pipe(replace('ThreadPoolScheduler.Instance', 'Rx.Unity.Concurrency.ThreadPoolOnlyScheduler.Instance'))
    .pipe(replace(/\[Trait\([^\(]*\)\]/g, '// $&'))
    
    .pipe(dest("../../Assets/Tests/Tests.System.Reactive"));
