using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Coconut.Ewys {
	public class LevelData {
		public int steps;
		public List<int[]> tiles = new();
		public List<EntityData> entities = new();
	}
	[JsonConverter(typeof(EntityDataConverter))]
	public abstract class EntityData {
#pragma warning disable IDE1006
		public abstract int type { get; }
#pragma warning restore IDE1006
		public int id;
		public int[] pos;
		public Side side = Side.Both;
		/// <summary>
		/// The default state of the entity.
		/// </summary>
		public int state;
	}
	[Flags]
	public enum Side {
		Invalid = 0,
		Solar = 1,
		Lunar = 2,
		Both = Solar | Lunar,
	}
	public class PlayerData : EntityData {
		public override int type => 0;
	}
	public class WeightData : EntityData {
		override public int type => 1;
	}
	/// <summary>
	/// Represents the data of an obstacle.
	/// </summary>
	public class ObstacleData : EntityData {
		public override int type => 4;
	}
	/// <summary>
	/// Represents the data of a treasure that the player needs to collect.
	/// </summary>
	public class TreasureData : EntityData {
		public override int type => 8;
	}
	/// <summary>
	/// Represents the data of a trigger that toggles the state of a target entity to a specified value for specified duration.
	/// </summary>
	public class TriggerData : EntityData {
		public override int type => 9;
		/// <summary>
		/// The ID of the target entity.
		/// </summary>
		public int target;
		/// <summary>
		/// The target flag(s).
		/// </summary>
		public int flag;
		/// <summary>
		/// Whether to toggle off the target flag(s).
		/// </summary>
		public bool inverse;
		/// <summary>
		/// The duration before it toggles off after stepped off. Special values:
		/// <list type="bullet">
		/// <item><term><c>-1</c></term><description>Does not toggle the flag(s) when stepped off.</description></item>
		/// <item><term><see cref="int.MaxValue" /></term><description>Does not toggle off once stepped on.</description></item>
		/// </list>
		/// </summary>
		public int duration = 0;
	}
	public class PortalData : EntityData {
		public override int type => 10;
		/// <summary>
		/// The ID of the target portal.
		/// </summary>
		public int? target;
	}
	public class EntityDataConverter : CustomCreationConverter<EntityData> {
		int _currentType;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			var obj = JObject.ReadFrom(reader);
			_currentType = obj["type"].ToObject<int>();
			return base.ReadJson(obj.CreateReader(), objectType, existingValue, serializer);
		}

		public override EntityData Create(Type objectType) {
			return _currentType switch {
				0 => new PlayerData(),
				1 => new WeightData(),
				4 => new ObstacleData(),
				8 => new TreasureData(),
				9 => new TriggerData(),
				10 => new PortalData(),
				_ => throw new ArgumentException("Invalid entity type"),
			};
		}
	}
}
