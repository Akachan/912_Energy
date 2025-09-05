#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor
{
    [CustomEditor(typeof(KnowledgeSo))]
    public class KnowledgeSoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var so = (KnowledgeSo)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Importación CSV", EditorStyles.boldLabel);

            if (GUILayout.Button("Importar desde CSV (URL o Archivo)"))
            {
                ImportFromCsv(so);
            }

            if (GUILayout.Button("Limpiar datos importados"))
            {
                ClearImportedData(so);
            }
        }

        private static void ClearImportedData(KnowledgeSo so)
        {
            if (so == null) return;

            so.SetImportedRows(new List<LevelKnowledgeRow>());
            so.BuildCache();

            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Limpiar datos", "Los datos importados y el caché han sido limpiados.", "OK");
        }

    
        // Punto de entrada solicitado
        public static void ImportFromCsv(KnowledgeSo so)
        {
            if (so == null)
            {
                Debug.LogError("KnowledgeSo es nulo.");
                return;
            }

            try
            {
                string csvText = TryLoadCsvText(so.CsvUrl);

                if (string.IsNullOrWhiteSpace(csvText))
                {
                    // Si no hay URL o falló la descarga, permitir elegir un archivo local
                    string path = EditorUtility.OpenFilePanel("Selecciona CSV", "", "csv");
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        csvText = File.ReadAllText(path, DetectEncoding(path));
                    }
                }

                if (string.IsNullOrWhiteSpace(csvText))
                {
                    EditorUtility.DisplayDialog("Importación CSV", "No se pudo obtener contenido CSV (URL vacía o archivo no válido).", "OK");
                    return;
                }

                var result = ParseCsvToRows(csvText, out var warnings);

                if (result == null || result.Count == 0)
                {
                    EditorUtility.DisplayDialog("Importación CSV", "No se importaron filas válidas. Revisa el formato y los encabezados.", "OK");
                    return;
                }

                // Reemplaza las filas en el ScriptableObject
                so.SetImportedRows(result);
                so.BuildCache();

                EditorUtility.SetDirty(so);
                AssetDatabase.SaveAssets();

                var msg = $"Importación exitosa: {result.Count} filas.";
                if (warnings.Count > 0)
                    msg += $"\nAdvertencias:\n- {string.Join("\n- ", warnings.Take(10))}" + (warnings.Count > 10 ? "\n(...)" : "");

                EditorUtility.DisplayDialog("Importación CSV", msg, "OK");
                Debug.Log(msg);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error al importar CSV: {ex.Message}\n{ex}");
                EditorUtility.DisplayDialog("Importación CSV", $"Error al importar CSV:\n{ex.Message}", "OK");
            }
        }

        // Descarga o lee el CSV desde la ruta/URL
        private static string TryLoadCsvText(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            source = source.Trim();

            try
            {
                // URL http/https
                if (source.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    return DownloadStringSync(source);
                }

                // file://
                if (source.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(source);
                    string path = uri.LocalPath;
                    if (File.Exists(path))
                        return File.ReadAllText(path, DetectEncoding(path));
                    return null;
                }

                // Ruta local
                if (File.Exists(source))
                {
                    return File.ReadAllText(source, DetectEncoding(source));
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"No se pudo leer CSV desde '{source}': {e.Message}");
            }

            return null;
        }

        // Descarga síncrona (Editor) con soporte TLS moderno
        private static string DownloadStringSync(string url)
        {
            try
            {
                // Primero intentamos con UnityWebRequest
                using (var req = UnityWebRequest.Get(url))
                {
                    var op = req.SendWebRequest();
                    while (!op.isDone)
                    {
                        // Bloqueo simple en Editor (evita freeze total)
                        System.Threading.Thread.Sleep(10);
                    }

#if UNITY_2020_2_OR_NEWER
                    if (req.result == UnityWebRequest.Result.Success)
#else
                if (!req.isNetworkError && !req.isHttpError)
#endif
                    {
                        return req.downloadHandler.text;
                    }

                    Debug.LogWarning($"UnityWebRequest falló ({req.responseCode}): {req.error}. Intentando WebClient...");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"UnityWebRequest Exception: {e.Message}. Intentando WebClient...");
            }

            // Fallback: WebClient
            try
            {
#pragma warning disable SYSLIB0014
                using (var wc = new WebClient())
                {
                    // Forzar TLS1.2 si es necesario
                    try
                    {
                        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                    }
                    catch { /* Ignorar si no aplica */ }

                    wc.Encoding = Encoding.UTF8;
                    return wc.DownloadString(url);
                }
#pragma warning restore SYSLIB0014
            }
            catch (Exception e2)
            {
                Debug.LogWarning($"WebClient falló: {e2.Message}");
                return null;
            }
        }

        private static Encoding DetectEncoding(string path)
        {
            // Simple: intenta detectar BOM UTF8; si no, usa UTF8 por defecto
            using (var fs = File.OpenRead(path))
            {
                if (fs.Length >= 3)
                {
                    int b1 = fs.ReadByte();
                    int b2 = fs.ReadByte();
                    int b3 = fs.ReadByte();
                    if (b1 == 0xEF && b2 == 0xBB && b3 == 0xBF)
                        return new UTF8Encoding(true);
                }
            }
            return new UTF8Encoding(false);
        }

        private static List<LevelKnowledgeRow> ParseCsvToRows(string csvText, out List<string> warnings)
        {
            warnings = new List<string>();

            var lines = csvText
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (lines.Count == 0)
            {
                warnings.Add("El CSV está vacío.");
                return new List<LevelKnowledgeRow>();
            }

            // Localiza la primera línea que parezca encabezado
            int headerIndex = 0;
            while (headerIndex < lines.Count && string.IsNullOrWhiteSpace(lines[headerIndex]))
                headerIndex++;

            if (headerIndex >= lines.Count)
            {
                warnings.Add("No se encontró encabezado.");
                return new List<LevelKnowledgeRow>();
            }

            string headerLine = TrimBom(lines[headerIndex]);
            char delimiter = DetectDelimiter(headerLine);

            var rawHeaders = SplitCsvLine(headerLine, delimiter);
            var headers = rawHeaders.Select(NormalizeHeader).ToList();

            int levelIdx = headers.FindIndex(h => h == "level" || h == "nivel");
            int kpsIdx = headers.FindIndex(h => h == "kps");
            int costIdx = headers.FindIndex(h => h == "cost"); // opcional

            if (levelIdx < 0 || kpsIdx < 0)
            {
                throw new Exception($"Encabezados requeridos: Level/Nivel y KPS. Encontrados: {string.Join(", ", rawHeaders)}. Delimitador detectado: '{delimiter}'");
            }

            var rows = new List<LevelKnowledgeRow>();
            for (int i = headerIndex + 1; i < lines.Count; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var cols = SplitCsvLine(line, delimiter);

                // Si la línea tiene menos columnas que el máximo índice requerido, la saltamos
                int minCols = Math.Max(levelIdx, Math.Max(kpsIdx, costIdx >= 0 ? costIdx : -1)) + 1;
                if (cols.Count < minCols)
                {
                    warnings.Add($"Línea {i + 1}: columnas insuficientes. Se esperaban al menos {minCols}, hay {cols.Count}.");
                    continue;
                }

                // Parseo Level
                if (!int.TryParse(CleanField(cols[levelIdx]), NumberStyles.Integer, CultureInfo.InvariantCulture, out int level))
                {
                    warnings.Add($"Línea {i + 1}: Level inválido ('{cols[levelIdx]}').");
                    continue;
                }

                // Parseo KPS
                if (!TryParseBigNumber(CleanField(cols[kpsIdx]), out var kps))
                {
                    warnings.Add($"Línea {i + 1}: KPS inválido ('{cols[kpsIdx]}').");
                    continue;
                }

                // Parseo Cost (opcional)
                BigNumber cost = default;
                if (costIdx >= 0)
                {
                    string rawCost = CleanField(cols[costIdx]);
                    if (!string.IsNullOrWhiteSpace(rawCost))
                    {
                        if (!TryParseBigNumber(rawCost, out cost))
                        {
                            warnings.Add($"Línea {i + 1}: Cost inválido ('{cols[costIdx]}'). Se usará valor por defecto 0.");
                            cost = default;
                        }
                    }
                }

                rows.Add(new LevelKnowledgeRow { Level = level, KPS = kps, Cost = cost });
            }

            return rows;
        }

        private static string TrimBom(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            if (s.Length > 0 && s[0] == '\uFEFF')
                return s.Substring(1);
            return s;
        }

        private static string NormalizeHeader(string h)
        {
            if (h == null) return string.Empty;
            h = TrimBom(h).Trim();
            if (h.Length >= 2 && h[0] == '"' && h[h.Length - 1] == '"')
                h = h.Substring(1, h.Length - 2);
            return h.Trim().ToLowerInvariant();
        }

        private static string CleanField(string f)
        {
            if (f == null) return string.Empty;
            f = f.Trim();
            if (f.Length >= 2 && f[0] == '"' && f[f.Length - 1] == '"')
            {
                // Deshacer doble comilla dentro de campo entrecomillado
                f = f.Substring(1, f.Length - 2).Replace("\"\"", "\"");
            }
            return f.Trim();
        }

        private static char DetectDelimiter(string headerLine)
        {
            int commas = headerLine.Count(c => c == ',');
            int semicolons = headerLine.Count(c => c == ';');
            int tabs = headerLine.Count(c => c == '\t');

            // Mayoría simple
            if (tabs >= commas && tabs >= semicolons) return '\t';
            if (semicolons > commas) return ';';
            return ','; // por defecto coma
        }

        // Split CSV con soporte de comillas y escapes ("")
        private static List<string> SplitCsvLine(string line, char delimiter)
        {
            var list = new List<string>();
            if (line == null)
            {
                list.Add(string.Empty);
                return list;
            }

            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        // Doble comilla -> carácter escapado
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i++; // salta la segunda comilla
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == delimiter)
                    {
                        list.Add(sb.ToString());
                        sb.Length = 0;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            list.Add(sb.ToString());
            return list;
        }

        // Conversión flexible a BigNumber (intenta varias firmas comunes por reflexión)
        private static bool TryParseBigNumber(string s, out BigNumber value)
        {
            value = null;

            if (string.IsNullOrWhiteSpace(s))
                return false;

            var normalized = s.Trim().Replace(',', '.');

            try
            {
                value = Calculator.TransformToBigNumber(normalized);
                return value != null;
            }
            catch
            {
                value = null;
                return false;
            }
        }
    }
}
#endif