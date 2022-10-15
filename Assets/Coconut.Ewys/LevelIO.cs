using Coconut.Ewys.Entity;
using Cryville.Common.Unity;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Coconut.Ewys {
	public class LevelIO {
		readonly string _path;
		readonly Transform _root;
		public LevelIO(string path, Transform root) {
			_path = path;
			_root = root;
		}
		public void Read() {
			foreach (Transform child in _root) GameObject.Destroy(child.gameObject);
			var level = JsonConvert.DeserializeObject<LevelData>(Resources.Load<TextAsset>("Levels/" + _path).text);
			foreach (var tile in level.tiles) {
				var go = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
				go.transform.SetParent(_root);
				go.transform.position = (Vector3Int)tile.ToVector2Int();
			}
			foreach (var entity in level.entities) {
				var go = GameObject.Instantiate(Resources.Load<GameObject>(
					string.Format(CultureInfo.InvariantCulture, "Prefabs/Entities/{0:D}" + entity.type)
				));
				go.transform.SetParent(_root);
				go.GetComponent<EntityBase>().FromData(entity);
			}
		}
		public void Write() {
#if !UNITY_EDITOR_WIN
			throw new System.InvalidOperationException("Writing level is only supported in the editor.");
#else
			var level = new LevelData();
			foreach (Transform child in _root) {
				if (child.TryGetComponent<EntityBase>(out var entity)) {
					level.entities.Add(entity.ToData());
				}
				else {
					level.tiles.Add(Vector2Int.RoundToInt(child.position).ToArray());
				}
			}
			File.WriteAllText(Application.dataPath + "/Resources/Levels/" + _path, JsonConvert.SerializeObject(level), Encoding.UTF8);
#endif
		}
	}
}
