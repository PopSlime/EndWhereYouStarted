using UnityEngine;

namespace Coconut.Ewys {
	/// <summary>
	/// Used to control UI 
	/// </summary>
	public class UIControl : MonoBehaviour {
		[SerializeField]
		GameObject m_explainInfo;

		/// <summary>
		/// Restart the game
		/// </summary>
		public void Remake() {

		}

		public void ShowExplainInfo() {
			m_explainInfo.SetActive(true);
		}

		public void HideExplainInfo() {
			m_explainInfo.SetActive(false);
		}
	}
}
