
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static Serialization.GameSerializer;

namespace Serialization {

	public abstract class GameSerializationAttribute : System.Attribute {
		public abstract bool IsTokenized(Type type);
		public virtual Detokenizer GetDetokenizer(Type type) => null;
		public virtual Tokenizer GetTokenizer(Type type) => null;
	}

	public sealed class NoTokenAttribute : GameSerializationAttribute {
		public override bool IsTokenized(Type type) => false;
	}

	public abstract class CustomTokenAttribute : GameSerializationAttribute {
		public sealed override bool IsTokenized(Type type) => true;
	}

}