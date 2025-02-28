using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerCastle : MonoBehaviour
{
   void OnTriggerEnter(Collider other)
   {
        if(other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
   }
}
