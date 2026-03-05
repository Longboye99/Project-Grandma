using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BloodPoolSpawn : MonoBehaviour
{
    [SerializeField]List<GameObject> bloodPoolList;
    [SerializeField] List<GameObject> tempBloodPoolList;

    private void Start()
    {
        tempBloodPoolList = new(bloodPoolList);

    }
    public void SpawnBloodPools(float interval, float baseSpeed, float varience)
    {
        StartCoroutine(SpawnPool(interval, baseSpeed, varience));
    }

    IEnumerator SpawnPool(float interval, float baseSpeed, float varience)
    {
        while(tempBloodPoolList.Count > 0)
        {
            int rd = Random.Range(0, tempBloodPoolList.Count);
            float variable = 1 + Random.Range(-varience, varience);
            tempBloodPoolList[rd].SetActive(true);
            tempBloodPoolList[rd].GetComponentInChildren<Animator>().speed = baseSpeed * variable;
            tempBloodPoolList.RemoveAt(rd);

            Debug.Log("spawn a pool" + interval * variable + ", " + baseSpeed * variable);
            yield return new WaitForSeconds(interval * variable);
        }
    }

    public void RemoveBloodPool()
    {
        StopAllCoroutines();
        foreach (GameObject obj in bloodPoolList)
        {
            obj.SetActive(false);
        }
    }
}
