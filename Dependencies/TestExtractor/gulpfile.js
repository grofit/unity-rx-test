const { src, dest } = require('gulp');
const replace = require('gulp-replace');

exports.default = () =>
    src([
        "../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/**/*.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/LicenseHeaderTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/Tests/AliasesTest.cs",
        "!../../../reactiveFork/Rx.NET/Source/tests/Tests.System.Reactive/obj/**/*.cs",
    ])
    .pipe(replace('[Fact]', '[Test]'))
    .pipe(replace('using Xunit;', 'using NUnit.Framework;'))
    .pipe(replace('Assert.Equal(', 'Assert.AreEqual('))
    .pipe(replace('Assert.NotEqual(', 'Assert.AreNotEqual('))
    .pipe(replace('Assert.Same(', 'Assert.AreSame('))
    .pipe(replace('Assert.NotSame(', 'Assert.AreNotSame('))
    .pipe(replace('Assert.Empty(', 'CollectionAssert.IsEmpty('))
    .pipe(dest("../../Assets/Tests/Tests.System.Reactive"));
