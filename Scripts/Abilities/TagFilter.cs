using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filters {
    [CreateAssetMenu(fileName = "TagFilter", menuName = "Ability/Filters/New Tag Filtering", order = 0)]
    public class TagFilter : FilteringStrategy
    {
        [SerializeField] string tagToFilter = "";
        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectToFilter)
        {
           foreach(var gameobject in objectToFilter)
            {
                if(gameobject.CompareTag(tagToFilter))
                {
                    yield return gameobject;
                }
            }
        }
    }
}
