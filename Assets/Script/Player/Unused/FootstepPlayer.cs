using UnityEngine;
using UnityEngine.Audio;

public class FootstepPlayer : MonoBehaviour
{
    [SerializeField] float frequency;
    [SerializeField] float volumn;
    float timer;
    public AudioClip[] footStepSounds;
    PointClickCameraMovement pointClickCameraMovement;

    bool isWalking;

    private void Awake()
    {
        pointClickCameraMovement = GetComponent<PointClickCameraMovement>();
    }

    private void Update()
    {
        if (pointClickCameraMovement.isWalking)
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
        Transform footstepPos = this.transform;
        footstepPos.position = this.transform.position - new Vector3(0, 4, 0);
        
        AudioClip clip = footStepSounds[random];
        GameManager.instance.sfxManager.PlaySoundFXClip(clip, footstepPos ,volumn);
    }
}
