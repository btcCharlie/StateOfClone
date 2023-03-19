using System.Collections.Generic;
using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance { get; private set; }
        public IHexGrid Grid { get; set; }

        public List<GameObject> Units { get; set; }
        public List<GameObject> SelectedUnits { get; set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this.gameObject);
            else
                Instance = this;

            Units = new List<GameObject>();
            SelectedUnits = new List<GameObject>();
        }

        public void ClickSelect(GameObject unitToAdd)
        {
            DeselectAll();
            SelectedUnits.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
        }

        public void ShiftClickSelect(GameObject unitToAdd)
        {
            if (!SelectedUnits.Contains(unitToAdd))
            {
                SelectedUnits.Add(unitToAdd);
                unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                unitToAdd.transform.GetChild(0).gameObject.SetActive(false);
                SelectedUnits.Remove(unitToAdd);
            }
        }

        public void DragSelect(GameObject unitToAdd)
        {

        }

        public void DeselectAll()
        {
            foreach (GameObject unit in SelectedUnits)
                unit.transform.GetChild(0).gameObject.SetActive(false);

            SelectedUnits.Clear();
        }

        public void Deselect(GameObject unitToDeselect)
        {

        }
    }
}
