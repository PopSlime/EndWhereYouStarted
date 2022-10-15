using Cryville.Common.Unity;
using UnityEngine;

namespace Coconut.Ewys.Entity {
	public abstract class EntityBase : MonoBehaviour {
		public void FromData(EntityData data) {
			transform.position = (Vector3Int)data.pos.ToVector2Int();
		}
		protected abstract void FromDataImpl(EntityData data);
		public EntityData ToData() {
			var result = ToDataImpl();
			result.pos = Vector2Int.RoundToInt(transform.position).ToArray();
			result.id = transform.GetSiblingIndex();
			return result;
		}
		public abstract EntityData ToDataImpl();
	}
}
