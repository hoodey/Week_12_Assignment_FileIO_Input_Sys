using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HomingProjectile : Projectile
{
    //Fields
    float minDist = Mathf.Infinity;
    Transform targetTransform;
    Transform startTransform;
    float startTime;
    float journeylength;
    float homingSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        //Grab creation time and transform
        startTime = Time.time;
        startTransform = transform;

        //Upon creating a projectile, find the closest enemy's transform
        List<Enemy> enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
        Transform[] enemyTransforms = new Transform[enemies.Count];
        for (int i = 0; i < enemyTransforms.Length; i++)
        {
            enemyTransforms[i] = enemies[i].transform;
        }
        targetTransform = GetClosestEnemy(enemyTransforms);

        //Calculate length between projectile and closest enemy
        journeylength = Vector3.Distance(startTransform.position, targetTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime > 0.50)
        {
            float distCovered = (Time.time - startTime) * homingSpeed;

            float fractionOfJourney = distCovered / journeylength;
            //Lerp towards the closest target
            transform.position = Vector3.Lerp(startTransform.position, targetTransform.position, fractionOfJourney);
        }

    }
    Transform GetClosestEnemy(Transform[] enemies)
    {
        Transform tMin = null;
        Vector3 currentPos = transform.position;
        foreach (Transform t in enemies)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }

}
