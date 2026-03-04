using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BloodPoolSpawn : MonoBehaviour
{
    [SerializeField]List<GameObject> bloodPoolList = new List<GameObject>();

    public void SpawnBloodPools(float interval, float baseSpeed, float varience)
    {
        StartCoroutine(SpawnPool(interval, baseSpeed, varience));
    }

    IEnumerator SpawnPool(float interval, float baseSpeed, float varience)
    {
        while(bloodPoolList.Count > 0)
        {
            int rd = Random.Range(0, bloodPoolList.Count);
            float variable = 1 + Random.Range(-varience, varience);
            bloodPoolList[rd].SetActive(true);
            bloodPoolList[rd].GetComponentInChildren<Animator>().speed = baseSpeed * variable;
            bloodPoolList.RemoveAt(rd);

            Debug.Log("spawn a pool" + interval * variable + ", " + baseSpeed * variable);
            yield return new WaitForSeconds(interval * variable);
        }
    }
}
