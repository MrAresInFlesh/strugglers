using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData generateTerrainMesh(float[,] p_heightMap, float p_heightFactor, AnimationCurve p_heightCurve)
    {
        AnimationCurve heightCurve = new AnimationCurve(p_heightCurve.keys);

        float heightSpawnLimiterDown = 1.5f;
        float heightSpawnLimiterUp = 5.5f;
        float mapSpawnLimiter = 115f;

        int meshsi = 1;
        int borderedSize = p_heightMap.GetLength(0);
        int meshSize = borderedSize - 2 * meshsi;

        // to make the position independant of the generation of the mesh
        int meshSizeUnsimplified = borderedSize - 2;

        // centering mesh with (width - 1) / -2
        float topLeftX = (meshSizeUnsimplified - 1) / -2f;
        float topLeftZ = (meshSizeUnsimplified - 1) / 2f;

        int verticesPerLine = (meshSize - 1) / meshsi + 1;
        MeshData meshData = new MeshData(verticesPerLine);

        int[,] vertexIndicesMap = new int[borderedSize, borderedSize];

        int meshVertexIndex = 0;
        int borderVertexIndex = -1;

        for (int y = 0; y < borderedSize; y += meshsi)
        {
            for (int x = 0; x < borderedSize; x += meshsi)
            {
                // verifying if the encounters the vertex is in the border of the map
                bool isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

                if (isBorderVertex)
                {
                    vertexIndicesMap[x, y] = borderVertexIndex;
                    borderVertexIndex--;
                }
                else
                {
                    vertexIndicesMap[x, y] = meshVertexIndex;
                    meshVertexIndex++;
                }
            }
        }

        for (int y = 0; y < borderedSize; y += meshsi)
        {
            for (int x = 0; x < borderedSize; x += meshsi)
            {
                int vertexIndex = vertexIndicesMap[x, y];

                // we give to the uvs the position in percent
                // meshData.uvs[vertexIndex] = new Vector2(x / (float)borderedSize, y /(float)borderedSize);
                // the idea here is to center the uvs map by taking off the increment as an offset of the coordinates.
                Vector2 percent = new Vector2((x - meshsi) / (float)meshSize, (y - meshsi) / (float)meshSize);
                float height = heightCurve.Evaluate(p_heightMap[x, y]) * p_heightFactor;

                Vector3 vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);

                meshData.addVertex(vertexPosition, percent, vertexIndex);

                // positions of the vertex containing water
                if (vertexPosition.y >= 0 && vertexPosition.y <= 0.17) meshData.waterPositions.Add(vertexPosition);
                // positions of the vertex with eligible spawn positions
                if (vertexPosition.y >= heightSpawnLimiterDown && vertexPosition.y < heightSpawnLimiterUp && vertexPosition.x < mapSpawnLimiter && vertexPosition.x > -mapSpawnLimiter && vertexPosition.z > -mapSpawnLimiter && vertexPosition.z < mapSpawnLimiter) meshData.spawnPositions.Add(vertexPosition);

                if (x < borderedSize - 1 && y < borderedSize - 1)
                {
                    int a = vertexIndicesMap[x, y];
                    int b = vertexIndicesMap[x + meshsi, y];
                    int c = vertexIndicesMap[x, y + meshsi];
                    int d = vertexIndicesMap[x + meshsi, y + meshsi];

                    meshData.addTriangle(a, d, c);
                    meshData.addTriangle(d, a, b);
                }

                vertexIndex++;

            }
        }

        meshData.bakeNormals();

        return meshData;
    }
}

public class MeshData
{
    public List<Vector3> spawnPositions = new List<Vector3>();
    public List<Vector3> waterPositions = new List<Vector3>();

    Vector3[] vertices, borderVertices, bakedNormals;
    int[] triangles, borderTriangles;
    Vector2[] uvs;
    int triangleIndex, borderTriangleIndex;

    public MeshData(int verticesPerLine)
    {
        vertices = new Vector3[verticesPerLine * verticesPerLine];
        uvs = new Vector2[verticesPerLine * verticesPerLine];
        triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

        borderVertices = new Vector3[verticesPerLine * 4 + 4];
        borderTriangles = new int[24 * verticesPerLine];
    }
    public void addVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            borderVertices[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }
    public void addTriangle(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            borderTriangles[borderTriangleIndex] = a;
            borderTriangles[borderTriangleIndex + 1] = b;
            borderTriangles[borderTriangleIndex + 2] = c;
            borderTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }
    Vector3[] calculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;

        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = surfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        int borderTriangleCount = borderTriangles.Length / 3;

        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = borderTriangles[normalTriangleIndex];
            int vertexIndexB = borderTriangles[normalTriangleIndex + 1];
            int vertexIndexC = borderTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = surfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if (vertexIndexA >= 0) vertexNormals[vertexIndexA] += triangleNormal;
            if (vertexIndexB >= 0) vertexNormals[vertexIndexB] += triangleNormal;
            if (vertexIndexC >= 0) vertexNormals[vertexIndexC] += triangleNormal;
        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }
    Vector3 surfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = (indexA < 0) ? borderVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? borderVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? borderVertices[-indexC - 1] : vertices[indexC];
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }
    public void bakeNormals()
    {
        bakedNormals = calculateNormals();
    }
    public Mesh createMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = bakedNormals;
        return mesh;
    }

}
