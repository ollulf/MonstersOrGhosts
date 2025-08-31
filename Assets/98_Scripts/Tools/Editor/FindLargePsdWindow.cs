#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FindLargePsdWindow : EditorWindow
{
    // ----------------- Einstellungen / State -----------------
    private float thresholdMb = 50f;
    private string searchRoot = "Assets";
    private Vector2 scroll;
    private bool isBusy = false;

    // Toggle: Guard A - Resources schützen
    private bool protectResources = true;

    // Anzeige: Anzahl verwaister .meta
    private int orphanMetaCount = -1;

    // Dateiendungen: Gruppen + per-Extension-Flags (für Größen-/Referenz-Scan & Duplikat-Scan)
    private class ExtGroup { public string Name; public string[] Exts; public bool Foldout = true; }
    private List<ExtGroup> extGroups;
    private Dictionary<string, bool> extFlags; // ".png" => true/false
    private string newExtInput = "";

    private List<FileEntry> results = new List<FileEntry>();

    // ---------- Duplikate ----------
    private class DuplicateGroup
    {
        public string Hash;          // SHA-256 hex
        public long Size;            // Bytes
        public List<string> Paths = new List<string>(); // Assets-relative
        public int CanonicalIndex = 0;
        public bool Foldout = false;
    }
    private List<DuplicateGroup> dupGroups = new List<DuplicateGroup>();
    private string dupScanInfo = "—";

    // ----------------- Datenstruktur Eintrag -----------------
    private class FileEntry
    {
        public string AssetPath;
        public long Bytes;
        public List<string> Scenes = new List<string>();
        public List<string> Prefabs = new List<string>();
        public bool Foldout;

        // Markierung: liegt unter Resources/
        public bool IsInResources;

        // Replacement (für Texturen/Sprites sinnvoll)
        public UnityEngine.Object Replacement; // Texture2D / Sprite / Spritesheet
        public bool MapSpritesByName = true;
    }

    private struct ReplaceResult
    {
        public string ScenePath;
        public int ComponentChanges;
        public int MaterialInstances;
        public int RendererMaterialsChanged;
        public bool Any => ComponentChanges > 0 || MaterialInstances > 0 || RendererMaterialsChanged > 0;
    }

    private struct Totals { public int comp; public int matInst; public int rend; }

    private class ReplacementConfig
    {
        public UnityEngine.Object Replacement;
        public bool MapByName;
    }

    // ----------------- Init / Menü -----------------
    [MenuItem("Tools/Find Large Assets")]
    public static void ShowWindow()
    {
        var w = GetWindow<FindLargePsdWindow>("Find Large Assets");
        w.minSize = new Vector2(1080, 740);
        w.EnsureExtUIInitialized();
        Defer(() =>
        {
            w.RefreshOrphanMetaCount();
        });
    }

    private void OnEnable()
    {
        EnsureExtUIInitialized();
    }

    private void EnsureExtUIInitialized()
    {
        if (extGroups != null && extFlags != null) return;

        // Gruppen definieren
        extGroups = new List<ExtGroup>
        {
            new ExtGroup { Name = "Bilder", Exts = new[]{
                ".psd", ".psb", ".png", ".jpg", ".jpeg", ".tga", ".tif", ".tiff", ".exr", ".hdr", ".bmp", ".gif", ".svg", ".webp"
            }},
            new ExtGroup { Name = "Audio", Exts = new[]{
                ".wav", ".mp3", ".ogg", ".aiff", ".aif", ".flac"
            }},
            new ExtGroup { Name = "Video", Exts = new[]{
                ".mp4", ".mov", ".avi", ".m4v", ".webm", ".mkv"
            }},
            new ExtGroup { Name = "Modelle", Exts = new[]{
                ".fbx", ".obj", ".gltf", ".glb", ".dae", ".3ds"
            }},
        };

        // Flags initialisieren (Bilder standardmäßig an, Rest aus)
        extFlags = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        foreach (var g in extGroups)
        {
            bool defaultOn = g.Name == "Bilder";
            foreach (var ext in g.Exts)
                extFlags[ext] = defaultOn;
        }
    }

    // ----------------- Helpers: Defer & Busy -----------------
    private static void Defer(Action a) { EditorApplication.delayCall += () => a(); }
    private void RunDeferred(Action action)
    {
        if (isBusy) return;
        isBusy = true;
        Defer(() =>
        {
            try { action(); }
            finally { isBusy = false; Repaint(); }
        });
    }

    private static bool IsInResourcesPath(string p)
        => p.IndexOf("/Resources/", StringComparison.OrdinalIgnoreCase) >= 0;

    // ----------------- GUI -----------------
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Suche nach großen Assets (Bilder / Audio / Video / Modelle) + Duplikate", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            thresholdMb = EditorGUILayout.FloatField(new GUIContent("Schwelle (MB)"), Mathf.Max(0.1f, thresholdMb));
            GUILayout.Space(20);
            protectResources = EditorGUILayout.ToggleLeft(
                new GUIContent("Resources-Ordner schützen", "Wenn aktiv, gelten Assets unter einem Resources/-Pfad nie als unreferenziert und Duplikate dort werden nicht automatisch gelöscht."),
                protectResources, GUILayout.Width(230));
        }

        // Root + Scan
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(new GUIContent("Wurzelordner"), GUILayout.Width(90));
            searchRoot = EditorGUILayout.TextField(searchRoot);
            using (new EditorGUI.DisabledScope(isBusy))
            {
                if (GUILayout.Button("Scan (Größe/Ref)", GUILayout.Width(150)))
                    RunDeferred(() =>
                    {
                        Scan();
                        RefreshOrphanMetaCount();
                    });

                if (GUILayout.Button("Duplikate scannen", GUILayout.Width(150)))
                    RunDeferred(() =>
                    {
                        ScanDuplicates();
                        RefreshOrphanMetaCount();
                    });
            }
        }

        EditorGUILayout.Space(4);

        // Statuszeile: Treffer + verwaiste .meta (mit Refresh)
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField($"Treffer (Größe>Schwelle): {results.Count}", EditorStyles.miniLabel, GUILayout.Width(220));

            string metaLabel = orphanMetaCount < 0 ? "—" : orphanMetaCount.ToString();
            EditorGUILayout.LabelField($"Verwaiste .meta: {metaLabel}", EditorStyles.miniLabel, GUILayout.Width(180));

            using (new EditorGUI.DisabledScope(isBusy))
            {
                if (GUILayout.Button("↻ .meta zählen", GUILayout.Width(120)))
                    RunDeferred(RefreshOrphanMetaCount);
            }

            GUILayout.FlexibleSpace();
            if (isBusy) EditorGUILayout.LabelField("Arbeite …", EditorStyles.miniLabel, GUILayout.Width(80));
        }

        EditorGUILayout.Space(4);

        // Dateitypen-Auswahl (Checkboxes) – betrifft Größen/Referenz-Scan und Duplikat-Scan
        DrawExtensionSelector();

        EditorGUILayout.Space(6);

        // ---------- Ergebnisse ----------
        using (var sv = new EditorGUILayout.ScrollViewScope(scroll))
        {
            scroll = sv.scrollPosition;

            DrawSizeAndReferenceSection();
            EditorGUILayout.Space(12);
            DrawDuplicatesSection();
        }

        EditorGUILayout.Space(8);
        DrawFooterButtons();
    }

    private void DrawSizeAndReferenceSection()
    {
        EditorGUILayout.LabelField("Große Dateien & Referenzen", EditorStyles.boldLabel);

        if (results.Count == 0)
        {
            EditorGUILayout.HelpBox("Keine Dateien > Schwelle gefunden (oder noch nicht gescannt).", MessageType.Info);
            return;
        }

        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("Größe", GUILayout.Width(90));
            GUILayout.Label("Datei (Asset-Pfad)", GUILayout.ExpandWidth(true));
            GUILayout.Label("Szenen", GUILayout.Width(60));
            GUILayout.Label("Prefabs", GUILayout.Width(70));
            GUILayout.Space(420);
        }

        var rows = results.ToArray(); // Snapshot gegen Collection-Änderungen

        foreach (var e in rows)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(FormatSize(e.Bytes), GUILayout.Width(90));
                e.Foldout = EditorGUILayout.Foldout(e.Foldout, e.AssetPath, true);
                GUILayout.Label(e.Scenes.Count.ToString(), GUILayout.Width(60));
                GUILayout.Label(e.Prefabs.Count.ToString(), GUILayout.Width(70));

                if (e.IsInResources)
                    GUILayout.Label("Resources", EditorStyles.miniLabel, GUILayout.Width(70));

                using (new EditorGUI.DisabledScope(isBusy))
                {
                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                        EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(e.AssetPath));

                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(e.AssetPath);

                    if (GUILayout.Button("Reveal", GUILayout.Width(60)))
                        EditorUtility.RevealInFinder(e.AssetPath);
                }
            }

            if (!e.Foldout) continue;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Austausch-Optionen (Prefabs zuerst, dann Szenen)", EditorStyles.boldLabel);
                using (new EditorGUILayout.HorizontalScope())
                {
                    e.Replacement = EditorGUILayout.ObjectField(new GUIContent("Ersatz-Asset"),
                        e.Replacement, typeof(UnityEngine.Object), false);

                    e.MapSpritesByName = EditorGUILayout.Toggle(new GUIContent("Sprites nach Name mappen"), e.MapSpritesByName, GUILayout.Width(200));

                    using (new EditorGUI.DisabledScope(isBusy))
                    {
                        if (GUILayout.Button("Auto finden (.png)", GUILayout.Width(140)))
                            TryAutoFindReplacement(e);

                        if (GUILayout.Button("… wählen", GUILayout.Width(90)))
                            PickReplacementViaFilePanel(e);
                    }
                }

                if (e.Replacement == null)
                    EditorGUILayout.HelpBox("Wähle ein Ersatz-Asset (PNG/Texture2D/Spritesheet) oder nutze „Auto finden (.png)“.", MessageType.Info);

                EditorGUILayout.Space(6);

                // ---- Szenen ----
                EditorGUILayout.LabelField("Szenen", EditorStyles.miniBoldLabel);
                var sceneList = e.Scenes.ToArray();
                if (sceneList.Length == 0)
                {
                    EditorGUILayout.HelpBox("Keine Szenen-Referenzen.", MessageType.None);
                }
                else
                {
                    foreach (var s in sceneList)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(12);
                            GUILayout.Label(s, GUILayout.ExpandWidth(true));

                            using (new EditorGUI.DisabledScope(isBusy))
                            {
                                if (GUILayout.Button("Ping", GUILayout.Width(50)))
                                    EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(s));

                                if (GUILayout.Button("Open", GUILayout.Width(50)))
                                {
                                    var sp = s;
                                    RunDeferred(() =>
                                    {
                                        EditorSceneManager.SaveOpenScenes();
                                        EditorSceneManager.OpenScene(sp, OpenSceneMode.Single);
                                    });
                                }

                                if (GUILayout.Button("Ping in Szene", GUILayout.Width(110)))
                                {
                                    var assetPath = e.AssetPath;
                                    var sp = s;
                                    RunDeferred(() => PingInScene_NoPrompt(assetPath, sp, true));
                                }

                                using (new EditorGUI.DisabledScope(e.Replacement == null))
                                {
                                    if (GUILayout.Button("Replace in Szene", GUILayout.Width(120)))
                                    {
                                        var entryPath = e.AssetPath;
                                        var sp = s;
                                        RunDeferred(() =>
                                        {
                                            ReplaceInPrefabsForEntry(e, true);
                                            AssetDatabase.SaveAssets();

                                            var cfg = SnapshotConfigFor(e);
                                            ScanAndRestoreReplacements(cfg);

                                            var refreshed = FindEntryByAssetPath(entryPath);
                                            bool stillUses = refreshed != null && refreshed.Scenes.Any(p => string.Equals(p, sp, StringComparison.OrdinalIgnoreCase));
                                            if (!stillUses)
                                            {
                                                EditorUtility.DisplayDialog("Nichts mehr in Szene zu tun",
                                                    "Nach dem Prefab-Austausch referenziert diese Szene das Asset nicht mehr.", "OK");
                                            }
                                            else
                                            {
                                                var res = ReplaceInScene(refreshed, sp, false);
                                                EditorSceneManager.SaveOpenScenes();
                                                AssetDatabase.SaveAssets();
                                                Scan();
                                                RefreshOrphanMetaCount();
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.Space(6);

                // ---- Prefabs ----
                EditorGUILayout.LabelField("Prefabs", EditorStyles.miniBoldLabel);
                var prefabList = e.Prefabs.ToArray();
                if (prefabList.Length == 0)
                {
                    EditorGUILayout.HelpBox("Keine Prefab-Referenzen.", MessageType.None);
                }
                else
                {
                    foreach (var p in prefabList)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(12);
                            GUILayout.Label(p, GUILayout.ExpandWidth(true));

                            using (new EditorGUI.DisabledScope(isBusy))
                            {
                                if (GUILayout.Button("Ping", GUILayout.Width(50)))
                                    EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(p));

                                if (GUILayout.Button("Select", GUILayout.Width(60)))
                                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(p);

                                if (GUILayout.Button("Open", GUILayout.Width(50)))
                                    AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(p));
                            }
                        }
                    }
                }

                // ---- Einzellöschung nur wenn komplett unreferenziert ----
                if (sceneList.Length == 0 && prefabList.Length == 0)
                {
                    EditorGUILayout.Space(6);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();
                        using (new EditorGUI.DisabledScope(isBusy))
                        {
                            if (GUILayout.Button(new GUIContent("Löschen…", "Dieses Asset endgültig löschen (mit Warnung)."), GUILayout.Width(110)))
                            {
                                var entry = e; // capture
                                RunDeferred(() =>
                                {
                                    DeleteEntryWithConfirmation(entry);
                                    RefreshOrphanMetaCount();
                                });
                            }
                        }
                    }
                }

                EditorGUILayout.Space(10);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    using (new EditorGUI.DisabledScope(isBusy || e.Replacement == null))
                    {
                        if (GUILayout.Button(new GUIContent("Nur Prefabs ersetzen",
                                "Ersetzt alle Verwendungen in referenzierenden Prefabs (Projekt-Assets)."),
                                GUILayout.Width(180)))
                        {
                            RunDeferred(() =>
                            {
                                ReplaceInPrefabsForEntry(e, false);
                                AssetDatabase.SaveAssets();
                                Scan();
                                RefreshOrphanMetaCount();
                            });
                        }
                    }
                }
            }
        }
    }

    private void DrawDuplicatesSection()
    {
        EditorGUILayout.LabelField("Duplikate (inhaltlich identisch, unabhängig von Dateiendung – gefiltert nach Auswahl oben)", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Status: " + dupScanInfo, EditorStyles.miniLabel);
            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(isBusy || dupGroups.Count == 0))
            {
                if (GUILayout.Button(new GUIContent("Alle Gruppen: Referenzen umlegen", "Lenkt alle Projekt-Referenzen auf die je Gruppe ausgewählte kanonische Datei um."), GUILayout.Width(260)))
                {
                    RunDeferred(() =>
                    {
                        int changedGroups = 0;
                        for (int i = 0; i < dupGroups.Count; i++)
                        {
                            if (dupGroups[i].Paths.Count < 2) continue;
                            if (ReplaceReferencesForDuplicateGroup(dupGroups[i], deleteDuplicates: false))
                                changedGroups++;
                        }
                        AssetDatabase.SaveAssets();
                        EditorSceneManager.SaveOpenScenes();
                        ScanDuplicates(); // neu einlesen
                        RefreshOrphanMetaCount();
                        EditorUtility.DisplayDialog("Duplikat-Umlenkung", $"Gruppen verarbeitet: {changedGroups}", "OK");
                    });
                }

                if (GUILayout.Button(new GUIContent("Alle Gruppen: Umlenken + Duplikate löschen", "Referenzen umlegen und alle nicht-kanonischen Dateien entfernen (Resources geschützt, wenn Toggle aktiv)."), GUILayout.Width(320)))
                {
                    RunDeferred(() =>
                    {
                        int changedGroups = 0;
                        for (int i = 0; i < dupGroups.Count; i++)
                        {
                            if (dupGroups[i].Paths.Count < 2) continue;
                            if (ReplaceReferencesForDuplicateGroup(dupGroups[i], deleteDuplicates: true))
                                changedGroups++;
                        }
                        AssetDatabase.SaveAssets();
                        EditorSceneManager.SaveOpenScenes();
                        ScanDuplicates();
                        RefreshOrphanMetaCount();
                        EditorUtility.DisplayDialog("Duplikate bereinigt", $"Gruppen verarbeitet: {changedGroups}", "OK");
                    });
                }
            }
        }

        if (dupGroups.Count == 0)
        {
            EditorGUILayout.HelpBox("Noch keine Duplikate gefunden. „Duplikate scannen“ starten.", MessageType.Info);
            return;
        }

        // Kopfzeile
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("#", GUILayout.Width(28));
            GUILayout.Label("Größe", GUILayout.Width(90));
            GUILayout.Label("Hash (SHA-256, gekürzt)", GUILayout.Width(220));
            GUILayout.Label("Kanonisch", GUILayout.Width(180));
            GUILayout.Label("Dateien", GUILayout.ExpandWidth(true));
            GUILayout.Space(210);
        }

        var snapshot = dupGroups.ToArray();
        for (int gi = 0; gi < snapshot.Length; gi++)
        {
            var g = snapshot[gi];

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label((gi + 1).ToString(), GUILayout.Width(28));
                GUILayout.Label(FormatSize(g.Size), GUILayout.Width(90));
                GUILayout.Label(g.Hash.Substring(0, Math.Min(40, g.Hash.Length)), GUILayout.Width(220));

                int newIndex = EditorGUILayout.Popup(g.CanonicalIndex, g.Paths.Select(p => Path.GetFileName(p)).ToArray(), GUILayout.Width(180));
                if (newIndex != g.CanonicalIndex) g.CanonicalIndex = newIndex;

                g.Foldout = EditorGUILayout.Foldout(g.Foldout, $"{g.Paths.Count} Dateien", true);

                using (new EditorGUI.DisabledScope(isBusy))
                {
                    if (GUILayout.Button("Umlenken", GUILayout.Width(90)))
                    {
                        var groupRef = g;
                        RunDeferred(() =>
                        {
                            if (ReplaceReferencesForDuplicateGroup(groupRef, deleteDuplicates: false))
                            {
                                AssetDatabase.SaveAssets();
                                EditorSceneManager.SaveOpenScenes();
                                ScanDuplicates();
                                RefreshOrphanMetaCount();
                            }
                        });
                    }
                    if (GUILayout.Button("Umlenken + löschen", GUILayout.Width(120)))
                    {
                        var groupRef = g;
                        RunDeferred(() =>
                        {
                            if (ReplaceReferencesForDuplicateGroup(groupRef, deleteDuplicates: true))
                            {
                                AssetDatabase.SaveAssets();
                                EditorSceneManager.SaveOpenScenes();
                                ScanDuplicates();
                                RefreshOrphanMetaCount();
                            }
                        });
                    }
                }
            }

            if (!g.Foldout) continue;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                for (int pi = 0; pi < g.Paths.Count; pi++)
                {
                    bool isCan = (pi == g.CanonicalIndex);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(12);
                        GUILayout.Label(isCan ? "●" : "○", GUILayout.Width(20));

                        // Kennzeichne Resources-Pfade
                        string label = g.Paths[pi];
                        if (IsInResourcesPath(label)) label += "  [Resources]";
                        GUILayout.Label(label, GUILayout.ExpandWidth(true));

                        using (new EditorGUI.DisabledScope(isBusy))
                        {
                            if (GUILayout.Button("Ping", GUILayout.Width(50)))
                                EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(g.Paths[pi]));
                            if (GUILayout.Button("Reveal", GUILayout.Width(60)))
                                EditorUtility.RevealInFinder(g.Paths[pi]);
                        }
                    }
                }
            }
        }
    }

    private void DrawFooterButtons()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUI.DisabledScope(isBusy))
            {
                if (GUILayout.Button("Alle Dateien auswählen"))
                {
                    Selection.objects = results
                        .Select(r => AssetDatabase.LoadMainAssetAtPath(r.AssetPath))
                        .Where(o => o != null)
                        .ToArray();
                }

                if (GUILayout.Button("Pfade in Zwischenablage"))
                {
                    EditorGUIUtility.systemCopyBuffer = string.Join("\n", results.Select(r =>
                    {
                        var flags = r.IsInResources ? " [Resources]" : "";
                        var lines = new List<string> { $"{r.AssetPath}{flags};{FormatSize(r.Bytes)}" };
                        if (r.Scenes.Count > 0) lines.AddRange(r.Scenes.Select(scene => $"  scene: {scene}"));
                        if (r.Prefabs.Count > 0) lines.AddRange(r.Prefabs.Select(prefab => $"  prefab: {prefab}"));
                        return string.Join("\n", lines);
                    }));
                }
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUI.DisabledScope(isBusy || !GetUnreferencedCandidates().Any()))
            {
                string hint = protectResources ? " (Resources geschützt)" : "";
                if (GUILayout.Button(new GUIContent("Alle unreferenzierten löschen…" + hint,
                        "Löscht alle Dateien ohne Szenen- oder Prefab-Referenzen (Resources werden – falls Schutz aktiv – übersprungen)."),
                        GUILayout.Width(300)))
                {
                    RunDeferred(() =>
                    {
                        DeleteAllUnreferencedWithConfirmation();
                        RefreshOrphanMetaCount();
                    });
                }
            }

            GUILayout.Space(10);

            using (new EditorGUI.DisabledScope(isBusy || !results.Any(r => r.Replacement != null && (r.Scenes.Count > 0 || r.Prefabs.Count > 0))))
            {
                if (GUILayout.Button(new GUIContent("Alle ersetzen (alle Szenen)",
                        "Erst Prefabs ersetzen + speichern + scannen; dann nur noch verbleibende Szenen ersetzen"),
                        GUILayout.Width(240)))
                {
                    RunDeferred(() =>
                    {
                        ReplaceAcrossAllEntriesAndScenes();
                        RefreshOrphanMetaCount();
                    });
                }
            }

            GUILayout.Space(10);

            using (new EditorGUI.DisabledScope(isBusy))
            {
                string btnText = orphanMetaCount >= 0
                    ? $"Verwaiste .meta aufräumen… ({orphanMetaCount})"
                    : "Verwaiste .meta aufräumen…";
                if (GUILayout.Button(new GUIContent(btnText,
                        "Sucht und löscht alle .meta-Dateien unter Assets/, zu denen keine Datei/kein Ordner mehr existiert."),
                        GUILayout.Width(260)))
                {
                    RunDeferred(DeleteAllOrphanMetaFilesWithConfirmation);
                }
            }
        }
    }

    // ---- UI: Extension Selector (für Größen-/Referenz-Scan und Duplikate) ----
    private void DrawExtensionSelector()
    {
        EditorGUILayout.LabelField("Dateitypen (ankreuzen für Scans):", EditorStyles.miniBoldLabel);

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Alle an", GUILayout.Width(80))) SetAllExtFlags(true);
                if (GUILayout.Button("Alle aus", GUILayout.Width(80))) SetAllExtFlags(false);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("Eigene Endung hinzufügen (z. B. .dds):", GUILayout.Width(230));
                newExtInput = EditorGUILayout.TextField(newExtInput, GUILayout.Width(120));
                using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(newExtInput)))
                {
                    if (GUILayout.Button("Hinzufügen", GUILayout.Width(100)))
                    {
                        var norm = NormalizeExt(newExtInput);
                        if (!string.IsNullOrEmpty(norm))
                            extFlags[norm] = true;
                        newExtInput = "";
                    }
                }
            }

            EditorGUILayout.Space(4);

            foreach (var g in extGroups)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        g.Foldout = EditorGUILayout.Foldout(g.Foldout, g.Name, true);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Alle an", GUILayout.Width(70))) SetGroup(g, true);
                        if (GUILayout.Button("Alle aus", GUILayout.Width(70))) SetGroup(g, false);
                    }

                    if (!g.Foldout) continue;

                    int cols = 3;
                    int perCol = Mathf.CeilToInt(g.Exts.Length / (float)cols);
                    for (int row = 0; row < perCol; row++)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            for (int c = 0; c < cols; c++)
                            {
                                int idx = row + c * perCol;
                                if (idx >= g.Exts.Length) { GUILayout.FlexibleSpace(); continue; }

                                string ext = g.Exts[idx];
                                bool val = extFlags.TryGetValue(ext, out var on) ? on : false;
                                bool newVal = EditorGUILayout.ToggleLeft(ext, val, GUILayout.Width(100));
                                if (newVal != val) extFlags[ext] = newVal;
                            }
                        }
                    }
                }
            }

            var grouped = new HashSet<string>(extGroups.SelectMany(gr => gr.Exts), StringComparer.OrdinalIgnoreCase);
            var custom = extFlags.Keys.Where(k => !grouped.Contains(k)).OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToArray();
            if (custom.Length > 0)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Eigene Endungen", EditorStyles.miniBoldLabel);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Alle an", GUILayout.Width(70))) foreach (var k in custom) extFlags[k] = true;
                        if (GUILayout.Button("Alle aus", GUILayout.Width(70))) foreach (var k in custom) extFlags[k] = false;
                    }

                    int cols = 4;
                    int perCol = Mathf.CeilToInt(custom.Length / (float)cols);
                    for (int row = 0; row < perCol; row++)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            for (int c = 0; c < cols; c++)
                            {
                                int idx = row + c * perCol;
                                if (idx >= custom.Length) { GUILayout.FlexibleSpace(); continue; }
                                string ext = custom[idx];
                                bool val = extFlags[ext];
                                bool newVal = EditorGUILayout.ToggleLeft(ext, val, GUILayout.Width(90));
                                if (newVal != val) extFlags[ext] = newVal;
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetAllExtFlags(bool value)
    {
        var keys = extFlags.Keys.ToArray();
        foreach (var k in keys) extFlags[k] = value;
    }
    private void SetGroup(ExtGroup g, bool value)
    {
        foreach (var ext in g.Exts) extFlags[ext] = value;
    }
    private static string NormalizeExt(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim();
        if (!s.StartsWith(".")) s = "." + s;
        s = s.ToLowerInvariant();
        if (s.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) return null;
        if (s.Length < 2 || s.Length > 16) return null;
        return s;
    }

    private IEnumerable<FileEntry> GetUnreferencedCandidates()
    {
        if (protectResources)
            return results.Where(r => !r.IsInResources && r.Scenes.Count == 0 && r.Prefabs.Count == 0);
        return results.Where(r => r.Scenes.Count == 0 && r.Prefabs.Count == 0);
    }

    // ----------------- GLOBAL: Prefabs -> Save -> Scan -> Szenen -----------------
    private void ReplaceAcrossAllEntriesAndScenes()
    {
        var activeBefore = results.Where(r => r.Replacement != null).ToList();
        int ei = 0;
        foreach (var e in activeBefore)
        {
            ei++;
            EditorUtility.DisplayProgressBar("Ersetze in Prefabs", Path.GetFileName(e.AssetPath), ei / (float)Math.Max(1, activeBefore.Count));
            ReplaceInPrefabsForEntry(e, true);
        }
        AssetDatabase.SaveAssets();

        var cfg = SnapshotConfigFor(activeBefore);
        ScanAndRestoreReplacements(cfg);

        var activeAfter = results.Where(r => r.Replacement != null && r.Scenes.Count > 0).ToList();
        var totalsByScene = new Dictionary<string, Totals>(StringComparer.OrdinalIgnoreCase);

        ei = 0;
        foreach (var e in activeAfter)
        {
            ei++;
            int si = 0;
            foreach (var scenePath in e.Scenes.ToArray())
            {
                si++;
                EditorUtility.DisplayProgressBar(
                    "Ersetze in Szenen",
                    $"{Path.GetFileName(e.AssetPath)} → {Path.GetFileName(scenePath)}",
                    (ei - 1 + (si / (float)Math.Max(1, e.Scenes.Count))) / Math.Max(1, activeAfter.Count)
                );

                var res = ReplaceInScene(e, scenePath, true);
                if (!res.Any) continue;

                Totals agg;
                if (!totalsByScene.TryGetValue(res.ScenePath, out agg))
                    agg = default(Totals);
                agg.comp += res.ComponentChanges;
                agg.matInst += res.MaterialInstances;
                agg.rend += res.RendererMaterialsChanged;
                totalsByScene[res.ScenePath] = agg;
            }
        }

        EditorUtility.ClearProgressBar();
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        Scan();
        RefreshOrphanMetaCount();

        int scenesTouched = totalsByScene.Count;
        int compSum = totalsByScene.Values.Sum(v => v.comp);
        int matSum = totalsByScene.Values.Sum(v => v.matInst);
        int rendSum = totalsByScene.Values.Sum(v => v.rend);

        var sb = new StringBuilder();
        sb.AppendLine($"Geänderte Szenen: {scenesTouched}");
        sb.AppendLine($"Komponenten geändert: {compSum}");
        sb.AppendLine($"Material-Instanzen erzeugt: {matSum}");
        sb.AppendLine($"Renderer mit neuen Materialien: {rendSum}");
        if (scenesTouched > 0)
        {
            sb.AppendLine("\nDetails:");
            foreach (var kv in totalsByScene.OrderBy(k => k.Key, StringComparer.OrdinalIgnoreCase))
                sb.AppendLine($"• {Path.GetFileName(kv.Key)}  (Comp {kv.Value.comp}, MatInst {kv.Value.matInst}, Renderer {kv.Value.rend})");
        }
        EditorUtility.DisplayDialog("Alle ersetzen – Ergebnis", sb.ToString(), "OK");
    }

    // ----------------- Pro-Eintrag: über alle Szenen ersetzen -----------------
    private void ReplaceForEntryAcrossScenes(FileEntry e)
    {
        if (e == null || e.Scenes == null || e.Scenes.Count == 0)
        {
            EditorUtility.DisplayDialog("Alle Szenen: Replace (Eintrag)", "Keine Szenen zu ersetzen.", "OK");
            return;
        }

        Totals totals = default(Totals);

        try
        {
            for (int i = 0; i < e.Scenes.Count; i++)
            {
                string scenePath = e.Scenes[i];

                EditorUtility.DisplayProgressBar(
                    "Alle Szenen: Replace (Eintrag)",
                    $"{Path.GetFileName(e.AssetPath)} → {Path.GetFileName(scenePath)}  ({i + 1}/{e.Scenes.Count})",
                    (i + 1) / (float)Math.Max(1, e.Scenes.Count)
                );

                var res = ReplaceInScene(e, scenePath, true);
                totals.comp += res.ComponentChanges;
                totals.matInst += res.MaterialInstances;
                totals.rend += res.RendererMaterialsChanged;
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
        }

        EditorUtility.DisplayDialog("Alle Szenen: Replace (Eintrag)",
            $"Asset: {Path.GetFileName(e.AssetPath)}\n" +
            $"Komponenten geändert: {totals.comp}\n" +
            $"Material-Instanzen erzeugt: {totals.matInst}\n" +
            $"Renderer mit neuen Materialien: {totals.rend}",
            "OK");
    }

    // ----------------- Prefab-Ersetzung (Projekt-Assets) -----------------
    private static void ReplaceInPrefabsForEntry(FileEntry e, bool silent)
    {
        var oldTargets = GetAssetTargets(e.AssetPath);
        if (oldTargets.Count == 0)
        {
            if (!silent) EditorUtility.DisplayDialog("Kein Ziel gefunden", "Konnte aus dem Asset keine referenzierbaren Objekte laden.", "OK");
            return;
        }

        var map = BuildReplacementMap(oldTargets, e.Replacement, e.MapSpritesByName);
        if (map.Count == 0)
        {
            if (!silent) EditorUtility.DisplayDialog("Kein passender Ersatz",
                "Es konnte kein kompatibles Mapping (Sprite/Texture) erzeugt werden.", "OK");
            return;
        }

        ReplaceInAllPrefabsWithMap(map, showProgress: true, progressTitle: "Prefabs ersetzen");

        if (!silent)
        {
            EditorUtility.DisplayDialog("Prefabs ersetzt",
                "Prefab-Referenzen wurden ersetzt (Details siehe Konsole/Änderungen).",
                "OK");
        }
    }

    // ----------------- Replace in EINER Szene -----------------
    private static ReplaceResult ReplaceInScene(FileEntry e, string scenePath, bool silent)
    {
        var result = new ReplaceResult { ScenePath = scenePath };

        if (e.Replacement == null)
        {
            if (!silent) EditorUtility.DisplayDialog("Kein Ersatz", "Bitte ein Ersatz-Asset wählen.", "OK");
            return result;
        }

        var oldTargets = GetAssetTargets(e.AssetPath);
        if (oldTargets.Count == 0)
        {
            if (!silent) EditorUtility.DisplayDialog("Kein Ziel gefunden", "Konnte aus dem Asset keine referenzierbaren Objekte laden.", "OK");
            return result;
        }

        var map = BuildReplacementMap(oldTargets, e.Replacement, e.MapSpritesByName);
        if (map.Count == 0)
        {
            if (!silent) EditorUtility.DisplayDialog("Kein passender Ersatz",
                "Es konnte kein kompatibles Mapping (Sprite/Texture) erzeugt werden.", "OK");
            return result;
        }

        if (!IsSceneOpen(scenePath, out var scene))
        {
            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        }

        var res = ReplaceInOpenSceneWithMap(scene, map, recordUndo: true);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        result.ComponentChanges = res.comp;
        result.MaterialInstances = res.matInst;
        result.RendererMaterialsChanged = res.rend;

        if (!silent)
        {
            EditorUtility.DisplayDialog("Austausch abgeschlossen",
                $"Szene: {Path.GetFileName(scenePath)}\n" +
                $"Komponenten geändert: {res.comp}\n" +
                $"Material-Instanzen erzeugt: {res.matInst}\n" +
                $"Renderer mit neuen Materialien: {res.rend}",
                "OK");
        }
        return result;
    }

    // ----------------- Utilitys: generischer Map-Austausch -----------------
    private static (int comp, int matInst, int rend) ReplaceInOpenSceneWithMap(Scene scene, Dictionary<UnityEngine.Object, UnityEngine.Object> map, bool recordUndo)
    {
        int compChanges = 0, matInstances = 0, rendererMaterialsChanged = 0;

        var roots = scene.GetRootGameObjects();
        foreach (var root in roots)
        {
            var stack = new Stack<Transform>();
            stack.Push(root.transform);

            while (stack.Count > 0)
            {
                var t = stack.Pop();
                for (int i = 0; i < t.childCount; i++) stack.Push(t.GetChild(i));

                var go = t.gameObject;

                var comps = go.GetComponents<Component>();
                foreach (var c in comps)
                {
                    if (c == null) continue;
                    if (ReplaceInSerializedObject(c, map, recordUndo))
                        compChanges++;
                }

                // Vorsicht bei Materialien: wir instanzieren, um Shared-Material-Assets nicht global zu ändern.
                var renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    var shared = renderer.sharedMaterials;
                    bool anyMatChanged = false;

                    for (int i = 0; i < shared.Length; i++)
                    {
                        var mat = shared[i];
                        if (mat == null) continue;

                        var deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { mat });
                        if (deps != null && deps.Any(d => d != null && map.ContainsKey(d)))
                        {
                            var inst = UnityEngine.Object.Instantiate(mat);
                            inst.name = mat.name + " (Instance)";
                            if (ReplaceInSerializedObject(inst, map, recordUndo))
                            {
                                shared[i] = inst;
                                anyMatChanged = true;
                                matInstances++;
                            }
                        }
                    }

                    if (anyMatChanged)
                    {
                        if (recordUndo) Undo.RecordObject(renderer, "Replace Material Textures");
                        renderer.sharedMaterials = shared;
                        EditorUtility.SetDirty(renderer);
                        rendererMaterialsChanged++;
                    }
                }
            }
        }

        return (compChanges, matInstances, rendererMaterialsChanged);
    }

    private static void ReplaceInAllPrefabsWithMap(Dictionary<UnityEngine.Object, UnityEngine.Object> map, bool showProgress, string progressTitle)
    {
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        var prefabs = prefabGuids.Select(AssetDatabase.GUIDToAssetPath).ToList();

        AssetDatabase.StartAssetEditing();
        try
        {
            for (int i = 0; i < prefabs.Count; i++)
            {
                var prefabPath = prefabs[i];
                if (showProgress && (i + 1) % 5 == 0)
                    EditorUtility.DisplayProgressBar(progressTitle, $"{i + 1}/{prefabs.Count}: {Path.GetFileName(prefabPath)}", (i + 1) / (float)Math.Max(1, prefabs.Count));

                var root = PrefabUtility.LoadPrefabContents(prefabPath);
                bool any = false;

                var stack = new Stack<Transform>();
                stack.Push(root.transform);
                while (stack.Count > 0)
                {
                    var t = stack.Pop();
                    for (int c = 0; c < t.childCount; c++) stack.Push(t.GetChild(c));

                    var go = t.gameObject;

                    var comps = go.GetComponents<Component>();
                    foreach (var c in comps)
                    {
                        if (c == null) continue;
                        if (ReplaceInSerializedObject(c, map, recordUndo: false))
                            any = true;
                    }

                    var renderer = go.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        var shared = renderer.sharedMaterials;
                        bool anyMatChanged = false;

                        for (int mi = 0; mi < shared.Length; mi++)
                        {
                            var mat = shared[mi];
                            if (mat == null) continue;

                            var deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { mat });
                            if (deps != null && deps.Any(d => d != null && map.ContainsKey(d)))
                            {
                                var inst = UnityEngine.Object.Instantiate(mat);
                                inst.name = mat.name + " (Instance)";
                                if (ReplaceInSerializedObject(inst, map, recordUndo: false))
                                {
                                    shared[mi] = inst;
                                    anyMatChanged = true;
                                }
                            }
                        }

                        if (anyMatChanged)
                        {
                            renderer.sharedMaterials = shared;
                            EditorUtility.SetDirty(renderer);
                            any = true;
                        }
                    }
                }

                if (any)
                {
                    PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                }
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.StopAssetEditing();
        }
    }

    private static void ReplaceInAllNonPrefabAssetsWithMap(Dictionary<UnityEngine.Object, UnityEngine.Object> map, bool showProgress, string progressTitle)
    {
        var allPaths = AssetDatabase.GetAllAssetPaths()
            .Where(p => p.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            .Where(p => !p.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
            .Where(p => !p.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
            .Where(p => !p.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
            .ToList();

        AssetDatabase.StartAssetEditing();
        try
        {
            for (int i = 0; i < allPaths.Count; i++)
            {
                var path = allPaths[i];
                if (showProgress && (i + 1) % 50 == 0)
                    EditorUtility.DisplayProgressBar(progressTitle, $"{i + 1}/{allPaths.Count}: {Path.GetFileName(path)}", (i + 1) / (float)Math.Max(1, allPaths.Count));

                var objs = AssetDatabase.LoadAllAssetsAtPath(path);
                bool any = false;

                foreach (var o in objs)
                {
                    if (o == null) continue;
                    if (ReplaceInSerializedObject(o, map, recordUndo: false))
                        any = true;
                }

                if (any)
                {
                    EditorUtility.SetDirty(AssetDatabase.LoadMainAssetAtPath(path));
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.StopAssetEditing();
        }
    }

    private static void ReplaceInAllScenesWithMap(Dictionary<UnityEngine.Object, UnityEngine.Object> map, bool showProgress, string progressTitle)
    {
        var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        var scenes = sceneGuids.Select(AssetDatabase.GUIDToAssetPath).ToList();

        for (int i = 0; i < scenes.Count; i++)
        {
            string scenePath = scenes[i];
            if (showProgress && (i + 1) % 2 == 0)
                EditorUtility.DisplayProgressBar(progressTitle, $"{i + 1}/{scenes.Count}: {Path.GetFileName(scenePath)}", (i + 1) / (float)Math.Max(1, scenes.Count));

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            var res = ReplaceInOpenSceneWithMap(scene, map, recordUndo: false);
            if (res.comp > 0 || res.matInst > 0 || res.rend > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            // Szene wieder schließen, um Speicher/UI zu entlasten
            EditorSceneManager.CloseScene(scene, true);
        }

        EditorUtility.ClearProgressBar();
    }

    private static bool ReplaceInSerializedObject(UnityEngine.Object target, Dictionary<UnityEngine.Object, UnityEngine.Object> map, bool recordUndo)
    {
        bool changed = false;
        var so = new SerializedObject(target);
        var it = so.GetIterator();
        bool enterChildren = true;

        while (it.NextVisible(enterChildren))
        {
            enterChildren = true;

            if (it.propertyType == SerializedPropertyType.ObjectReference)
            {
                var oldRef = it.objectReferenceValue;
                if (oldRef != null && map.TryGetValue(oldRef, out var newRef))
                {
                    if (oldRef.GetType() == newRef.GetType())
                    {
                        if (recordUndo) Undo.RecordObject(target, "Replace Asset Reference");
                        it.objectReferenceValue = newRef;
                        changed = true;
                    }
                }
            }
        }

        if (changed)
        {
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
        return changed;
    }

    // ---------- Mapping-Helfer ----------
    private static Dictionary<UnityEngine.Object, UnityEngine.Object> BuildReplacementMap(HashSet<UnityEngine.Object> oldTargets, UnityEngine.Object replacement, bool mapSpritesByName)
    {
        var map = new Dictionary<UnityEngine.Object, UnityEngine.Object>();

        string repPath = AssetDatabase.GetAssetPath(replacement);
        var repAll = AssetDatabase.LoadAllAssetsAtPath(repPath) ?? Array.Empty<UnityEngine.Object>();
        var repTex = repAll.OfType<Texture2D>().FirstOrDefault() ?? replacement as Texture2D;
        var repSprites = repAll.OfType<Sprite>().ToArray();

        foreach (var old in oldTargets)
        {
            if (old is Texture2D oldTex)
            {
                if (repTex != null)
                    map[oldTex] = repTex;
            }
            else if (old is Sprite oldSprite)
            {
                Sprite newSprite = null;
                if (mapSpritesByName && repSprites.Length > 0)
                    newSprite = repSprites.FirstOrDefault(s => string.Equals(s.name, oldSprite.name, StringComparison.OrdinalIgnoreCase));

                if (newSprite == null && repSprites.Length > 0)
                    newSprite = repSprites[0];

                if (newSprite != null)
                    map[oldSprite] = newSprite;
            }
        }

        return map;
    }

    private static Dictionary<UnityEngine.Object, UnityEngine.Object> BuildGenericMapFromAssetToAsset(string fromAssetPath, string toAssetPath)
    {
        var map = new Dictionary<UnityEngine.Object, UnityEngine.Object>();

        var fromAll = AssetDatabase.LoadAllAssetsAtPath(fromAssetPath) ?? Array.Empty<UnityEngine.Object>();
        var toAll = AssetDatabase.LoadAllAssetsAtPath(toAssetPath) ?? Array.Empty<UnityEngine.Object>();

        // Index Ziel nach (Type, Name)
        var idx = new Dictionary<(Type type, string name), UnityEngine.Object>();
        foreach (var to in toAll)
        {
            if (to == null) continue;
            idx[(to.GetType(), to.name)] = to;
        }

        // Fallbacks nach Typ (erstes Vorkommen)
        var typeFirst = new Dictionary<Type, UnityEngine.Object>();
        foreach (var to in toAll)
        {
            if (to == null) continue;
            if (!typeFirst.ContainsKey(to.GetType()))
                typeFirst[to.GetType()] = to;
        }

        foreach (var from in fromAll)
        {
            if (from == null) continue;

            UnityEngine.Object candidate = null;
            if (idx.TryGetValue((from.GetType(), from.name), out var exact))
                candidate = exact;
            else if (typeFirst.TryGetValue(from.GetType(), out var byType))
                candidate = byType;

            if (candidate != null)
                map[from] = candidate;
        }

        return map;
    }

    // ---------- Ping/Find ----------
    private static void PingInScene_NoPrompt(string assetPath, string scenePath, bool selectAll)
    {
        var targets = GetAssetTargets(assetPath);
        if (targets.Count == 0)
        {
            EditorUtility.DisplayDialog("Kein Ziel gefunden", "Konnte aus dem Asset keine referenzierbaren Objekte laden.", "OK");
            return;
        }

        if (!IsSceneOpen(scenePath, out var scene))
        {
            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        }

        var found = FindObjectsReferencing(scene, targets);

        if (found.Count > 0)
        {
            Selection.objects = selectAll ? found.Cast<UnityEngine.Object>().ToArray()
                                          : new UnityEngine.Object[] { found[0] };

            EditorGUIUtility.PingObject(found[0]);
            if (SceneView.lastActiveSceneView != null)
                SceneView.lastActiveSceneView.FrameSelected();
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Keine direkte Szene-Referenz",
                "In dieser Szene wurden keine GameObjects gefunden, die das Asset direkt (oder über Materialien) referenzieren.",
                "OK"
            );
        }
    }

    private static List<GameObject> FindObjectsReferencing(Scene scene, HashSet<UnityEngine.Object> targets)
    {
        var hits = new List<GameObject>();
        var roots = scene.GetRootGameObjects();

        foreach (var root in roots)
        {
            var stack = new Stack<Transform>();
            stack.Push(root.transform);

            while (stack.Count > 0)
            {
                var t = stack.Pop();
                for (int i = 0; i < t.childCount; i++) stack.Push(t.GetChild(i));

                var go = t.gameObject;
                var deps = EditorUtility.CollectDependencies(new UnityEngine.Object[] { go });
                if (deps != null && deps.Any(d => d != null && targets.Contains(d)))
                    hits.Add(go);
            }
        }

        return hits.Distinct().OrderBy(g => g.name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static bool IsSceneOpen(string scenePath, out Scene scene)
    {
        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            var s = EditorSceneManager.GetSceneAt(i);
            if (string.Equals(s.path, scenePath, StringComparison.OrdinalIgnoreCase))
            {
                scene = s;
                return true;
            }
        }
        scene = default(Scene);
        return false;
    }

    private static HashSet<UnityEngine.Object> GetAssetTargets(string assetPath)
    {
        var set = new HashSet<UnityEngine.Object>();
        var main = AssetDatabase.LoadMainAssetAtPath(assetPath);
        if (main != null) set.Add(main);

        var subs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        foreach (var o in subs)
            if (o != null) set.Add(o);

        // Für „PNG ersetzen“-Feature sind v. a. Texture2D/Sprite interessant,
        // aber für generische Umlenkung lassen wir ALLE referenzierbaren Subassets drin.
        return set;
    }

    private static void TryAutoFindReplacement(FileEntry e)
    {
        var pngCandidate = Path.ChangeExtension(e.AssetPath, ".png");
        var obj = AssetDatabase.LoadMainAssetAtPath(pngCandidate);
        if (obj != null)
        {
            e.Replacement = obj;
            EditorGUIUtility.PingObject(obj);
        }
        else
        {
            EditorUtility.DisplayDialog("Nichts gefunden",
                $"Kein PNG unter:\n{pngCandidate}\n\nBitte manuell wählen.", "OK");
        }
    }

    private static void PickReplacementViaFilePanel(FileEntry e)
    {
        string abs = EditorUtility.OpenFilePanelWithFilters("PNG wählen (im Projekt)", Application.dataPath,
            new[] { "PNG", "png" });
        if (string.IsNullOrEmpty(abs)) return;

        abs = abs.Replace("\\", "/");
        var assetsRoot = Application.dataPath.Replace("\\", "/");
        if (!abs.StartsWith(assetsRoot, StringComparison.OrdinalIgnoreCase))
        {
            EditorUtility.DisplayDialog("Nicht im Projekt", "Bitte wähle eine Datei innerhalb des Projekts (unter Assets/).", "OK");
            return;
        }

        string rel = "Assets" + abs.Substring(assetsRoot.Length);
        var obj = AssetDatabase.LoadMainAssetAtPath(rel);
        if (obj != null)
        {
            e.Replacement = obj;
            EditorGUIUtility.PingObject(obj);
        }
    }

    // ----------------- Scan (Größe/Referenzen) -----------------
    private void Scan()
    {
        results.Clear();

        string root = searchRoot;
        if (string.IsNullOrWhiteSpace(root) || !root.StartsWith("Assets", StringComparison.OrdinalIgnoreCase))
            root = "Assets";

        string absRoot = Path.GetFullPath(root);
        string absAssets = Path.GetFullPath(Application.dataPath);

        // Aktive Extensions aus Auswahl
        var activeExts = new HashSet<string>(extFlags.Where(kv => kv.Value).Select(kv => kv.Key), StringComparer.OrdinalIgnoreCase);
        if (activeExts.Count == 0)
        {
            EditorUtility.DisplayDialog("Keine Dateitypen ausgewählt",
                "Bitte wähle mindestens eine Dateiendung in der Checkbox-Liste aus.", "OK");
            Repaint();
            return;
        }

        long thresholdBytes = (long)(thresholdMb * 1024f * 1024f);

        try
        {
            EditorUtility.DisplayProgressBar("Suche Assets", "Dateien scannen …", 0f);

            var files = Directory.EnumerateFiles(absRoot, "*.*", SearchOption.AllDirectories)
                                 .Where(p => activeExts.Contains(Path.GetExtension(p)))
                                 .ToList();

            int total = Math.Max(1, files.Count);
            for (int i = 0; i < files.Count; i++)
            {
                if ((i + 1) % 25 == 0)
                    EditorUtility.DisplayProgressBar("Suche Assets", $"Prüfe {i + 1}/{total}", (i + 1) / (float)total);

                string absPath = files[i];
                long size = 0;
                try { size = new FileInfo(absPath).Length; } catch { }

                if (size > thresholdBytes)
                {
                    string rel = absPath.Replace("\\", "/");
                    string assetsRootNormalized = absAssets.Replace("\\", "/");

                    if (rel.StartsWith(assetsRootNormalized, StringComparison.OrdinalIgnoreCase))
                    {
                        rel = "Assets" + rel.Substring(assetsRootNormalized.Length);
                        results.Add(new FileEntry
                        {
                            AssetPath = rel,
                            Bytes = size,
                            IsInResources = IsInResourcesPath(rel)
                        });
                    }
                }
            }

            // Szenen-Referenzen sammeln
            if (results.Count > 0)
            {
                var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
                var scenes = sceneGuids.Select(AssetDatabase.GUIDToAssetPath).ToList();

                int sTotal = Math.Max(1, scenes.Count);
                for (int sIdx = 0; sIdx < scenes.Count; sIdx++)
                {
                    if ((sIdx + 1) % 5 == 0)
                        EditorUtility.DisplayProgressBar("Szenen analysieren", $"Prüfe {sIdx + 1}/{sTotal}", (sIdx + 1) / (float)sTotal);

                    string scenePath = scenes[sIdx];
                    string[] deps = AssetDatabase.GetDependencies(scenePath, true);
                    if (deps == null || deps.Length == 0) continue;

                    var depSet = new HashSet<string>(deps, StringComparer.OrdinalIgnoreCase);
                    foreach (var r in results)
                        if (depSet.Contains(r.AssetPath))
                            r.Scenes.Add(scenePath);
                }

                foreach (var r in results)
                    r.Scenes = r.Scenes.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList();

                // Prefab-Referenzen sammeln
                var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
                var prefabPaths = prefabGuids.Select(AssetDatabase.GUIDToAssetPath).ToList();

                int pTotal = Math.Max(1, prefabPaths.Count);
                for (int pIdx = 0; pIdx < prefabPaths.Count; pIdx++)
                {
                    if ((pIdx + 1) % 10 == 0)
                        EditorUtility.DisplayProgressBar("Prefabs analysieren", $"Prüfe {pIdx + 1}/{pTotal}", (pIdx + 1) / (float)pTotal);

                    string prefabPath = prefabPaths[pIdx];
                    string[] deps = AssetDatabase.GetDependencies(prefabPath, true);
                    if (deps == null || deps.Length == 0) continue;

                    var depSet = new HashSet<string>(deps, StringComparer.OrdinalIgnoreCase);
                    foreach (var r in results)
                        if (depSet.Contains(r.AssetPath))
                            r.Prefabs.Add(prefabPath);
                }

                foreach (var r in results)
                    r.Prefabs = r.Prefabs.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList();
            }

            results = results.OrderByDescending(r => r.Bytes)
                             .ThenBy(r => r.AssetPath, StringComparer.OrdinalIgnoreCase)
                             .ToList();
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            try { orphanMetaCount = FindOrphanMetaFiles().Count; } catch { orphanMetaCount = -1; }
            Repaint();
        }
    }

    // ----------------- Duplikate: Scan & Verarbeiten -----------------
    private void ScanDuplicates()
    {
        dupGroups.Clear();
        dupScanInfo = "scanne…";

        // Aktive Extensions für Duplikat-Scan
        var activeExts = new HashSet<string>(extFlags.Where(kv => kv.Value).Select(kv => kv.Key), StringComparer.OrdinalIgnoreCase);
        if (activeExts.Count == 0)
        {
            dupScanInfo = "keine Dateitypen ausgewählt";
            Repaint();
            EditorUtility.DisplayDialog("Keine Dateitypen", "Bitte wähle mindestens eine Dateiendung aus (oben in der Liste).", "OK");
            return;
        }

        string root = searchRoot;
        if (string.IsNullOrWhiteSpace(root) || !root.StartsWith("Assets", StringComparison.OrdinalIgnoreCase))
            root = "Assets";

        string absRoot = AbsoluteFromAssetsRel(root);

        try
        {
            EditorUtility.DisplayProgressBar("Duplikate scannen", "Dateiliste erstellen …", 0f);

            // 1) Dateien (ohne .meta) einsammeln, NUR gewählte Endungen
            var allFiles = Directory.EnumerateFiles(absRoot, "*.*", SearchOption.AllDirectories)
                .Where(p => !p.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                .Where(p => activeExts.Contains(Path.GetExtension(p)))
                .ToList();

            // 2) Relativ filtern auf Assets/
            string assetsAbs = Application.dataPath.Replace("\\", "/");
            var relFiles = new List<string>();
            foreach (var abs in allFiles)
            {
                var norm = abs.Replace("\\", "/");
                if (!norm.StartsWith(assetsAbs, StringComparison.OrdinalIgnoreCase)) continue;
                relFiles.Add("Assets" + norm.Substring(assetsAbs.Length));
            }

            // 3) Gruppieren nach Größe
            var groupsBySize = relFiles
                .GroupBy(p => new FileInfo(AbsoluteFromAssetsRel(p)).Length)
                .Where(g => g.Key > 0 && g.Count() > 1) // nur potentielle Duplikate
                .OrderByDescending(g => g.Key)
                .ToList();

            int total = Math.Max(1, groupsBySize.Sum(g => g.Count()));
            int processed = 0;

            using (var sha = SHA256.Create())
            {
                foreach (var g in groupsBySize)
                {
                    // 4) Innerhalb gleicher Größe nach Hash clustern
                    var byHash = new Dictionary<string, List<string>>();
                    foreach (var rel in g)
                    {
                        processed++;
                        if (processed % 50 == 0)
                            EditorUtility.DisplayProgressBar("Duplikate scannen",
                                $"Hashing ({processed}/{total}) …",
                                processed / (float)total);

                        string abs = AbsoluteFromAssetsRel(rel);
                        try
                        {
                            using (var fs = new FileStream(abs, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                var hash = sha.ComputeHash(fs);
                                string hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                                if (!byHash.TryGetValue(hex, out var list))
                                    list = byHash[hex] = new List<string>();
                                list.Add(rel);
                            }
                        }
                        catch
                        {
                            // Datei evtl. gelockt – überspringen
                        }
                    }

                    foreach (var kv in byHash)
                    {
                        if (kv.Value.Count < 2) continue;
                        dupGroups.Add(new DuplicateGroup
                        {
                            Hash = kv.Key,
                            Size = g.Key,
                            Paths = kv.Value.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList(),
                            CanonicalIndex = 0,
                            Foldout = false
                        });
                    }
                }
            }

            dupGroups = dupGroups
                .OrderByDescending(d => d.Size)
                .ThenBy(d => d.Hash, StringComparer.OrdinalIgnoreCase)
                .ToList();

            dupScanInfo = dupGroups.Count == 0 ? "keine Duplikate" : $"Gruppen: {dupGroups.Count}";
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            Repaint();
        }
    }

    private bool ReplaceReferencesForDuplicateGroup(DuplicateGroup g, bool deleteDuplicates)
    {
        if (g == null || g.Paths == null || g.Paths.Count < 2) return false;
        int can = Mathf.Clamp(g.CanonicalIndex, 0, g.Paths.Count - 1);
        string canonical = g.Paths[can];

        // 1) Mapping aufbauen: für jede Nicht-Kanonische Datei -> auf Kanonische
        var globalMap = new Dictionary<UnityEngine.Object, UnityEngine.Object>();
        for (int i = 0; i < g.Paths.Count; i++)
        {
            if (i == can) continue;
            var from = g.Paths[i];
            var map = BuildGenericMapFromAssetToAsset(from, canonical);
            foreach (var kv in map)
                if (!globalMap.ContainsKey(kv.Key)) globalMap[kv.Key] = kv.Value;
        }
        if (globalMap.Count == 0) return false;

        // 2) Projektweit referenzen ersetzen
        ReplaceInAllNonPrefabAssetsWithMap(globalMap, showProgress: true, progressTitle: "Assets umlenken");
        ReplaceInAllPrefabsWithMap(globalMap, showProgress: true, progressTitle: "Prefabs umlenken");
        ReplaceInAllScenesWithMap(globalMap, showProgress: true, progressTitle: "Szenen umlenken");

        AssetDatabase.SaveAssets();
        EditorSceneManager.SaveOpenScenes();

        // 3) Optional: Duplikate löschen (alle ausser kanonisch)
        if (deleteDuplicates)
        {
            int deleted = 0, failed = 0, skipped = 0;
            try
            {
                for (int i = 0; i < g.Paths.Count; i++)
                {
                    if (i == can) continue;
                    string path = g.Paths[i];

                    // Guard A: Resources nicht löschen (wenn aktiv)
                    if (protectResources && IsInResourcesPath(path)) { skipped++; continue; }

                    EditorUtility.DisplayProgressBar("Duplikate löschen", Path.GetFileName(path), (i + 1) / (float)Math.Max(1, g.Paths.Count));
                    if (DeleteAssetWithFallback(path)) deleted++; else failed++;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (skipped > 0)
                Debug.LogWarning($"{skipped} Duplikate unter Resources/ wurden wegen aktivem Schutz nicht gelöscht.");
        }

        return true;
    }

    // ----------------- Format/Utils -----------------
    private static string FormatSize(long bytes)
    {
        const double KB = 1024.0, MB = KB * 1024.0, GB = MB * 1024.0;
        if (bytes >= GB) return (bytes / GB).ToString("0.00") + " GB";
        if (bytes >= MB) return (bytes / MB).ToString("0.00") + " MB";
        if (bytes >= KB) return (bytes / KB).ToString("0.0") + " KB";
        return bytes + " B";
    }

    // ----------------- Löschen (ohne erneute Projektprüfung) -----------------
    private void DeleteEntryWithConfirmation(FileEntry e)
    {
        if (e == null) return;
        if (!(e.Scenes.Count == 0 && e.Prefabs.Count == 0))
        {
            EditorUtility.DisplayDialog("Nicht unreferenziert",
                "Dieses Asset hat noch Szenen- oder Prefab-Referenzen und wird nicht gelöscht.", "OK");
            return;
        }

        // Guard A
        if (protectResources && e.IsInResources)
        {
            EditorUtility.DisplayDialog("Resources-Asset geschützt",
                "Dieses Asset liegt unter einem Resources/-Pfad und der Schutz ist aktiv.\n"
                + "Deaktiviere den Schalter „Resources-Ordner schützen“, wenn du es trotzdem löschen willst.",
                "OK");
            return;
        }

        bool ok = EditorUtility.DisplayDialog("Datei löschen?",
            $"Soll die Datei wirklich gelöscht werden?\n\n{e.AssetPath}\nGröße: {FormatSize(e.Bytes)}",
            "Löschen", "Abbrechen");

        if (!ok) return;

        bool deleted = DeleteAssetWithFallback(e.AssetPath);
        AssetDatabase.SaveAssets();
        Scan();
        RefreshOrphanMetaCount();

        if (!deleted)
            EditorUtility.DisplayDialog("Löschen fehlgeschlagen", $"Konnte {e.AssetPath} nicht löschen.", "OK");
    }

    private void DeleteAllUnreferencedWithConfirmation()
    {
        var candidates = GetUnreferencedCandidates().ToList();
        if (candidates.Count == 0)
        {
            EditorUtility.DisplayDialog("Keine unreferenzierten Dateien",
                protectResources
                    ? "Es gibt keine Einträge ohne Szenen-/Prefab-Referenzen außerhalb von Resources/ (Schutz aktiv)."
                    : "Es gibt keine Einträge ohne Szenen- oder Prefab-Referenzen.",
                "OK");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Unreferenzierte Dateien: {candidates.Count}");
        foreach (var c in candidates.Take(20))
        {
            var flag = c.IsInResources ? " [Resources]" : "";
            sb.AppendLine($"• {c.AssetPath}{flag}");
        }
        if (candidates.Count > 20) sb.AppendLine("• …");

        if (!EditorUtility.DisplayDialog("Alle unreferenzierten löschen?", sb.ToString(), "Löschen", "Abbrechen"))
            return;

        int deleted = 0, failed = 0;

        try
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                var c = candidates[i];
                EditorUtility.DisplayProgressBar("Lösche Dateien", Path.GetFileName(c.AssetPath), (i + 1) / (float)Math.Max(1, candidates.Count));
                if (DeleteAssetWithFallback(c.AssetPath)) deleted++; else failed++;
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            Scan();
            RefreshOrphanMetaCount();
        }

        EditorUtility.DisplayDialog("Lösch-Ergebnis",
            $"Gelöscht: {deleted}\nFehlgeschlagen: {failed}",
            "OK");
    }

    // Harte Löschung inkl. .meta
    private static bool DeleteAssetWithFallback(string assetPath)
    {
        bool mainDeleted = false;

        // 1) Papierkorb (wenn verfügbar)
        try
        {
            var m = typeof(AssetDatabase).GetMethod("MoveAssetToTrash", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            if (m != null)
            {
                mainDeleted = (bool)m.Invoke(null, new object[] { assetPath });
            }
        }
        catch { /* Ignorieren */ }

        // 2) Normales Delete
        if (!mainDeleted)
        {
            try { mainDeleted = AssetDatabase.DeleteAsset(assetPath); } catch { }
        }

        // 3) Direkt im Dateisystem
        if (!mainDeleted)
        {
            try
            {
                string abs = AbsoluteFromAssetsRel(assetPath);
                if (File.Exists(abs))
                {
                    File.SetAttributes(abs, FileAttributes.Normal);
                    File.Delete(abs);
                    mainDeleted = !File.Exists(abs);
                }
                else if (Directory.Exists(abs))
                {
                    FileUtil.DeleteFileOrDirectory(abs);
                    mainDeleted = !Directory.Exists(abs);
                }
            }
            catch { /* noop */ }
        }

        // .meta nur löschen, wenn Hauptdatei jetzt wirklich weg ist
        try
        {
            string abs = AbsoluteFromAssetsRel(assetPath);
            bool mainExists = File.Exists(abs) || Directory.Exists(abs);
            if (!mainExists)
            {
                DeleteMetaFileFor(assetPath);
            }
        }
        catch { /* noop */ }

        AssetDatabase.Refresh();
        return mainDeleted;
    }

    private static bool DeleteMetaFileFor(string assetPath)
    {
        string relMeta = assetPath.EndsWith(".meta", StringComparison.OrdinalIgnoreCase)
            ? assetPath
            : assetPath + ".meta";

        bool removed = false;

        try { removed = AssetDatabase.DeleteAsset(relMeta); } catch { }

        if (!removed)
        {
            try { FileUtil.DeleteFileOrDirectory(relMeta); removed = true; } catch { }
        }

        if (!removed)
        {
            try
            {
                string absMeta = AbsoluteFromAssetsRel(relMeta);
                if (File.Exists(absMeta))
                {
                    File.SetAttributes(absMeta, FileAttributes.Normal);
                    File.Delete(absMeta);
                    removed = !File.Exists(absMeta);
                }
            }
            catch { }
        }

        return removed;
    }

    private static string AbsoluteFromAssetsRel(string assetsRelativePath)
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName.Replace("\\", "/");
        string combined = Path.GetFullPath(Path.Combine(projectRoot, assetsRelativePath)).Replace("\\", "/");
        return combined;
    }

    // ----------------- Orphan .meta: Zählen & Aufräumen -----------------
    private void RefreshOrphanMetaCount()
    {
        try { orphanMetaCount = FindOrphanMetaFiles().Count; }
        catch { orphanMetaCount = -1; }
        Repaint();
    }

    private void DeleteAllOrphanMetaFilesWithConfirmation()
    {
        var orphans = FindOrphanMetaFiles();
        if (orphans.Count == 0)
        {
            orphanMetaCount = 0;
            EditorUtility.DisplayDialog("Keine verwaisten .meta-Dateien", "Unter Assets/ wurden keine verwaisten .meta gefunden.", "OK");
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Verwaiste .meta-Dateien: {orphans.Count}");
        foreach (var m in orphans.Take(30))
            sb.AppendLine("• " + m);
        if (orphans.Count > 30) sb.AppendLine("• …");

        if (!EditorUtility.DisplayDialog("Verwaiste .meta aufräumen?", sb.ToString(), "Löschen", "Abbrechen"))
            return;

        int deleted = 0, failed = 0;
        try
        {
            for (int i = 0; i < orphans.Count; i++)
            {
                string relMeta = orphans[i];
                EditorUtility.DisplayProgressBar("Lösche .meta", relMeta, (i + 1) / (float)Math.Max(1, orphans.Count));
                if (DeleteMetaFileFor(relMeta)) deleted++; else failed++;
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        RefreshOrphanMetaCount();

        EditorUtility.DisplayDialog("Meta-Aufräumen beendet",
            $"Gelöschte .meta: {deleted}\nFehlgeschlagen: {failed}", "OK");
    }

    private static List<String> FindOrphanMetaFiles()
    {
        var list = new List<string>();
        string assetsAbs = Application.dataPath.Replace("\\", "/");

        foreach (var metaAbs in Directory.EnumerateFiles(assetsAbs, "*.meta", SearchOption.AllDirectories))
        {
            string baseAbs = metaAbs.Substring(0, metaAbs.Length - 5); // ohne .meta
            bool exists = File.Exists(baseAbs) || Directory.Exists(baseAbs);
            if (!exists)
            {
                string rel = "Assets" + metaAbs.Replace("\\", "/").Substring(assetsAbs.Length);
                list.Add(rel);
            }
        }

        list.Sort(StringComparer.OrdinalIgnoreCase);
        return list;
    }

    // ----------------- Replacements sichern/wiederherstellen über Scan -----------------
    private Dictionary<string, ReplacementConfig> SnapshotConfigFor(IEnumerable<FileEntry> entries)
    {
        var dict = new Dictionary<string, ReplacementConfig>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in entries)
        {
            if (e.Replacement == null) continue;
            dict[e.AssetPath] = new ReplacementConfig { Replacement = e.Replacement, MapByName = e.MapSpritesByName };
        }
        return dict;
    }
    private Dictionary<string, ReplacementConfig> SnapshotConfigFor(FileEntry e) => SnapshotConfigFor(new[] { e });

    private void ScanAndRestoreReplacements(Dictionary<string, ReplacementConfig> cfg)
    {
        Scan();
        foreach (var r in results)
        {
            if (cfg.TryGetValue(r.AssetPath, out var c))
            {
                r.Replacement = c.Replacement;
                r.MapSpritesByName = c.MapByName;
            }
        }
        RefreshOrphanMetaCount();
        Repaint();
    }

    private FileEntry FindEntryByAssetPath(string assetPath)
    {
        return results.FirstOrDefault(r => string.Equals(r.AssetPath, assetPath, StringComparison.OrdinalIgnoreCase));
    }
}
#endif
