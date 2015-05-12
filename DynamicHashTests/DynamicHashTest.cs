using System;
using System.Collections.Generic;
using DynamicHash;
using NUnit.Framework;
using Is = NUnit.Framework.Is;

// ReSharper disable InconsistentNaming
namespace DynamicHashTests
{
    [TestFixture]
    public class DynamicHashTest
    {
        private dynamic d;

        [SetUp]
        public void SetUp()
        {
            d = DHash.New;
        }

        [Test]
        public void With_SetsString()
        {
            dynamic obj = DHash.New;

            obj.hai = "1";
            obj.WithNum(777).WithMoreStuff("hahah");

            string result = obj.MoreStuff;

            Console.Write(obj._json);

            Assert.That(result,Is.EqualTo("hahah"));
        }

        [Test]
        public void Set_Int_RetreiveAsInt()
        {
            d.thing = 1;

            Assert.That((int)d.thing, Is.EqualTo(1));
        }

        [Test]
        public void GetField_IntOnDHash_RetreiveAsInt()
        {
            d.thing = 1;

            Assert.That((d as DHash).GetField("thing"), Is.EqualTo(1));
        }


        [Test]
        public void GetField_IntOnDynamic_RetreiveAsInt()
        {
            d.thing = 1;

            Assert.That(d.GetField("thing"), Is.EqualTo(1));
        }

        [Test]
        public void GetField_MultipleProperties_RetreivesCorrectValue()
        {
            d.thing = 1;

            Assert.That(d.GetField("iDoNotExist", "thing"), Is.EqualTo(1));
        }

        [Test]
        public void GetField_MultiplePropertiesWhenNoneExist_ReturnsNull()
        {
            d.thing = 1;

            object result = d.GetField("iDoNotExist", "thing2");
            
            Assert.That(result, Is.EqualTo(null));
        }

        [Test]
        public void GetFieldTyped_MultiplePropertiesWhenNoneExist_ReturnsNull()
        {
            d.thing = "1";

            int result = d.GetField<int>("iDoNotExist", "thing");

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void Set_List_RetreiveAsInt()
        {
            d.thing = new List<int>{1,2,3};

            List<int> list = d.thing;

            CollectionAssert.AreEqual(list, new List<int>{1,2,3});
        }


        [Test]
        public void Get_ByStringKey()
        {
            d.duck = "quack";

            string result = d["duck"];

            Assert.That(result, Is.EqualTo("quack"));

        }

        [Test]
        public void Set_Guid_RetrieveAsString()
        {
            Guid guid = Guid.NewGuid();

            d.thing = guid;

            Assert.That((string)d.thing, Is.EqualTo(guid.ToString()));
        }


        [Test]
        public void Set_Double_RetrieveAsDouble()
        {
            d.thing = 123456789123456789123456789.123456789;

            Assert.That((double)d.thing, Is.EqualTo(123456789123456789123456789.123456789));
        }


        [Test]
        public void _json_WihDoubleValue()
        {
            d.thing = 123456789123456789123456789.123456789;

            string json = d._json;

            Assert.That(json,Is.EqualTo("{\"thing\":1.2345678912345679E+26}"));
        }

        [Test]
        public void FromJson_WihDoubleValue()
        {
            d = DHash.FromJson("{\"thing\":1.2345678912345679E+26}");

            Assert.That((double)d.thing, Is.EqualTo(123456789123456789123456789.123456789));
        }


        [Test]
        public void Set_GuidAsString_RetrieveAsGuid()
        {
            Guid guid = Guid.NewGuid();

            d.thing = guid.ToString();

            Assert.That((Guid)d.thing, Is.EqualTo(guid));
        }


        [Test]
        public void Set_Boolean_WorksFine()
        {
            d.isADuck = true;

            bool isADuck = d.isADuck;

            Assert.That(isADuck, Is.EqualTo(true));
        }

        [Test]
        public void Set_DateTime_RetrieveAsString()
        {
            var dt = new DateTime();

            d.thing = dt.ToString();

            string dtAsString = d.thing;
            Assert.That(dtAsString, Is.EqualTo(dt.ToString()));
        }

        [Test]
        public void Set_ArrayString_WorksFine()
        {
            d[0] = "string";

            string result = d[0];

            Assert.That(result, Is.EqualTo("string"));
        }


        [Test]
        public void Length_OnArrayWithOneItem_WorksFine()
        {
            Set_ArrayString_WorksFine();

            Assert.That(d.Length(), Is.EqualTo(1));
        }

        [Test]
        public void Length_OnArrayWithOneItemAtHighIndex_WorksFine()
        {
            d[123] = 1;

            Assert.That(d.Length(), Is.EqualTo(124));
        }

        [Test]
        public void Get_OnArrayIndexWhenNotSet_ReturnNull()
        {
            d[123] = 1;

            string result = d[10];

            Assert.That(result, Is.Null);
        }



        [Test]
        public void Get_OnNestedArray_CanAccessLength()
        {
            d.numbers[123] = 1;

            int result = d.numbers.Length();

            Assert.That(result, Is.EqualTo(124));
        }


        [Test]
        public void Set_PropertyAtArrayIndex()
        {
            d[0].duck = "quack";

            string result = d[0].duck;

            Assert.That(result, Is.EqualTo("quack"));
        }

        [Test]
        public void Set_NestedArray()
        {
            d[0][0] = "duck";

            string result = d[0][0];

            Assert.That(result, Is.EqualTo("duck"));
        }

        [Test]
        public void Get_OnUnsetNestedArrayValue()
        {
            string result = d[1][2][3];

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Set_ArrayAsString_RemovesArray()
        {
            d.duck[0] = "quack";

            d.duck = "quack";

            string result = d.duck[0];

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Set_StringAsArray_RemovesString()
        {
            d.duck = "quack";

            d.duck[0] = "quack";

            string result = d.duck;

            Assert.That(result, Is.Null);
        }
        [Test]
        public void Set_ArrayAsString_AddsString()
        {
            Set_ArrayAsString_RemovesArray();

            string result = d.duck;

            Assert.That(result, Is.EqualTo("quack"));
        }


        [Test]
        public void _names_ReturnsKeys()
        {
            d.duck = "quack";
            d.dog = "woof";

            CollectionAssert.AreEquivalent(d._names, new[] { "dog", "duck" });
        }

        [Test]
        public void Merge()
        {
            d.duck = "quack";
            dynamic e = DHash.New.WithDog("woof");

            d.Merge(e);

            Assert.That((string)d.Dog,Is.EqualTo("woof"));
        }

        [Test]
        public void Merge_WithClash()
        {
            d.duck = "quack";
            dynamic e = DHash.New.WithDuck("quack!");

            d.Merge(e);

            Assert.That((string)d.Duck, Is.EqualTo("quack!"));
        }

        [Test]
        public void FluentAPI_WorksFine()
        {
            d.duck = "quack";
            d.dog = "woof";

            d.WithDog("woof").WithDuck("quack");

            string result = d.Duck;

            Assert.That(result, Is.EqualTo("quack"));
        }


        [Test]
        public void _json_SimpleProperties()
        {
            d.duck = "quack";
            d.isADuck = true;
            d.age = 5;
            d.foo = null;

            Assert.That(d._json, Is.EqualTo(@"{""duck"":""quack"",""isADuck"":true,""age"":5,""foo"":null}"));
        }
        

        [Test]
        public void _json_ArrayWithKey()
        {
            d.array[0] = null;
            d.array[1] = "quack";
            d.array[2] = true;
            d.array[4] = 5;

            Assert.That(d._json, Is.EqualTo(@"{""array"":[null,""quack"",true,null,5]}"));
        }

        [Test]
        public void Get_ObjectNestedInArray()
        {
            d.array[0].age = 5;

            int result = d.array[0].age;

            Assert.That(result,Is.EqualTo(5));
        }

        [Test]
        public void _json_ObjectNestedInArray()
        {
            d.array[0].duck = "quack";
            d.array[0].isADuck = true;
            d.array[0].age = 5;
            d.array[0].foo = null;

            Assert.That(d._json, Is.EqualTo(@"{""array"":[{""duck"":""quack"",""isADuck"":true,""age"":5,""foo"":null}]}"));
        }

        [Test]
        public void _json_NestedObject()
        {
            d.i.am.a.duck = "quack";

            Assert.That(d._json, Is.EqualTo(@"{""i"":{""am"":{""a"":{""duck"":""quack""}}}}"));
        }
    }
}