using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshEditor : EditorWindow
{
    public GameObject currentGameObject;
    //public MeshData meshData;

    public Mesh mesh;
    public Vector3[] verts;
    public Vector3 vertPos;
    public GameObject[] handles;

    public bool[] showVerts;
    public bool[] showHandles;

    private const string TAG_HANDLE = "VertHandle";

    public bool _destroy;
    public bool hasData;

    [MenuItem("Tools/Mesh Editor")]
    static void Init()
    {
        MeshEditor window = (MeshEditor)EditorWindow.GetWindow(typeof(MeshEditor), false, "Mesh Editor");
        window.Show();
    }

    private void OnEnable()
    {
        hasData = false;
        if (Selection.activeGameObject != null)
            GetData();
    }

    private void GetData()
    {
        currentGameObject = Selection.activeGameObject;
        if (Selection.activeGameObject.GetComponent<MeshFilter>() != null)
        {
            hasData = true;
            mesh = currentGameObject.GetComponent<MeshFilter>().sharedMesh;
            verts = mesh.vertices;
            showVerts = new bool[verts.Length];
            foreach (Vector3 vert in verts)
            {
                vertPos = currentGameObject.transform.TransformPoint(vert);
                GameObject handle = new GameObject(TAG_HANDLE);
                //         handle.hideFlags = HideFlags.DontSave;
                handle.transform.position = vertPos;
                handle.transform.parent = currentGameObject.transform;
                handle.tag = TAG_HANDLE;
                handle.AddComponent<VertHandleGizmo>()._parent = this;
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Mesh Vertex Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        _destroy = EditorGUILayout.Toggle("Active", _destroy);

        if(!_destroy)
            DrawGameObjectData();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if(GUILayout.Button("Clear Data"))
        {
            hasData = false;
        }
    }

    private void DrawGameObjectData()
    {
        EditorGUILayout.LabelField("Mesh Vertex Editor", EditorStyles.boldLabel);
        EditorGUILayout.ObjectField("GameObject", currentGameObject, typeof(GameObject), true);
        if (hasData)
        {
            EditorGUILayout.Vector3Field("vertPos", vertPos);
            EditorGUILayout.IntField("VertCount", verts.Length);
            for (int i = 0; i < verts.Length; i++)
            {
                showVerts[i] = EditorGUILayout.Foldout(showVerts[i], i.ToString(), true, EditorStyles.foldout);
                if (showVerts[i])
                {
                    verts[i] = EditorGUILayout.Vector3Field("vert", verts[i]);
                }
            }
        }
        else
        {
            EditorGUILayout.Vector3Field("vertPos", Vector3.zero);
            EditorGUILayout.IntField("VertCount", 0);
        }
    }

    private void Update()
    {
        if (!_destroy)
        {
            if (Selection.activeGameObject != null)
            {
                if (!hasData && Selection.activeGameObject.GetComponent<MeshFilter>() != null)
                {
                    GetData();
                }
            }
            else
            {
                GameObject[] handles = GameObject.FindGameObjectsWithTag(TAG_HANDLE);
                foreach (GameObject handle in handles)
                {
                    DestroyImmediate(handle);
                }
            }

            if (hasData)
            {
                handles = GameObject.FindGameObjectsWithTag(TAG_HANDLE);

                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i] = handles[i].transform.localPosition;
                }

                mesh.vertices = verts;
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
            }
        }
        else
        {
            foreach (GameObject handle in handles)
            {
                DestroyImmediate(handle);
            }
        }
        Repaint();
    }
}

[System.Serializable]
public class MeshData
{
    public Mesh mesh;
    public Vector3[] verts;
    public Vector3 vertPos;
    public GameObject[] handles;
}

[ExecuteInEditMode]
public class VertHandleGizmo : MonoBehaviour
{

    private static float CURRENT_SIZE = 0.1f;

    public float _size = CURRENT_SIZE;
    public MeshEditor _parent;
    public bool _destroy;

    private float _lastKnownSize = CURRENT_SIZE;

    void Update()
    {
        // Change the size if the user requests it
        if (_lastKnownSize != _size)
        {
            _lastKnownSize = _size;
            CURRENT_SIZE = _size;
        }

        // Ensure the rest of the gizmos know the size has changed...
        if (CURRENT_SIZE != _lastKnownSize)
        {
            _lastKnownSize = CURRENT_SIZE;
            _size = _lastKnownSize;
        }

        if (_destroy)
            DestroyImmediate(_parent);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, Vector3.one * CURRENT_SIZE);
    }

}
