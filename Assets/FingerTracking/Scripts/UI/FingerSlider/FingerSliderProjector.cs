using UnityEngine;

namespace FingerTracking.UI.FingerSlider
{

    public class FingerSliderProjector
    {
        private Vector3[] jointPositions;

        private float[] magnitudes;

        private float[] jointRanges;
        private float SliderRange { get { return jointRanges[jointRanges.Length - 1]; } }

        private float[] edgeValues;
        private float[] sliderValues;

        float maxPointerDistance;

        public FingerSliderProjector(Vector3[] jointPositions, float maxPointerDistance)
        {
            this.jointPositions = jointPositions;
            this.maxPointerDistance = maxPointerDistance;

            magnitudes = new float[jointPositions.Length - 1];

            jointRanges = new float[jointPositions.Length];

            edgeValues = new float[jointPositions.Length - 1];
            sliderValues = new float[jointPositions.Length - 1];
        }

        public float GetValue(Vector3 pointer)
        {
            UpdateMagnitudes();

            int firstEdge = -1;
            float pointerDistance = Mathf.Infinity;

            for (int i = 0; i < jointPositions.Length - 1; i++)
            {
                Vector3 a = jointPositions[i];
                Vector3 b = jointPositions[i + 1];
                float mag = magnitudes[i];

                float edgeValue = Vector3.Dot(pointer - a, b - a) / (mag * mag);

                if (i == 0) edgeValue = Mathf.Max(edgeValue, 0);
                if (i == jointPositions.Length - 2) edgeValue = Mathf.Min(edgeValue, 1);

                edgeValues[i] = edgeValue;

                if (edgeValue >= 0 && edgeValue <= 1)
                {
                    if (firstEdge < 0) firstEdge = i;

                    var pointOnEdge = a + edgeValue * (b - a);
                    var dist = Vector3.Distance(pointOnEdge, pointer);
                    if (dist < pointerDistance) pointerDistance = dist;

                    sliderValues[i] = (jointRanges[i] + edgeValue * mag) / SliderRange;
                }
                else sliderValues[i] = float.NaN;
            }

            if (firstEdge >= 0)
            {
                if (pointerDistance > maxPointerDistance)
                    return float.NaN;

                if (
                    firstEdge == sliderValues.Length - 1
                    || float.IsNaN(sliderValues[firstEdge + 1])
                )
                    return sliderValues[firstEdge];

                return GetConcaveGapValue(firstEdge, pointer);
            }
            else return GetConvexGapValue(pointer);
        }

        private void UpdateMagnitudes()
        {
            float sum = 0f;
            for (int i = 0; i < jointPositions.Length - 1; i++)
            {
                float mag = (jointPositions[i + 1] - jointPositions[i]).magnitude;
                magnitudes[i] = mag;
                jointRanges[i] = sum;
                sum += mag;
            }
            jointRanges[jointPositions.Length - 1] = sum;
        }

        private float GetConvexGapValue(Vector3 pointer)
        {
            for (int i = 0; i < edgeValues.Length; i++)
                if (edgeValues[i] < 0)
                {
                    var dist = Vector3.Distance(jointPositions[i], pointer);
                    return dist <= maxPointerDistance
                        ? jointRanges[i] / SliderRange
                        : float.NaN;
                }

            return float.NaN;
        }

        private float GetConcaveGapValue(int id, Vector3 pointer)
        {
            var val0 = sliderValues[id];
            var val1 = sliderValues[id + 1];

            var bound0 = GetBoundVector(id, pointer);
            var bound1 = GetBoundVector(id + 1, pointer);

            var toPoint = jointPositions[id + 1] - pointer;

            var fullAngle = Vector3.Angle(bound0, bound1);
            var angle = Vector3.Angle(bound0, toPoint);

            var weight = 1f - angle / fullAngle;

            return val0 + weight * (val1 - val0);
        }

        private Vector3 GetBoundVector(int id, Vector3 pointer)
        {
            var a = jointPositions[id];
            var b = jointPositions[id + 1];
            var ab = b - a;
            var cross = Vector3.Cross(ab, pointer - b);
            return Vector3.Cross(ab, cross);
        }

    }

}
