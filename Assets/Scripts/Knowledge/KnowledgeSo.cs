using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct LevelKnowledgeRow
{
    public int Level;
    public BigNumber KPS;   // Texto en notación científica, p. ej.: "1.5e3"
    public BigNumber Cost;
}

//Este struct es para cuardar los datos en el diccionario (el Nivel será la Key del Dict)
[Serializable]
public struct LevelKnowledgeData
{
    public BigNumber KPS;
    public BigNumber Cost;
}

[CreateAssetMenu(fileName = "New Knowledge", menuName = "Knowledge")]
public class KnowledgeSo : ScriptableObject
{
    [Header("Importación CSV")]
    [Tooltip("URL pública del CSV para este tipo de Energy Source (por ejemplo, .../pub?output=csv).")]
    [SerializeField] private string csvUrl;

    [Tooltip("Filas importadas desde CSV (una por nivel).")]
    [SerializeField] private List<LevelKnowledgeRow> rows;

    [NonSerialized] private Dictionary<int, LevelKnowledgeData> _cache;    
    
    public string CsvUrl
    {
        get => csvUrl;
        set => csvUrl = value;
    }

    //Esto devuelve las filas... será necesario?
    public IReadOnlyList<LevelKnowledgeRow> Rows => rows;

    public int GetMaxLevel()
    {
        return rows.Count > 0 ? rows[rows.Count - 1].Level : 0;
    }
    public BigNumber GetKps(int level)
    {
        return TryGetKnowledgeData(level, out var data) ? data.KPS : new BigNumber(0, 0);
    }

    public BigNumber GetKnowledgeCost(int level)
    {
        return TryGetKnowledgeData(level, out var data) ? data.Cost : new BigNumber(0, 0);
    }
    
    public void BuildCache()
    {
        _cache = new Dictionary<int, LevelKnowledgeData>(rows.Count);
        for (int i = 0; i < rows.Count; i++)
        {
            //transformo
            var r = rows[i];
            _cache[r.Level] = new LevelKnowledgeData() { KPS = r.KPS, Cost = r.Cost };
        }
    }

    //Asegura que haya caché, si no hay, lo crea.
    private void EnsureCache()
    {
       
          if (_cache == null)
         
            BuildCache();
        
    }

    //Te devuelve el DataLevel dado un nivel
    public bool TryGetKnowledgeData(int nivel, out LevelKnowledgeData data)
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
    public void SetImportedRows(List<LevelKnowledgeRow> newRows)
    {
        _cache = null;
       
        
        rows = new List<LevelKnowledgeRow>();
        rows = newRows;
       
        
        
        
        
    }
#endif
}
