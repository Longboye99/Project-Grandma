using UnityEngine;
using UnityEngine.Audio;

public class FootstepPlayer : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] float frequency;
    float timer;
    public AudioClip[] footStepSounds;
    PlayerMovementController m_Controller;
    bool isWalking;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        m_Controller = GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        if (m_Controller.curSpeed > 0)
        {
            timer += Time.deltaTime;
            if (timer >= frequency)
            {
                timer = 0;
                FootStep();
            }
        }
        else
        {
            timer = 0;
        }

        
    }

    private void FootStep()
    {
        int random = Random.Range(0, footStepSounds.Length);
        AudioClip clip = footStepSounds[random];
        audioSource.PlayOneShot(clip);
    }
}
