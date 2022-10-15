using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Coconut.Ewys {
	public class LevelData {
		public List<int[]> tiles = new();
		public List<EntityData> entities = new();
	}
	[JsonConverter(typeof(EntityDataConverter))]
	public abstract class EntityData {
#pragma warning disable IDE1006
		public abstract int type { get; }
#pragma warning restore IDE1006
		public int id;
		public float[] pos;
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
	/// <summary>
	/// Represents the data of an obstacle.
	/// </summary>
	public class ObstacleData : EntityData {
		public override int type => 4;
	}
	/// <summary>
	/// Represents the data of a star that the player needs to collect.
	/// </summary>
	public class StarData : EntityData {
		public override int type => 8;
	}
	/// <summary>
	/// Represents the data of a trigger that toggles the state of a target entity to a specified value for specified duration.
	/// </summary>
	public class TriggerData : EntityData {
		public override int type => 9;
		/// <summary>
		/// The target entity.
		/// </summary>
		public int target;
		/// <summary>
		/// The target value.
		/// </summary>
		public int value;
		public int duration = 0;
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
				4 => new ObstacleData(),
				_ => throw new ArgumentException("Invalid entity type"),
			};
		}
	}
}
