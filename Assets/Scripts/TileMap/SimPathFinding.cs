using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimPathFinding : MonoBehaviour
{
    [Header("输入数据")]
    [SerializeField] Vector2Int _startCell = Vector2Int.zero;
    [SerializeField] Vector2Int _endCell = Vector2Int.zero;
    [SerializeField] bool _doPathFinding = false;
    [SerializeField] bool _eraseRoute = false;

    [Header("输出数据")]
    [SerializeField] List<Vector2> _passedPoints = new List<Vector2>();
	[SerializeField] List<Vector2Int> _passedCells = new List<Vector2Int>();

    [HideInInspector] public bool[,] map = new bool[0, 0];
    [HideInInspector] public MapData data;

	/// <summary>
	/// 获取从start单元格到end单元格的路径
	/// </summary>
	/// <param name="start">起点单元格坐标</param>
	/// <param name="end">终点单元格坐标</param>
	/// <param name="passedPoints">路线所经过的点，世界坐标</param>
	/// <param name="passedCells">路线所经过的单元格，单元格坐标</param>
	/// <returns>是否有从start到end的路径</returns>
	bool GetRoute(Vector2Int start, Vector2Int end, out List<Vector2> passedPoints, out List<Vector2Int> passedCells)
    {
        passedPoints = new List<Vector2>();
        passedCells = new List<Vector2Int>();
        return true;
    }

    /// <summary>
    /// passedPoints所指示的路径是否可以合法连通
    /// </summary>
    bool IsValidRoute(List<Vector2> passedPoints)
	{
        return true;
	}

    /// <summary>
    /// passedCells所指示的路径是否可以合法连通
    /// </summary>
    bool IsValidRoute(List<Vector2Int> passedCells)
    {
        return true;
    }

    private void OnDrawGizmos()
    {
        if (_doPathFinding)
        {
            _doPathFinding = false;
            GetRoute(_startCell, _endCell, out _passedPoints, out _passedCells);
        }

        if (_eraseRoute)
        {
            _eraseRoute = false;
            _passedPoints.Clear();
            _passedCells.Clear();
        }

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Gizmos.color =
                    _endCell == new Vector2Int(x, y) ? Color.magenta :
                    _startCell == new Vector2Int(x, y) ? Color.green :
                    _passedCells.Contains(new Vector2Int(x, y)) ? Color.yellow :
                    map[x, y] ? Color.red : Color.white;

                var center = transform.position + new Vector3(x, y, 0);
                Gizmos.DrawCube(center, Vector3.one);
            }
        }

        Gizmos.color =
            !IsValidRoute(_passedCells) ? Color.red :
            !IsValidRoute(_passedPoints) ? Color.green :
            Color.black;

        for (int i = 1; i < _passedPoints.Count; i++)
        {
            Vector3 p1 = transform.position + (Vector3)_passedPoints[i - 1];
            Vector3 p2 = transform.position + (Vector3)_passedPoints[i];
            Gizmos.DrawLine(p1, p2);
        }
    }
}

[System.Serializable]
public struct MapData
{
    [SerializeField] int count;
    [SerializeField] bool[] data;

    public void Save(bool[,] map)
    {
        count = map.GetLength(0);
        if (map.Length != data.Length)
            data = new bool[map.Length];

        for (int i = 0; i < map.Length; i++)
            data[i] = map[i / count, i % count];
    }

    public bool[,] Load()
    {
        if (count == 0)
            return new bool[0, 0];
        bool[,] map = new bool[count, data.Length / count];
        for (int i = 0; i < data.Length; i++)
            map[i / count, i % count] = data[i];

        return map;
    }
}

#region EDITOR
#if UNITY_EDITOR
[CustomEditor(typeof(SimPathFinding))]
public class RandomTileEditor : Editor
{
    int width;
    int height;

	private void OnEnable()
    {
        var spf = target as SimPathFinding;
        spf.map = spf.data.Load();

        width = spf.map.GetLength(0);
        height = spf.map.GetLength(1);
    }

	private void OnDisable()
	{
        var spf = target as SimPathFinding;
        spf.data.Save(spf.map);
    }

	public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("地图");
        EditorGUILayout.BeginHorizontal();
        width = EditorGUILayout.DelayedIntField(width);
        height = EditorGUILayout.DelayedIntField(height);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        var spf = (target as SimPathFinding);
        var map = spf.map;

        if (map.GetLength(0) != width || map.GetLength(1) != height)
        {
            var newMap = new bool[width, height];
			for (int x = 0; x < width && x < map.GetLength(0); x++)
			{
				for (int y = 0; y < height && y < map.GetLength(1); y++)
				{
                    newMap[x, y] = map[x, y];
				}
			}

            map = newMap;
		}

        var orig = false;
        var changed = false;
        EditorGUILayout.BeginHorizontal(GUILayout.Width(2));
		for (int x = 0; x < width; x++)
		{
            EditorGUILayout.BeginVertical(GUILayout.Height(2));
			for (int y = height-1; y >= 0; y--)
			{
                orig = map[x, y];
                map[x, y] = EditorGUILayout.Toggle(map[x, y]);
                if (orig != map[x, y])
                    changed = true;
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck() || changed)
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            spf.map = map;
            spf.data.Save(spf.map);
        }
    }
}
#endif
#endregion