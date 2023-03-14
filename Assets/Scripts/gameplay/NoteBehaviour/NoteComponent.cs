using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteComponent : MonoBehaviour, INote
{
    public NoteData Data { get; set; }
    private Track m_track;
    public Track Track => m_track;
    private float m_timeCreated;

    private float m_distanceDone = 0;
    
    //time from the slot (perfect time) to the end of the track.basically the extra time the player has to hit the note after the perfect time.
    private float m_extraTime = 0;

    private float m_totalTime = 0;

    public INote.NoteState State { get; set; } = INote.NoteState.DEAD;

    public Action OnStarted;
    public Action OnHit;
    public Action OnMissed;
    public Action OnDied;
    public Action OnAttack;
    
    public void Initialize(NoteData noteData, Track track, float timeCreated)
    {
        Data = noteData;
        m_track = track;
        transform.position = track.Begin.position;
        m_timeCreated = timeCreated;

        m_extraTime = (track.DistanceTotal - track.DistanceToSlot) / GameManager.Instance.SongData.Speed;
        m_totalTime = Data.Time + m_extraTime;
        
        State = INote.NoteState.ACTIVE;
        OnStarted?.Invoke();
    }

    public void ManualUpdate(float time)
    {
        //time progression of the note on the track [0,1]
        var timeProgression = Mathf.InverseLerp(m_timeCreated, m_totalTime, time);
        
        switch (State)
        {
            case INote.NoteState.DEAD :
                break;
            case INote.NoteState.ACTIVE:
                UpdateActiveState(timeProgression);
                break;
            case INote.NoteState.HIT :
                UpdateHitState();
                break;
            case INote.NoteState.MISS:
                UpdateMissState(timeProgression);
                break;
            case INote.NoteState.ATTACK:
                UpdateAttackState();
                break;
        }
    }

    protected virtual void UpdateActiveState(float timeProgression)
    {
        MoveToTarget(timeProgression);
        if(timeProgression >= 1)
            Miss();
    }

    protected virtual void UpdateMissState(float timeProgression)
    {
        MoveToTarget(timeProgression);
        if (timeProgression >= 1)
        {
            State = INote.NoteState.ATTACK;
            OnAttack?.Invoke();
        }
    }

    protected virtual void UpdateHitState()
    {
        
    }

    protected virtual void UpdateAttackState()
    {
        
    }

    void MoveToTarget(float timeProgression)
    {
        m_distanceDone = Mathf.Lerp(0, m_track.DistanceTotal, timeProgression);

        var position = m_track.Begin.position + m_track.Direction * m_distanceDone;
        transform.position = position;
    }

    public void CleanUp()
    {
        m_track.RemoveNote(this);
        Data = null;
        m_track = null;
    }

    public void Dispose()
    {
        Destroy(gameObject);
    }

    public virtual void Hit(ScoreAccuracy accuracy)
    {
        if(State == INote.NoteState.DEAD)
            return;
        
        OnHit?.Invoke();
        LR.EventDispatcher.Instance.Publish(new NoteHitEventData{Note = this, Accuracy = accuracy});
        
        m_track.DeactivateNote(this);
        Die(true);
    }

    public void Miss()
    {
        if(State == INote.NoteState.DEAD)
            return;
        
        OnMissed?.Invoke();
        LR.EventDispatcher.Instance.Publish(new NoteMissedEventData{Note = this});
        
        m_track.DeactivateNote(this);
        State = INote.NoteState.MISS;
    }
    
    public void Die(bool isHit)
    {
        State = INote.NoteState.DEAD;
        OnDied?.Invoke();
        LR.EventDispatcher.Instance.Publish(new NoteDiedEventData{Note = this, IsHit = isHit});
    }

    public virtual NoteInputResult CheckInput(GameplayInputActionInfos inputActionInfos, float currentTime)
    {
        if (State != INote.NoteState.ACTIVE)
            return NoteInputResult.None;
        
        //check input ype for this note
        bool isCorrectInputType = false;

        switch (inputActionInfos.inputType)
        {
            case InputActionType.TAP_STARTED:
                if (Data.Type == NoteType.SIMPLE)
                    isCorrectInputType = true;
                break;
        }

        if (!isCorrectInputType)
            return NoteInputResult.None;

        float deltaTime = Mathf.Abs(currentTime - Data.Time);
        var accuracy = GameManager.Instance.ScoreManager.ScoreData.GetAccuracy(deltaTime);
//        Debug.Log($"delta : {deltaTime} cur : {currentTime} note : {m_noteData.Time}  ACC : {accuracy}");

        return new NoteInputResult
        {
            HitResult = accuracy != ScoreAccuracy.MISSED ? NoteHitResult.HIT : NoteHitResult.MISSED,
            Accuracy =  accuracy
        };
    }
}