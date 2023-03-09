using System.Collections;
using System.Collections.Generic;
using LR;
using UnityEngine;

public class GameParticleEffectsManager : MonoBehaviour
{
    [SerializeField] private GameObject m_bloodSplatterShotPrefab;

    private Pool<ParticleSystem> m_enemyBloodSplatterShotsPool;

    public void Initialize()
    {
        m_enemyBloodSplatterShotsPool = new Pool<ParticleSystem>(CreateNewBloodSplatter,CleanUpBloodSplatter,DisposeBloodSplatter);
        m_enemyBloodSplatterShotsPool.Capacity = 4;
        
        LR.EventDispatcher.Instance.Subscribe<NoteHitEventData>(OnNoteHit);
    }

    void OnNoteHit(NoteHitEventData noteEventData)
    {
        var particleSystem = m_enemyBloodSplatterShotsPool.Take();
        particleSystem.transform.position = noteEventData.Note.transform.position;
        
        //rotate
        var eulerAngles = particleSystem.transform.rotation.eulerAngles; 
        eulerAngles.y = noteEventData.Note.Track.Direction.x < 0 ? 0 : 180;
        particleSystem.transform.rotation = Quaternion.Euler(eulerAngles);
        
        particleSystem.Play();
        StartCoroutine(WaitForEndBloodSplatter(particleSystem));
    }

#region BLOOD_SPLATTER_1

    ParticleSystem CreateNewBloodSplatter()
    {
        var go = Instantiate(m_bloodSplatterShotPrefab);
        go.transform.SetParent(transform);
        return go.GetComponent<ParticleSystem>();
    }

    void CleanUpBloodSplatter(ParticleSystem ps)
    {
        ps.Stop();
    }

    void DisposeBloodSplatter(ParticleSystem ps)
    {
        Destroy(ps.gameObject);
    }

    IEnumerator WaitForEndBloodSplatter(ParticleSystem ps)
    {
        while (ps.isPlaying)
        {
            yield return null;
        }
        m_enemyBloodSplatterShotsPool.Return(ps);
    }
#endregion

}
