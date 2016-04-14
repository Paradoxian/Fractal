using UnityEngine;
using System.Collections;

public class TerrainScript : MonoBehaviour {
	Mesh mesh;
	float minx =  1e10f;
	float maxx = -1e10f;
	float SCALE = 0.25f;
	float minz = -1e10f;
	float maxz =  1e10f;
	float mu;
	float mv;
	float bu;
	float bv;
	float roughness = 0.5f; // set as a floating point number from 0-1
	int VERT;
	int Count;
	float [,] heights;
	public int maxSize = 50;
	Vector3[] points;
	Vector2[] uv;
	float [,] fractal (float [,] height, float roughness){
		int maxSize = height.GetLength (1);
		float [,] newHeights = new float[maxSize * 2 -1,maxSize * 2 -1];
		for (int row = 0 ; row < maxSize; row++){
			for (int col = 0; col < maxSize; col++){
				newHeights[row * 2,col * 2] = height[row,col];
			}
		}
		//sets the directions for each cardinal direction N,E,S,W,NE,NW,SW,SE
		Vector2 [] direction = {new Vector2(0,1), new Vector2 (1,1), new Vector2(1,0), new Vector2(1,-1),
					new Vector2(0,-1),new Vector2(-1,-1), new Vector2(-1,0), new Vector2(-1,1)};
		//Sets the number of points every pass
		int newSize = maxSize * 2 - 1;
		//Doubles the points for the diagonal directions
		for (int row = 1; row < newSize; row += 2){
			for (int col = 1; col < newSize; col += 2){
				float total = 0;
				int count = 0;
				//Handles directions 1,3,5,7
				for (int d = 1; d < 8; d += 2){
					int r0 = (int)(row + direction[d].y);
					int c0 = (int)(col + direction[d].x);
					if (r0 >= 0 && r0 < newSize && c0 >= 0 && c0 < newSize){
						total += newHeights[r0,c0];
						count++;
					}
				}
				//average calculations	
				float average = total / count;
				//set displacement value
				float displace = 0;
				//for rows 1,3,5,7 do the averages
				for (int d = 1; d < 8; d += 2){
					int r0 = (int)(row + direction[d].y);
					int c0 = (int)(col + direction[d].x);
					if (r0 >= 0 && r0 < newSize && c0 >= 0 && c0 < newSize){
						displace += Mathf.Abs(newHeights[r0,c0] - average);
					}
				}
				//final displacement calculation
				displace /= count;
				//set the new points after the first fractal pass
				newHeights[row , col] = average + displace * roughness * Random.value * Mathf.Sign(Random.value -0.5f);
			}
		}
		//for rows 0,2,4,6,8
		for (int row = 0; row < newSize; row ++){
			for (int col = 1 - row % 2; col < newSize; col += 2){
				float total = 0;
				int count = 0;
				//for directions 0,2,4,6,8 set the heights and increment the count
				for (int d = 0; d < 8; d += 2){
					int r0 = (int)(row + direction[d].y);
					int c0 = (int)(col + direction[d].x);
					if (r0 >= 0 && r0 < newSize && c0 >= 0 && c0 < newSize){
						total += newHeights[r0,c0];
						count++;
					}
				}
				//create average
				float average = total / count;
				
				float displace = 0;
				//for rows 0,2,4,6,8 calculate the average
				for (int d = 0; d < 8; d += 2){
					int r0 = (int)(row + direction[d].y);
					int c0 = (int)(col + direction[d].x);
					if (r0 >= 0 && r0 < newSize && c0 >= 0 && c0 < newSize){
						displace += Mathf.Abs(newHeights[r0,c0] - average);
					}
				}
				//set the displacement
				displace /= count;
				//set the points
				newHeights[row , col] = average + displace * roughness * Random.value * Mathf.Sign(Random.value -0.5f);
			}
		}
		//return the final points after fractal
		return newHeights;
}
	void Start () {
		MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
		mesh = new Mesh ();
		heights = new float[maxSize,maxSize];
		int idx = 0;

		heights = new float [maxSize,maxSize];
		for (int row = 0 ;row < maxSize; row++) {
			for (int col = 0 ;col < maxSize; col++) {
				heights[row,col] = 4 * Mathf.Sin((row) * 0.1f);
			}
		}
		//Two fractal pass calls
		heights = fractal (heights, roughness);
		maxSize = heights.GetLength(0);
		heights = fractal (heights, roughness);
		maxSize = heights.GetLength(0);

		VERT = maxSize * maxSize;

		uv = new Vector2 [maxSize * maxSize];
		Count = 6 * ((maxSize - 1) * (maxSize - 1));
		points = new Vector3 [maxSize * maxSize];

		for (int row = 0; row < maxSize; row++){
			for(int col = 0; col < maxSize; col++){
				points[idx++] = new Vector3 (col * SCALE, heights[row,col], row * SCALE);
			}
		}
		//Set the UV values
		Debug.Log ("Set UVs");
		for (int i = 0; i < maxSize; i++){
			for (int j = 0; j < maxSize; j++){
				if(points[i].x > maxx){
					maxx = points[i].x;
				}
				if(points[i].x < minx){
					minx = points[i].x;
				}
				if(points[i].z > maxz){
					maxz = points[i].z;
				}
				if(points[i].z < minz){
					minz = points[i].z;
				}
			}
		}
		//calculate the lines
		mu = 1/(maxx - minx);
		bu = 0 - minx * mu;

		mv = 1/(maxz - minz);
		bv = 0 - minz * mv;
		for (int i = 0; i < VERT; i++){
			uv[i] = new Vector2 (points[i].x * mu + bu, points[i].z * mv + bv);
		}
		int index = 0;
		//create the trangles from points
		int[] triangle = new int[Count];
		for (int i = 0; i < maxSize-1; i++) {
			for (int j = 0; j < maxSize-1; j++) {
				int first = (maxSize) * i + j;

				triangle[index ++] = first + maxSize;
				triangle[index ++] = first + 1;
				triangle[index ++] = first;



				triangle[index ++] = first + maxSize + 1;
				triangle[index ++] = first + 1;
				triangle[index ++] = first + maxSize;

				if (first + maxSize + 2 >= points.Length){
				}
			}
		}
		mesh.vertices = points;
		mesh.uv = uv;
		mesh.triangles = triangle;
		mesh.RecalculateNormals ();
		mesh.name = "Terrain Mesh";
		filter.mesh = mesh;
	}


	// Update is called once per frame
	void Update () {
	
	}
}
