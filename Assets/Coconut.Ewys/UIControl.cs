using UnityEngine;
using UnityEngine.UI;

namespace Coconut.Ewys {
	/// <summary>
	/// Used to control UI 
	/// </summary>
	public class UIControl : MonoBehaviour {
		[SerializeField]
		GameObject m_explainInfo;

		[SerializeField]
		/// <summary>
		/// the transform of hour hand
		/// </summary>
		RectTransform m_hourHandTransform;

		[SerializeField]
		Text stepNumberText;
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
		/// <summary>
		/// When player costinng the step, call this function to rotate the clock and decline step number of showing
		/// </summary>
		public void RotateClock(int remainStep, int totalStep) {
			//TODO Animation
			stepNumberText.text = remainStep.ToString();
			m_hourHandTransform.Rotate(new Vector3(0, 0, 360 / totalStep));
		}
	}
}
