using UnityEngine;

public class ImpactGrenade : BaseGrenade
{
    private void OnTriggerEnter( Collider other )
    {
        ExplosionFx();
        DealDamage();
    }
}
