using UnityEngine;
using StateOfClone.Core;
using System.IO;
using System.Collections.Generic;

namespace StateOfClone.Units
{
    public class UnitMove : MonoBehaviour, IHexUnit
    {
        public IHexGrid Grid { get; set; }
        public IHexCell Location { get; set; }
        public float Orientation { get; set; }

        public int Speed => _data.Speed;

        public int VisionRange => _data.VisionRange;

        [SerializeField] private UnitData _data;

        private Transform _body, _turret;

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

        private void Awake()
        {
            _body = transform.GetChild(0);
            _turret = transform.GetChild(1);
        }

        public void AimTurret(Vector3 point)
        {
            point.y = _turret.position.y;
            _turret.LookAt(point);
        }
    }
}
