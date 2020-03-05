using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class WorldTime_Tests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void SimpleDeclare()
        {
            WorldTime wt = new WorldTime(1, 2, 3, 4, 5);

            Assert.AreEqual(1, wt.year);
            Assert.AreEqual(2, wt.month);
            Assert.AreEqual(3, wt.day);
            Assert.AreEqual(4, wt.hour);
            Assert.AreEqual(5, wt.minute);
        }
        [Test]
        public void Equals()
        {
            WorldTime one = new WorldTime(1, 1, 1, 1, 0);
            WorldTime two = new WorldTime(1, 1, 1, 1, 0);

            Assert.AreEqual(one, two);
        }
        [Test]
        public void IncrementMinute()
        {
            WorldTime wt = new WorldTime(1, 1, 1, 1, 0);
            WorldTime expected = new WorldTime(1, 1, 1, 1, 1);

            wt.Tick();

            Assert.AreEqual(expected, wt);

        }
        [Test]
        public void IncrementHour()
        {
            WorldTime wt = new WorldTime(1, 1, 1, 1, 0);
            WorldTime expected = new WorldTime(1, 1, 1, 2, 0);

            wt.AdvanceHour();

            Assert.AreEqual(expected, wt);
        }
        [Test]
        public void IncrementDay()
        {
            WorldTime wt = new WorldTime(1, 1, 1, 1, 0);
            WorldTime expected = new WorldTime(1, 1, 2, 1, 0);

            wt.AdvanceDay();

            Assert.AreEqual(expected, wt);
        }
        [Test]
        public void IncrementMonth()
        {
            WorldTime wt = new WorldTime(1, 1, 1, 1, 0);
            WorldTime expected = new WorldTime(1, 2, 1, 1, 0);

            wt.AdvanceMonth();

            Assert.AreEqual(expected, wt);
        }
        [Test]
        public void IncrementYear()
        {
            WorldTime wt = new WorldTime(1, 1, 1, 1, 0);
            WorldTime expected = new WorldTime(2, 1, 1, 1, 0);

            wt.AdvanceYear();

            Assert.AreEqual(expected, wt);
        }

        [Test]
        public void Increment60Minutes()
        {
            WorldTime wt = new WorldTime(1, 1, 1, 1, 0);
            WorldTime expected = new WorldTime(1, 1, 1, 2, 0);

            wt.Tick(60);

            Assert.AreEqual(expected, wt);

        }
        [Test]
        public void Increment24Hours()
        {
            WorldTime wt = new WorldTime(1, 1, 1, 1, 0);
            WorldTime expected = new WorldTime(1, 1, 2, 1, 0);

            wt.AdvanceHour(24);

            Assert.AreEqual(expected, wt);
        }
        [Test]
        public void Increment28Days()
        {
            WorldTime wt = new WorldTime(1, 1, 1, 1, 0);
            WorldTime expected = new WorldTime(1, 2, 1, 1, 0);

            wt.AdvanceDay(28);

            Assert.AreEqual(expected, wt);
        }
        [Test]
        public void Increment4Months()
        {
            WorldTime wt = new WorldTime(1, 1, 1, 1, 0);
            WorldTime expected = new WorldTime(2, 1, 1, 1, 0);

            wt.AdvanceMonth(4);

            Assert.AreEqual(expected, wt);
        }


        [Test]
        public void ComparisonYear()
        {
            WorldTime a = new WorldTime(1, 2, 1, 0, 0);
            WorldTime b = new WorldTime(2, 1, 1, 0, 0);

            Assert.IsTrue(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a > b);
            Assert.IsFalse(a >= b);
        }
        [Test]
        public void ComparisonMonth()
        {
            WorldTime a = new WorldTime(1, 1, 2, 0, 0);
            WorldTime b = new WorldTime(1, 2, 1, 0, 0);

            Assert.IsTrue(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a > b);
            Assert.IsFalse(a >= b);
        }
        [Test]
        public void ComparisonDay()
        {
            WorldTime a = new WorldTime(1, 1, 1, 1, 0);
            WorldTime b = new WorldTime(1, 1, 2, 0, 0);

            Assert.IsTrue(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a > b);
            Assert.IsFalse(a >= b);
        }
        [Test]
        public void ComparisonHour()
        {
            WorldTime a = new WorldTime(1, 1, 1, 0, 1);
            WorldTime b = new WorldTime(1, 1, 1, 1, 0);

            Assert.IsTrue(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a > b);
            Assert.IsFalse(a >= b);

        }
        [Test]
        public void ComparisonMinute()
        {
            WorldTime a = new WorldTime(1, 1, 1, 0, 0);
            WorldTime b = new WorldTime(1, 1, 1, 0, 1);

            Assert.IsTrue(a < b);
            Assert.IsTrue(a <= b);
            Assert.IsFalse(a > b);
            Assert.IsFalse(a >= b);
        }
    }
}
