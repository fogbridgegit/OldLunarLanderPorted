using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WWI.Samples.LunarModule
{

    public class Spin : MonoBehaviour
    {
        public float SpinX;
        public float SpinY;
        public float SpinZ;

        void Update()
        {
            transform.Rotate(SpinX * Time.deltaTime, SpinY * Time.deltaTime, SpinZ * Time.deltaTime, Space.Self);
        }
    }

}