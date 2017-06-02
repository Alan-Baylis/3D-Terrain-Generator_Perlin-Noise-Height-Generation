﻿using UnityEngine;
using System.Collections;

/*Creates a grid of vertices as a Mesh*/

public static class MeshGenerator {

	public static VertexData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail) {
		AnimationCurve heightCurve = new AnimationCurve (_heightCurve.keys);

		int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail * 2;

		int borderedSize = heightMap.GetLength (0);
		int meshSize = borderedSize - 2*meshSimplificationIncrement;
		int meshSizeUnsimplified = borderedSize - 2;

		float topLeftX = (meshSizeUnsimplified - 1) / -2f;
		float topLeftZ = (meshSizeUnsimplified - 1) / 2f;


		int verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

		VertexData meshData = new VertexData (verticesPerLine);

		int[,] vertexIndicesMap = new int[borderedSize,borderedSize];
		int meshVertexIndex = 0;
		int borderVertexIndex = -1;

		for (int y = 0; y < borderedSize; y += meshSimplificationIncrement) {
			for (int x = 0; x < borderedSize; x += meshSimplificationIncrement) {
				bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

				if (isBorderVertex) {
					vertexIndicesMap [x, y] = borderVertexIndex;
					borderVertexIndex--;
				} else {
					vertexIndicesMap [x, y] = meshVertexIndex;
					meshVertexIndex++;
				}
			}
		}

		for (int y = 0; y < borderedSize; y += meshSimplificationIncrement) {
			for (int x = 0; x < borderedSize; x += meshSimplificationIncrement) {
				int vertexIndex = vertexIndicesMap [x, y];
				Vector2 percent = new Vector2 ((x-meshSimplificationIncrement) / (float)meshSize, (y-meshSimplificationIncrement) / (float)meshSize);
				float height = heightCurve.Evaluate (heightMap [x, y]) * heightMultiplier;
				Vector3 vertexPosition = new Vector3 (topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);

				meshData.AddVertex (vertexPosition, percent, vertexIndex);

				if (x < borderedSize - 1 && y < borderedSize - 1) {
					int a = vertexIndicesMap [x, y];
					int b = vertexIndicesMap [x + meshSimplificationIncrement, y];
					int c = vertexIndicesMap [x, y + meshSimplificationIncrement];
					int d = vertexIndicesMap [x + meshSimplificationIncrement, y + meshSimplificationIncrement];
					meshData.AddTriangle (a,d,c);
					meshData.AddTriangle (d,a,b);
				}

				vertexIndex++;
			}
		}

		return meshData;

	}
}

public class VertexData {
	Vector3[] vertexBuffer;
	int[] indexBuffer;
	Vector2[] uvCoords;

	Vector3[] borderVertexArray;
	int[] borderIndexArray;

	int triangleIndex;
	int borderTriangleIndex;

	public VertexData(int verticesPerLine) {
		vertexBuffer = new Vector3[verticesPerLine * verticesPerLine];
		uvCoords = new Vector2[verticesPerLine * verticesPerLine];
		indexBuffer = new int[(verticesPerLine-1)*(verticesPerLine-1)*6];

		borderVertexArray = new Vector3[verticesPerLine * 4 + 4];
		borderIndexArray = new int[24 * verticesPerLine];
	}

	public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex) {
		if (vertexIndex < 0) {
			borderVertexArray [-vertexIndex - 1] = vertexPosition;
		} else {
			vertexBuffer [vertexIndex] = vertexPosition;
			uvCoords [vertexIndex] = uv;
		}
	}

	public void AddTriangle(int a, int b, int c) {
		if (a < 0 || b < 0 || c < 0) {
			borderIndexArray [borderTriangleIndex] = a;
			borderIndexArray [borderTriangleIndex + 1] = b;
			borderIndexArray [borderTriangleIndex + 2] = c;
			borderTriangleIndex += 3;
		} else {
			indexBuffer [triangleIndex] = a;
			indexBuffer [triangleIndex + 1] = b;
			indexBuffer [triangleIndex + 2] = c;
			triangleIndex += 3;
		}
	}

	Vector3[] CalculateNormals() {

		Vector3[] vertexNormals = new Vector3[vertexBuffer.Length];
		int triangleCount = indexBuffer.Length / 3;
		for (int i = 0; i < triangleCount; i++) {
			int normalTriangleIndex = i * 3;
			int vertexIndexA = indexBuffer [normalTriangleIndex];
			int vertexIndexB = indexBuffer [normalTriangleIndex + 1];
			int vertexIndexC = indexBuffer [normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices (vertexIndexA, vertexIndexB, vertexIndexC);
			vertexNormals [vertexIndexA] += triangleNormal;
			vertexNormals [vertexIndexB] += triangleNormal;
			vertexNormals [vertexIndexC] += triangleNormal;
		}

		int borderTriangleCount = borderIndexArray.Length / 3;
		for (int i = 0; i < borderTriangleCount; i++) {
			int normalTriangleIndex = i * 3;
			int vertexIndexA = borderIndexArray [normalTriangleIndex];
			int vertexIndexB = borderIndexArray [normalTriangleIndex + 1];
			int vertexIndexC = borderIndexArray [normalTriangleIndex + 2];

			Vector3 triangleNormal = SurfaceNormalFromIndices (vertexIndexA, vertexIndexB, vertexIndexC);
			if (vertexIndexA >= 0) {
				vertexNormals [vertexIndexA] += triangleNormal;
			}
			if (vertexIndexB >= 0) {
				vertexNormals [vertexIndexB] += triangleNormal;
			}
			if (vertexIndexC >= 0) {
				vertexNormals [vertexIndexC] += triangleNormal;
			}
		}


		for (int i = 0; i < vertexNormals.Length; i++) {
			vertexNormals [i].Normalize ();
		}

		return vertexNormals;

	}

	Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC) {
		Vector3 pointA = (indexA < 0)?borderVertexArray[-indexA-1] : vertexBuffer [indexA];
		Vector3 pointB = (indexB < 0)?borderVertexArray[-indexB-1] : vertexBuffer [indexB];
		Vector3 pointC = (indexC < 0)?borderVertexArray[-indexC-1] : vertexBuffer [indexC];

		Vector3 sideAB = pointB - pointA;
		Vector3 sideAC = pointC - pointA;
		return Vector3.Cross (sideAB, sideAC).normalized;
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh ();
		mesh.vertices = vertexBuffer;
		mesh.triangles = indexBuffer;
		mesh.uv = uvCoords;
		mesh.normals = CalculateNormals ();
		return mesh;
	}

}