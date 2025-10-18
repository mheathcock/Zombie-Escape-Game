using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
   public ParticleSystem bloodParticles;

    public void PlayBloodSplatter()
    {
        if (bloodParticles != null)
        {
            bloodParticles.Play();
        }
    }
}
