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
					SetExplainText("按<color=#00cc00>方向键</color>移动\n透明的<color=#0088ff>星星</color>只有晚上才能捡到，要把所有星星拿到才行哦");
					break;
				case 1:
					SetExplainText("移动时不能越过<color=#0088ff>障碍物</color>和<color=#0088ff>边界</color>");
					break;
				case 2:
					SetExplainText("把<color=#0088ff>箱子</color>推到<color=#0088ff>魔法阵</color>上就能打开<color=#0088ff>宝箱</color>\n宝箱里也有星星哦");
					break;
				case 3:
					SetExplainText("进了<color=#0088ff>虫洞</color>就会被传送到另一端\n必须在<color=#ff0000>日落</color>前返回营地哦");
					break;
				case 4:
					SetExplainText("按下 <color=#00cc00>Tab 键</color>可以切换操作的人物");
					break;
				case 5:
					SetExplainText("有的箱子到了夜晚也会继续存在");
					break;
				case 12:
					SetExplainText("<color=#ff8800>恭喜你，通过了 demo 的所有关卡</color>");
					m_stepNumberText.gameObject.SetActive(false);
					return;
				default:
					SetExplainText("加油吧，少年");
					break;
			}

			m_stepNumberText.text = LevelController.Instance.Step.ToString(CultureInfo.InvariantCulture);

		}

		public void SetExplainText(string explainText) {
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
