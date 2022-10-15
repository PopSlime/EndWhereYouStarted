using UnityEngine;

namespace Coconut.Ewys.Entity {
	public abstract class EntityBase : MonoBehaviour {
		public abstract void FromData(EntityData data);
		public abstract EntityData ToData();
	}
}
