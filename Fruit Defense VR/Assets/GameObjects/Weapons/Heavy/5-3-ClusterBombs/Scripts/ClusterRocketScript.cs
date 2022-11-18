using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterRocketScript : BaseGrenade
{
    public Rigidbody mRigidBody;
    private bool mThrustersOn = false;

    private Vector3 mTarget = new Vector3(-10, 5, 5);

    private IEnumerator StartThrustersAfterSeconds()
    {
        yield return new WaitForSeconds( .8f );
        mThrustersOn = true;
    }

    void Start()
    {
        StartCoroutine( StartThrustersAfterSeconds() );
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ( mThrustersOn )
        {

            Vector3 towardsTarget = mTarget - transform.position;
            Quaternion goal = Quaternion.LookRotation(towardsTarget);
            transform.rotation = Quaternion.Lerp( transform.rotation, goal, 5 * Time.deltaTime );
            mRigidBody.AddRelativeForce( 0f, 0f, 12f);

            if ((transform.position - mTarget).sqrMagnitude < 3)
            {
                ExplosionFx();
                mThrustersOn = false;
                Destroy( mRigidBody );
            }
        }
    }
}
