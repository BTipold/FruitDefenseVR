using UnityEngine;

public class StickyGrenade : BaseGrenade
{
    private void OnTriggerEnter( Collider other )
    {
        //todo
        //parent to other collider
        //remove RB
        //timer
        ExplosionFx();
        DealDamage();
    }
}
