public interface INote
{
    public enum NoteState { ACTIVE, DEAD}
    public NoteState State { get; set; }
    NoteData Data { get; set; }
    public void Initialize(NoteData noteData, Track track, float timeCreated);

    public void ManualUpdate(float time);
}