
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

		delegate JToken Tokenizer(object obj);
		delegate object Detokenizer(Type type, JToken j, object obj);

		class Converter {
			public Tokenizer tokenizer;
			public Detokenizer detokenizer;

			public Converter(Tokenizer serializer, Detokenizer detokenizer) {
				this.tokenizer = serializer;
				this.detokenizer = detokenizer;
			}
		}

		static Dictionary<Type, Converter> unityTypes = new() {
			{ typeof(Vector2), new((object obj) => { var v = (Vector2)obj; return new JArray(v.x, v.y); }, null) },
			{ typeof(Vector3), new((object obj) => { var v = (Vector3)obj; return new JArray(v.x, v.y, v.z); }, null) },
			{ typeof(Vector4), new((object obj) => { var v = (Vector4)obj; return new JArray(v.x, v.y, v.z, v.w); }, null) },
			{ typeof(Vector2Int), new((object obj) => { var v = (Vector2Int)obj; return new JArray(v.x, v.y); }, null) },
			{ typeof(Vector3Int), new((object obj) => { var v = (Vector3Int)obj; return new JArray(v.x, v.y, v.z); }, null) },
			{ typeof(Rect), new((object obj) => { var v = (Rect)obj; return new JArray(v.x, v.y, v.width, v.height); }, null) },
			{ typeof(RectInt), new((object obj) => { var v = (RectInt)obj; return new JArray(v.x, v.y, v.width, v.height); }, null) },
			{ typeof(Quaternion), new((object obj) => { var v = (Quaternion)obj; return new JArray(v.x, v.y, v.z, v.w); }, null) },
			{ typeof(Matrix4x4), new((object obj) => null, null) },
			{ typeof(Color), new((object obj) => { var v = (Color)obj; return new JArray(v.r, v.g, v.b, v.a); }, null) },
			{ typeof(Color32), new((object obj) => { var v = (Color32)obj; return new JArray(v.r, v.g, v.b, v.a); }, null) },
			{ typeof(LayerMask), new((object obj) => { var v = (LayerMask)obj; return new JValue(v.value); }, null) },
			{ typeof(AnimationCurve), new((object obj) => null, null) },
			{ typeof(Gradient), new((object obj) => null, null) },
			{ typeof(RectOffset), new((object obj) => { var v = (RectOffset)obj; return new JArray(v.left, v.right, v.top, v.bottom); }, null) },
		};

		static HashSet<Type> skippedTypes;
		static HashSet<(Type, string)> skippedFields;

		static void DeserializeGame(string json, Game game) {
			var objs = new Dictionary<uint, IGameSerializable>() { { 1, game } };
			var stack = new Stack<uint>();
			stack.Push(1);

			var data = JObject.Parse(json);
			var props = data.Properties().ToDictionary(v => uint.Parse(v.Name), v => v.Value as JObject);

			while (stack.Any()) {
				var id = stack.Pop();
				var jo = props[id];
				Deserialize(id, jo);
			}

			IGameSerializable GetObj(uint id, IGameSerializable defaultObj = null) {
				if (objs.TryGetValue(id, out var obj)) return obj;
				if (defaultObj != null && (Object)defaultObj != null) return objs[id] = defaultObj;
				var type = Type.GetType((string)props[id]["$type"]);
				if (!typeof(IGameSerializable).IsAssignableFrom(type)) throw new InvalidOperationException($"Type is not legal ({type.FullName})");
				return objs[id] = (IGameSerializable)ScriptableObject.CreateInstance(type);
			}

			void Deserialize(uint id, JObject jo) {
				var obj = GetObj(id);
				DefaultDetokenizer(obj.GetType(), jo, obj);

				object DefaultDetokenizer(Type type, JToken j, object obj) {

					var jo = (JObject)j;

					obj ??= InstantiateType(type);

					foreach (var pair in GetFields(type)) {
						var field = pair.field;
						var detokenizer = pair.detokenizer ?? DefaultDetokenizer;

						if (typeof(IGameSerializable).IsAssignableFrom(field.FieldType)) {
							var id = (uint)((JValue)jo[field.Name]).Value;
							if (!objs.TryGetValue(id, out var objValue)) {
								objValue = GetObj(id);
								stack.Append(id);
							}
							field.SetValue(obj, objValue);
							continue;
						}

						var value = field.GetValue(obj);

						bool isArray = typeof(Array).IsAssignableFrom(field.FieldType);
						bool isList = field.FieldType.IsGenericTypeOf(typeof(List<>));
						if (isArray || isList) {
							var array = (IList)(value ?? InstantiateType(field.FieldType));
							var ja = (JArray)jo[field.Name];

							Type eType;
							if (isArray) {
								eType = value.GetType().GetElementType();
							} else {
								var cur = value.GetType();
								while (true) {
									if (cur.IsGenericType && cur.GetGenericTypeDefinition() == typeof(List<>)) {
										eType = cur.GenericTypeArguments[0];
										break;
									}
									cur = cur.BaseType;
								}
							}

							if (array.Count != ja.Count) {
								if (isArray) array = Resize((Array)array, ja.Count);
								else ResizeList(ref array, ja.Count);
							}

							if (typeof(IGameSerializable).IsAssignableFrom(eType)) {
								for (int i = 0; i < ja.Count; i++) {
									var jt = ja[i];
									var id = (uint)((JValue)jt).Value;

									if (!objs.TryGetValue(id, out var objValue)) {
										objValue = GetObj(id);
										stack.Append(id);
									}
									array[i] = objValue;
								}
							} else {
								var arrDetokenizer = GetArraTypeDetokenizer(field.FieldType) ?? DefaultDetokenizer;
								for (int i = 0; i < ja.Count; i++) {
									var jt = ja[i];
									array[i] = arrDetokenizer(eType, jt, array[i]);
								}
							}

							field.SetValue(obj, array);
							continue;
						}

						field.SetValue(obj, detokenizer(field.FieldType, jo[field.Name], value));
					}

					return obj;
				}
			}
		}

		static void ResizeList(ref IList list, int newSize) {
			if (list.Count < newSize) {
				if (list.Count == 0) {
					list.Add((dynamic)default);
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

		static object InstantiateType(Type type) {
			var ci = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, null, null);
			if (ci != null) return ci.Invoke(null);
			return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
		}

		static string SerializeGame(Game game) {
			skippedTypes = new();
			skippedFields = new();
			const int maxDepth = 7;
			var depthReached = 0;
			var stack = new Stack<(uint, IGameSerializable)>();
			uint topId = 1;
			stack.Push((topId, game));
			var res = new Dictionary<uint, JToken>() { { topId, null } };
			var toId = new Dictionary<object, uint>() { { game, topId } };
			while (stack.Any()) {
				var pair = stack.Pop();
				Serialize(pair.Item1, pair.Item2);
			}
			if (depthReached > 0) Debug.LogWarning($"Max depth reached {depthReached} times.");
			if (skippedTypes.Count > 0) Debug.LogWarning($"Skipped {skippedTypes.Count} type(s).");
			if (skippedFields.Count > 0) Debug.LogWarning($"Skipped {skippedFields.Count} field(s).");
			return JsonConvert.SerializeObject(res, Formatting.Indented);

			void Serialize(uint id, IGameSerializable obj) {
				var depth = 0;
				if (obj is ISerializationCallbackReceiver irc) irc.OnBeforeSerialize();
				res[id] = DefaultTokenizer(obj);

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
					foreach (var pair in GetFields(type)) {
						var value = pair.field.GetValue(obj);

						if (value == null) {
							jo.Add(pair.field.Name, null);
							continue;
						} else if (value is IGameSerializable igs) {
							if ((Object)igs == null) {
								jo.Add(pair.field.Name, null);
								continue;
							}
							if (toId.TryGetValue(value, out var otherId)) {
								jo.Add(pair.field.Name, otherId);
								continue;
							} else {
								stack.Push((++topId, igs));
								toId[value] = topId;
								res[topId] = null;
								jo.Add(pair.field.Name, topId);
								continue;
							}
						}

						if (value is Array || value.GetType().IsGenericTypeOf(typeof(List<>))) {
							var arrayToken = new JArray();
							Type eType;
							if (value is Array) {
								eType = value.GetType().GetElementType();
							} else {
								var cur = value.GetType();
								while (true) {
									if (cur.IsGenericType && cur.GetGenericTypeDefinition() == typeof(List<>)) {
										eType = cur.GenericTypeArguments[0];
										break;
									}
									cur = cur.BaseType;
								}
							}
							if (typeof(IGameSerializable).IsAssignableFrom(eType)) {
								foreach (var e in (IList)value) {
									if (e is null) {
										arrayToken.Add(null);
									} else if (toId.TryGetValue(e, out var otherId)) {
										arrayToken.Add(otherId);
									} else {
										stack.Push((++topId, (IGameSerializable)e));
										toId[e] = topId;
										res[topId] = null;
										arrayToken.Add(topId);
									}
									continue;
								}
							} else {
								if (typeof(Object).IsAssignableFrom(eType)) continue;
								var arrayTokenizer = GetArraTypeTokenizer(eType) ?? DefaultTokenizer;
								foreach (var v in (IList)value) {
									arrayToken.Add(arrayTokenizer(v));
								}
							}
							jo.Add(pair.field.Name, arrayToken);
							continue;
						}

						if (value is ISerializationCallbackReceiver irc) irc.OnBeforeSerialize();

						var tokenizer = pair.tokenizer ?? DefaultTokenizer;
						jo.Add(pair.field.Name, tokenizer(value));
					}
					depth--;
					return jo;
				}
			}
		}

		static IEnumerable<(FieldInfo field, Tokenizer tokenizer, Detokenizer detokenizer)> GetFields(Type type) {

			var fields = new List<(FieldInfo, Tokenizer, Detokenizer)>();

			foreach (var field in GetAllFields(type)) {
				if (ValidateField(field, out var tokenizer, out var detokenizer)) {
					fields.Add((field, tokenizer, detokenizer));
				}
			}

			return fields;
		}

		static bool ValidateField(FieldInfo field, out Tokenizer tokenizer, out Detokenizer detokenizer) {
			tokenizer = null;
			detokenizer = null;
			if (field.IsPublic) {
				if (System.Attribute.IsDefined(field, typeof(NonSerializedAttribute))) {
					return false;
				}
			} else {
				if (!System.Attribute.IsDefined(field, typeof(SerializeField))) {
					return false;
				}
			}

			var fieldType = field.FieldType;

			if (fieldType == typeof(string) || fieldType.IsPrimitive || fieldType.IsEnum) {
				tokenizer = value => new JValue(value);
				detokenizer = (type, j, obj) => j.Value<object>();
			} else {
				if (fieldType.IsClass || fieldType.IsValueType) {
					if (!typeof(IGameSerializable).IsAssignableFrom(fieldType)) {
						if (fieldType.IsAbstract) return false;
						if (unityTypes.TryGetValue(fieldType, out var converter)) {
							tokenizer = converter.tokenizer;
							detokenizer = converter.detokenizer;
						} else {
							if (fieldType.IsSubclassOf(typeof(Object))) {
								skippedTypes.Add(fieldType);
								skippedFields.Add((field.DeclaringType, field.Name));
								return false;
							}
							if (!System.Attribute.IsDefined(fieldType, typeof(SerializableAttribute), inherit: false)) {
								return false;
							}
							if (System.Attribute.IsDefined(fieldType, typeof(NonSerializedAttribute), inherit: false)) {
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		static Detokenizer GetArraTypeDetokenizer(Type type) {
			if (type.IsPrimitive || type == typeof(string) || type.IsEnum) {
				return (type, j, obj) => j.Value<object>();
			}
			if (unityTypes.TryGetValue(type, out var converter)) {
				return converter.detokenizer;
			}
			return null;
		}

		static Tokenizer GetArraTypeTokenizer(Type type) {
			if (type.IsPrimitive || type == typeof(string) || type.IsEnum) {
				return value => new JValue(value);
			}
			if (unityTypes.TryGetValue(type, out var converter)) {
				return converter.tokenizer;
			}
			return null;
		}


		static IEnumerable<FieldInfo> GetAllFields(Type type) {
			if (type == null) return Enumerable.Empty<FieldInfo>();
			return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				.Concat(GetAllFields(type.BaseType));
		}

	}

}