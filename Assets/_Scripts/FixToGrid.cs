using System.Threading.Tasks;
using UnityEngine;

public class FixToGrid : MonoBehaviour
{
    [SerializeField] private bool snapOnEnable = true;
    public static Grid GeneralGrid { 
        get {
            if (_grid == null)
                _grid = GameManager.Instance.GeneralGrid;
            return _grid;
        }
    }
    private static Grid _grid;
    private void OnEnable()
    {
        if(snapOnEnable)
            SnapToGrid();
    }
    public static Vector3Int SnapToGrid(Transform t)
    {
        Vector3Int cell = GeneralGrid.WorldToCell(t.position);
        t.position = GeneralGrid.GetCellCenterWorld(cell);
        return cell;
    }
    [ContextMenu("Snap To Grid")]
    public async void SnapToGrid()
    {
        await Task.Delay(100);
        Vector3Int cell = GeneralGrid.WorldToCell(transform.position);
        transform.position = GeneralGrid.GetCellCenterWorld(cell);
    }
}
