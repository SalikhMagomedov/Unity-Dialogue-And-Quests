using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Core
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField] private Disjunction[] and;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return and.All(disjunction => disjunction.Check(evaluators));
        }

        [System.Serializable]
        public class Disjunction
        {
            [SerializeField] private Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                return or.Any(predicate => predicate.Check(evaluators));
            }
        }
        
        [System.Serializable]
        private class Predicate
        {
            [SerializeField] private string predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negate;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                return evaluators
                    .Select(evaluator => evaluator.Evaluate(predicate, parameters))
                    .Where(result => result != null)
                    .All(result => result != negate);
            }
        }
    }
}