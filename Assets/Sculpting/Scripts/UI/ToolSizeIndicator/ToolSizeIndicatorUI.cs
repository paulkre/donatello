using UnityEngine;

namespace VRSculpting.UI.ToolSizeIndicator {
	using Settings;

	[RequireComponent(typeof(LineRenderer))]
	public class ToolSizeIndicatorUI : UI {

		private static int resolution = 64;
		private static float step = (2 * Mathf.PI) / resolution;
		private static float lineWidth = .001f;
		private static float activeDuration = .5f;

		private LineRenderer lineRenderer;
		private Vector3[] points;

		private float alpha = 0.0f;
		private float lastChange;

		private float Radius {
			set {
				transform.localScale = Vector3.one * value;
			}
		}

		public override void Init(Menu menu) {
			points = new Vector3[resolution + 2];
			for (int i = 0; i < points.Length; i++) {
				float angle = i * step;
				points[i] = new Vector3(
					Mathf.Sin(angle),
					Mathf.Cos(angle)
				);
			}
			lineRenderer = GetComponent<LineRenderer>();
			lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;
			lineRenderer.positionCount = points.Length;
			lineRenderer.SetPositions(points);

			Radius = menu.ToolSize.Value / 2;
			menu.ToolSize.OnChange += (param, lastValue) => {
				Radius = param.Value / 2;
				lastChange = Time.time;
			};
		}

		private void Update() {
			transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - transform.position);

			float targetAlpha = Time.time - lastChange <= activeDuration ? 1f : 0f;
			alpha += (targetAlpha - alpha) * .25f;
			lineRenderer.startColor = lineRenderer.endColor = new Color(1, 1, 1, alpha);
		}

	}

}

