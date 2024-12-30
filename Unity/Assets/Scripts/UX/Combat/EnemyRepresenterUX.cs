namespace SpaceDeck.UX
{
    using SFDDCards;
    using SFDDCards.Evaluation.Actual;
    using SFDDCards.UX;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.UX;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class EnemyRepresenterUX : MonoBehaviour
    {
        [SerializeReference]
        private EnemyUX EnemyRepresentationPF;
        [SerializeReference]
        private Transform EnemyRepresentationTransform;
        [SerializeReference]
        private CentralGameStateController CentralGameStateControllerInstance;

        [Obsolete("Transition to " + nameof(_SpawnedEnemiesLookup))]
        public Dictionary<Enemy, EnemyUX> SpawnedEnemiesLookup { get; set; } = new Dictionary<Enemy, EnemyUX>();
        public readonly Dictionary<Entity, EnemyUX> _SpawnedEnemiesLookup = new Dictionary<Entity, EnemyUX>();
        public List<EnemyUX> OrderedEnemyList { get; set; } = new List<EnemyUX>();

        Coroutine EnemyPositionAdjustmentCoroutine { get; set; } = null;

        public void _AddEnemies(IEnumerable<Entity> toAdd)
        {
            foreach (Entity curEnemy in toAdd)
            {
                this._AddEnemy(curEnemy);
            }

            this.SituateEnemyPositions();
        }

        public void _AddEnemy(Entity toAdd, bool skipSituating = false)
        {
            EnemyUX newEnemy = Instantiate(this.EnemyRepresentationPF, this.EnemyRepresentationTransform);
            newEnemy._SetFromEnemy(toAdd, this.CentralGameStateControllerInstance);

            this._SpawnedEnemiesLookup.Add(toAdd, newEnemy);
            this.OrderedEnemyList.Add(newEnemy);

            if (!skipSituating)
            {
                this.SituateEnemyPositions();
            }
        }

        [Obsolete("Transition to " + nameof(_RemoveEnemy))]
        public void RemoveEnemy(Enemy toRemove)
        {
            if (this.SpawnedEnemiesLookup.TryGetValue(toRemove, out EnemyUX ux))
            {
                EnemyUX representation = this.SpawnedEnemiesLookup[toRemove];
                this.SpawnedEnemiesLookup.Remove(toRemove);
                this.OrderedEnemyList.Remove(representation);
                Destroy(representation.gameObject);
            }
        }

        public void _RemoveEnemy(Entity toRemove)
        {
            if (this._SpawnedEnemiesLookup.TryGetValue(toRemove, out EnemyUX ux))
            {
                EnemyUX representation = this._SpawnedEnemiesLookup[toRemove];
                this._SpawnedEnemiesLookup.Remove(toRemove);
                this.OrderedEnemyList.Remove(representation);
                Destroy(representation.gameObject);
            }
        }

        public void Annihilate()
        {
            for (int ii = this.EnemyRepresentationTransform.childCount - 1; ii >= 0; ii--)
            {
                Destroy(this.EnemyRepresentationTransform.GetChild(ii).gameObject);
            }

            this.SpawnedEnemiesLookup.Clear();
            this._SpawnedEnemiesLookup.Clear();
            this.OrderedEnemyList.Clear();

            if (this.EnemyPositionAdjustmentCoroutine != null)
            {
                StopCoroutine(this.EnemyPositionAdjustmentCoroutine);
                this.EnemyPositionAdjustmentCoroutine = null;
            }
        }

        void SituateEnemyPositions()
        {
            if (this.EnemyPositionAdjustmentCoroutine != null)
            {
                StopCoroutine(this.EnemyPositionAdjustmentCoroutine);
            }

            this.EnemyPositionAdjustmentCoroutine = StartCoroutine(this.EnemySituationCoroutine());
        }

        IEnumerator EnemySituationCoroutine()
        {
            Dictionary<EnemyUX, Vector3> startPositions = new Dictionary<EnemyUX, Vector3>();
            Dictionary<EnemyUX, Vector3> targetPositions = new Dictionary<EnemyUX, Vector3>();

            Vector3 basePosition = this.EnemyRepresentationTransform.position;

            // How many enemies should there be before using the full offset?
            const float enemiesForMaximumOffset = 4;

            // If there is that many enemies, how far to the left should the leftmost be, how far to the right should the rightmost be?
            const float maximumXOffset = 3f;

            // If there is exactly one or zero enemies, then the maximum used boundary offset is 0, right in the middle
            // Otherwise, divide the maximum offset by the number of enemies (maximum 4) to determine what offset is being used
            float usedBoundary = this.OrderedEnemyList.Count <= 1 ? 0 : maximumXOffset / Mathf.Min(enemiesForMaximumOffset, this.OrderedEnemyList.Count);

            for (int ii = 0; ii < this.OrderedEnemyList.Count; ii++)
            {
                EnemyUX curEnemy = this.OrderedEnemyList[ii];
                startPositions.Add(curEnemy, curEnemy.transform.position);

                if (this.OrderedEnemyList.Count <= 1)
                {
                    targetPositions.Add(curEnemy, basePosition);
                }
                else
                {
                    float targetPosition = Mathf.Lerp(-usedBoundary, usedBoundary, usedBoundary / (this.OrderedEnemyList.Count - 1) * ii);
                    targetPositions.Add(curEnemy, basePosition + Vector3.right * targetPosition);
                }
            }

            const float tweeningTime = .3f;
            float curTweeningTime = 0;

            do
            {
                foreach (EnemyUX curEnemy in this.OrderedEnemyList)
                {
                    Vector3 targetPosition = targetPositions[curEnemy];
                    Vector3 startingPosition = targetPositions[curEnemy];
                    Vector3 newPosition = Vector3.Lerp(startingPosition, targetPosition, curTweeningTime / tweeningTime);
                    curEnemy.transform.position = newPosition;
                }

                curTweeningTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            } while (curTweeningTime < tweeningTime);

            this.EnemyPositionAdjustmentCoroutine = null;
        }
    }
}