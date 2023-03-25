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

        private List<ISelectable> units, selectedUnits;

        public ReadOnlyCollection<ISelectable> Units
        {
            get { return units.AsReadOnly(); }
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;

            units = new List<ISelectable>();
            selectedUnits = new List<ISelectable>();
        }

        public void RegisterUnit(ISelectable unit)
        {
            units.Add(unit);
        }

        public void UnregisterUnit(ISelectable unit)
        {
            units.Remove(unit);
        }

        public void ClickSelect(ISelectable unitToAdd)
        {
            DeselectAll();
            selectedUnits.Add(unitToAdd);
            unitToAdd.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.OnSelected.Invoke();
            unitToAdd.gameObject.GetComponent<UnitMove>().enabled = true;
        }

        public void ShiftClickSelect(ISelectable unitToAdd)
        {
            if (!selectedUnits.Contains(unitToAdd))
            {
                selectedUnits.Add(unitToAdd);
                unitToAdd.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                unitToAdd.OnSelected.Invoke();
                // unitToAdd.gameObject.GetComponent<UnitMove>().enabled = true;
            }
            else
            {
                unitToAdd.OnDeselected.Invoke();
                // unitToAdd.gameObject.GetComponent<UnitMove>().enabled = false;
                unitToAdd.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                selectedUnits.Remove(unitToAdd);
            }
        }

        public void DragSelect(ISelectable unitToAdd)
        {
            if (selectedUnits.Contains(unitToAdd))
                return;

            selectedUnits.Add(unitToAdd);
            unitToAdd.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.OnSelected.Invoke();
            // unitToAdd.gameObject.GetComponent<UnitMove>().enabled = true;
        }

        public void DeselectAll()
        {
            foreach (ISelectable unit in selectedUnits)
            {
                unit.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                unit.OnDeselected.Invoke();
                // unit.gameObject.GetComponent<UnitMove>().enabled = false;
            }

            selectedUnits.Clear();
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
