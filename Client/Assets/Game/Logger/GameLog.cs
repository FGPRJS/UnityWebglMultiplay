using TMPro;
using UnityEngine;

namespace Game.Logger
{
    public class GameLog : MonoBehaviour
    {
        public TMP_Text textObject;

        public string Log
        {
            set => this.textObject.text = value;
        }
    }
}
