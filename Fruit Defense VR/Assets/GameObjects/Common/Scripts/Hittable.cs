// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using UnityEngine;


// ---------------------------------------------------------------
// @Summary: objects that derive from hitable will display 
//   visual and auditory effects when hit by a raycast. Derive 
//   from this class if you need a class to be "hitable" or 
//   simply attach the hitable script to a GameObject. 
// ---------------------------------------------------------------
public class Hittable : MonoBehaviour
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    public GameObject mHitEffect = null;
    public AudioClip  mHitSound  = null;
    public float      mVolume    = 0f;

    // ---------------------------------------------------------------
    // @Summary: impact is called by weapon scripts when raycast hits
    //   a hitable object. 
    // @Param: hit - reference to the raycast that hit this object. 
    //   This reference is needed to find the angle that the hit 
    //   effect should display at. 
    // @Return: void
    // ---------------------------------------------------------------
    public virtual void Impact( RaycastHit hit, float magnitude=1.0f )
    {
        if (mHitEffect != null)
        {
            // Create a new game object to hold these effects in case the
            // parent is destroyed.
            GameObject effect = Instantiate(mHitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            effect.transform.localScale = new Vector3( magnitude, magnitude, magnitude );
            effect.AddComponent<AudioSource>();

            AudioSource hitSource = effect.GetComponent<AudioSource>();
            hitSource.pitch = Random.Range(0.75f, 1.25f);
            hitSource.volume = mVolume * Random.Range(0.75f, 1.25f);
            hitSource.PlayOneShot(mHitSound);

            // Self-destruct when effect is complete.
            Destroy(effect, 1f);
        }
    }
}
