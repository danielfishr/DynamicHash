using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;

namespace DynamicHash
{
    using Newtonsoft.Json.Linq;

    [Serializable]
    public class DHash : DynamicObject
    {
        // ReSharper disable InconsistentNaming
        private readonly List<string> __names = new List<string>();
        // ReSharper restore InconsistentNaming
        private object _value;
        private Dictionary<string, object> _arrayEntries = new Dictionary<string, object>();
        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        private DHash()
        {
        }

        public DHash(object value)
        {
            _value = value;
			if (value is JArray)
            {
                var values = value as JArray;
                for(int i =0;i<values.Count;i++)
                {
                    _arrayEntries.Add(i.ToString(),values[i]);
                }
            }
        }

        public object GetField(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (_properties.ContainsKey(propertyName))
                {
                    return (_properties[propertyName] as DHash)._value;
                }
            }
            return null;
        }

        public T GetField<T>(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (_properties.ContainsKey(propertyName))
                {
                    object result;
                    if ((_properties[propertyName] as DHash).TryConvert(out result, typeof(T)))
                    {
                        return (T) result;
                    }
                }
            }
            return default(T);
        }

        public static dynamic New
        {
            get { return new DHash(); }
        }

        // ReSharper disable InconsistentNaming - want to minimize the chance of clashing with names in client code
        public string _json// ReSharper restore InconsistentNaming
        {
            get
            {
                var jsonDictionary = new Dictionary<string, object>();
                jsonDictionary = ToJsonDictionary(jsonDictionary) as Dictionary<string, object>;
                string json = JsonConvert.SerializeObject(jsonDictionary);
                return json;
            }
        }

        // ReSharper disable InconsistentNamingi - want to minimize the chance of clashing with names in client code
        public List<string> _names// ReSharper restore InconsistentNaming
        {
            get { return __names; }
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_properties.ContainsKey(binder.Name))
            {
                result = _properties[binder.Name];
                return true;
            }
            AddDHash(binder.Name);
            result = _properties[binder.Name];
            return true;
        }

        public DHash AddDHash(string name)
        {
            var dhash = new DHash();
            __names.Add(name);
            _properties[name] = dhash;
            return dhash;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            Type type = binder.Type;
            // Converting to string.  
            if (TryConvert(out result, type)) return true;

            return base.TryConvert(binder, out result);
        }

        private bool TryConvert(out object result, Type type)
        {
            result = null;
            if (type == typeof (String))
            {
                if (!_properties.Any() && !_arrayEntries.Any() && _value == null)
                {
                    result = null;
                    return true;
                }
            }

            // Converting to string.  
            if (type == typeof (String))
            {
                if (_value == null)
                {
                    result = null;
                    return true;
                }
                result = _value.ToString();
                return true;
            }
            if (type == typeof (bool))
            {
                result = Convert.ToBoolean(_value);
                return true;
            }
            if (type == typeof (int))
            {
                result = Convert.ToInt32(_value);
                return true;
            }
            if (type == typeof (double))
            {
                result = Convert.ToDouble(_value);
                return true;
            }

            if (type == typeof (List<int>))
            {
                result = _value;
                return true;
            }

            if (type == typeof(int[]))
            {
                if (_value is JArray)
                {
                    var array = _value as JArray;
					List<int> ints = new List<int>();
                    foreach (var val in array)
                    {
                        ints.Add((int) val);
                    }
                    result = ints.ToArray();
					return true;
                }
                result = _value;
                return true;
            }

            if (type == typeof(string[]))
            {
                if (_value is JArray)
                {
                    var array = _value as JArray;
                    var strings = new List<string>();
                    foreach (var val in array)
                    {
                        strings.Add(val.ToString());
                    }
                    result = strings.ToArray();
                    return true;
                }
                result = _value;
                return true;
            }

            if (type == typeof (Guid))
            {
                result = _value;
                if (result is string)
                {
                    result = Guid.Parse((string) result);
                }
                return true;
            }
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            _value = null;
            _properties = new Dictionary<string, object>();
            _arrayEntries[indexes[0].ToString()] = value;
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (!_arrayEntries.ContainsKey(indexes[0].ToString()) && Is.AnInt(indexes[0]) && _value == null)
            {
                _arrayEntries[indexes[0].ToString()] = new DHash();
                result = _arrayEntries[indexes[0].ToString()];
                return true;
            }
            if (_arrayEntries.ContainsKey(indexes[0].ToString()) && Is.AnInt(indexes[0]))
            {
                result = _arrayEntries[indexes[0].ToString()];
                return true;
            }
            if (_properties.ContainsKey(indexes[0].ToString()) && !Is.AnInt(indexes[0]))
            {
                result = _properties[indexes[0].ToString()];
                return true;
            }
            result = null;
            return true;
        }


        public static dynamic FromJson(string json)
        {
            IDictionary<string, object> jsonDictionary = new Dictionary<string, object>();
            jsonDictionary = JsonConvert.DeserializeObject<IDictionary<string, object>>(
                json, new JsonConverter[] {new MyConverter()});
            return FromJsonDictionary(jsonDictionary);
        }

        private static dynamic FromJsonDictionary(IDictionary<string, object> jsonDictionary)
        {
            var dhash = new DHash();
            return FromJsonDictionary(dhash, jsonDictionary);
        }

        private static DHash FromJsonDictionary(DHash dhash, IDictionary<string, object> jsonDictionary)
        {
            foreach (var o in jsonDictionary)
            {
                if (o.Value is IDictionary<string, object>)
                {
                    DHash nested = dhash.AddDHash(o.Key);
                    FromJsonDictionary(nested, o.Value as IDictionary<string, object>);
                }
                else
                {
                    dhash.TrySetMember(o.Key, o.Value);
                }
            }
            return dhash;
        }

        private object ToJsonDictionary(Dictionary<string, object> jsonDictionary)
        {
            if (_properties.Any())
            {
                foreach (var property in _properties)
                {
                    jsonDictionary.Add(property.Key, ToJsonDictionary(property.Value));
                }
                return jsonDictionary;
            }
            if (_arrayEntries.Any())
            {
                int maxKey = _arrayEntries.Select(x => Convert.ToInt32(x.Key)).Max();
                var list = new List<object>();
                for (int i = 0; i <= maxKey; i++)
                {
                    if (_arrayEntries.ContainsKey(i.ToString()))
                    {
                        list.Add(ToJsonDictionary(_arrayEntries[i.ToString()]));
                    }
                    else
                    {
                        list.Add(null);
                    }
                }
                return list.ToArray();
            }
            if (_value != null)
            {
                return _value;
            }

            return null;
        }

        private object ToJsonDictionary(object obj)
        {
            var jsonDictionary = new Dictionary<string, object>();
            if (obj is DHash)
            {
                var dhash = obj as DHash;

                if (dhash._properties.Any())
                {
                    foreach (var property in dhash._properties)
                    {
                        jsonDictionary.Add(property.Key, ToJsonDictionary(property.Value));
                    }
                    return jsonDictionary;
                }
                if (dhash._arrayEntries.Any())
                {
                    int maxKey = dhash._arrayEntries.Select(x => Convert.ToInt32(x.Key)).Max();
                    var list = new List<object>();
                    for (int i = 0; i <= maxKey; i++)
                    {
                        if (dhash._arrayEntries.ContainsKey(i.ToString()))
                        {
                            list.Add(ToJsonDictionary(dhash._arrayEntries[i.ToString()]));
                        }
                        else
                        {
                            list.Add(null);
                        }
                    }
                    return list.ToArray();
                }
                return dhash._value;
            }
            return obj;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return TrySetMember(binder.Name, value);
        }

        private bool TrySetMember(string name, object value)
        {
            __names.Add(name);
            _arrayEntries = new Dictionary<string, object>();
            _properties[name] = new DHash(value);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.Name == "Length")
            {
                result = _arrayEntries.ToList().ConvertAll(x => Convert.ToInt32(x.Key)).Max() + 1;
                return true;
            }
            if (binder.Name.StartsWith("With"))
            {
                string memberName = binder.Name.Replace("With", string.Empty);
                if (TrySetMember(memberName, args[0]))
                {
                    result = this;
                    return true;
                }
            }
            if (binder.Name == "ContainsKey")
            {
                string key = (string) args[0];
                result = __names.Contains(key);
                return true;
            }
            if (binder.Name == "Merge")
            {
                foreach (DHash arg in args)
                {
                    Merge(_properties,arg._properties);
                    Merge(_properties,arg._arrayEntries);
                }
                result = null;
                return true;
            }
            dynamic method = _properties[binder.Name];
            result = method(args[0].ToString(), args[1].ToString());
            return true;
        }

        private void Merge(Dictionary<string, object> dictionary1, Dictionary<string, object> dictionary2)
        {
            foreach (var item in dictionary2)
            {
                if (dictionary1.ContainsKey(item.Key))
                {
                    dictionary1[item.Key] = item.Value;
                }
                else
                {
                    dictionary1.Add(item.Key, item.Value);
                }
            }
        }

        internal static dynamic Array(int count)
        {
            return new DHash[count].ToList().ConvertAll(x => new DHash()).ToArray();
        }
    }
}