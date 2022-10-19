using System.Collections;
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
			stepNumberText.text = remainStep.ToString();
			//TODO If mulitple coroutines exist at the same time
			StartCoroutine(RotateAround(360 / totalStep,0.1f));
		}

		IEnumerator RotateAround(float angel, float time)//携程函数 IEnumerator开头
		{
			float number = time/Time.fixedDeltaTime;//计算总量
			float nextAngel = angel / number;//计算要转多少 固定帧率

			for (int i = 0; i < number; i++) {
				transform.Rotate(new Vector3(0, 0, nextAngel));//转向
				yield return new WaitForFixedUpdate();//固定帧率下一帧再执行
			}

		}
	}
}
