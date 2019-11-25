/*
 * https://jekel.me/2015/Least-Squares-Sphere-Fit/
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;

namespace SphericalRegressionMATH {

    public class SphericalRegressionAccord {

        #region Public Fileds
        private List<UnityEngine.Vector3> _data;
        public List<UnityEngine.Vector3> Data {
            get {
                return _data;
            }
            set {
                _data = value;
            }
        }

        public UnityEngine.Vector3 center;
        public double radius;
        public double radius_sdev;
        #endregion

        #region Public Methods
        public void Fit() {
            double[,] A = new double[Data.Count, 4];
            for (int i = 0; i < Data.Count; i++) {
                A[i, 0] = 2 * Data[i].x;
                A[i, 1] = 2 * Data[i].y;
                A[i, 2] = 2 * Data[i].z;
                A[i, 3] = 1d;
            }

            double[,] f = new double[Data.Count, 1];
            for (int i = 0; i < Data.Count; i++) {
                f[i, 0] = 
                    (Data[i].x * Data[i].x) + 
                    (Data[i].y * Data[i].y) + 
                    (Data[i].z * Data[i].z);
            }

            double[,] x = { { 0, 0, 0, 1 } };
            try {
                x = A.Solve(f, leastSquares: true);
            } catch (Exception e) {
                Debug.Log("ERROR: " + e.Message);
            }

            center[0] = (float)x[0, 0];
            center[1] = (float)x[1, 0];
            center[2] = (float)x[2, 0];

            radius = CalcRadius(x);
            radius_sdev = CalcRadiusError();
        }
        #endregion

        #region Private Method
        private double CalcRadius(double[,] x) {
            double r;

            r = Math.Sqrt(
                (x[3, 0] +
                    (x[0, 0] * x[0, 0]) +
                    (x[1, 0] * x[1, 0]) +
                    (x[2, 0] * x[2, 0])
                ));

            return r;
        }

        private double CalcRadiusError() {
            double delta = 0d;
            double sum = 0d;
            int n = Data.Count;

            foreach (UnityEngine.Vector3 point in Data) {
                delta = (point - center).magnitude - radius;
                sum += delta * delta;
            }

            return Math.Sqrt(sum / (n * (n - 1d)));
        }
        #endregion

        #region Print
        private void PrintArray(double[] array) {
            int rowLength = array.GetLength(0);

            Debug.Log("\t");
            for (int i = 0; i < rowLength; i++) {
                    Debug.Log(string.Format("{0:0.00} ", array[i]));
            }
            Debug.Log(Environment.NewLine + Environment.NewLine);
        }

        private void PrintMatrix(double[,] matrix) {
            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);

            for (int i = 0; i < rowLength; i++) {
                Debug.Log("\t");
                for (int j = 0; j < colLength; j++) {
                    Debug.Log(string.Format("{0:0.00} ", matrix.GetValue(i, j)));
                }
                Debug.Log(Environment.NewLine + Environment.NewLine);
            }
        }
        #endregion
    }
}
