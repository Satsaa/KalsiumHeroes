
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
				var texts = JsonConvert.DeserializeObject<Dictionary<string, string>>(ta.text);
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
		return Lang.texts.TryGetValue(strId, out str);
	}

	public static bool TryGetStr(string strId, out string str, object arg0) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = String.Format(str, arg0);
			return true;
		}
		return false;
	}
	public static bool TryGetStr(string strId, out string str, object arg0, object arg1) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = String.Format(str, arg0, arg1);
			return true;
		}
		return false;
	}
	public static bool TryGetStr(string strId, out string str, object arg0, object arg1, object arg2) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = String.Format(str, arg0, arg1, arg2);
			return true;
		}
		return false;
	}
	public static bool TryGetStr(string strId, out string str, params object[] args) {
		if (Lang.texts.TryGetValue(strId, out str)) {
			str = String.Format(str, args);
			return true;
		}
		return false;
	}

	public static string GetStr(string strId) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return res;
		return strId;
	}
	public static string GetStr(string strId, object arg0) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, arg0);
		return strId;
	}
	public static string GetStr(string strId, object arg0, object arg1) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, arg0, arg1);
		return strId;
	}
	public static string GetStr(string strId, object arg0, object arg1, object arg2) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, arg0, arg1, arg2);
		return strId;
	}
	public static string GetStr(string strId, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, args);
		return strId;
	}

	public static string GetStr(string strId, string defaultStr) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return res;
		return defaultStr;
	}
	public static string GetStr(string strId, string defaultStr, object arg0) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, arg0);
		return defaultStr;
	}
	public static string GetStr(string strId, string defaultStr, object arg0, object arg1) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, arg0, arg1);
		return defaultStr;
	}
	public static string GetStr(string strId, string defaultStr, object arg0, object arg1, object arg2) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, arg0, arg1, arg2);
		return defaultStr;
	}
	public static string GetStr(string strId, string defaultStr, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, args);
		return defaultStr;
	}

	[Serializable]
	struct Pair {
		[SerializeField] public string key;
		[SerializeField] public string value;
	}

	public class LangFormatProvider : IFormatProvider, ICustomFormatter {

		public readonly static LangFormatProvider instance = new LangFormatProvider();

		public object GetFormat(Type formatType) {
			return this;
		}

		public string Format(string format, object arg, IFormatProvider formatProvider) {
			// {0:?apple|apples}
			if (format[0] == '?') {
				// {bool:?yes|no}
				if (arg is bool argBool) {
					for (int i = 0; i < format.Length; i++) {
						var c = format[i];
						if (c == '|') {
							return argBool
								? format.Substring(i + 1, format.Length - i - 1)
								: format.Substring(1, i - 1);
						}
					}
					// {int:?apple|apples}
				} else if (arg is IComparable argComp) {
					for (int i = 0; i < format.Length; i++) {
						var c = format[i];
						if (c == '|') {
							return argComp.CompareTo(1) == 1
								? format.Substring(1, i - 1)
								: format.Substring(i + 1, format.Length - i - 1);
						}
					}
				}
			}
			return DefaultFormat(format, arg);
		}

		string DefaultFormat(string format, object arg) {
			if (arg is IFormattable fmt) return fmt.ToString(format, CultureInfo.CurrentCulture);
			else if (arg != null) return arg.ToString();
			else return string.Empty;
		}
	}
}