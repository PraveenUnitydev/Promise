    using UnityEngine;
     
   
    public class FreeLookUserInput : MonoBehaviour
    {
    public GameObject Cm1;
    public GameObject Cm2;
    public float MouX;
       private void Update() {
        if (Input.GetButtonDown("Fire2")) 
        {
          
            Cm1.SetActive(false);
            Cm2.SetActive(true);
        }else if (Input.GetButtonUp("Fire2")) 
        {
            Cm2.SetActive(false);
            Cm1.SetActive(true);
        }
         
       }
    }
    
