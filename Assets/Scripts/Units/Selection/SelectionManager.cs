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

        private List<GameObject> units, selectedUnits;

        public ReadOnlyCollection<GameObject> Units
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

            units = new List<GameObject>();
            selectedUnits = new List<GameObject>();
        }

        public void RegisterUnit(GameObject unit)
        {
            units.Add(unit);
        }

        public void UnregisterUnit(GameObject unit)
        {
            units.Remove(unit);
        }

        public void ClickSelect(IClickable unitToAdd)
        {
            DeselectAll();
            selectedUnits.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.OnSelected.Invoke();
            unitToAdd.GetComponent<UnitMove>().enabled = true;
        }

        public void ShiftClickSelect(GameObject unitToAdd)
        {
            if (!selectedUnits.Contains(unitToAdd))
            {
                selectedUnits.Add(unitToAdd);
                unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
                unitToAdd.OnSelected.Invoke();
                unitToAdd.GetComponent<UnitMove>().enabled = true;
            }
            else
            {
                unitToAdd.OnDeselected.Invoke();
                unitToAdd.GetComponent<UnitMove>().enabled = false;
                unitToAdd.transform.GetChild(0).gameObject.SetActive(false);
                selectedUnits.Remove(unitToAdd);
            }
        }

        public void DragSelect(GameObject unitToAdd)
        {
            if (selectedUnits.Contains(unitToAdd))
                return;

            selectedUnits.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.OnSelected.Invoke();
            unitToAdd.GetComponent<UnitMove>().enabled = true;
        }

        public void DeselectAll()
        {
            foreach (GameObject unit in selectedUnits)
            {
                unit.transform.GetChild(0).gameObject.SetActive(false);
                unitToAdd.OnDeselected.Invoke();
                unit.GetComponent<UnitMove>().enabled = false;
            }

            selectedUnits.Clear();
        }

        public void Deselect(GameObject unitToDeselect)
        {

        }

        public static GameObject GetClickableObject(RaycastHit hit)
        {
            GameObject clickableObject = hit.collider.gameObject;
            while (!clickableObject.TryGetComponent<IClickable>(out _))
            {
                // there has to be a clickable object because we're in
                // a clickable layer - if not, an error is due anyway
                clickableObject = clickableObject.transform.parent.gameObject;
            }

            return clickableObject;
        }
    }
}
