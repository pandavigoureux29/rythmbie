using UnityEngine.SocialPlatforms.Impl;

public interface INote
{
    public enum NoteState { ACTIVE, HIT, MISS, ATTACK, DEAD}
    public NoteState State { get; set; }
    NoteData Data { get; set; }
    public void Initialize(NoteData noteData, Track track, float timeCreated);

    public void ManualUpdate(float time);
}

public struct NoteInputResult
{
    public NoteHitResult HitResult;
    public ScoreAccuracy Accuracy;

    public static NoteInputResult None => new NoteInputResult { HitResult = NoteHitResult.NONE };
}