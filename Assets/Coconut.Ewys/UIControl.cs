using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Coconut.Ewys {
	/// <summary>
	/// Used to control UI 
	/// </summary>
	public class UIControl : MonoBehaviour {
		public static UIControl Instance;

		[SerializeField]
		GameObject m_explainInfo;

		[SerializeField]
		/// <summary>
		/// the transform of hour hand
		/// </summary>
		RectTransform m_hourHandTransform;

		[SerializeField]
		Text m_stepNumberText;
		[SerializeField]
		Text m_explainText;

		private void Awake() {
			Instance = this;
			switch (LevelController.CurrentLevel) {
				case 0:
					SetExplainText("有点透明的星星只有晚上才会出现\n要把所有的星星都拿到才行哦");
					break;
				case 1:
					SetExplainText("碰到障碍物和边界的话，就会动不了");
					break;
				case 2:
					SetExplainText("把箱子推到踏板上，就能打开宝箱，宝箱里也有星星哦");
					break;
				case 3:
					SetExplainText("进了传送门，就会被传送到另一边");
					break;
				case 4:
					SetExplainText("按下 Tab 键，就可以切换人哦，请注意合理分配步数");
					break;
				case 6:
					SetExplainText("有的箱子到了夜晚也会继续存在");
					break;
				default:
					SetExplainText("加油吧，少年");
					break;
			}

			m_stepNumberText.text = LevelController.Instance.Step.ToString(CultureInfo.InvariantCulture);

		}

		void SetExplainText(string explainText) {
			this.m_explainText.text = explainText;
			//LevelController.
		}

		/// <summary>
		/// Restart the game
		/// </summary>
		public void Remake() {
			SceneManager.LoadScene("level");
		}

		public void Return() {
			SceneManager.LoadScene("Menu");
		}

		public void HideExplainInfo() {
			m_explainInfo.SetActive(false);
		}

		void RotateClock(int remainStep, int totalStep, bool isUndo, FlagAtomDelegate d) {
			m_stepNumberText.text = remainStep.ToString();
			StartCoroutine(CoRotateClock(360f / totalStep, 0.5f, isUndo ? 1 : -1, d));
		}

		IEnumerator CoRotateClock(float angle, float time, int dir, FlagAtomDelegate d) {
			float number = time / Time.fixedDeltaTime;
			float nextAngle = dir * angle / number;
			for (int i = 0; i < number; i++) {
				m_hourHandTransform.transform.Rotate(new Vector3(0, 0, nextAngle));
				yield return new WaitForFixedUpdate();
			}
			d();
		}

		public class RotateClockAtom : OperationAtom {
			readonly int remainStep;
			readonly int totalStep;
			readonly bool isUndo;

			public RotateClockAtom(int remainStep, int totalStep, bool isUndo = false) {
				this.remainStep = remainStep;
				this.totalStep = totalStep;
				this.isUndo = isUndo;
			}

			protected override void DoImpl(FlagAtomDelegate d) {
				Instance.RotateClock(remainStep, totalStep, isUndo, d);
			}

			protected override void UndoImpl(FlagAtomDelegate d) => throw new NotSupportedException();
		}
	}
}
