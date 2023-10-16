using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    ShapeGenerator shapeGenerator;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.axisA = new Vector3(localUp.y,localUp.z,localUp.x);
        this.axisB = Vector3.Cross(localUp,axisA);
    }

    public void ConstructMesh() {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution-1) * (resolution-1) * 2 * 3];
        Vector2[] uv = mesh.uv; // store the existing biome color information before rebuilding mesh

        int triIndex = 0;
        for (int y=0; y<resolution; y++) {
            for (int x=0; x<resolution; x++) {
                int index = x + y * resolution;
                Vector2 percent = new Vector2(x,y) / (resolution-1);
                Vector3 pointOnUnitCube = localUp + SmoothVertexPosition(percent.x * 2 - 1) * axisA + SmoothVertexPosition(percent.y * 2 - 1) * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[index] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                if (x != resolution-1 && y != resolution-1) {
                    triangles[triIndex++] = index;
                    triangles[triIndex++] = index + resolution + 1;
                    triangles[triIndex++] = index + resolution;
                    triangles[triIndex++] = index;
                    triangles[triIndex++] = index + 1;
                    triangles[triIndex++] = index + resolution + 1;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv; // re-assign the saved biome color information
    }

    // adjusts vertices to be more uniform along the seams between faces when normalized to a sphere
    private float SmoothVertexPosition(float pos) {
        // return pos;
        // return (1 - Mathf.Cos(Mathf.PI * pos / 2)) * pos / Mathf.Abs(pos);
        return Mathf.Tan(pos * Mathf.PI / 4);
    }

    // update biome colors for each point based on position
    public void UpdateUVs(ColorGenerator colorGenerator) {
        Vector2[] uv = new Vector2[resolution * resolution];

        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {
                int index = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + SmoothVertexPosition(percent.x * 2 - 1) * axisA + SmoothVertexPosition(percent.y * 2 - 1) * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                uv[index] = new Vector2(colorGenerator.BiomePercentFromPoint(pointOnUnitSphere),0);
            }
        }
        mesh.uv = uv;
    }
}
