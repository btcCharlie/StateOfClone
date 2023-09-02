using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Collections.Generic;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    [RequireComponent(typeof(UnitMove))]
    public class Unit : MonoBehaviour, IHexUnit, ISelectable, IMoveable
    {
        public IHexGrid Grid { get; set; }
        public IHexCell Location { get; set; }
        public float Orientation { get; set; }

        public int Speed => (int)UnitData.MaxSpeed;
        public float CurrentSpeed => UnitMove.CurrentSpeed;

        public int VisionRange => UnitData.VisionRange;

        [field: SerializeField] public UnitData UnitData { get; private set; }

        private Transform _body, _turret;

        public UnityEvent OnSelected { get; set; }
        public UnityEvent OnDeselected { get; set; }

        public UnitMove UnitMove { get; private set; }

        private void Awake()
        {
            _body = transform.GetChild(0);
            _turret = transform.GetChild(1);

            OnSelected ??= new UnityEvent();
            OnDeselected ??= new UnityEvent();

            UnitMove = GetComponent<UnitMove>();
        }

        private void Start()
        {
            SelectionManager.Instance.RegisterUnit(this);
        }

        private void OnDestroy()
        {
            SelectionManager.Instance.UnregisterUnit(this);
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
