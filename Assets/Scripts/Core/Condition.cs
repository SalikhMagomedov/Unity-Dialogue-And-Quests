using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Core
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField] private string predicate;
        [SerializeField] private string[] parameters;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return evaluators
                .Select(evaluator => evaluator.Evaluate(predicate, parameters))
                .Where(result => result != null)
                .All(result => result != false);
        }
    }
}