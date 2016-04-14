using UnityEngine;
using System.Collections;

public class PoolMeshCreate : MonoBehaviour {
	Mesh mesh;
	const float displacement = 16.0f;
	const float multiplier = displacement / 2;
	const int ring = 10;
	const int sector = 16;
	Vector3[] vertex;
	Vector3[,] rings;
	// Use this for initialization
	void Start () {
		MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
		mesh = new Mesh ();
		vertex = new Vector3[ring  * (sector + 1)];
		rings = new Vector3[ring, (sector +1)];
		Vector2[] uv = new Vector2[ring * (sector + 1)];

		for (int i = 0; i < ring; i++) {
			for (int j = 0; j <= sector; j++) {
				float imrad = Mathf.Deg2Rad * j * 360 / sector;
				vertex [i * (sector + 1) + j] = new Vector3 (Mathf.Cos (imrad) * i, -displacement + (Mathf.Pow(i,0.25f) * multiplier), Mathf.Sin (imrad) * i);
				rings[i,j] = vertex[i * (sector + 1) + j];
				uv[i * (sector + 1) +j] = new Vector2(j/sector,i/ring);

			}
		}
		int index = 0;
		int[] triangle = new int[ring * 6 * sector];
		for (int i = 0; i < ring-1; i++) {
			for (int j = 0; j < sector; j++) {
				int first = (sector + 1) * i + j;
				triangle[index ++] = first + sector + 1;
				triangle[index ++] = first;
				triangle[index ++] = first + 1;
				triangle[index ++] = first + sector + 2;
				triangle[index ++] = first + sector + 1;
				triangle[index ++] = first + 1;
				Debug.Log ("idx" + index + "I:" + i + "J:" + j);
			}
		}
		mesh.vertices = vertex;
		mesh.uv = uv;
		mesh.triangles = triangle;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		filter.mesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
