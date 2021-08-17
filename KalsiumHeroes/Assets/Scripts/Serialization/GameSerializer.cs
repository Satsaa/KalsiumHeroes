
namespace Serialization {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Object = UnityEngine.Object;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Newtonsoft.Json.Serialization;
	using System.Reflection;
	using System.IO;
	using Muc.Extensions;
	using System.Collections;

	public static class GameSerializer {

		public static string Serialize(Game game) {
			return SerializeGame(game);
		}

		public static void Deserialize(string json, Game target) {
			DeserializeGame(json, target);
		}

		public delegate JToken Tokenizer(object obj);
		public delegate object Detokenizer(Type type, JToken j, object obj);

		class Converter {
			public Tokenizer tokenizer;
			public Detokenizer detokenizer;

			public Converter(Tokenizer serializer, Detokenizer detokenizer) {
				this.tokenizer = serializer;
				this.detokenizer = detokenizer;
			}
		}

		static Dictionary<Type, Converter> unityTypes = new() {
			{
				typeof(Vector2),
				new(
					(o) => { var v = (Vector2)o; return new JArray(v.x, v.y); },
					(t, j, o) => { var a = (JArray)j; return new Vector2((float)a[0], (float)a[1]); }
				)
			},
			{
				typeof(Vector3),
				new(
					(o) => { var v = (Vector3)o; return new JArray(v.x, v.y, v.z); },
					(t, j, o) => { var a = (JArray)j; return new Vector3((float)a[0], (float)a[1], (float)a[2]); }
				)
			},
			{
				typeof(Vector4),
				new(
					(o) => { var v = (Vector4)o; return new JArray(v.x, v.y, v.z, v.w); },
					(t, j, o) => { var a = (JArray)j; return new Vector4((float)a[0], (float)a[1], (float)a[2], (float)a[3]); }
				)
			},
			{
				typeof(Vector2Int),
				new(
					(o) => { var v = (Vector2Int)o; return new JArray(v.x, v.y); },
					(t, j, o) => { var a = (JArray)j; return new Vector2Int((int)a[0], (int)a[1]); }
				)
			},
			{
				typeof(Vector3Int),
				new(
					(o) => { var v = (Vector3Int)o; return new JArray(v.x, v.y, v.z); },
					(t, j, o) => { var a = (JArray)j; return new Vector3Int((int)a[0], (int)a[1], (int)a[2]); }
				)
			},
			{
				typeof(Rect),
				new(
					(o) => { var v = (Rect)o; return new JArray(v.x, v.y, v.width, v.height); },
					(t, j, o) => { var a = (JArray)j; return new Rect((float)a[0], (float)a[1], (float)a[2], (float)a[3]); }
				)
			},
			{
				typeof(RectInt),
				new(
					(o) => { var v = (RectInt)o; return new JArray(v.x, v.y, v.width, v.height); },
					(t, j, o) => { var a = (JArray)j; return new RectInt((int)a[0], (int)a[1], (int)a[2], (int)a[3]); }
				)
			},
			{
				typeof(Quaternion),
				new(
					(o) => { var v = (Quaternion)o; return new JArray(v.x, v.y, v.z, v.w); },
					(t, j, o) => { var a = (JArray)j; return new Quaternion((float)a[0], (float)a[1], (float)a[2], (float)a[3]); }
				)
			},
			{
				typeof(Color),
				new(
					(o) => { var v = (Color)o; return new JArray(v.r, v.g, v.b, v.a); },
					(t, j, o) => { var a = (JArray)j; return new Color((float)a[0], (float)a[1], (float)a[2], (float)a[3]); }
				)
			},
			{
				typeof(Color32),
				new(
					(o) => { var v = (Color32)o; return new JArray(v.r, v.g, v.b, v.a); },
					(t, j, o) => { var a = (JArray)j; return new Color32((byte)a[0], (byte)a[1], (byte)a[2], (byte)a[3]); }
				)
			},
			{
				typeof(LayerMask),
				new(
					(o) => { var v = (LayerMask)o; return new JValue(v.value); },
					(t, j, o) => { var v = (JValue)j; return (LayerMask)(int)v; }
				)
			},
			{
				typeof(RectOffset),
				new(
					(o) => { var v = (RectOffset)o; return new JArray(v.left, v.right, v.top, v.bottom); },
					(t, j, o) => { var a = (JArray)j; return new RectOffset((int)a[0], (int)a[1], (int)a[2], (int)a[3]); }
				)
			},
			{
				typeof(AnimationCurve),
				new(
					null,
					null
				)
			},
			{
				typeof(Matrix4x4),
				new(
					null,
					null
				)
			},
			{
				typeof(Gradient),
				new(
					null,
					null
				)
			},
		};

		static void DeserializeGame(string json, Game game) {
			using (new Muc.Stopwatch("Deserialization took")) {
				var objs = new Dictionary<uint, IGameSerializable>() { { 1, game } };
				var cbList = new HashSet<ISerializationCallbackReceiver>();
				var stack = new Stack<uint>();
				stack.Push(1);

				var data = JObject.Parse(json);
				var props = data.Properties().ToDictionary(v => uint.Parse(v.Name), v => v.Value as JObject);

				while (stack.Any()) {
					var id = stack.Pop();
					var jo = props[id];
					Deserialize(id, jo);
				}

				foreach (var icb in cbList) {
					try {
						icb.OnAfterDeserialize();
					} catch (System.Exception e) {
						Debug.LogError($"Exception occurred during `{icb.GetType().Name} ISerializationCallbackReceiver.OnAfterDeserialize`: ${e}", icb as Object);
					}
				}

				IGameSerializable GetObj(uint id) {
					if (objs.TryGetValue(id, out var obj)) return obj;
					var type = Type.GetType((string)props[id]["$type"]);
					if (!typeof(IGameSerializable).IsAssignableFrom(type)) throw new InvalidOperationException($"Type is not legal ({type.FullName})");
					var res = objs[id] = (IGameSerializable)ScriptableObject.CreateInstance(type);
					(res as Object).name = $"Instance ({id})";
					return res;
				}

				void Deserialize(uint id, JObject jo) {
					var obj = GetObj(id);
					if (DefaultDetokenizer(obj.GetType(), jo, obj) is ISerializationCallbackReceiver isg) {
						cbList.Add(isg);
					}


					object DefaultDetokenizer(Type type, JToken j, object obj) {

						var jo = (JObject)j;

						obj ??= InstantiateType(type);

						foreach (var d in GetFields(type)) {
							var detokenizer = d.detokenizer ?? DefaultDetokenizer;
							var value = d.field.GetValue(obj);

							// List
							if (d.isList) {
								var list = (IList)(value ?? InstantiateType(d.field.FieldType));
								var ja = (JArray)jo[d.field.Name];

								if (list.Count != ja.Count) {
									if (list is Array arr) list = Resize(arr, ja.Count);
									else ResizeList(ref list, d.listItemType, ja.Count);
								}

								if (d.isIgs) {
									for (int i = 0; i < ja.Count; i++) {
										var jt = ja[i];
										if (jt.Type == JTokenType.Null) {
											d.field.SetValue(obj, null);
										} else {
											var id = (uint)jt;
											if (!objs.TryGetValue(id, out var objValue)) {
												objValue = GetObj(id);
												stack.Push(id);
											}
											QueueOnAfterDeserialize(list[i] = objValue, d);
										}
									}
								} else {
									if (d.serializeReference) {
										// { "$type": ... , "$value": ... }
										for (int i = 0; i < ja.Count; i++) {
											var jt = ja[i];
											if (
												jt is JObject jto
												&& jto["$value"] is JToken jval
												&& jto["$type"] is JValue jv
												&& jv.Value is string str
												&& Type.GetType(str) is Type jtype
												&& d.listItemType.IsAssignableFrom(jtype)
											) {
												if (jval is JValue jvalval && jvalval.Value == null) list[i] = null;
												else {
													QueueOnAfterDeserialize(list[i] = detokenizer(jtype, jval, list[i]), d);
												}
											} else {
												list[i] = null;
											}
										}
									} else {
										for (int i = 0; i < ja.Count; i++) {
											var jt = ja[i];
											var val = detokenizer(d.listItemType, jt, list[i]);
											QueueOnAfterDeserialize(list[i] = val, d);
										}
									}
								}

								d.field.SetValue(obj, list);
								QueueOnAfterDeserialize(list, d);
								continue;
							}

							// SerializeReference
							if (d.serializeReference) {
								// { "$type": ... , "$value": ... }
								if (
									jo["$value"] is JToken jval
									&& jo["$type"] is JValue jv
									&& jv.Value is string str
									&& Type.GetType(str) is Type jtype
									&& d.field.FieldType.IsAssignableFrom(jtype)
								) {
									if (jval is JValue jvalval && jvalval.Value == null) d.field.SetValue(obj, null);
									else {
										var value1 = detokenizer(jtype, jval, value);
										d.field.SetValue(obj, value1);
										QueueOnAfterDeserialize(value1, d);
									}
									continue;
								} else {
									d.field.SetValue(obj, null);
									continue;
								}
							}

							// Ref
							if (d.isIgs) {
								var jref = (JValue)jo[d.field.Name];
								if (jref.Type == JTokenType.Null) {
									d.field.SetValue(obj, null);
								} else {
									var id = (uint)jref;
									if (!objs.TryGetValue(id, out var objValue)) {
										objValue = GetObj(id);
										stack.Push(id);
									}
									d.field.SetValue(obj, objValue);
									QueueOnAfterDeserialize(objValue, d);
								}
								continue;
							}

							// Default
							var value2 = detokenizer(d.field.FieldType, jo[d.field.Name], value);
							d.field.SetValue(obj, value2);
							QueueOnAfterDeserialize(value2, d);
						}

						return obj;
					}

					void QueueOnAfterDeserialize(object obj, FieldData d) {
						if (d.isCallbackReceiver && !Object.Equals(obj, null)) {
							cbList.Add((ISerializationCallbackReceiver)obj);
						}
					}
				}
			}
		}

		static void ResizeList(ref IList list, Type type, int newSize) {
			if (list.Count < newSize) {
				if (list.Count == 0) {
					var value = typeof(IGameSerializable).IsAssignableFrom(type) ? null : InstantiateType(type, true);
					list.Add(value);
				}
				while (list.Count < newSize) {
					list.Add(list[0]);
				}
			} else if (list.Count > newSize) {
				do {
					list.RemoveAt(list.Count - 1);
				} while (list.Count > newSize);
			}
		}

		static Array Resize(Array array, int newSize) {
			Type elementType = array.GetType().GetElementType();
			Array newArray = Array.CreateInstance(elementType, newSize);
			Array.Copy(array, newArray, Math.Min(array.Length, newArray.Length));
			return newArray;
		}

		static object InstantiateType(Type type, bool allowAbstract = false) {
			if (type == typeof(string)) return null;
			if (allowAbstract && type.IsAbstract) return null;
			var ci = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
			if (ci != null) return ci.Invoke(null);
			return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
		}

		static string SerializeGame(Game game) {
			using (new Muc.Stopwatch("Serialization took")) {
				const int maxDepth = 7;
				var depthReached = 0;
				var stack = new Stack<(uint, IGameSerializable)>();
				uint topId = 1;
				stack.Push((topId, game));
				var tokens = new Dictionary<uint, JToken>() { { topId, null } };
				var toId = new Dictionary<object, uint>() { { game, topId } };
				while (stack.Any()) {
					var pair = stack.Pop();
					Serialize(pair.Item1, pair.Item2);
				}
				if (depthReached > 0) Debug.LogWarning($"Max depth reached {depthReached} times.");
				var res = JsonConvert.SerializeObject(tokens, Formatting.Indented);
				return res;

				void Serialize(uint id, IGameSerializable obj) {
					var depth = 0;
					if (obj is ISerializationCallbackReceiver irc) irc.OnBeforeSerialize();
					tokens[id] = DefaultTokenizer(obj);

					JToken DefaultTokenizer(object obj) {
						if (++depth > maxDepth) {
							depthReached++;
							return JValue.CreateNull();
						}
						var jo = new JObject();
						var type = obj.GetType();
						if (depth == 1) { // First item is IGameSerializable and needs a type field
							jo.Add("$type", type.GetShortQualifiedName());
						}

						foreach (var d in GetFields(type)) {
							var value = d.field.GetValue(obj);
							var tokenizer = d.tokenizer ?? DefaultTokenizer;

							if (d.isCallbackReceiver && !Object.Equals(value, null)) ((ISerializationCallbackReceiver)value).OnBeforeSerialize();

							// List
							if (d.isList) {
								var arrayToken = new JArray();
								if (d.isIgs) {
									if (value != null) {
										foreach (var e in (IList)value) {
											if (e is null) {
												arrayToken.Add(null);
											} else if (toId.TryGetValue(e, out var otherId)) {
												arrayToken.Add(otherId);
											} else {
												stack.Push((++topId, (IGameSerializable)e));
												toId[e] = topId;
												tokens[topId] = null;
												arrayToken.Add(topId);
											}
											continue;
										}
									}
								} else {
									if (d.isUnityObject) continue;

									if (d.serializeReference) {
										foreach (var v in (IList)value) {
											if (v is null) {
												arrayToken.Add(JValue.CreateNull());
											} else {
												arrayToken.Add(new JObject(
													new JProperty("$type", v.GetType().GetShortQualifiedName()),
													new JProperty("$value", tokenizer(v))
												));
											}
										}
									} else {
										if (value != null) {
											foreach (var v in (IList)value) {
												arrayToken.Add(tokenizer(v));
											}
										}
									}
								}
								jo.Add(d.field.Name, arrayToken);
								continue;
							}

							// Null?
							if (Object.Equals(value, null)) {
								jo.Add(d.field.Name, null);
								continue;
							}

							// Ref
							if (d.isIgs) {
								var igs = (IGameSerializable)value;
								if (toId.TryGetValue(value, out var otherId)) {
									jo.Add(d.field.Name, otherId);
									continue;
								} else {
									stack.Push((++topId, igs));
									toId[value] = topId;
									tokens[topId] = null;
									jo.Add(d.field.Name, topId);
									continue;
								}
							}

							// SerializeReference
							if (d.serializeReference) {
								jo.Add(d.field.Name, new JObject(
									new JProperty("$type", value.GetType().GetShortQualifiedName()),
									new JProperty("$value", tokenizer(value))
								));
								continue;
							}

							// Default
							jo.Add(d.field.Name, tokenizer(value));
						}
						depth--;
						return jo;
					}
				}
			}
		}

		static Dictionary<Type, List<FieldData>> fieldDataListCache = new();

		static IEnumerable<FieldData> GetFields(Type type) {

			if (fieldDataListCache.TryGetValue(type, out var res)) {
				return res;
			}

			var fields = new List<FieldData>();

			foreach (var field in GetAllFields(type)) {
				if (ValidateField(field, out var data)) {
					fields.Add(data);
				}
			}

			return fieldDataListCache[type] = fields;
		}

		private class FieldData {

			public FieldInfo field;
			public Tokenizer tokenizer;
			public Detokenizer detokenizer;
			public Type listItemType;
			public bool isIgs;
			public bool isUnityObject;
			public bool isCallbackReceiver;
			public bool serializeReference;

			public bool isList => listItemType != null;
		}

		static Dictionary<FieldInfo, (bool r, FieldData d)> fieldDataCache = new();

		static bool ValidateField(FieldInfo field, out FieldData d) {
			if (fieldDataCache.TryGetValue(field, out var cached)) {
				d = cached.d;
				return cached.r;
			}
			d = new();
			d.field = field;
			if (field.IsPublic) {
				if (System.Attribute.IsDefined(field, typeof(NonSerializedAttribute))) {
					return (fieldDataCache[field] = (false, d)).r;
				}
				d.serializeReference = System.Attribute.IsDefined(field, typeof(SerializeReference));
			} else {
				d.serializeReference = System.Attribute.IsDefined(field, typeof(SerializeReference));
				if (!d.serializeReference && !System.Attribute.IsDefined(field, typeof(SerializeField))) {
					return (fieldDataCache[field] = (false, d)).r;
				}
			}

			var gsa = field.GetCustomAttribute<GameSerializationAttribute>();
			if (gsa != null) {
				if (!gsa.IsTokenized(field.FieldType)) return (fieldDataCache[field] = (false, d)).r;
				d.tokenizer = gsa.GetTokenizer(field.FieldType);
				d.detokenizer = gsa.GetDetokenizer(field.FieldType);
				if (d.tokenizer != null && d.detokenizer != null) return (fieldDataCache[field] = (true, d)).r;
				if (field.FieldType == typeof(string) || field.FieldType.IsPrimitive || field.FieldType.IsEnum) {
					d.tokenizer ??= value => new JValue(value);
					d.detokenizer ??= (type, j, obj) => j.Value<object>();
				}
				return (fieldDataCache[field] = (true, d)).r;
			}

			var isArray = typeof(Array).IsAssignableFrom(field.FieldType);
			var listType = field.FieldType.GetGenericTypeOf(typeof(List<>));
			if (isArray || listType != null) {
				d.listItemType = isArray ? field.FieldType.GetElementType() : listType;
				d.isIgs = typeof(IGameSerializable).IsAssignableFrom(d.listItemType);
				d.isUnityObject = typeof(Object).IsAssignableFrom(d.listItemType);
				d.isCallbackReceiver = typeof(ISerializationCallbackReceiver).IsAssignableFrom(d.listItemType);
				return (fieldDataCache[field] = (ValidateType(d.listItemType, d, d.serializeReference, out d.tokenizer, out d.detokenizer), d)).r;
			}

			d.isIgs = typeof(IGameSerializable).IsAssignableFrom(field.FieldType);
			d.isUnityObject = typeof(Object).IsAssignableFrom(field.FieldType);
			d.isCallbackReceiver = typeof(ISerializationCallbackReceiver).IsAssignableFrom(field.FieldType);
			return (fieldDataCache[field] = (ValidateType(field.FieldType, d, d.serializeReference, out d.tokenizer, out d.detokenizer), d)).r;
		}

		static bool ValidateType(Type type, FieldData d, bool serializeReference, out Tokenizer tokenizer, out Detokenizer detokenizer) {
			tokenizer = null;
			detokenizer = null;

			var gsa = type.GetCustomAttribute<GameSerializationAttribute>(true);
			if (gsa != null) {
				if (!gsa.IsTokenized(type)) return false;
				tokenizer = gsa.GetTokenizer(type);
				detokenizer = gsa.GetDetokenizer(type);
				if (tokenizer != null && detokenizer != null) return true;
			}

			if (type == typeof(string) || type.IsPrimitive || type.IsEnum) {
				tokenizer = value => new JValue(value);
				if (type.IsEnum) {
					detokenizer = (type, j, obj) => Enum.ToObject(type, (Int64)((JValue)j).Value);
				} else {
					detokenizer = (type, j, obj) => Convert.ChangeType(((JValue)j).Value, type);
				}
			} else {
				if (gsa != null) return true;
				if (type.IsClass || type.IsValueType) {
					if (!d.isIgs) {
						if (!serializeReference && type.IsAbstract) return false;
						if (unityTypes.TryGetValue(type, out var converter)) {
							tokenizer = converter.tokenizer;
							detokenizer = converter.detokenizer;
							if (tokenizer == null || detokenizer == null) return false;
						} else {
							if (d.isUnityObject) return false;
							if (!System.Attribute.IsDefined(type, typeof(SerializableAttribute), false)) return false;
							if (System.Attribute.IsDefined(type, typeof(NonSerializedAttribute), false)) return false;
						}
					}
				}
			}
			return true;
		}

		static IEnumerable<FieldInfo> GetAllFields(Type type) {
			if (type == null) return Enumerable.Empty<FieldInfo>();
			return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				.Concat(GetAllFields(type.BaseType));
		}

	}

}