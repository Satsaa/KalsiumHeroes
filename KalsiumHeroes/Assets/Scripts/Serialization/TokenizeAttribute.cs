
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static Serialization.GameSerializer;

namespace Serialization {

	public class TokenizeAttribute : System.Attribute {

		protected internal bool applyToList;

		public TokenizeAttribute(bool applyToList = false) => this.applyToList = applyToList;

		public virtual bool IsTokenized(Type type) => true;
		public virtual Detokenizer GetDetokenizer(Type type) => null;
		public virtual Tokenizer GetTokenizer(Type type) => null;
	}

	public sealed class DoNotTokenizeAttribute : TokenizeAttribute {
		public DoNotTokenizeAttribute(bool applyToList = false) : base(applyToList) { }
		public override bool IsTokenized(Type type) => false;
	}

}