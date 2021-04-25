
namespace IHas {

	using UnityEngine;

	// Helper interfaces mainly for ValueSetters

	public interface IHas {
	}

	public interface IHasSprite : IHas {
		public Sprite sprite { get; }
	}

	public interface IHasLore : IHas {
		public TextSource lore { get; }
	}

	public interface IHasDisplayName : IHas {
		public TextSource displayName { get; }
	}

	public interface IHasDescription : IHas {
		public TextSource description { get; }
	}

}