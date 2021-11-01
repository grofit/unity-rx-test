// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System.Reactive;
using NUnit.Framework;
using Rx.Unity.Tests.Helper;

namespace ReactiveTests.Tests
{

    public partial class UnitTest
    {
        [Test]
        public void Unit()
        {
            var u1 = new Unit();
            var u2 = new Unit();
            Assert.True(u1.Equals(u2));
            Assert.False(u1.Equals(""));
            Assert.False(u1.Equals(null));
            Assert.True(u1 == u2);
            Assert.False(u1 != u2);
            XunitAssert.Equal(0, u1.GetHashCode());
            XunitAssert.Equal("()", u1.ToString());
        }
    }
}
