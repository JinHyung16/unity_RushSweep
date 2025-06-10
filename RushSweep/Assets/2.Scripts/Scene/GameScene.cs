using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugh.Game
{
    public class GameScene : MonoBehaviour
    {
        [Header("Cameras")]
        [SerializeField]
        Camera MainCamera;

        public Transform DefaultRoot;
        public Transform UIRoot;

        public void Start()
        {
            
        }
    }
}