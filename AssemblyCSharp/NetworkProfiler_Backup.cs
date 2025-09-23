#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using FishNet.Managing;
using FishNet.Managing.Statistic;
using GameKit.Dependencies.Utilities;
using UnityEditor;
using UnityEngine;

namespace FishNet.Editing
{
    public class NetworkProfilerWindow : EditorWindow
    {
        /// <summary>
        /// Current instances of this window.
        /// </summary>
        internal static readonly List<NetworkProfilerWindow> Instances = new();

        #region Private.
        private Vector2 collectiveScrollViewPosition;
        private Vector2 detailsScrollViewPosition;
        private int selectedSampleIndex = -1;
        private int hoveredSampleIndex = -1;
        private const float barWidth = 20f;
        private const float labelWidth = 50f;
        /// <summary>
        /// Expanded state of details trees.
        /// </summary>
        private readonly Dictionary<string, bool> _detailTreeStates = new(32);
        /// <summary>
        /// Expanded state of packet trees.
        /// </summary>
        private readonly Dictionary<string, bool> _packetTreeStates = new(32);

        // private RPCType lastRpcRpcType;
        // private BitPacker lastRpcPacker;
        /// <summary>
        /// The next time the window dimensions can be saved.
        /// </summary>
        private float _nextWindowSaveTime;
        /// <summary>
        /// Current window size.
        /// </summary>
        private Vector2 _windowSize;
        /// <summary>
        /// True if this instance should be running.
        /// </summary>
        private bool _isEnabled;
        /// <summary>
        /// True if on the server tab, false if on the client.
        /// </summary>
        private bool _onServerTab;
        /// <summary>
        /// Traffic statistics for this instance. 
        /// </summary>
        private NetworkTrafficStatistics _networkTrafficStatistics;
        /// <summary>
        /// Currently recorded statistics.
        /// </summary>
        private readonly Dictionary<uint, ProfiledTickData> _profiledTickData = new();
        /// <summary>
        /// Data which contains the largest bytes be it in or out.
        /// </summary>
        private ProfiledTickData _largestBytesData;
        #endregion

        #region Consts/readonly.
        /// <summary>
        /// Name of this window.
        /// </summary>
        private const string WINDOW_NAME = "FishNet Network Profiler";
        /// <summary>
        /// EditorPrefs key to save last window size.
        /// </summary>
        private const string WINDOW_SIZE_PREFIX_PREF_NAME = "FishNet_NetworkProfilerWindowSize_";
        /// <summary>
        /// EditorPrefs float X name.
        /// </summary>
        private const string FLOAT_X_PREF_NAME = "X";
        /// <summary>
        /// EditorPrefs float Y name.
        /// </summary>
        private const string FLOAT_Y_PREF_NAME = "Y";
        /// <summary>
        /// Maximum size the window can be.
        /// </summary>
        private readonly Vector2 _defaultWindowSize = new(400f, 225f);
        /// <summary>
        /// Minimum size the window must be.
        /// </summary>
        private readonly Vector2 _minimumWindowSize = new(250f, 125f);
        /// <summary>
        /// Allow saving window size at most this often.
        /// </summary>
        private const float WINDOW_SIZE_SAVE_INTERVAL = 0.5f;
        #endregion

        #region Initialize and deinitialize.
        /// <summary>
        /// Initializes Instances if an instance is open.
        /// </summary>
        internal static void InitializeInstances(NetworkManager manager)
        {
            if (Instances.Count == 0)
                return;

            foreach (NetworkProfilerWindow window in Instances)
                window.InitializeIfNeeded(manager);
        }

        /// <summary>
        /// Initializes if current traffic statistics is null.
        /// </summary>
        private void InitializeIfNeeded(NetworkManager manager)
        {
            if (_networkTrafficStatistics != null)
                return;

            if (manager.StatisticsManager.TryGetNetworkTrafficStatistics(out _networkTrafficStatistics))
                _networkTrafficStatistics.OnNetworkTraffic += NetworkTrafficStatistics_OnNetworkTraffic;
        }

        private void OnEnable()
        {
            Instances.Add(this);

            _windowSize = GetEditorPrefs(WINDOW_SIZE_PREFIX_PREF_NAME, _minimumWindowSize);
        }

        private void OnDisable()
        {
            Instances.Remove(this);

            SaveWindowSize(force: true);

            if (_networkTrafficStatistics != null)
                _networkTrafficStatistics.OnNetworkTraffic -= NetworkTrafficStatistics_OnNetworkTraffic;
        }
        #endregion

        /// <summary>
        /// Called when new traffic statistics are received.
        /// </summary>
        private void NetworkTrafficStatistics_OnNetworkTraffic(uint tick, BidirectionalNetworkTraffic serverTraffic, BidirectionalNetworkTraffic clientTraffic)
        {
            ProfiledTickData tickData = ResettableObjectCaches<ProfiledTickData>.Retrieve();

            if (!tickData.TryInitialize(tick, serverTraffic, clientTraffic))
            {
                ResettableObjectCaches<ProfiledTickData>.Store(tickData);
                return;
            }

            /* Make sure data is not already added. This should not be possible. */
            if (!_profiledTickData.TryAdd(tick, tickData))
            {
                NetworkManagerExtensions.LogError($"Tick [{tick}] has already been added to data.");
                StoreProfiledTickData(tickData);

                return;
            }

            Repaint();
        }

        /// <summary>
        /// Called when profiled data is added or removed.
        /// </summary>
        private void DataAddedOrRemoved(ProfiledTickData tickData, bool wasAdded)
        {
            /* If added simply see if the bytes in data are larger than the current
             * largest data. */
            if (wasAdded)
            {
                void UpdateAgainstLargest(ref ProfiledTickData lCurrentLargest)
                {
                    if (lCurrentLargest == null || tickData.ClientTraffic.
                }
            }
        }

        #region GUI Rendering
        private void OnGUI()
        {
            DrawButtons();

            void DrawButtons()
            {
                GUILayout.BeginHorizontal();

                string changeEnabledString = _isEnabled ? "Stop Profiling" : "Start Profiling";
                //Toggle profiling.
                if (GUILayout.Button(changeEnabledString))
                    _isEnabled = !_isEnabled;

                //Clear current statistics.
                if (GUILayout.Button("Clear"))
                {
                    ClearProfiledTickData(retainedMinimumTick: 0);
                    selectedSampleIndex = -1;
                    Repaint();
                }


                GUILayout.EndHorizontal();
            }


            DrawGraph();

            //todo If there is an entry selected or hovered then draw it.
        }
        #endregion

        #region Graph Management
        private void DrawGraph()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Network Traffic Graph", EditorStyles.boldLabel);

            float totalWidth = 800f;

            // Create a horizontal layout for the graph and labels
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);

            // Draw the Y-axis labels
            EditorGUILayout.BeginVertical(GUILayout.Width(labelWidth));

            const float graphHeight = 500f;

            DrawBytesColumn();

            void DrawBytesColumn()
            {
                ulong totalBytes = 
            }

            for (int i = 4; i >= 0; i--)
            {
                float value = maxValue * i / 4;
                string label = NetworkTrafficStatistics.FormatBytesToLargest(value);
                EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth), GUILayout.Height(graphHeight / 5));
            }

            EditorGUILayout.EndVertical();

            // Create a scroll view for the graph
            collectiveScrollViewPosition = EditorGUILayout.BeginScrollView(collectiveScrollViewPosition, GUILayout.Height(graphHeight + 20));

            // Draw the graph background
            Rect graphRect = GUILayoutUtility.GetRect(totalWidth, graphHeight);

            // Draw background
            EditorGUI.DrawRect(graphRect, new(0.2f, 0.2f, 0.2f, 1));

            // Draw grid lines
            Handles.color = new(0.3f, 0.3f, 0.3f, 1);
            float gridSegmentHeight = graphRect.height / 5;
            for (int i = 0; i <= 5; i++)
            {
                float y = graphRect.y + (graphRect.height - i * gridSegmentHeight);
                Handles.DrawLine(new(graphRect.x, y, 0), new(graphRect.x + graphRect.width, y, 0));
            }


            // Draw data points
            if (receivedRpcData.Count > 0)
            {
                const float spacing = 2f;
                hoveredSampleIndex = -1; // Reset hover index at the start of drawing

                for (int i = 0; i < receivedRpcData.Count; i++)
                {
                    float x = graphRect.x + (i * barWidth + i * spacing);
                    float currentY = graphRect.y + graphRect.height;

                    // Create a click/hover rect for the entire bar
                    Rect barRect = new Rect(x, graphRect.y, barWidth, graphRect.height);

                    // Check for hover
                    if (barRect.Contains(Event.current.mousePosition))
                    {
                        hoveredSampleIndex = i;

                        // Show tooltip with sample information
                        if (i >= 0 && i < Statistics.samples.Count)
                        {
                            var sample = Statistics.samples[i];
                            string tooltip = $"Frame {i}\n" + $"Received RPCs: {FormatBytes(sample.receivedRpcs.Sum(rpc => rpc.data.length))}\n" + $"Sent RPCs: {FormatBytes(sample.sentRpcs.Sum(rpc => rpc.data.length))}\n" + $"Received Broadcasts: {FormatBytes(sample.receivedBroadcasts.Sum(b => b.data.length))}\n" + $"Sent Broadcasts: {FormatBytes(sample.sentBroadcasts.Sum(b => b.data.length))}\n" + $"Forwarded: {FormatBytes(sample.forwardedBytes.Sum())}";

                            GUI.tooltip = tooltip;
                        }

                        Repaint(); // Repaint to update hover effect
                    }

                    // Determine if this bar is selected or hovered
                    bool isSelected = i == selectedSampleIndex;
                    bool isHovered = i == hoveredSampleIndex;

                    // Draw a highlight for selected or hovered bars
                    if (isSelected || isHovered)
                    {
                        Color highlightColor = isSelected
                            ? new(1f, 1f, 1f, 0.3f)
                            : // White for selected
                            new Color(0.8f, 0.8f, 0.8f, 0.2f); // Light gray for hovered

                        EditorGUI.DrawRect(barRect, highlightColor);
                    }

                    // Draw received RPCs
                    float height = receivedRpcData[i] / maxValue * graphRect.height;
                    EditorGUI.DrawRect(new(x, currentY - height, barWidth, height), new(0.2f, 0.8f, 0.2f, 0.8f));
                    currentY -= height;

                    // Draw sent RPCs
                    height = _sentRpcBytes[i] / maxValue * graphRect.height;
                    EditorGUI.DrawRect(new(x, currentY - height, barWidth, height), new(0.8f, 0.2f, 0.2f, 0.8f));
                    currentY -= height;

                    // Draw received broadcasts
                    height = receivedBroadcastData[i] / maxValue * graphRect.height;
                    EditorGUI.DrawRect(new(x, currentY - height, barWidth, height), new(0.2f, 0.2f, 0.8f, 0.8f));
                    currentY -= height;

                    // Draw sent broadcasts
                    height = _sentBroadcastBytes[i] / maxValue * graphRect.height;
                    EditorGUI.DrawRect(new(x, currentY - height, barWidth, height), new(0.8f, 0.8f, 0.2f, 0.8f));
                    currentY -= height;

                    // Draw forwarded bytes
                    height = forwardedBytesData[i] / maxValue * graphRect.height;
                    EditorGUI.DrawRect(new(x, currentY - height, barWidth, height), new(0.8f, 0.2f, 0.8f, 0.8f));

                    // Handle click to select sample
                    if (Event.current.type == EventType.MouseDown && barRect.Contains(Event.current.mousePosition))
                    {
                        // Toggle selection - if already selected, deselect it
                        if (selectedSampleIndex == i)
                        {
                            selectedSampleIndex = -1;
                        }
                        else
                        {
                            selectedSampleIndex = i;

                            // Pause the editor if in play mode
                            if (Application.isPlaying)
                            {
                                EditorApplication.isPaused = true;
                            }
                        }
                        Repaint();
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Add a more visible resize handle at the bottom of the graph
            Rect resizeHandleRect = GUILayoutUtility.GetRect(0, 10);

            // Draw a visual indicator for the resize handle
            Color originalColor = GUI.color;
            GUI.color = new(0.7f, 0.7f, 0.7f, 1f);
            EditorGUI.DrawRect(resizeHandleRect, new(0.5f, 0.5f, 0.5f, 1f));

            // Draw a grip texture to indicate draggability
            Rect gripRect = new(resizeHandleRect.x + resizeHandleRect.width / 2 - 20, resizeHandleRect.y + resizeHandleRect.height / 2 - 2, 40, 4);
            EditorGUI.DrawRect(new(gripRect.x, gripRect.y, gripRect.width, 1), Color.white);
            EditorGUI.DrawRect(new(gripRect.x, gripRect.y + 3, gripRect.width, 1), Color.white);

            GUI.color = originalColor;

            // Add cursor feedback
            EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeVertical);

            // Handle resize events
            if (Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition))
            {
                isResizingGraph = true;
                resizeStartY = Event.current.mousePosition.y;
                resizeStartHeight = graphHeight;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseUp && isResizingGraph)
            {
                isResizingGraph = false;
                // Save the graph height to EditorPrefs when resizing is complete
                EditorPrefs.SetFloat(WINDOW_SIZE_PREF_NAME, graphHeight);
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseDrag && isResizingGraph)
            {
                float deltaY = Event.current.mousePosition.y - resizeStartY;
                float newHeight = Mathf.Clamp(resizeStartHeight + deltaY, minGraphHeight, maxGraphHeight);

                // Only repaint if the height actually changed
                if (!Mathf.Approximately(newHeight, graphHeight))
                {
                    graphHeight = newHeight;
                    Repaint();
                }

                Event.current.Use();
            }

            EditorGUILayout.EndVertical();
        }
        #endregion

        /// <summary>
        /// Clears all stored profile ticks.
        /// </summary>
        private void ClearProfiledTickData(uint retainedMinimumTick)
        {
            List<uint> keysToRemove = CollectionCaches<uint>.RetrieveList();

            //Remove any entries before tick.
            foreach (KeyValuePair<uint, ProfiledTickData> kvp in _profiledTickData)
            {
                uint tick = kvp.Value.Tick;

                if (tick >= retainedMinimumTick)
                    continue;

                keysToRemove.Add(tick);

                StoreProfiledTickData(kvp.Value);
            }

            //Quick clear if to remove all.
            if (keysToRemove.Count == _profiledTickData.Count)
            {
                _profiledTickData.Clear();
            }
            else
            {
                foreach (uint v in keysToRemove)
                    _profiledTickData.Remove(v);
            }

            CollectionCaches<uint>.Store(keysToRemove);
        }

        /// <summary>
        /// Clears a ProfiledTickData.
        /// </summary>
        private void StoreProfiledTickData(ProfiledTickData value) => ResettableObjectCaches<ProfiledTickData>.Store(value);

        #region Sample Management
        private void DrawSelectedSample(MultiwayTrafficCollection trafficCollection)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Sample Details", EditorStyles.boldLabel);

            // Create a scroll view for the sample details
            try
            {
                // Use GUILayout.ExpandWidth(false) to prevent horizontal expansion
                detailsScrollViewPosition = EditorGUILayout.BeginScrollView(detailsScrollViewPosition, GUILayout.ExpandWidth(false));

                // Wrap content in a vertical layout that expands to fill available width
                EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                if (sample.receivedRpcs.Count > 0)
                {
                    Color originalBgColor = GUI.backgroundColor;
                    GUI.backgroundColor = new(0.3f, 0.9f, 0.3f, 0.2f);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.backgroundColor = originalBgColor;

                    GUIStyle headerStyle = new(EditorStyles.foldout);
                    Color headerColor = new(0.2f, 0.8f, 0.2f, 1f);
                    headerStyle.normal.textColor = headerColor;
                    headerStyle.onNormal.textColor = headerColor;
                    headerStyle.focused.textColor = headerColor;
                    headerStyle.onFocused.textColor = headerColor;
                    headerStyle.active.textColor = headerColor;
                    headerStyle.onActive.textColor = headerColor;
                    headerStyle.fontStyle = FontStyle.Bold;
                    bool sectionExpanded = EditorGUILayout.Foldout(GetDetailTreeState("section_received_rpcs", true), "Received RPCs", true, headerStyle);
                    SetDetailTreeState("section_received_rpcs", sectionExpanded);

                    if (sectionExpanded)
                    {
                        EditorGUI.indentLevel++;
                        // Aggregate received RPCs by type and method
                        var aggregatedReceivedRpcs = sample.receivedRpcs.GroupBy(rpc => new { rpc.type, rpc.method }).Select(group => new
                        {
                            Type = group.Key.type,
                            Method = group.Key.method,
                            Count = group.Count(),
                            TotalBytes = group.Sum(rpc => rpc.data.length),
                            Items = group.ToList()
                        }).OrderByDescending(rpc => rpc.TotalBytes);

                        foreach (var rpcGroup in aggregatedReceivedRpcs)
                        {
                            // Create a foldout for each RPC group
                            string label = $"{rpcGroup.Type.GetFriendlyTypeName()}.{rpcGroup.Method} ({FormatBytes(rpcGroup.TotalBytes)}) - {rpcGroup.Count} calls";
                            bool expanded = EditorGUILayout.Foldout(GetDetailTreeState($"received_{rpcGroup.Type.Name}_{rpcGroup.Method}", false), label, true);
                            SetDetailTreeState($"received_{rpcGroup.Type.Name}_{rpcGroup.Method}", expanded);

                            if (expanded)
                            {
                                EditorGUI.indentLevel++;
                                foreach (var rpc in rpcGroup.Items)
                                {
                                    string packetKey = $"received_rpc_{rpc.type.Name}_{rpc.method}_{rpc.GetHashCode()}";
                                    bool isExpanded = GetPacketTreeState(packetKey);

                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Space(30); // Increased indentation space

                                    EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                                    // Temporarily decrease indent level for the foldout
                                    EditorGUI.indentLevel--;
                                    GUILayout.BeginHorizontal();
                                    bool newExpanded = EditorGUILayout.Foldout(isExpanded, $"{FormatBytes(rpc.data.length)} bytes", true);
                                    if (rpc.context != null)
                                        EditorGUILayout.ObjectField(rpc.context, typeof(UnityEngine.Object), true);
                                    GUILayout.EndHorizontal();
                                    EditorGUI.indentLevel++;

                                    if (newExpanded != isExpanded)
                                    {
                                        SetPacketTreeState(packetKey, newExpanded);
                                        Repaint();
                                    }

                                    // Show packet data if expanded
                                    if (isExpanded)
                                    {
                                        EditorGUILayout.BeginVertical();
                                        EditorGUILayout.TextArea(GetRpcOrBroadcastDataString(rpc.data, rpc.type, rpc.rpcType, rpc.method));
                                        EditorGUILayout.EndVertical();
                                    }

                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }

                if (sample.sentRpcs.Count > 0)
                {
                    Color originalBgColor = GUI.backgroundColor;
                    GUI.backgroundColor = new(0.9f, 0.3f, 0.3f, 0.2f);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.backgroundColor = originalBgColor;

                    GUIStyle headerStyle = new GUIStyle(EditorStyles.foldout);
                    Color headerColor = new(0.8f, 0.2f, 0.2f, 1f);
                    headerStyle.normal.textColor = headerColor;
                    headerStyle.onNormal.textColor = headerColor;
                    headerStyle.focused.textColor = headerColor;
                    headerStyle.onFocused.textColor = headerColor;
                    headerStyle.active.textColor = headerColor;
                    headerStyle.onActive.textColor = headerColor;
                    headerStyle.fontStyle = FontStyle.Bold;
                    bool sectionExpanded = EditorGUILayout.Foldout(GetDetailTreeState("section_sent_rpcs", true), "Sent RPCs", true, headerStyle);
                    SetDetailTreeState("section_sent_rpcs", sectionExpanded);

                    if (sectionExpanded)
                    {
                        EditorGUI.indentLevel++;
                        // Aggregate sent RPCs by type and method
                        var aggregatedSentRpcs = sample.sentRpcs.GroupBy(rpc => new { rpc.type, rpc.method }).Select(group => new
                        {
                            Type = group.Key.type,
                            Method = group.Key.method,
                            Count = group.Count(),
                            TotalBytes = group.Sum(rpc => rpc.data.length),
                            Items = group.ToList()
                        }).OrderByDescending(rpc => rpc.TotalBytes);

                        foreach (var rpcGroup in aggregatedSentRpcs)
                        {
                            // Create a foldout for each RPC group
                            string label = $"{rpcGroup.Type.GetFriendlyTypeName()}.{rpcGroup.Method} ({FormatBytes(rpcGroup.TotalBytes)}) - {rpcGroup.Count} calls";
                            bool expanded = EditorGUILayout.Foldout(GetDetailTreeState($"sent_{rpcGroup.Type.Name}_{rpcGroup.Method}", false), label, true);
                            SetDetailTreeState($"sent_{rpcGroup.Type.Name}_{rpcGroup.Method}", expanded);

                            if (expanded)
                            {
                                EditorGUI.indentLevel++;
                                foreach (var rpc in rpcGroup.Items)
                                {
                                    string packetKey = $"sent_rpc_{rpc.type.Name}_{rpc.method}_{rpc.GetHashCode()}";
                                    bool isExpanded = GetPacketTreeState(packetKey);

                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Space(30); // Increased indentation space

                                    EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                                    // Temporarily decrease indent level for the foldout
                                    EditorGUI.indentLevel--;
                                    GUILayout.BeginHorizontal();
                                    bool newExpanded = EditorGUILayout.Foldout(isExpanded, $"{FormatBytes(rpc.data.length)} bytes", true);
                                    if (rpc.context != null)
                                        EditorGUILayout.ObjectField(rpc.context, typeof(UnityEngine.Object), true);
                                    GUILayout.EndHorizontal();
                                    EditorGUI.indentLevel++;

                                    if (newExpanded != isExpanded)
                                    {
                                        SetPacketTreeState(packetKey, newExpanded);
                                        Repaint();
                                    }

                                    // Show packet data if expanded
                                    if (isExpanded)
                                    {
                                        EditorGUILayout.BeginVertical();
                                        EditorGUILayout.TextArea(GetRpcOrBroadcastDataString(rpc.data, rpc.type, rpc.rpcType, rpc.method));
                                        EditorGUILayout.EndVertical();
                                    }

                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }

                if (sample.receivedBroadcasts.Count > 0)
                {
                    Color originalBgColor = GUI.backgroundColor;
                    GUI.backgroundColor = new(0.3f, 0.3f, 0.9f, 0.2f);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.backgroundColor = originalBgColor;

                    GUIStyle headerStyle = new GUIStyle(EditorStyles.foldout);
                    Color headerColor = new(0.4f, 0.6f, 1.0f, 1f);
                    headerStyle.normal.textColor = headerColor;
                    headerStyle.onNormal.textColor = headerColor;
                    headerStyle.focused.textColor = headerColor;
                    headerStyle.onFocused.textColor = headerColor;
                    headerStyle.active.textColor = headerColor;
                    headerStyle.onActive.textColor = headerColor;
                    headerStyle.fontStyle = FontStyle.Bold;
                    bool sectionExpanded = EditorGUILayout.Foldout(GetDetailTreeState("section_received_broadcasts", true), "Received Broadcasts", true, headerStyle);
                    SetDetailTreeState("section_received_broadcasts", sectionExpanded);

                    if (sectionExpanded)
                    {
                        EditorGUI.indentLevel++;
                        // Aggregate received broadcasts by type
                        var aggregatedReceivedBroadcasts = sample.receivedBroadcasts.GroupBy(broadcast => broadcast.type).Select(group => new
                        {
                            Type = group.Key,
                            Count = group.Count(),
                            TotalBytes = group.Sum(broadcast => broadcast.data.length),
                            Items = group.ToList()
                        }).OrderByDescending(broadcast => broadcast.TotalBytes);

                        foreach (var broadcastGroup in aggregatedReceivedBroadcasts)
                        {
                            // Create a foldout for each broadcast group
                            string label = $"{broadcastGroup.Type.GetFriendlyTypeName()} ({FormatBytes(broadcastGroup.TotalBytes)}) - {broadcastGroup.Count} broadcasts";
                            bool expanded = EditorGUILayout.Foldout(GetDetailTreeState($"received_broadcast_{broadcastGroup.Type.Name}", false), label, true);
                            SetDetailTreeState($"received_broadcast_{broadcastGroup.Type.Name}", expanded);

                            if (expanded)
                            {
                                EditorGUI.indentLevel++;
                                foreach (var broadcast in broadcastGroup.Items)
                                {
                                    string packetKey = $"received_broadcast_{broadcast.type.Name}_{broadcast.GetHashCode()}";
                                    bool isExpanded = GetPacketTreeState(packetKey);

                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Space(30); // Increased indentation space

                                    EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                                    // Temporarily decrease indent level for the foldout
                                    EditorGUI.indentLevel--;
                                    bool newExpanded = EditorGUILayout.Foldout(isExpanded, $"{FormatBytes(broadcast.data.length)} bytes", true);
                                    EditorGUI.indentLevel++;

                                    if (newExpanded != isExpanded)
                                    {
                                        SetPacketTreeState(packetKey, newExpanded);
                                        Repaint();
                                    }

                                    // Show packet data if expanded
                                    if (isExpanded)
                                    {
                                        EditorGUILayout.BeginVertical();
                                        EditorGUILayout.TextArea(GetRpcOrBroadcastDataString(broadcast.data, broadcast.type, default));
                                        EditorGUILayout.EndVertical();
                                    }

                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }

                if (sample.sentBroadcasts.Count > 0)
                {
                    Color originalBgColor = GUI.backgroundColor;
                    GUI.backgroundColor = new(0.9f, 0.9f, 0.3f, 0.2f);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.backgroundColor = originalBgColor;

                    GUIStyle headerStyle = new GUIStyle(EditorStyles.foldout);
                    Color headerColor = new(0.8f, 0.8f, 0.2f, 1f);
                    headerStyle.normal.textColor = headerColor;
                    headerStyle.onNormal.textColor = headerColor;
                    headerStyle.focused.textColor = headerColor;
                    headerStyle.onFocused.textColor = headerColor;
                    headerStyle.active.textColor = headerColor;
                    headerStyle.onActive.textColor = headerColor;
                    headerStyle.fontStyle = FontStyle.Bold;
                    bool sectionExpanded = EditorGUILayout.Foldout(GetDetailTreeState("section_sent_broadcasts", true), "Sent Broadcasts", true, headerStyle);
                    SetDetailTreeState("section_sent_broadcasts", sectionExpanded);

                    if (sectionExpanded)
                    {
                        EditorGUI.indentLevel++;
                        // Aggregate sent broadcasts by type
                        var aggregatedSentBroadcasts = sample.sentBroadcasts.GroupBy(broadcast => broadcast.type).Select(group => new
                        {
                            Type = group.Key,
                            Count = group.Count(),
                            TotalBytes = group.Sum(broadcast => broadcast.data.length),
                            Items = group.ToList()
                        }).OrderByDescending(broadcast => broadcast.TotalBytes);

                        foreach (var broadcastGroup in aggregatedSentBroadcasts)
                        {
                            // Create a foldout for each broadcast group
                            string label = $"{broadcastGroup.Type.GetFriendlyTypeName()} ({FormatBytes(broadcastGroup.TotalBytes)}) - {broadcastGroup.Count} broadcasts";
                            bool expanded = EditorGUILayout.Foldout(GetDetailTreeState($"sent_broadcast_{broadcastGroup.Type.Name}", false), label, true);
                            SetDetailTreeState($"sent_broadcast_{broadcastGroup.Type.Name}", expanded);

                            if (expanded)
                            {
                                EditorGUI.indentLevel++;
                                foreach (var broadcast in broadcastGroup.Items)
                                {
                                    string packetKey = $"sent_broadcast_{broadcast.type.Name}_{broadcast.GetHashCode()}";
                                    bool isExpanded = GetPacketTreeState(packetKey);

                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Space(30); // Increased indentation space

                                    EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                                    // Temporarily decrease indent level for the foldout
                                    EditorGUI.indentLevel--;
                                    bool newExpanded = EditorGUILayout.Foldout(isExpanded, $"{FormatBytes(broadcast.data.length)} bytes", true);
                                    EditorGUI.indentLevel++;

                                    if (newExpanded != isExpanded)
                                    {
                                        SetPacketTreeState(packetKey, newExpanded);
                                        Repaint();
                                    }

                                    // Show packet data if expanded
                                    if (isExpanded)
                                    {
                                        EditorGUILayout.BeginVertical();
                                        EditorGUILayout.TextArea(GetRpcOrBroadcastDataString(broadcast.data, broadcast.type, default));
                                        EditorGUILayout.EndVertical();
                                    }

                                    EditorGUILayout.EndVertical();
                                    EditorGUILayout.EndHorizontal();
                                }
                                EditorGUI.indentLevel--;
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }

                // Draw Forwarded Bytes
                if (sample.forwardedBytes.Count > 0)
                {
                    Color originalBgColor = GUI.backgroundColor;
                    GUI.backgroundColor = new(0.9f, 0.3f, 0.9f, 0.2f);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUI.backgroundColor = originalBgColor;

                    GUIStyle headerStyle = new GUIStyle(EditorStyles.foldout);
                    Color headerColor = new(0.8f, 0.2f, 0.8f, 1f);
                    headerStyle.normal.textColor = headerColor;
                    headerStyle.onNormal.textColor = headerColor;
                    headerStyle.focused.textColor = headerColor;
                    headerStyle.onFocused.textColor = headerColor;
                    headerStyle.active.textColor = headerColor;
                    headerStyle.onActive.textColor = headerColor;
                    headerStyle.fontStyle = FontStyle.Bold;
                    bool sectionExpanded = EditorGUILayout.Foldout(GetDetailTreeState("section_forwarded_bytes", true), "Forwarded Bytes", true, headerStyle);
                    SetDetailTreeState("section_forwarded_bytes", sectionExpanded);

                    if (sectionExpanded)
                    {
                        EditorGUI.indentLevel++;
                        int totalBytes = sample.forwardedBytes.Sum();
                        EditorGUILayout.LabelField($"Total: {FormatBytes(totalBytes)}");
                        EditorGUILayout.LabelField($"Count: {sample.forwardedBytes.Count} packets");
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
            catch
            {
                // Make sure to close all layout groups in case of exception
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSample(TickSample sample, int index)
        {
            // Determine if this sample is selected
            bool isSelected = index == selectedSampleIndex;

            // Use a different style for the selected sample
            GUIStyle boxStyle = isSelected ? new(EditorStyles.helpBox) { normal = { background = EditorGUIUtility.whiteTexture }, border = new(2, 2, 2, 2) } : EditorStyles.helpBox;

            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("Overview", EditorStyles.boldLabel);

            // Draw summary
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"RPCs: {sample.receivedRpcs.Count} received, {sample.sentRpcs.Count} sent");
            EditorGUILayout.LabelField($"Broadcasts: {sample.receivedBroadcasts.Count} received, {sample.sentBroadcasts.Count} sent");
            EditorGUILayout.LabelField($"Forwarded: {FormatBytes(sample.forwardedBytes.Sum())} in {sample.forwardedBytes.Count} packets");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        #endregion

        #region Helper Methods
        // Helper method to get foldout state
        private bool GetDetailTreeState(string key, bool defaultValue = false)
        {
            _detailTreeStates.TryAdd(key, defaultValue);
            return _detailTreeStates[key];
        }

        // Helper method to set foldout state
        private void SetDetailTreeState(string key, bool value)
        {
            _detailTreeStates[key] = value;
        }

        // Helper method to get expanded packet state
        private bool GetPacketTreeState(string key)
        {
            if (_packetTreeStates.TryAdd(key, false))
                return false;
            return _packetTreeStates[key];
        }

        // Helper method to set expanded packet state
        private void SetPacketTreeState(string key, bool value)
        {
            _packetTreeStates[key] = value;
        }

        //private static readonly Dictionary<Type, object> _deserializedObjects = new();

        // static bool ShouldIgnore(RPCType rpcType, Type paramType, int index, int count)
        // {
        //     if (index == count - 1 && paramType == typeof(RPCInfo))
        //         return true;
        //
        //     if (index == 0 && rpcType == RPCType.TargetRPC && paramType == typeof(PlayerID))
        //         return true;
        //
        //     return false;
        // }

        // private static string PrintRPC(Type type, BitPacker tempPacker, string methodName, RPCType rpcType)
        // {
        //     MethodInfo method = type.GetMethod(methodName);
        //
        //     if (method == null)
        //         return $"Failed to find method {methodName} in type {type.Name}";
        //
        //     ParameterInfo[] parameters = method.GetParameters();
        //     StringBuilder _sb = new StringBuilder();
        //
        //     for (int i = 0; i < parameters.Length; i++)
        //     {
        //         ParameterInfo param = parameters[i];
        //         Type paramType = param.ParameterType;
        //         string paramName = param.Name;
        //
        //         if (paramType.IsGenericParameter)
        //             continue;
        //
        //         // if (ShouldIgnore(rpcType, paramType, i, parameters.Length))
        //         //     continue;
        //
        //         object obj = _deserializedObjects.GetValueOrDefault(paramType);
        //         Packer.Read(tempPacker, paramType, ref obj);
        //         _deserializedObjects[paramType] = obj;
        //
        //         _sb.AppendLine($"Parameter {i + 1:00}: {paramName} ({paramType.Name}) = {obj}");
        //     }
        //
        //     return _sb.ToString();
        // }
        //
        // private static string PrintBroadcast(Type type, BitPacker tempPacker)
        // {
        //     var typeIdx = default(PackedUInt);
        //     object obj = _deserializedObjects.GetValueOrDefault(type);
        //     Packer<PackedUInt>.Read(tempPacker, ref typeIdx);
        //     Packer.Read(tempPacker, type, ref obj);
        //     _deserializedObjects[type] = obj;
        //     return $"{obj}";
        // }
        #endregion

        #region Windows.
        /// <summary>
        /// Opens any current instance of the Network Profiler.
        /// </summary>
        [MenuItem("Tools/Fish-Networking/Utility/Network Profiler")]
        public static void ShowNetworkProfiler() => ShowNetworkProfiler(newInstance: false);

        /// <summary>
        /// Opens a new instance of the Network Profiler.
        /// </summary>
        [MenuItem("Tools/Fish-Networking/Utility/New Network Profiler")]
        public static void ShowNewNetworkProfiler() => ShowNetworkProfiler(newInstance: true);

        /// <summary>
        /// Shows the Network Profiler as a single or new instance.
        /// </summary>
        private static void ShowNetworkProfiler(bool newInstance)
        {
            /* If newInstance then always create, otherwise only create
             * if there is currently not an instance. */
            NetworkProfilerWindow window = newInstance || Instances.Count == 0 ? window = CreateInstance<NetworkProfilerWindow>() : Instances[0];
            //Naming is excessive if instance was already made, but retting name without checks reduces branching/code complexity.
            window.titleContent = new(WINDOW_NAME, image: null, WINDOW_NAME);

            window.Show();
        }

        /// <summary>
        /// Returns a saved Vector2.
        /// </summary>
        private Vector2 GetEditorPrefs(string keyPrefix, Vector2 defaultValue)
        {
            Vector2 result = default;

            result.x = EditorPrefs.GetFloat(keyPrefix + FLOAT_X_PREF_NAME, defaultValue.x);
            result.y = EditorPrefs.GetFloat(keyPrefix + FLOAT_Y_PREF_NAME, defaultValue.y);

            return result;
        }

        /// <summary>
        /// Saves a vector2.
        /// </summary>
        private void SaveEditorPrefs(string keyPrefix, Vector2 value)
        {
            EditorPrefs.SetFloat(keyPrefix + FLOAT_X_PREF_NAME, value.x);
            EditorPrefs.SetFloat(keyPrefix + FLOAT_Y_PREF_NAME, value.y);
        }

        /// <summary>
        /// Saves current window size if it differs from last.
        /// </summary>
        /// <param name="force"></param>
        private void SaveWindowSize(bool force)
        {
            //Not enough time has passed.
            if (!force && Time.realtimeSinceStartup < _nextWindowSaveTime)
                return;

            _nextWindowSaveTime = Time.realtimeSinceStartup;

            //Size is unchanged.
            if (position.size == _windowSize)
                return;

            SaveEditorPrefs(WINDOW_SIZE_PREFIX_PREF_NAME, position.size);
        }
        #endregion
    }
}
#endif