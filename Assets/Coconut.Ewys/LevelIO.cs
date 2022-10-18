using Coconut.Ewys.Entity;
using Cryville.Common.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Coconut.Ewys {
	public partial class LevelController {
		LevelData _level;
		readonly Dictionary<Vector2Int, List<EntityBase>> _tiles = new();
		readonly List<EntityBase> _entities = new();
		readonly List<Player> _players = new();
		public void Read(string path) {
			if (_level != null) throw new InvalidOperationException("Level already loaded.");
			foreach (Transform child in transform) GameObject.Destroy(child.gameObject);
			_level = JsonConvert.DeserializeObject<LevelData>(Resources.Load<TextAsset>("Levels/" + path).text);
			foreach (var tile in _level.tiles) {
				var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
				go.transform.SetParent(transform);
				var pos = tile.ToVector2Int();
				go.transform.position = (Vector3Int)pos;
				_tiles.Add(pos, new());
			}
			foreach (var entity in _level.entities) {
				var go = GameObject.Instantiate(Resources.Load<GameObject>(
					string.Format(CultureInfo.InvariantCulture, "Prefabs/Entities/{0:D}", entity.type)
				));
				go.transform.SetParent(transform);
				var comp = go.GetComponent<EntityBase>();
				comp.FromData(entity);
				comp.OnPhaseUpdate(Side.Solar, false);
				comp.PositionUpdate += OnEntityPositionUpdate;
				_tiles[comp.Position].Add(comp);
				_entities.Add(comp);
				if (comp is Player p) _players.Add(p);
			}
		}

		public void Write(string path) {
#if !UNITY_EDITOR_WIN
			throw new InvalidOperationException("Writing level is only supported in the editor.");
#else
			var level = new LevelData();
			foreach (Transform child in transform) {
				if (child.TryGetComponent<EntityBase>(out var entity)) {
					level.entities.Add(entity.ToData());
				}
				else {
					level.tiles.Add(Vector2Int.RoundToInt(child.position).ToArray());
				}
			}
			File.WriteAllText(Application.dataPath + "/Resources/Levels/" + path, JsonConvert.SerializeObject(level), Encoding.UTF8);
#endif
		}
	}
}
