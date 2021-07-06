
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
					failMessage = GetText("Lang_FileCorrupted");
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
				failMessage = GetText("Lang_CannotLoadLanguage");
			}
		} catch (System.Exception) {
			failMessage = GetText("Lang_FileCorrupted");
		}
		return false;
	}

	public static string GetText(string strId) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return res;
		return strId;
	}
	public static string GetText(string strId, object arg0) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, arg0);
		return strId;
	}
	public static string GetText(string strId, object arg0, object arg1) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, arg0, arg1);
		return strId;
	}
	public static string GetText(string strId, object arg0, object arg1, object arg2) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, arg0, arg1, arg2);
		return strId;
	}
	public static string GetText(string strId, params object[] args) {
		if (Lang.texts != null && Lang.texts.TryGetValue(strId, out var res)) return String.Format(LangFormatProvider.instance, res, args);
		return strId;
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
			if (format[0] == '?' && arg is IComparable argComp) {
				for (int i = 0; i < format.Length; i++) {
					var c = format[i];
					if (c == '|') {
						return argComp.CompareTo(1) == 1
							? format.Substring(1, i - 1)
							: format.Substring(i + 1, format.Length - i - 1);
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