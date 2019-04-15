using System.Diagnostics;
using NUnit.Framework;

namespace ReactiveTests
{
    [TestFixture]
    public class AssemblyVersionTests
    {
        [Test]
        public void should_be_using_correct_assembly_versions()
        {
            var location = typeof(System.Reactive.Unit).Assembly.Location;
            var version = FileVersionInfo.GetVersionInfo(location).FileVersion;

            Assert.True(version.StartsWith("4.1.4"));
        }
    }
}