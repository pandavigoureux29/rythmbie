using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SongEditorCamera : MonoBehaviour {
		
	[SerializeField] SongEditorManager m_manager;
	[SerializeField] List<GameObject> m_timeBars;
	[SerializeField] Transform m_playBarTransf;

	[SerializeField] float m_cameraZ = 0.0f;
	[SerializeField] float m_cameraY = 0.6f;
	[HideInInspector] [SerializeField] float m_currentTime = 0;

	[SerializeField] private float m_deltaTimeBars = 10;

	float m_initPlayBarPosX = 0;

	SongEditorNote m_lastNote;
	string m_timeText = "";
	string m_beginCopyText="0";
	string m_endCopyText="0";
	string m_pasteTimeText = "0";

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void ManualUpdate(){
		//Debug.Log ( SceneView.lastActiveSceneView.pivot );
		MoveSceneViewCamera ();
		DrawGUI ();
		UpdatePlayBar ();
		PlaceTimeBars ();

	}

	void DrawGUI(){
		//move left
		if (GUI.Button (new Rect (0, 0, 50, 50), "<")) {
			if( m_currentTime > 0 )
				m_currentTime --;
			PlaceTimeBars ();
		}
        if (GUI.Button(new Rect(0, 50, 50, 20), "<<"))
        {
            m_currentTime-= m_currentTime > 10 ? 10 : m_currentTime;
            PlaceTimeBars();
        }
        GUI.TextField( new Rect (50, 0, 20, 20), ""+m_currentTime) ;
		//move right
		if (GUI.Button (new Rect (80, 0, 50, 50), ">")) {
			m_currentTime++;
			PlaceTimeBars ();
		}
        if (GUI.Button(new Rect(80, 50, 50, 20), ">>"))
        {
            m_currentTime+= 10;
            PlaceTimeBars();
        }

		//PLAY
		if (m_manager.CurrentMode == SongEditorManager.Mode.PLAY) {
			if (GUI.Button (new Rect (250, 0, 55, 50), "Stop")) {
				m_manager.Stop();
				Utils.SetLocalPositionX(m_playBarTransf, m_initPlayBarPosX);
			}
		} else {			
			if (GUI.Button (new Rect (250, 0, 55, 50), "Play")) {
				m_initPlayBarPosX = m_playBarTransf.localPosition.x;
				m_manager.Play();
			}
		}
		//playback speed
		if (m_manager.AudioComponent.pitch > 0.9f) {
			if (GUI.Button (new Rect (307, 0, 30, 25), "" + m_manager.AudioComponent.pitch)) {
				m_manager.AudioComponent.pitch = 0.5f;
			}
		} else {
			if (GUI.Button (new Rect (307, 0, 30, 25), "" + m_manager.AudioComponent.pitch)) {
				m_manager.AudioComponent.pitch = 1f;
			}
		}
		                                       

		//NOTE EDIT
		if (m_manager.CurrentNote) {
			//TIME
			if (GUI.Button (new Rect (350, 0, 10, 20), "-" )) {
				m_manager.CurrentNote.time -= 0.05f;
				m_timeText = ""+m_manager.CurrentNote.time;
			}
			if( m_manager.CurrentNote != m_lastNote){
				m_lastNote = m_manager.CurrentNote;
				m_timeText = ""+m_manager.CurrentNote.time;
			}
			m_timeText = GUI.TextField( new Rect (360, 0, 90, 20), m_timeText) ;
			float time = 0.0f;
			bool b = float.TryParse( m_timeText, out time  );
			if( b && m_timeText.LastIndexOf(".") != m_timeText.Length-1) 
				m_manager.CurrentNote.time = time;

			if (GUI.Button (new Rect (450, 0, 10, 20), "+" )) {
				m_manager.CurrentNote.time += 0.05f;
				m_timeText = ""+m_manager.CurrentNote.time;
			}
			//TYPE
			if (GUI.Button (new Rect (350, 20, 70, 25), "" + m_manager.CurrentNote.type.ToString())) {
				m_manager.CurrentNote.ToggleType();
			}

			//HEAD
			m_manager.CurrentNote.head = GUI.Toggle(new Rect (420, 20, 34, 30), m_manager.CurrentNote.head,"hd") ;

		}

		//COPY PASTE

		//Begin copy
		m_beginCopyText = GUI.TextField( new Rect (500, 30, 50, 20), m_beginCopyText) ;
		float timeBeginCopy = 0.0f;
		bool bBC = float.TryParse( m_beginCopyText, out timeBeginCopy  );

		//End copy
		m_endCopyText = GUI.TextField( new Rect (550, 30, 50, 20), m_endCopyText) ;
		float timeEndCopy = 0.0f;
		bBC = bBC && float.TryParse( m_endCopyText, out timeEndCopy  );

		//copy
		if ( GUI.Button (new Rect (500, 0, 120, 25), "copy") && bBC) {
			m_manager.CopyNotes( timeBeginCopy, timeEndCopy );
		}


		//time paste
		m_pasteTimeText = GUI.TextField( new Rect (630, 30, 50, 20), m_pasteTimeText) ;
		float timePaste = 0.0f;
		bBC = float.TryParse( m_pasteTimeText, out timePaste  );
		//paste
		if (bBC && GUI.Button (new Rect (630, 0, 50, 25), "paste")) {
			m_manager.PasteNotes( timePaste  );
		}
	}

	void MoveSceneViewCamera()
	{
		
#if UNITY_EDITOR
		Vector3 position = SceneView.lastActiveSceneView.pivot;
		//in play mode, follow the playbar
		if (m_manager.CurrentMode == SongEditorManager.Mode.PLAY) {
			position.x = m_playBarTransf.position.x;
		//in other mode
		} else {
			position.z = m_cameraZ;
			position.y = m_cameraY;
			position.x = (m_currentTime + 2) * m_manager.UnitsPerSeconds;
		}
		SceneView.lastActiveSceneView.pivot = position;
		SceneView.lastActiveSceneView.Repaint();
#endif
	}

	void PlaceTimeBars(){		
#if UNITY_EDITOR
		int time = (int) (SceneView.lastActiveSceneView.pivot.x / m_deltaTimeBars) -3;
		if (time < 0)
			time = 0;
		for (int i=0; i < m_timeBars.Count; i++) {
			GameObject timeBar = m_timeBars[i];
			Utils.SetLocalPositionX(timeBar.transform, m_deltaTimeBars * (float)time);
			//Text
			TextMesh tm = timeBar.GetComponentInChildren<TextMesh>();
			tm.text = ""+ time;
			time ++;
		}
#endif
	}

	#region PLAY_BAR

	public Transform PlayBar{
		get{
			return m_playBarTransf;
		}
	}

	public void ReplacePlayBar(Vector3 _mousePosition){
		Utils.SetLocalPositionX (m_playBarTransf, _mousePosition.x);
	}

	void UpdatePlayBar(){
		if (m_manager.CurrentMode == SongEditorManager.Mode.PLAY) {
			float time = m_manager.AudioComponent.timeSamples / (float)m_manager.AudioComponent.clip.frequency ;
			float x = m_manager.ComputeNoteXByTime(time);
			Utils.SetLocalPositionX( m_playBarTransf, x);
		}
	}

	#endregion

    public void Reset()
    {
        m_currentTime = 0;
        PlaceTimeBars();
        Utils.SetLocalPositionX(m_playBarTransf, m_initPlayBarPosX);
    }
}
