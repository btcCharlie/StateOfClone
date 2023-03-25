using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }
        public IHexGrid Grid { get; set; }

        private List<ISelectable> _units, _selectedUnits;

        public ReadOnlyCollection<ISelectable> Units
        {
            get { return _units.AsReadOnly(); }
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;

            _units = new List<ISelectable>();
            _selectedUnits = new List<ISelectable>();
        }

        public void RegisterUnit(ISelectable unit)
        {
            _units.Add(unit);
        }

        public void UnregisterUnit(ISelectable unit)
        {
            _units.Remove(unit);
        }

        public void ClickSelect(ISelectable unitToAdd)
        {
            DeselectAll();
            _selectedUnits.Add(unitToAdd);
            unitToAdd.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.OnSelected.Invoke();
            unitToAdd.gameObject.GetComponent<UnitMove>().enabled = true;
        }

        public void ShiftClickSelect(ISelectable unitToAdd)
        {
            if (!_selectedUnits.Contains(unitToAdd))
            {
                _selectedUnits.Add(unitToAdd);
                unitToAdd.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                unitToAdd.OnSelected.Invoke();
                Debug.Log("selected" + unitToAdd.gameObject.name);
            }
            else
            {
                unitToAdd.OnDeselected.Invoke();
                unitToAdd.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                _selectedUnits.Remove(unitToAdd);
                Debug.Log("deselected" + unitToAdd.gameObject.name);
            }
        }

        public void DragSelect(ISelectable unitToAdd)
        {
            if (_selectedUnits.Contains(unitToAdd))
                return;

            _selectedUnits.Add(unitToAdd);
            unitToAdd.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.OnSelected.Invoke();
        }

        public void DeselectAll()
        {
            foreach (ISelectable unit in _selectedUnits)
            {
                unit.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                unit.OnDeselected.Invoke();
            }

            _selectedUnits.Clear();
        }

        public void Deselect(GameObject unitToDeselect)
        {

        }

        public static ISelectable GetClickableObject(RaycastHit hit)
        {
            GameObject clickableObject = hit.collider.gameObject;
            ISelectable selectable;
            while (!clickableObject.TryGetComponent(out selectable))
            {
                // there has to be a clickable object because we're in
                // a clickable layer - if not, an error is due anyway
                clickableObject = clickableObject.transform.parent.gameObject;
            }

            return selectable;
        }
    }
}
