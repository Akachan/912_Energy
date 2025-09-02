using System;
using System.Collections.Generic;
using UnityEngine;

//Este struct es para levantar las filas del CVS
[Serializable]
public struct LevelRow
{
    public int Level;
    public BigNumber EPS;   // Texto en notación científica, p. ej.: "1.5e3"
    public BigNumber Cost;
}

//Este struct es para cuardar los datos en el diccionario (el Nivel será la Key del Dict)
[Serializable]
public struct LevelData
{
    public BigNumber EPS;
    public BigNumber Cost;
}

[CreateAssetMenu(fileName = "New EnergySource", menuName = "EnergySource")]
public partial class EnergySourceSo : ScriptableObject
{
    [SerializeField] private string energySourceName;
    [SerializeField] private BigNumber costToUnlock;
    
    [Header("Importación CSV")]
    [Tooltip("URL pública del CSV para este tipo de Energy Source (por ejemplo, .../pub?output=csv).")]
    [SerializeField] private string csvUrl;

    [Tooltip("Filas importadas desde CSV (una por nivel).")]
    [SerializeField] private List<LevelRow> rows = new List<LevelRow>();

    [NonSerialized] private Dictionary<int, LevelData> _cache;

    public string CsvUrl
    {
        get => csvUrl;
        set => csvUrl = value;
    }

    //Esto devuelve las filas... será necesario?
    public IReadOnlyList<LevelRow> Rows => rows;

    public int GetMaxLevel()
    {
        return rows.Count > 0 ? rows[rows.Count - 1].Level : 0;
    }
    public BigNumber GetEps(int level)
    {
        return TryGetLevelData(level, out var data) ? data.EPS : new BigNumber(0, 0);
    }

    public BigNumber GetCost(int level)
    {
        return TryGetLevelData(level, out var data) ? data.Cost : new BigNumber(0, 0);
    }
    
    public BigNumber GetCostToUnlock()
    {
        return costToUnlock;
    }

    public string EnergySourceName => energySourceName;
    
    public void BuildCache()
    {
        _cache = new Dictionary<int, LevelData>(rows.Count);
        for (int i = 0; i < rows.Count; i++)
        {
            //transformo
            var r = rows[i];
            _cache[r.Level] = new LevelData { EPS = r.EPS, Cost = r.Cost };
        }
    }

    //Asegura que haya caché, si no hay, lo crea.
    private void EnsureCache()
    {
        if (_cache == null)
            BuildCache();
    }

    //Te devuelve el DataLevel dado un nivel
    public bool TryGetLevelData(int nivel, out LevelData data)
    {
        EnsureCache();
        return _cache.TryGetValue(nivel, out data);
    }

    //Vacía el caché
    public void ClearImportedRows()
    {
        rows.Clear();
        _cache = null;
    }

#if UNITY_EDITOR
    // Método usado por el Editor para reemplazar todas las filas de una vez.
    public void SetImportedRows(List<LevelRow> newRows)
    {
        rows = newRows ?? new List<LevelRow>();
        _cache = null;
    }
#endif
}