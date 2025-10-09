using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_UI : MonoBehaviour
{

    public void LordScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }
}



public class RandomSoundPlayer : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip[] soundClips; // เก็บเสียงทั้ง 3 เสียง
    [SerializeField] private AudioSource audioSource;
    
    [Header("Timing Settings")]
    [SerializeField] private float checkInterval = 30f; // ตรวจสอบทุก 30 วินาที
    [SerializeField] [Range(0f, 1f)] private float soundProbability = 0.5f; // โอกาสที่จะเล่นเสียง (50%)
    
    private float timer;

    void Start()
    {
        // ตรวจสอบว่ามี AudioSource หรือไม่
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        timer = checkInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0f)
        {
            CheckAndPlaySound();
            timer = checkInterval; // รีเซ็ตตัวจับเวลา
        }
    }

    void CheckAndPlaySound()
    {
        // ตรวจสอบว่ามีเสียงในอาร์เรย์หรือไม่
        if (soundClips == null || soundClips.Length == 0)
        {
            Debug.LogWarning("ไม่มีเสียงในอาร์เรย์!");
            return;
        }
        
        // สุ่มว่าจะเล่นเสียงหรือไม่
        float randomValue = Random.value;
        
        if (randomValue < soundProbability)
        {
            // สุ่มเลือกเสียง 1 ใน 3
            int randomIndex = Random.Range(0, soundClips.Length);
            AudioClip selectedClip = soundClips[randomIndex];
            
            if (selectedClip != null)
            {
                audioSource.PlayOneShot(selectedClip);
                Debug.Log($"เล่นเสียง: {selectedClip.name} (Index: {randomIndex})");
            }
        }
        else
        {
            Debug.Log("ไม่เล่นเสียงในรอบนี้");
        }
    }
}