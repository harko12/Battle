using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderWall : MonoBehaviour
{
    public float Height = 30f;
    public ColliderWallSegment[] Points;

    public bool generate = false;

    private void Update()
    {
        if (generate)
        {
            generate = false;
            Generate();
        }
    }

    public void Generate()
    {
        UpdateWalls();
    }

    void UpdateWalls()
    {
        if (Points.Length > 1)
        {
            var length = Points.Length;
            for (int lcv = 0; lcv < length; lcv++)
            {
                var segment = Points[lcv];
                var point = segment.transform;
                point.rotation = Quaternion.identity;
                point.position.Set(point.position.x, 0, point.position.z);
                var pos1 = point.position;
                var nextIndex = lcv + 1;
                if (nextIndex == length)
                {
                    nextIndex = 0;
                }
                var pos2 = Points[nextIndex].transform.position;
                var dir = pos2 - pos1;
                var dist = Vector3.Distance(pos1, pos2) * .5f;
                var pos3 = pos1 + ((pos2 - pos1) / 2);
                var boxCollider = GenerateBox(segment, pos1, pos2);
//                Debug.DrawLine(pos1, pos2, Color.cyan);
//                Debug.DrawLine(pos3, pos3 + dir.normalized * 50, Color.red);
            }
        }
    }

    private BoxCollider GenerateBox(ColliderWallSegment go, Vector3 point1, Vector3 point2)
    {
        // You can change that line to provide another MeshFilter
        MeshFilter filter = go.GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();
        #region Vertices
        /*
        Debug.DrawLine(point1, point2);
        Debug.DrawLine(point1, point1 + new Vector3(0, Height, 0));
        Debug.DrawLine(point2, point2 + new Vector3(0, Height, 0));
        Debug.DrawLine(point1 + new Vector3(0, Height, 0), point2 + new Vector3(0, Height, 0));
        */
        var dist = Vector3.Distance(point1, point2);
        var rotToPoint = Quaternion.LookRotation(point2 - point1); // the rotation we will want to apply to the mesh after the collider was put on

        // make the mesh oriented to vector3.forward so the bounds dont' get too wide
        var point3 = point1 + Vector3.forward * dist;
        point1 = go.transform.InverseTransformPoint(point1);
        point3 = go.transform.InverseTransformPoint(point3);

        Vector3 p0 = point1;
        Vector3 p1 = point1;
        Vector3 p2 = point3;
        Vector3 p3 = point3;

        Vector3 p4 = point1 + new Vector3(0, Height, 0);
        Vector3 p5 = point1 + new Vector3(0, Height, 0);
        Vector3 p6 = point3 + new Vector3(0, Height, 0);
        Vector3 p7 = point3 + new Vector3(0, Height, 0);



        Vector3[] vertices = new Vector3[]
        {
	// Bottom
	p0, p1, p2, p3,
 
	// Left
	p7, p4, p0, p3,
 
	// Front
	p4, p5, p1, p0,
 
	// Back
	p6, p7, p3, p2,
 
	// Right
	p5, p6, p2, p1,
 
	// Top
	p7, p6, p5, p4
        };
        #endregion

        #region Normales
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 front = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;

        Vector3[] normales = new Vector3[]
        {
	// Bottom
	down, down, down, down,
 
	// Left
	left, left, left, left,
 
	// Front
	front, front, front, front,
 
	// Back
	back, back, back, back,
 
	// Right
	right, right, right, right,
 
	// Top
	up, up, up, up
        };
        #endregion

        #region UVs
        Vector2 _00 = new Vector2(0f, 0f);
        Vector2 _10 = new Vector2(1f, 0f);
        Vector2 _01 = new Vector2(0f, 1f);
        Vector2 _11 = new Vector2(1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
	// Bottom
	_11, _01, _00, _10,
 
	// Left
	_11, _01, _00, _10,
 
	// Front
	_11, _01, _00, _10,
 
	// Back
	_11, _01, _00, _10,
 
	// Right
	_11, _01, _00, _10,
 
	// Top
	_11, _01, _00, _10,
        };
        #endregion

        #region Triangles
        int[] triangles = new int[]
        {
	// Bottom
	3, 1, 0,
    3, 2, 1,			
 
	// Left
	3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
    3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	// Front
	3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
    3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	// Back
	3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
    3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	// Right
	3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
    3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	// Top
	3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
    3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

        };
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        StartCoroutine(ResetCollider(go, rotToPoint));
        return null;
    }

    IEnumerator ResetCollider(ColliderWallSegment go, Quaternion rotation)
    {
        var coll = go.GetComponent<BoxCollider>();
        Destroy(coll);
        yield return 0;
        coll = go.gameObject.AddComponent<BoxCollider>() as BoxCollider;
        go.transform.localRotation = rotation; // rotate the wall to the right orientation after the collider gets put on
    }
}
