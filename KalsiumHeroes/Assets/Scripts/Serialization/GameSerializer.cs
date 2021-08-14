
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

	public static class GameSerializer {

		public static string Serialize(Game game) {
			var serializer = new Serializer();
			return serializer.SerializeGame(game);
		}

		public class Serializer {

			public class Converter {
				public Func<object, JToken> tokenizer;
				public Func<JToken, object> detokenizer;

				public Converter(Func<object, JToken> serializer, Func<object, JToken> deserializer) {
					this.tokenizer = serializer;
					this.detokenizer = deserializer;
				}
			}

			public static Dictionary<Type, Converter> unityTypes = new() {
				{ typeof(Vector2), new((object obj) => { var v = (Vector2)obj; return Disassemble(v.x, v.y); }, null) },
				{ typeof(Vector3), new((object obj) => { var v = (Vector3)obj; return Disassemble(v.x, v.y, v.z); }, null) },
				{ typeof(Vector4), new((object obj) => { var v = (Vector4)obj; return Disassemble(v.x, v.y, v.z, v.w); }, null) },
				{ typeof(Vector2Int), new((object obj) => { var v = (Vector2Int)obj; return Disassemble(v.x, v.y); }, null) },
				{ typeof(Vector3Int), new((object obj) => { var v = (Vector3Int)obj; return Disassemble(v.x, v.y, v.z); }, null) },
				{ typeof(Rect), new((object obj) => { var v = (Rect)obj; return Disassemble(v.x, v.y, v.width, v.height); }, null) },
				{ typeof(RectInt), new((object obj) => { var v = (RectInt)obj; return Disassemble(v.x, v.y, v.width, v.height); }, null) },
				{ typeof(Quaternion), new((object obj) => { var v = (Quaternion)obj; return Disassemble(v.x, v.y, v.z, v.w); }, null) },
				{ typeof(Matrix4x4), new((object obj) => null, null) },
				{ typeof(Color), new((object obj) => { var v = (Color)obj; return Disassemble(v.r, v.g, v.b, v.a); }, null) },
				{ typeof(Color32), new((object obj) => { var v = (Color32)obj; return Disassemble(v.r, v.g, v.b, v.a); }, null) },
				{ typeof(LayerMask), new((object obj) => { var v = (LayerMask)obj; return new JValue(v.value); }, null) },
				{ typeof(AnimationCurve), new((object obj) => null, null) },
				{ typeof(Gradient), new((object obj) => null, null) },
				{ typeof(RectOffset), new((object obj) => { var v = (RectOffset)obj; return Disassemble(v.left, v.right, v.top, v.bottom); }, null) },
			};

			protected static JArray Disassemble(params object[] values) {
				var res = new JArray();
				foreach (var value in values) {
					res.Add(value);
				}
				return res;
			}

			protected HashSet<Type> skippedTypes;
			protected HashSet<(Type, string)> skippedFields;

			public string SerializeGame(Game game) {
				skippedTypes = new();
				skippedFields = new();
				const int maxDepth = 7;
				var depthReached = 0;
				var stack = new Stack<(uint, IGameSerializable)>();
				stack.Push(((uint)1, game));
				uint id = 1;
				var res = new Dictionary<uint, JToken>() { { 1, null } };
				var toId = new Dictionary<object, uint>() { { game, 1 } };
				while (stack.Any()) {
					var pair = stack.Pop();
					SerializeObject(pair.Item1, pair.Item2);
				}
				if (depthReached > 0) Debug.LogWarning($"Max depth reached {depthReached} times.");
				if (skippedTypes.Count > 0) Debug.LogWarning($"Skipped {skippedTypes.Count} type(s).");
				if (skippedFields.Count > 0) Debug.LogWarning($"Skipped {skippedFields.Count} field(s).");
				return JsonConvert.SerializeObject(res, Formatting.Indented);

				void SerializeObject(uint objId, object obj) {
					var depth = 0;
					if (obj is ISerializationCallbackReceiver irc) irc.OnBeforeSerialize();
					res[objId] = DefaultTokenizer(obj);

					JToken DefaultTokenizer(object obj) {
						if (++depth > maxDepth) {
							depthReached++;
							return JValue.CreateNull();
						}
						var jo = new JObject();
						foreach (var pair in GetFields(obj.GetType())) {
							var value = pair.field.GetValue(obj);

							if (value is null) {
								jo.Add(pair.field.Name, null);
								continue;
							} else if (value is IGameSerializable igs) {
								if (toId.TryGetValue(value, out var otherId)) {
									jo.Add(pair.field.Name, otherId);
									continue;
								} else {
									stack.Push((++id, igs));
									toId[value] = id;
									res[id] = null;
									jo.Add(pair.field.Name, id);
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
									foreach (var e in value as System.Collections.IList) {
										if (e is null) {
											arrayToken.Add(null);
										} else if (toId.TryGetValue(e, out var otherId)) {
											arrayToken.Add(otherId);
										} else {
											stack.Push((++id, (IGameSerializable)e));
											toId[e] = id;
											res[id] = null;
											arrayToken.Add(id);
										}
										continue;
									}
								} else {
									if (typeof(Object).IsAssignableFrom(eType)) continue;
									var arrayTokenizer = GetArraTypeTokenizer(eType) ?? DefaultTokenizer;
									foreach (var v in value as System.Collections.IList) {
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

			protected IEnumerable<(FieldInfo field, Func<object, JToken> tokenizer)> GetFields(Type type) {

				var fields = new List<(FieldInfo, Func<object, JToken>)>();

				foreach (var field in GetAllFields(type)) {
					Func<object, JToken> tokenizer = null;
					if (field.IsPublic) {
						if (System.Attribute.IsDefined(field, typeof(NonSerializedAttribute))) {
							continue;
						}
					} else {
						if (!System.Attribute.IsDefined(field, typeof(SerializeField))) {
							continue;
						}
					}

					var fieldType = field.FieldType;

					if (fieldType == typeof(string) || fieldType.IsPrimitive || fieldType.IsEnum) {
						tokenizer = value => new JValue(value);
					} else {
						if (fieldType.IsClass || fieldType.IsValueType) {
							if (!typeof(IGameSerializable).IsAssignableFrom(fieldType)) {
								if (fieldType.IsAbstract) continue;
								if (unityTypes.TryGetValue(fieldType, out var converter)) {
									tokenizer = converter.tokenizer;
								} else {
									if (fieldType.IsSubclassOf(typeof(Object))) {
										skippedTypes.Add(fieldType);
										skippedFields.Add((field.DeclaringType, field.Name));
										continue;
									}
									if (!System.Attribute.IsDefined(fieldType, typeof(SerializableAttribute), inherit: false)) {
										continue;
									}
									if (System.Attribute.IsDefined(fieldType, typeof(NonSerializedAttribute), inherit: false)) {
										continue;
									}
								}
							}
						}
					}

					fields.Add((field, tokenizer));
				}

				return fields;
			}

			Func<object, JToken> GetArraTypeTokenizer(Type type) {
				if (type.IsPrimitive || type == typeof(string) || type.IsEnum) {
					return value => new JValue(value);
				}
				if (unityTypes.TryGetValue(type, out var converter)) {
					return converter.tokenizer;
				}
				return null;
			}


			public static IEnumerable<FieldInfo> GetAllFields(Type type) {
				if (type == null) return Enumerable.Empty<FieldInfo>();
				return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Concat(GetAllFields(type.BaseType));
			}
		}

	}

}