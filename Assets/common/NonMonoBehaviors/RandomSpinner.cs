using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.common
{
    public class RandomSpinner<T> 
    {
        private List<DropZone<T>> dropZones;

        private int totalSize;
        private int listSize;

        public RandomSpinner()
        {
            dropZones = new List<DropZone<T>>();
            listSize = 0;
        }

        public void addNewPossibility(int spinnerSize, T thing)
        {
            dropZones.Add(new DropZone<T>(thing, spinnerSize));
        }

        public T getRandom()
        {
            if (listSize != dropZones.Count)
            {
                makeSpinner();
            }

            if (!dropZones.Any())
            {
                return default(T);
            }

            int draw = Mathf.FloorToInt(Random.value* totalSize);

            foreach (var dropZone in dropZones)
            {
                if (draw < dropZone.cumulativePosition)
                {
                    return dropZone.returnVal;
                }
            }

            return dropZones.Last().returnVal;

        }

        private void makeSpinner()
        {
            int spot = 0;
            dropZones.ForEach(zone =>
            {
                spot += zone.Space;
                zone.cumulativePosition = spot;
            });

            totalSize = spot;
        }

        internal class DropZone<TX>
        {
            public TX returnVal;
            public int cumulativePosition;
            private int space;

            public DropZone(TX thing, int chance)
            {
                returnVal = thing;
                space = chance;
            }

            public int Space
            {
                get { return space; }
            }
        }
    }
}