
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Text.RegularExpressions;
using Muc.Components.Extended;
using Muc.Data;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using System.Text;

public class Lang : Singleton<Lang> {

	[field: SerializeField, Tooltip("Path to translations root folder without leading or trailing slash.")]
	public string translationsPath { get; private set; } = "Lang";

	[field: SerializeField, Tooltip("Name of the language in use or to be used. Language names are C# CultureInfo names.")]
	public string language { get; private set; } = "en-US";

	[field: SerializeField, Tooltip("List of languages. Language names are C# CultureInfo names.")]
	public List<string> languages { get; private set; } = new List<string>() { "en-US" };


	static Dictionary<string, string> texts;


	protected override void Awake() {
		base.Awake();
		if (!LoadLanguage(language, out var msg)) {
			Debug.LogError($"Failed to load language \"{language}\" at startup: ${msg}");
		}
	}

	static Regex ends = new Regex(@"^\/|\/$");
	public static bool LoadLanguage(string language, out string failMessage) {
		try {
			var ta = Resources.Load<TextAsset>($"{Lang.instance.translationsPath}/{language}");
			try {

				var texts = new Dictionary<string, string>(StringComparer.Ordinal);
				JsonConvert.PopulateObject(ta.text, texts);
				if (texts == null) {
					failMessage = GetStr("Lang_FileCorrupted");
					return false;
				}
				Lang.texts = texts;
				Lang.instance.language = language;
				failMessage = null;
				try {
					CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture(language);
				} catch (System.Exception) {
					Debug.LogWarning($"Could not set specific culture of {language}");
					try {
						CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture(new Regex(@"-.*").Replace(language, ""));
					} catch (System.Exception) {
						Debug.LogWarning($"Could not set specific culture of {new Regex(@"-.*").Replace(language, "")}. It is set to the invariant culture.");
						CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
					}
					throw;
				}
				return true;
			} catch (System.Exception) {
				failMessage = GetStr("Lang_CannotLoadLanguage");
			}
		} catch (System.Exception) {
			failMessage = GetStr("Lang_FileCorrupted");
		}
		return false;
	}

	public static bool HasStr(string strId) {
		return Lang.texts.ContainsKey(strId);
	}

	public static bool TryGetStr(string strId, out string str) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = Format(str, null, null, null);
			return true;
		}
		return false;
	}
	public static bool TryGetStr(string strId, out string str, IAttribute a, DataObjectData s, DataObjectData d) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = Format(str, a, s, d);
			return true;
		}
		return false;
	}
	public static bool TryGetStr(string strId, out string str, DataObjectData s, DataObjectData d) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = Format(str, null, s, d);
			return true;
		}
		return false;
	}
	public static bool TryGetStr(string strId, out string str, DataObjectData s) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = Format(str, null, s, null);
			return true;
		}
		return false;
	}
	public static bool TryGetStr(string strId, out string str, IAttribute a) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = Format(str, a, null, null);
			return true;
		}
		return false;
	}

	public static bool TryGetStrArgs(string strId, out string str, params object[] args) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = Format(str, null, null, null, args);
			return true;
		}
		return false;
	}
	public static bool TryGetStrArgs(string strId, out string str, IAttribute a, DataObjectData s, DataObjectData d, params object[] args) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = Format(str, a, s, d, args);
			return true;
		}
		return false;
	}
	public static bool TryGetStrArgs(string strId, out string str, DataObjectData s, DataObjectData d, params object[] args) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = Format(str, null, s, d, args);
			return true;
		}
		return false;
	}
	public static bool TryGetStrArgs(string strId, out string str, DataObjectData s, params object[] args) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = Format(str, null, s, null, args);
			return true;
		}
		return false;
	}
	public static bool TryGetStrArgs(string strId, out string str, IAttribute a, params object[] args) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = Format(str, a, null, null, args);
			return true;
		}
		return false;
	}


	public static string GetStr(string strId) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(in res, null, null, null);
		return strId;
	}
	public static string GetStr(string strId, string defaultStr) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(in res, null, null, null);
		return defaultStr;
	}
	public static string GetStr(string strId, string defaultStr, IAttribute a, DataObjectData s, DataObjectData d) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, a, s, d);
		return defaultStr;
	}
	public static string GetStr(string strId, string defaultStr, DataObjectData s, DataObjectData d) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, null, s, d);
		return defaultStr;
	}
	public static string GetStr(string strId, string defaultStr, DataObjectData s) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, null, s, s);
		return defaultStr;
	}
	public static string GetStr(string strId, string defaultStr, IAttribute a) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, a, null, null);
		return defaultStr;
	}
	public static string GetStr(string strId, IAttribute a, DataObjectData s, DataObjectData d) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, a, s, d);
		return strId;
	}
	public static string GetStr(string strId, DataObjectData s, DataObjectData d) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, null, s, d);
		return strId;
	}
	public static string GetStr(string strId, DataObjectData s) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, null, s, s);
		return strId;
	}
	public static string GetStr(string strId, IAttribute a) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, a, null, null);
		return strId;
	}

	public static string GetStrArgs(string strId, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, null, null, null, args);
		return strId;
	}
	public static string GetStrArgs(string strId, string defaultStr, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, null, null, null, args);
		return defaultStr;
	}
	public static string GetStrArgs(string strId, string defaultStr, IAttribute a, DataObjectData s, DataObjectData d, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, a, s, d, args);
		return defaultStr;
	}
	public static string GetStrArgs(string strId, string defaultStr, DataObjectData s, DataObjectData d, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, null, s, d, args);
		return defaultStr;
	}
	public static string GetStrArgs(string strId, string defaultStr, DataObjectData s, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, null, s, s, args);
		return defaultStr;
	}
	public static string GetStrArgs(string strId, string defaultStr, IAttribute a, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, a, null, null, args);
		return defaultStr;
	}
	public static string GetStrArgs(string strId, IAttribute a, DataObjectData s, DataObjectData d, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, a, s, d, args);
		return strId;
	}
	public static string GetStrArgs(string strId, DataObjectData s, DataObjectData d, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, null, s, d, args);
		return strId;
	}
	public static string GetStrArgs(string strId, DataObjectData s, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, null, s, s, args);
		return strId;
	}
	public static string GetStrArgs(string strId, IAttribute a, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return Format(res, a, null, null, args);
		return strId;
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnScriptsRloaded() {
		if (Lang.instance) Lang.LoadLanguage(Lang.instance.language, out var _);
	}
#endif

	[Serializable]
	struct Pair {
		[SerializeField] public string key;
		[SerializeField] public string value;
	}

	/*
	We implement a custom formatting function for template strings fitting the needs of our project.
	Primary goals are to allow getting values from DataObjects and Attributes.
	Syntax:

		{s.cooldown} -> "0/3" // s = source DataObject
		{d.cooldown} -> "2/3" // d = current DataObject
		{d.cooldown.0} -> "2" // First value of cooldown
		{d.cooldown.1} -> "3" // Second value of cooldown
		{d.cooldown.1:F2} -> "3.00" // Second value with formatting option

		{d.range.e} -> "true" // Range is enabled?

		// Pluralization
		{d.charges.0?charge|charges} -> "charge" (1)
		{d.charges.0?charge|charges} -> "charges" (5)

		// Conditional formatting and nested formatting
		{d.range.e?infinite|{d.range}} -> "infinite" (false)
		{d.range.e?infinite|{d.range}} -> "5" (true)

		// String by strId
		{MyString} -> "My cool string"
		{MyInfiniteString} -> "My cool reapeating string {MyInfiniteString}" -> ... // Loops and the application crashes or throws... maybe a max depth
		{MyDynamicString_{d.abilityType}} -> {MyDynamicString_WeaponSkill} -> "My dynamic string specific for Weapon Skills"

		// Maybe in the future we can do this too
		{MyDynamicString_{d.targetType}} -> {MyDynamicString_Enemy}, {MyDynamicString_Wall} -> "Dynamic enemy, Dynamic wall" // Enum flags with multiple values
		{MyDynamicString_{d.targetType}: ; } -> {MyDynamicString_Enemy} ; {MyDynamicString_Wall} -> "Dynamic enemy ; Dynamic wall" // Enum flags with custom separator
	
		// Normal formatting...
		{0} -> "1"
		{0:F2} -> "1.00"

		// not implemented
		// Custom string manipulations (This will prevent these characters being used as a separator)
		{MyString}   -> my COOL string
		{MyString:U} -> My COOL string
		{MyString:C} -> MY COOL STRING
		{MyString:L} -> my cool string

	*/

	const char esc = '/';
	public static string Format(in string str, IAttribute a, DataObjectData s, DataObjectData d, params object[] args) => Format(in str, a, s, d, 0, args);
	private static string Format(in string str, IAttribute a, DataObjectData s, DataObjectData d, int depth, params object[] args) {
		var acc = new StringBuilder(str.Length);
		for (int i = 0; i < str.Length; i++) {
			var c = str[i];
			switch (c) {
				case esc:
					if (i + 1 >= str.Length) throw new SyntaxException("Unexpected end of string.");
					var next = str[i + 1];
					if (next == '{' || next == esc) {
						acc.Append(next);
						i++;
					} else {
						acc.Append(c);
					}
					break;
				case '{':
					i++;
					acc.Append(FormatToken(in str, true, false, depth, i, out i));
					break;
				default:
					acc.Append(c);
					break;
			}
		}
		return acc.ToString();

		string FormatToken(in string str, bool isEntry, bool isParam, int depth, int start, out int end) {
			char prevSpecial = default;
			int wordStart = start;
			var cur = new StringBuilder(str.Length);
			string selector = null;
			string branch1 = null;
			string branch2 = null;

			object Evaluate() {
				if (selector == null) throw new SyntaxException("No selector?");
				var splitted = selector.Split('.');
				switch (splitted.Length) {
					case 1: {
							switch (splitted[0]) {
								case "a":
									if (a == null) throw new SyntaxException("Attribute is not supplied.");
									if (branch1 != null || branch2 != null) throw new SyntaxException("The attribute by itself cannot be branched. Use 'a.e' or 'a.0' for example.");
									return a.Format(d == s);
								case "s":
								case "d":
									throw new SyntaxException("The 'd' and 's' main selectors require you to select a field eg 'd.range'.");
								default:
									if (splitted[0].Length == 1) {
										var val = args[Int32.Parse(splitted[0])];
										if (branch1 != null) {
											if (branch2 == null) throw new SyntaxException("No second branch.");
											return val switch {
												bool v => v ? branch2 : branch1,
												IComparable v => v.CompareTo(1) == 0 ? branch2 : branch1,
												_ => val,
											};
										}
										return val;
									} else {
										if (depth > 10) return "[RECURSIVE]";
										return isParam || isEntry ? Format(GetStr(splitted[0]), a, s, d, depth + 1) : splitted[0];
										string GetStr(string strId) {
											if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return res;
											return strId;
										}
									}
							}
						}
					case 2: {
							switch (splitted[0]) {
								case "a":
									if (splitted.Length != 2) throw new SyntaxException("Too many sub selectors.");
									if (a == null) throw new SyntaxException("Attribute is not supplied.");
									var val = splitted[1] switch {
										"e" => a.GetEnabled().value,
										"0" => a.GetValue(0).value,
										"1" => a.GetValue(1).value,
										"2" => a.GetValue(2).value,
										"3" => a.GetValue(3).value,
										"4" => a.GetValue(4).value,
										"5" => a.GetValue(5).value,
										"6" => a.GetValue(6).value,
										"7" => a.GetValue(7).value,
										"8" => a.GetValue(8).value,
										"9" => a.GetValue(9).value,
										_ => throw new SyntaxException($"Invalid sub selector {splitted[1]}"),
									};
									if (branch1 != null) {
										if (branch2 == null) throw new SyntaxException("No second branch.");
										return splitted[1] switch {
											"e" => a.GetEnabled().value ? branch2 : branch1,
											_ => val switch {
												bool v => v ? branch2 : branch1,
												IComparable v => v.CompareTo(1) == 0 ? branch2 : branch1,
												_ => val,
											},
										};
									}
									return val;
								case "s":
									if (s == null) throw new SyntaxException("Source is not supplied.");
									return DoDataObject(s);
								case "d":
									if (d == null) throw new SyntaxException("Data is not supplied.");
									return DoDataObject(d);
								default:
									throw new SyntaxException($"Invalid main selector {splitted[0]}");
							}

							object DoDataObject(DataObjectData o) {
								var fieldSub = splitted[1];
								var field = o.GetType().GetField(fieldSub, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
								if (field == null) throw new MissingFieldException($"Field {fieldSub} not found.");
								var fieldVal = field.GetValue(o);
								if (fieldVal is IAttribute att) {
									if (branch1 != null) {
										if (branch2 == null) throw new SyntaxException("No second branch.");
										var val = att.GetEnabled()?.value ?? att.GetValue(0).value;
										return val switch {
											bool v => v ? branch2 : branch1,
											IComparable v => v.CompareTo(1) == 0 ? branch2 : branch1,
											_ => val,
										};
									}
									return att.Format(d == s);
								} else {
									if (branch1 != null) {
										if (branch2 == null) throw new SyntaxException("No second branch.");
										return fieldVal switch {
											bool v => v ? branch2 : branch1,
											IComparable v => v.CompareTo(1) == 0 ? branch2 : branch1,
											_ => fieldVal,
										};
									}
									return fieldVal;
								}
							}
						}
					case 3: {
							switch (splitted[0]) {
								case "a":
									throw new SyntaxException("Too many sub selectors for attribute.");
								case "s":
									if (s == null) throw new SyntaxException("Source is not supplied.");
									return DoDataObject(s);
								case "d":
									if (d == null) throw new SyntaxException("Data is not supplied.");
									return DoDataObject(d);
								default:
									throw new SyntaxException($"Invalid main selector {splitted[0]}");
							}
							object DoDataObject(DataObjectData o) {
								var fieldSub = splitted[1];
								var field = o.GetType().GetField(fieldSub, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
								if (field == null) throw new MissingFieldException($"Field {fieldSub} not found.");
								var fieldVal = field.GetValue(o);
								if (fieldVal is IAttribute att) {
									var val = splitted[2] switch {
										"e" => att.GetEnabled().value,
										"0" => att.GetValue(0).value,
										"1" => att.GetValue(1).value,
										"2" => att.GetValue(2).value,
										"3" => att.GetValue(3).value,
										"4" => att.GetValue(4).value,
										"5" => att.GetValue(5).value,
										"6" => att.GetValue(6).value,
										"7" => att.GetValue(7).value,
										"8" => att.GetValue(8).value,
										"9" => att.GetValue(9).value,
										_ => throw new SyntaxException($"Invalid sub selector {splitted[2]}"),
									};
									if (branch1 != null) {
										if (branch2 == null) throw new SyntaxException("No second branch.");
										return splitted[2] switch {
											"e" => att.GetEnabled().value ? branch2 : branch1,
											_ => val switch {
												bool v => v ? branch2 : branch1,
												IComparable v => v.CompareTo(1) == 0 ? branch2 : branch1,
												_ => val,
											},
										};
									}
									return val;
								} else {
									throw new SyntaxException($"Trying to select a value in a non attribute field ({fieldSub})");
								}
							}
						}
					default:
						throw new SyntaxException($"Selector is too long ({selector})");
				}
			}

			for (int i = start; i < str.Length; i++) {
				var c = str[i];
				switch (c) {
					case esc:
						if (i + 1 >= str.Length) throw new SyntaxException("Unexpected end of string.");
						var next = str[i + 1];
						switch (next) {
							case '{':
							case '}':
							case '|':
							case ':':
							case '?':
							case esc:
								cur.Append(next);
								i++;
								break;
							default:
								cur.Append(c);
								break;
						}
						break;
					case '{':
						i++;
						cur.Append(FormatToken(in str, false, prevSpecial != default, depth + 1, i, out i));
						break;
					case '}':
						if (prevSpecial == default) {
							selector = cur.ToString();
						} else if (prevSpecial == '|') {
							branch2 = cur.ToString();
						}
						end = i;
						return Evaluate().ToString();
					case '|':
						if (prevSpecial != '?') {
							cur.Append(c);
							break;
						}
						prevSpecial = c;
						branch1 = cur.ToString();
						cur.Clear();
						break;
					case ':':
						if (prevSpecial == '|') {
							branch2 = cur.ToString();
							end = i;
						}
						if (prevSpecial == '?') {
							cur.Append(c);
							break;
						}
						var format = "";
						for (; i < str.Length; i++) {
							c = str[i];
							switch (c) {
								case esc:
									switch (str[i + 1]) {
										case '}':
											format += '}';
											i++;
											break;
										default:
											format += c;
											break;
									}
									break;
								case '}':
									end = i;
									var eval = Evaluate();
									if (eval is IFormattable formattable)
										return formattable.ToString(format, null);
									return eval.ToString();
								default:
									format += c;
									break;
							}
						}
						throw new SyntaxException("Unexpected end of string.");
					case '?':
						if (prevSpecial != default) {
							cur.Append(c);
							break;
						}
						selector = cur.ToString();
						prevSpecial = c;
						cur.Clear();
						break;
					default:
						cur.Append(c);
						break;
				}
			}
			throw new SyntaxException("Unexpected end of string.");
		}
	}


	[System.Serializable]
	public class SyntaxException : System.Exception { public SyntaxException(string message) : base(message) { } }

}