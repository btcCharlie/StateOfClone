using System.Collections.Generic;
using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }
        public IHexGrid Grid { get; set; }
        public List<IHexUnit> SelectedUnits { get; set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this.gameObject);
            else
                Instance = this;
        }


    }
}
