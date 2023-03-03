using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

[CustomEditor (typeof( SongEditorManager))]
public class SongEditor : Editor {

	SongEditorManager m_target;

    AudioClip m_lastSong;   
	
	void OnEnable(){
		m_target = (SongEditorManager)target;
        m_lastSong = m_target.music;
	}

	public override void OnInspectorGUI(){
		
		base.OnInspectorGUI ();

        //Check if we loaded a new song
        if( m_target.music != m_lastSong )
        {
            m_target.Reset();
        }
        m_lastSong = m_target.music;

		GUILayout.Space (5.0f);
		//EXPORT
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});

		m_target.music =(AudioClip) EditorGUILayout.ObjectField ("Music", m_target.music, typeof (AudioClip), true);
		if( m_target.music == null)
			m_target.songName = EditorGUILayout.TextField ("DebugName", m_target.songName);
		m_target.difficulty = (BattleEngine.Difficulty) EditorGUILayout.EnumPopup ("Diff",m_target.difficulty);
		m_target.timeSpeed = EditorGUILayout.FloatField ("Time speed", m_target.timeSpeed);

		if (GUILayout.Button ("Export")) {
			m_target.Export(m_target.music.name,m_target.difficulty);
		}
		
		GUILayout.Space (5.0f);
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		GUILayout.Space (5.0f);
		m_target.songImportName = EditorGUILayout.TextField ("Import Name", m_target.songImportName);
		m_target.songImportDifficulty = (BattleEngine.Difficulty) EditorGUILayout.EnumPopup ("Diff",m_target.songImportDifficulty);

		if (GUILayout.Button ("Import")) {
			m_target.Import();
		}

		GUILayout.Space (15.0f);
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		if (GUILayout.Button (" recheck & sort")) {
			m_target.RecheckAll();
		}
		
		GUILayout.Space (15.0f);
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
		if (GUILayout.Button ("ClearAll")) {
			m_target.ClearAll();
		}
	}

	//Check the scene
	void OnSceneGUI(){
		Handles.BeginGUI();
		m_target.OnSceneGUI();
		Handles.EndGUI();

		int controlID = GUIUtility.GetControlID (FocusType.Passive);
		Event e = Event.current;
		Ray ray = Camera.current.ScreenPointToRay ( 
		                                           new Vector2(e.mousePosition.x, 
		            -e.mousePosition.y + Camera.current.pixelHeight) 
		                                           );
		Vector3 mousePos = ray.origin;

		if (e.isMouse && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)) {
			GUIUtility.hotControl = controlID;
			e.Use ();
			
			m_target.OnSceneClick (mousePos);
		} else {
			m_target.OnSceneRelease(mousePos);
		}
		
		//Clean hotControl
		if (e.isMouse && e.type == EventType.MouseUp) {
			GUIUtility.hotControl = 0;
		}
	}


}
