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
					SetExplainText("�е�͸��������ֻ�����ϲŻ����\nҪ�����е����Ƕ��õ�����Ŷ");
					break;
				case 1:
					SetExplainText("�����ϰ���ͱ߽�Ļ����ͻᶯ����");
					break;
				case 2:
					SetExplainText("�������Ƶ�̤���ϣ����ܴ򿪱��䣬������Ҳ������Ŷ");
					break;
				case 3:
					SetExplainText("���˴����ţ��ͻᱻ���͵���һ��");
					break;
				case 4:
					SetExplainText("���� Tab �����Ϳ����л���Ŷ����ע�������䲽��");
					break;
				case 6:
					SetExplainText("�е����ӵ���ҹ��Ҳ���������");
					break;
				default:
					SetExplainText("���Ͱɣ�����");
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
			StartCoroutine(CoRotateClock(360f / totalStep, 0.1f, isUndo ? 1 : -1, d));
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
