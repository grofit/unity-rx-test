// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT License.
// See the LICENSE file in the project root for more information.

using System.Reactive;
using NUnit.Framework;
using Rx.Unity.Tests.Helper;

namespace ReactiveTests.Tests
{
    public partial class PriorityQueueTest
    {
        [Test]
        public void Enqueue_dequeue()
        {
            var q = new PriorityQueue<int>();

            for (var i = 0; i < 16; i++)
            {
                XunitAssert.Equal(0, q.Count);

                q.Enqueue(i);

                XunitAssert.Equal(1, q.Count);
                XunitAssert.Equal(i, q.Peek());
                XunitAssert.Equal(1, q.Count);
                XunitAssert.Equal(i, q.Dequeue());
                XunitAssert.Equal(0, q.Count);
            }
        }

        [Test]
        public void Enqueue_all_dequeue_all()
        {
            var q = new PriorityQueue<int>();

            for (var i = 0; i < 33; i++)
            {
                q.Enqueue(i);
                XunitAssert.Equal(i + 1, q.Count);
            }

            XunitAssert.Equal(33, q.Count);

            for (var i = 0; i < 33; i++)
            {
                XunitAssert.Equal(33 - i, q.Count);
                XunitAssert.Equal(i, q.Peek());
                XunitAssert.Equal(i, q.Dequeue());
            }

            XunitAssert.Equal(0, q.Count);
        }

        [Test]
        public void Reverse_Enqueue_all_dequeue_all()
        {
            var q = new PriorityQueue<int>();

            for (var i = 32; i >= 0; i--)
            {
                q.Enqueue(i);
                XunitAssert.Equal(33 - i, q.Count);
            }

            XunitAssert.Equal(33, q.Count);

            for (var i = 0; i < 33; i++)
            {
                XunitAssert.Equal(33 - i, q.Count);
                XunitAssert.Equal(i, q.Peek());
                XunitAssert.Equal(i, q.Dequeue());
            }

            XunitAssert.Equal(0, q.Count);
        }

        [Test]
        public void Remove_from_middle()
        {
            var q = new PriorityQueue<int>();

            for (var i = 0; i < 33; i++)
            {
                q.Enqueue(i);
            }

            q.Remove(16);

            for (var i = 0; i < 16; i++)
            {
                XunitAssert.Equal(i, q.Dequeue());
            }

            for (var i = 16; i < 32; i++)
            {
                XunitAssert.Equal(i + 1, q.Dequeue());
            }
        }

        [Test]
        public void Repro_329()
        {
            var queue = new PriorityQueue<int>();

            queue.Enqueue(2);
            queue.Enqueue(1);
            queue.Enqueue(5);
            queue.Enqueue(2);

            XunitAssert.Equal(1, queue.Dequeue());
            XunitAssert.Equal(2, queue.Dequeue());
            XunitAssert.Equal(2, queue.Dequeue());
            XunitAssert.Equal(5, queue.Dequeue());
        }
    }
}
