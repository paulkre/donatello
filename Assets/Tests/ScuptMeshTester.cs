﻿using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;

using VRSculpting.SculptMesh;
using VRSculpting.Settings;
using VRSculpting.Sculptor;
using VRSculpting.Tools;
using VRSculpting.SculptMesh.Modification;

namespace Tests {

	public class ScuptMeshTester {
		
		private SculptMesh sculptMesh;

		[UnityTest, Order(0)]
		public IEnumerator BuildMesh() {
			var timer = new Timer("build-mesh");

			Mesh mesh = null;

			timer.PrintTime(() => mesh = IcoSphereCreator.Create(7, .5f), "Create mesh");

			var wrapper = new GameObject().AddComponent<MeshWrapperBehaviour>();
			timer.PrintTime(() => sculptMesh = new SculptMesh(wrapper, mesh), "Parse mesh");

			timer.PrintTotalTime();
			timer.SaveCsv();

			Assert.AreEqual(163842, mesh.vertices.Length);

			yield return null;
		}

		[UnityTest, Order(1)]
		public IEnumerator ModifyMesh() {
			var timer = new Timer("modify-mesh");

			var menu = new Menu(ToolType.Move);

			var deformer = new Deformer(sculptMesh);

			var state = new SculptState {
				position = Vector3.up * .5f,
				worldToLocal = sculptMesh.Wrapper.MeshTransform.worldToLocalMatrix
			};

			timer.PrintTime(() => {
				deformer.UpdateMask(state);
			}, "Select vertices");

			Vector3 offset = Vector3.up * Random.value;
			int pickId = Random.Range(0, deformer.Selection.Length);

			var vec = sculptMesh.Points[deformer.Selection[pickId]];
			var weight = deformer.Weights[pickId];

			timer.PrintTime(() => {
				var deformation = deformer.Deformation;
				for (int i = 0; i < deformer.SelectionCount; ++i)
					deformation[i] = offset;
			}, "Create deform field");

			timer.PrintTime(() => sculptMesh.UpdateMeshData(), "Update mesh data");

			Debug.Log($"{deformer.SelectionCount} vertices modified out of {sculptMesh.Points.Length}.\n");

			timer.PrintTotalTime();
			timer.SaveCsv();

			Assert.Less(timer.TotalTime, 100);
			Assert.AreEqual(sculptMesh.Points[deformer.Selection[pickId]], vec + weight * offset);

			yield return null;
		}

	}
}
