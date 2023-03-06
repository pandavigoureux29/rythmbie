using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SongEditorNote))]
public class SongEditorNoteInspector : Editor {
	SongEditorNote m_note;
	// Use this for initialization
	void OnEnable () {
		m_note = (SongEditorNote)target;
	}
	
	public override void OnInspectorGUI(){
		//TYPE
		EditorGUI.BeginChangeCheck ();
		m_note.type = (NoteType) EditorGUILayout.EnumPopup ("Type",m_note.type);
		if (EditorGUI.EndChangeCheck ()) {
			m_note.OnTypeChanged();
		}
		//TIME
		EditorGUI.BeginChangeCheck ();
		float newTime = m_note.time;
		newTime =  EditorGUILayout.FloatField ("Time", newTime);
		if (EditorGUI.EndChangeCheck ()) {
			m_note.time = newTime;
		}
		
		base.OnInspectorGUI ();
	}
}