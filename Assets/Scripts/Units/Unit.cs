using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Collections.Generic;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    public class Unit : MonoBehaviour, IHexUnit, IClickable
    {
        public IHexGrid Grid { get; set; }
        public IHexCell Location { get; set; }
        public float Orientation { get; set; }

        public int Speed => _data.Speed;

        public int VisionRange => _data.VisionRange;

        [SerializeField] private UnitData _data;

        private Transform _body, _turret;

        public UnityEvent OnSelected { get; set; }
        public UnityEvent OnDeselected { get; set; }

        private void Awake()
        {
            _body = transform.GetChild(0);
            _turret = transform.GetChild(1);

        }

        private void Start()
        {
            SelectionManager.Instance.RegisterUnit(this.gameObject);

            if (OnSelected == null)
                OnSelected = new UnityEvent();
            if (OnDeselected == null)
                OnDeselected = new UnityEvent();
        }

        private void OnDestroy()
        {
            SelectionManager.Instance.UnregisterUnit(this.gameObject);
        }

        public void Die()
        {
            throw new System.NotImplementedException();
        }

        public int GetMoveCost(IHexCell fromCell, IHexCell toCell, HexDirection direction)
        {
            throw new System.NotImplementedException();
        }

        public bool IsValidDestination(IHexCell cell)
        {
            throw new System.NotImplementedException();
        }

        public void Save(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Travel(List<IHexCell> path)
        {
            throw new System.NotImplementedException();
        }

        public void ValidateLocation()
        {
            throw new System.NotImplementedException();
        }

        public void AimTurret(Vector3 point)
        {
            point.y = _turret.position.y;
            _turret.LookAt(point);
        }
    }
}
