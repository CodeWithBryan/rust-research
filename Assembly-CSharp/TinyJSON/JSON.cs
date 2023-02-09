using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace TinyJSON
{
	// Token: 0x020009A0 RID: 2464
	public static class JSON
	{
		// Token: 0x06003A3C RID: 14908 RVA: 0x00157180 File Offset: 0x00155380
		public static Variant Load(string json)
		{
			if (json == null)
			{
				throw new ArgumentNullException("json");
			}
			return Decoder.Decode(json);
		}

		// Token: 0x06003A3D RID: 14909 RVA: 0x00157196 File Offset: 0x00155396
		public static string Dump(object data)
		{
			return JSON.Dump(data, EncodeOptions.None);
		}

		// Token: 0x06003A3E RID: 14910 RVA: 0x001571A0 File Offset: 0x001553A0
		public static string Dump(object data, EncodeOptions options)
		{
			if (data != null)
			{
				Type type = data.GetType();
				if (!type.IsEnum && !type.IsPrimitive && !type.IsArray)
				{
					foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
					{
						if (methodInfo.GetCustomAttributes(false).AnyOfType(typeof(BeforeEncode)) && methodInfo.GetParameters().Length == 0)
						{
							methodInfo.Invoke(data, null);
						}
					}
				}
			}
			return Encoder.Encode(data, options);
		}

		// Token: 0x06003A3F RID: 14911 RVA: 0x0015721B File Offset: 0x0015541B
		public static void MakeInto<T>(Variant data, out T item)
		{
			item = JSON.DecodeType<T>(data);
		}

		// Token: 0x06003A40 RID: 14912 RVA: 0x0015722C File Offset: 0x0015542C
		private static Type FindType(string fullName)
		{
			if (fullName == null)
			{
				return null;
			}
			Type type;
			if (JSON.typeCache.TryGetValue(fullName, out type))
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				type = assemblies[i].GetType(fullName);
				if (type != null)
				{
					JSON.typeCache.Add(fullName, type);
					return type;
				}
			}
			return null;
		}

		// Token: 0x06003A41 RID: 14913 RVA: 0x0015728C File Offset: 0x0015548C
		private static T DecodeType<T>(Variant data)
		{
			if (data == null)
			{
				return default(T);
			}
			Type type = typeof(T);
			if (type.IsEnum)
			{
				return (T)((object)Enum.Parse(type, data.ToString(CultureInfo.InvariantCulture)));
			}
			if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
			{
				return (T)((object)Convert.ChangeType(data, type));
			}
			if (type == typeof(Guid))
			{
				return (T)((object)new Guid(data.ToString(CultureInfo.InvariantCulture)));
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() == 1)
				{
					return (T)((object)JSON.decodeArrayMethod.MakeGenericMethod(new Type[]
					{
						type.GetElementType()
					}).Invoke(null, new object[]
					{
						data
					}));
				}
				ProxyArray proxyArray = data as ProxyArray;
				if (proxyArray == null)
				{
					throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
				}
				int[] array = new int[type.GetArrayRank()];
				if (!proxyArray.CanBeMultiRankArray(array))
				{
					throw new DecodeException("Error decoding multidimensional array; JSON data doesn't seem fit this structure.");
				}
				Type elementType = type.GetElementType();
				if (elementType == null)
				{
					throw new DecodeException("Array element type is expected to be not null, but it is.");
				}
				Array array2 = Array.CreateInstance(elementType, array);
				MethodInfo methodInfo = JSON.decodeMultiRankArrayMethod.MakeGenericMethod(new Type[]
				{
					elementType
				});
				try
				{
					methodInfo.Invoke(null, new object[]
					{
						proxyArray,
						array2,
						1,
						array
					});
				}
				catch (Exception innerException)
				{
					throw new DecodeException("Error decoding multidimensional array. Did you try to decode into an array of incompatible rank or element type?", innerException);
				}
				return (T)((object)Convert.ChangeType(array2, typeof(T)));
			}
			else
			{
				if (typeof(IList).IsAssignableFrom(type))
				{
					return (T)((object)JSON.decodeListMethod.MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[]
					{
						data
					}));
				}
				if (typeof(IDictionary).IsAssignableFrom(type))
				{
					return (T)((object)JSON.decodeDictionaryMethod.MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[]
					{
						data
					}));
				}
				ProxyObject proxyObject = data as ProxyObject;
				if (proxyObject == null)
				{
					throw new InvalidCastException("ProxyObject expected when decoding into '" + type.FullName + "'.");
				}
				string typeHint = proxyObject.TypeHint;
				T t;
				if (typeHint != null && typeHint != type.FullName)
				{
					Type type2 = JSON.FindType(typeHint);
					if (type2 == null)
					{
						throw new TypeLoadException("Could not load type '" + typeHint + "'.");
					}
					if (!type.IsAssignableFrom(type2))
					{
						throw new InvalidCastException(string.Concat(new string[]
						{
							"Cannot assign type '",
							typeHint,
							"' to type '",
							type.FullName,
							"'."
						}));
					}
					t = (T)((object)Activator.CreateInstance(type2));
					type = type2;
				}
				else
				{
					t = Activator.CreateInstance<T>();
				}
				foreach (KeyValuePair<string, Variant> keyValuePair in ((IEnumerable<KeyValuePair<string, Variant>>)((ProxyObject)data)))
				{
					FieldInfo fieldInfo = type.GetField(keyValuePair.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (fieldInfo == null)
					{
						foreach (FieldInfo fieldInfo2 in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
						{
							foreach (object obj in fieldInfo2.GetCustomAttributes(true))
							{
								if (JSON.decodeAliasAttrType.IsInstanceOfType(obj) && ((DecodeAlias)obj).Contains(keyValuePair.Key))
								{
									fieldInfo = fieldInfo2;
									break;
								}
							}
						}
					}
					if (fieldInfo != null)
					{
						bool flag = fieldInfo.IsPublic;
						foreach (object o in fieldInfo.GetCustomAttributes(true))
						{
							if (JSON.excludeAttrType.IsInstanceOfType(o))
							{
								flag = false;
							}
							if (JSON.includeAttrType.IsInstanceOfType(o))
							{
								flag = true;
							}
						}
						if (flag)
						{
							MethodInfo methodInfo2 = JSON.decodeTypeMethod.MakeGenericMethod(new Type[]
							{
								fieldInfo.FieldType
							});
							if (type.IsValueType)
							{
								object obj2 = t;
								fieldInfo.SetValue(obj2, methodInfo2.Invoke(null, new object[]
								{
									keyValuePair.Value
								}));
								t = (T)((object)obj2);
							}
							else
							{
								fieldInfo.SetValue(t, methodInfo2.Invoke(null, new object[]
								{
									keyValuePair.Value
								}));
							}
						}
					}
					PropertyInfo propertyInfo = type.GetProperty(keyValuePair.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (propertyInfo == null)
					{
						foreach (PropertyInfo propertyInfo2 in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
						{
							foreach (object obj3 in propertyInfo2.GetCustomAttributes(false))
							{
								if (JSON.decodeAliasAttrType.IsInstanceOfType(obj3) && ((DecodeAlias)obj3).Contains(keyValuePair.Key))
								{
									propertyInfo = propertyInfo2;
									break;
								}
							}
						}
					}
					if (propertyInfo != null && propertyInfo.CanWrite && propertyInfo.GetCustomAttributes(false).AnyOfType(JSON.includeAttrType))
					{
						MethodInfo methodInfo3 = JSON.decodeTypeMethod.MakeGenericMethod(new Type[]
						{
							propertyInfo.PropertyType
						});
						if (type.IsValueType)
						{
							object obj4 = t;
							propertyInfo.SetValue(obj4, methodInfo3.Invoke(null, new object[]
							{
								keyValuePair.Value
							}), null);
							t = (T)((object)obj4);
						}
						else
						{
							propertyInfo.SetValue(t, methodInfo3.Invoke(null, new object[]
							{
								keyValuePair.Value
							}), null);
						}
					}
				}
				foreach (MethodInfo methodInfo4 in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (methodInfo4.GetCustomAttributes(false).AnyOfType(typeof(AfterDecode)))
					{
						MethodBase methodBase = methodInfo4;
						object obj5 = t;
						object[] parameters;
						if (methodInfo4.GetParameters().Length != 0)
						{
							(parameters = new object[1])[0] = data;
						}
						else
						{
							parameters = null;
						}
						methodBase.Invoke(obj5, parameters);
					}
				}
				return t;
			}
		}

		// Token: 0x06003A42 RID: 14914 RVA: 0x001578E0 File Offset: 0x00155AE0
		private static List<T> DecodeList<T>(Variant data)
		{
			List<T> list = new List<T>();
			ProxyArray proxyArray = data as ProxyArray;
			if (proxyArray == null)
			{
				throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
			}
			foreach (Variant data2 in ((IEnumerable<Variant>)proxyArray))
			{
				list.Add(JSON.DecodeType<T>(data2));
			}
			return list;
		}

		// Token: 0x06003A43 RID: 14915 RVA: 0x00157948 File Offset: 0x00155B48
		private static Dictionary<TKey, TValue> DecodeDictionary<TKey, TValue>(Variant data)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			Type typeFromHandle = typeof(TKey);
			ProxyObject proxyObject = data as ProxyObject;
			if (proxyObject == null)
			{
				throw new DecodeException("Variant is expected to be a ProxyObject here, but it is not.");
			}
			foreach (KeyValuePair<string, Variant> keyValuePair in ((IEnumerable<KeyValuePair<string, Variant>>)proxyObject))
			{
				TKey key = (TKey)((object)(typeFromHandle.IsEnum ? Enum.Parse(typeFromHandle, keyValuePair.Key) : Convert.ChangeType(keyValuePair.Key, typeFromHandle)));
				TValue value = JSON.DecodeType<TValue>(keyValuePair.Value);
				dictionary.Add(key, value);
			}
			return dictionary;
		}

		// Token: 0x06003A44 RID: 14916 RVA: 0x001579F4 File Offset: 0x00155BF4
		private static T[] DecodeArray<T>(Variant data)
		{
			ProxyArray proxyArray = data as ProxyArray;
			if (proxyArray == null)
			{
				throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
			}
			T[] array = new T[proxyArray.Count];
			int num = 0;
			foreach (Variant data2 in ((IEnumerable<Variant>)proxyArray))
			{
				array[num++] = JSON.DecodeType<T>(data2);
			}
			return array;
		}

		// Token: 0x06003A45 RID: 14917 RVA: 0x00157A68 File Offset: 0x00155C68
		private static void DecodeMultiRankArray<T>(ProxyArray arrayData, Array array, int arrayRank, int[] indices)
		{
			int count = arrayData.Count;
			for (int i = 0; i < count; i++)
			{
				indices[arrayRank - 1] = i;
				if (arrayRank < array.Rank)
				{
					JSON.DecodeMultiRankArray<T>(arrayData[i] as ProxyArray, array, arrayRank + 1, indices);
				}
				else
				{
					array.SetValue(JSON.DecodeType<T>(arrayData[i]), indices);
				}
			}
		}

		// Token: 0x06003A46 RID: 14918 RVA: 0x00157AC8 File Offset: 0x00155CC8
		public static void SupportTypeForAOT<T>()
		{
			JSON.DecodeType<T>(null);
			JSON.DecodeList<T>(null);
			JSON.DecodeArray<T>(null);
			JSON.DecodeDictionary<short, T>(null);
			JSON.DecodeDictionary<ushort, T>(null);
			JSON.DecodeDictionary<int, T>(null);
			JSON.DecodeDictionary<uint, T>(null);
			JSON.DecodeDictionary<long, T>(null);
			JSON.DecodeDictionary<ulong, T>(null);
			JSON.DecodeDictionary<float, T>(null);
			JSON.DecodeDictionary<double, T>(null);
			JSON.DecodeDictionary<decimal, T>(null);
			JSON.DecodeDictionary<bool, T>(null);
			JSON.DecodeDictionary<string, T>(null);
		}

		// Token: 0x06003A47 RID: 14919 RVA: 0x00157B37 File Offset: 0x00155D37
		private static void SupportValueTypesForAOT()
		{
			JSON.SupportTypeForAOT<short>();
			JSON.SupportTypeForAOT<ushort>();
			JSON.SupportTypeForAOT<int>();
			JSON.SupportTypeForAOT<uint>();
			JSON.SupportTypeForAOT<long>();
			JSON.SupportTypeForAOT<ulong>();
			JSON.SupportTypeForAOT<float>();
			JSON.SupportTypeForAOT<double>();
			JSON.SupportTypeForAOT<decimal>();
			JSON.SupportTypeForAOT<bool>();
			JSON.SupportTypeForAOT<string>();
		}

		// Token: 0x040034C1 RID: 13505
		private static readonly Type includeAttrType = typeof(Include);

		// Token: 0x040034C2 RID: 13506
		private static readonly Type excludeAttrType = typeof(Exclude);

		// Token: 0x040034C3 RID: 13507
		private static readonly Type decodeAliasAttrType = typeof(DecodeAlias);

		// Token: 0x040034C4 RID: 13508
		private static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

		// Token: 0x040034C5 RID: 13509
		private const BindingFlags instanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		// Token: 0x040034C6 RID: 13510
		private const BindingFlags staticBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		// Token: 0x040034C7 RID: 13511
		private static readonly MethodInfo decodeTypeMethod = typeof(JSON).GetMethod("DecodeType", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x040034C8 RID: 13512
		private static readonly MethodInfo decodeListMethod = typeof(JSON).GetMethod("DecodeList", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x040034C9 RID: 13513
		private static readonly MethodInfo decodeDictionaryMethod = typeof(JSON).GetMethod("DecodeDictionary", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x040034CA RID: 13514
		private static readonly MethodInfo decodeArrayMethod = typeof(JSON).GetMethod("DecodeArray", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		// Token: 0x040034CB RID: 13515
		private static readonly MethodInfo decodeMultiRankArrayMethod = typeof(JSON).GetMethod("DecodeMultiRankArray", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
	}
}
