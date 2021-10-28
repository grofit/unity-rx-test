// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace ReactiveTests.Tests
{

    public partial class PrivateTypesTest : ReactiveTest
    {
        [Test]
        public void EitherValueRoundtrip()
        {
            {
                var value = 42;
                var left = (Either<int, string>.Left)Either<int, string>.CreateLeft(value);
                Assert.AreEqual(value, left.Value);
            }
            {
                var value = "42";
                var right = (Either<int, string>.Right)Either<int, string>.CreateRight(value);
                Assert.AreEqual(value, right.Value);
            }
        }

        [Test]
        public void EitherEqualsEquatable()
        {
            {
                var value = 42;
                var left = (Either<int, string>.Left)Either<int, string>.CreateLeft(value);

                Assert.True(left.Equals(left));
                Assert.False(left.Equals(null));

                var other = (Either<int, string>.Left)Either<int, string>.CreateLeft(value + 1);
                Assert.False(left.Equals(other));
            }
            {
                var value = "42";
                var right = (Either<int, string>.Right)Either<int, string>.CreateRight(value);

                Assert.True(right.Equals(right));
                Assert.False(right.Equals(null));

                var other = (Either<int, string>.Right)Either<int, string>.CreateRight(value + "1");
                Assert.False(right.Equals(other));
            }
        }

        [Test]
        public void EitherEqualsObject()
        {
            {
                var value = 42;
                var left = (Either<int, string>.Left)Either<int, string>.CreateLeft(value);

                Assert.True(left.Equals((object)left));
                Assert.False(left.Equals((object)null));

                var other = (Either<int, string>.Left)Either<int, string>.CreateLeft(value + 1);
                Assert.False(left.Equals((object)other));
            }
            {
                var value = "42";
                var right = (Either<int, string>.Right)Either<int, string>.CreateRight(value);

                Assert.True(right.Equals((object)right));
                Assert.False(right.Equals((object)null));

                var other = (Either<int, string>.Right)Either<int, string>.CreateRight(value + "1");
                Assert.False(right.Equals((object)other));
            }
        }

        [Test]
        public void EitherGetHashCode()
        {
            {
                var left = (Either<int, string>.Left)Either<int, string>.CreateLeft(42);
                var other = (Either<int, string>.Left)Either<int, string>.CreateLeft(43);
                Assert.AreNotEqual(left.GetHashCode(), other.GetHashCode());
            }
            {
                var right = (Either<int, string>.Right)Either<int, string>.CreateRight("42");
                var other = (Either<int, string>.Right)Either<int, string>.CreateRight("43");
                Assert.AreNotEqual(right.GetHashCode(), other.GetHashCode());
            }
        }

        [Test]
        public void EitherToString()
        {
            {
                var left = (Either<int, string>.Left)Either<int, string>.CreateLeft(42);
                Assert.True(left.ToString() == "Left(42)");
            }
            {
                var right = (Either<int, string>.Right)Either<int, string>.CreateRight("42");
                Assert.True(right.ToString() == "Right(42)");
            }
        }

        [Test]
        public void EitherSwitchFunc()
        {
            {
                var value = 42;
                var left = (Either<int, string>.Left)Either<int, string>.CreateLeft(value);
                Assert.AreEqual(left.Switch(l => l, r => r.Length), value);
            }
            {
                var value = "42";
                var right = (Either<int, string>.Right)Either<int, string>.CreateRight(value);
                Assert.AreEqual(right.Switch(l => l, r => r.Length), value.Length);
            }
        }

        [Test]
        public void EitherSwitchAction()
        {
            {
                var value = 42;
                var left = (Either<int, string>.Left)Either<int, string>.CreateLeft(value);
                var res = 0;
                left.Switch(l => { res = 1; }, r => { res = 2; });
                Assert.AreEqual(1, res);
            }
            {
                var value = "42";
                var right = (Either<int, string>.Right)Either<int, string>.CreateRight(value);
                var res = 0;
                right.Switch(l => { res = 1; }, r => { res = 2; });
                Assert.AreEqual(2, res);
            }
        }
    }

    internal class EitherBase
    {
        protected object _value;

        public override bool Equals(object obj)
        {
            var equ = _value.GetType().GetMethods().Where(m => m.Name == "Equals" && m.GetParameters()[0].ParameterType == typeof(object)).Single();
            return (bool)equ.Invoke(_value, new object[] { obj is EitherBase ? ((EitherBase)obj)._value : obj });
        }

        public override int GetHashCode()
        {
            return (int)_value.GetType().GetMethod(nameof(GetHashCode)).Invoke(_value, null);
        }

        public override string ToString()
        {
            return (string)_value.GetType().GetMethod(nameof(ToString)).Invoke(_value, null);
        }
    }

    internal class Either<TLeft, TRight> : EitherBase
    {
        public static Either<TLeft, TRight> CreateLeft(TLeft value)
        {
            return new Left(System.Reactive.Either<TLeft, TRight>.CreateLeft(value));
        }

        public static Either<TLeft, TRight> CreateRight(TRight value)
        {
            return new Right(System.Reactive.Either<TLeft, TRight>.CreateRight(value));
        }
        
        public TResult Switch<TResult>(Func<TLeft, TResult> caseLeft, Func<TRight, TResult> caseRight)
        {
            return _value switch
            {
                System.Reactive.Either<TLeft, TRight>.Left left => left.Switch(caseLeft, caseRight),
                System.Reactive.Either<TLeft, TRight>.Right right => right.Switch(caseLeft, caseRight),
                _ => throw new InvalidOperationException($"This instance was created using an unsupported type {_value.GetType()} for a {nameof(_value)}"),
            };

            //var mth = _value.GetType().GetMethods().Where(m => m.Name == nameof(Switch) && m.ReturnType != typeof(void)).Single().MakeGenericMethod(typeof(TResult));
            //return (TResult)mth.Invoke(_value, new object[] { caseLeft, caseRight });
        }

        public void Switch(Action<TLeft> caseLeft, Action<TRight> caseRight)
        {
            switch (_value)
            {
                case System.Reactive.Either<TLeft, TRight>.Left left:
                    left.Switch(caseLeft, caseRight);
                    break;

                case System.Reactive.Either<TLeft, TRight>.Right right:
                    right.Switch(caseLeft, caseRight);
                    break;

                default:
                    throw new InvalidOperationException($"This instance was created using an unsupported type {_value.GetType()} for a {nameof(_value)}");
            }

            //var mth = _value.GetType().GetMethods().Where(m => m.Name == nameof(Switch) && m.ReturnType == typeof(void)).Single();
            //mth.Invoke(_value, new object[] { caseLeft, caseRight });
        }

        public sealed class Left : Either<TLeft, TRight>, IEquatable<Left>
        {
            public TLeft Value
            {
                get
                {
                    return (TLeft)_value.GetType().GetProperty(nameof(Value)).GetValue(_value, null);
                }
            }

            public Left(System.Reactive.Either<TLeft, TRight> value)
            {
                _value = value;
            }

            public bool Equals(Left other)
            {
                var equ = _value.GetType().GetMethods().Where(m => m.Name == nameof(Equals) && m.GetParameters()[0].ParameterType != typeof(object)).Single();
                return (bool)equ.Invoke(_value, new object[] { other?._value });
            }
        }

        public sealed class Right : Either<TLeft, TRight>, IEquatable<Right>
        {
            public TRight Value
            {
                get
                {
                    return (TRight)_value.GetType().GetProperty(nameof(Value)).GetValue(_value, null);
                }
            }

            public Right(System.Reactive.Either<TLeft, TRight> value)
            {
                _value = value;
            }

            public bool Equals(Right other)
            {
                var equ = _value.GetType().GetMethods().Where(m => m.Name == nameof(Equals) && m.GetParameters()[0].ParameterType != typeof(object)).Single();
                return (bool)equ.Invoke(_value, new object[] { other?._value });
            }
        }
    }
}
