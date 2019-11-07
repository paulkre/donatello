using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;

using VRSculpting.SculptMesh;
using VRSculpting.SculptMesh.Modification;

namespace Tests {

	public class ScuptMeshTester {
		
		private ISculptMesh sculptMesh;

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

			var deformer = new Deformer(sculptMesh);

			timer.PrintTime(() => {
				deformer.UpdateMask(Vector3.up * .5f, .25f, 2f);
			}, "Select vertices");

			Vector3 offset = Vector3.up * Random.value;
			int pickId = Random.Range(0, deformer.Mask.Length);

			var vec = sculptMesh.Points[deformer.Mask[pickId]];
			var weight = deformer.Weights[pickId];

			timer.PrintTime(() => {
				var deformation = deformer.Deformation;
				for (int i = 0; i < deformer.MaskCount; ++i)
					deformation[i] = offset;
			}, "Create deform field");

			timer.PrintTime(() => sculptMesh.HandleDeformation(), "Apply deform field");

			timer.PrintTime(() => sculptMesh.UpdateMeshData(), "Update mesh data");

			Debug.Log($"{deformer.MaskCount} vertices modified out of {sculptMesh.Points.Length}.\n");

			timer.PrintTotalTime();
			timer.SaveCsv();

			Assert.Less(timer.TotalTime, 100);
			Assert.AreEqual(sculptMesh.Points[deformer.Mask[pickId]], vec + weight * offset);

			yield return null;
		}

	}
}
