// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information. 

namespace System.Reactive.Concurrency
{
    public static class SchedulerDefaults
    {
        private static IScheduler _timeBasedOperations;
        private static IScheduler _asyncConversions;

        internal static IScheduler ConstantTimeOperations => ImmediateScheduler.Instance;
        internal static IScheduler TailRecursion => ImmediateScheduler.Instance;
        internal static IScheduler Iteration => CurrentThreadScheduler.Instance;

        public static IScheduler TimeBasedOperations
        {
            get => _timeBasedOperations ??= DefaultScheduler.Instance;
            set => _timeBasedOperations = value;
        }

        public static IScheduler AsyncConversions
        {
            get => _asyncConversions ??= DefaultScheduler.Instance;
            set => _asyncConversions = value;
        }
    }
}
