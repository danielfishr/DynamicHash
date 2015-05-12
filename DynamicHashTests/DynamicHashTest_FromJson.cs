using DynamicHash;
using NUnit.Framework;
using Is = NUnit.Framework.Is;

// ReSharper disable InconsistentNaming
namespace DynamicHashTests
{
    [TestFixture]
    public class DynamicHashTest_FromJson
    {
        private dynamic d;

        [SetUp]
        public void SetUp()
        {
            string json = @"{""duck"":""quack"",""isADuck"":true,""age"":5,""foo"":null,""bar"":{baz:1},""numbers"":[0,1,3],""strs"":[""c"",""d""]}";

            d = DHash.FromJson(json);
        }

        [Test]
        public void FromJson_CanSetBool()
        {
            Assert.That((bool)d.isADuck, Is.EqualTo(true));
        }

        [Test]
        public void FromJson_ArrayAccess()
        {
            Assert.That((object)d.numbers, Is.Not.Null);
        }


        [Test]
        public void FromJson_ArrayAccess2()
        {
            int[] numbers = d.numbers;

            CollectionAssert.AreEquivalent(numbers,new []{0,1,3});
        }

        [Test]
        public void FromJson_StringArrayAccess()
        {
            string[] strs = d.strs;

            CollectionAssert.AreEquivalent(strs, new[] { "c", "d" });
        }
        [Test]
        public void FromJson_ArrayCount()
        {
            Assert.That((int)d.numbers.Length(), Is.EqualTo(3));
        }

        [Test]
        public void FromJson_CanSetInt()
        {
            Assert.That((int)d.age, Is.EqualTo(5));
        }

        [Test]
        public void FromJson_CanSetString()
        {
            Assert.That((string)d.duck, Is.EqualTo("quack"));
        }

        [Test]
        public void FromJson_NestedObjected()
        {
            Assert.That((int)d.bar.baz, Is.EqualTo(1));
        }

        [Test]
        public void FromJson_CanAssignNull()
        {
            Assert.Null((string)d.foo);
        }

    }
}