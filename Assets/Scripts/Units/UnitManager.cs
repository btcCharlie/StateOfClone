using System;
using UnityEngine;

namespace StateOfClone.Units
{
    public class UnitManager : MonoBehaviour
    {
        public static UnitManager Instance;

        [SerializeField] private GameObject _unitPrefab;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }

        internal void SpawnUnit(Vector3 point)
        {
            GameObject unit = Instantiate(_unitPrefab, point, Quaternion.identity);
            unit.transform.SetParent(transform, true);
        }
    }
}
