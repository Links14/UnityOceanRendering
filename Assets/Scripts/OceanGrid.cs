using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class OceanGrid : MonoBehaviour
{
    [SerializeField] private int resolution = 100;
    [SerializeField] private float length = 100f;

    [SerializeField] private MeshFilter meshFilter;

    private const int spacePerQuad = 6; // 1 quad needs space for 2 tris. each tri is 3 pts,
                                        // we store the index of each pt for re-use

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        if (resolution <= 0) return;

        int verticiesPerSide = resolution + 1;

        // normal 1D array, use indexing equation to treat as 2D
        Vector3[] vertices = new Vector3[verticiesPerSide * verticiesPerSide];
        // grid is thought of as quads, but there will be twice as many triangles as quads.
        //   So though there are 4 pts per quad, we need 6 pts for the two triangles that make one quad
        int[] triangles = new int[resolution * resolution * spacePerQuad];

        // generate verticies
        for (int row = 0; row < verticiesPerSide; row++)
        {
            for (int col = 0; col < verticiesPerSide; col++)
            {
                int index = col * verticiesPerSide + row;

                float percentX = (float)col / resolution;
                float percentZ = (float)row / resolution;

                // - 0.5f offsets the point to 50% in the negative direction such that the
                //   center is the origin instead of the bottom left
                float worldX = (percentX - 0.1f) * length; // 50% from edge
                float worldZ = (percentZ - 0.1f) * length; // 10% from edge

                vertices[index] = new Vector3(worldX, 0f, worldZ);
            }
        }

        // generate triangles
        int triangleIndex = 0;

        for (int row = 0; row < resolution; row++)
        {
            for (int col = 0; col < resolution; col++)
            {
                // identify quad
                // current index
                int bottomLeft = row * verticiesPerSide + col;
                int bottomRight = bottomLeft + 1; // to the right of current index
                int topLeft = bottomLeft + verticiesPerSide; // in the row above the current index
                int topRight = topLeft + 1; // in the row above and one to the right of the current index

                // split into triangles
                // create lower left triangle - counter-clockwise winding
                triangles[triangleIndex] = topLeft;
                triangleIndex++;
                triangles[triangleIndex] = bottomLeft;
                triangleIndex++;
                triangles[triangleIndex] = bottomRight;
                triangleIndex++;

                // create upper right triangle - counter-clockwise winding
                triangles[triangleIndex] = topLeft;
                triangleIndex++;
                triangles[triangleIndex] = bottomRight;
                triangleIndex++;
                triangles[triangleIndex] = topRight;
                triangleIndex++;
            }
        }

        Mesh mesh = new()
        {
            name = "Ocean Grid",
            vertices = vertices,
            triangles = triangles
        };

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
