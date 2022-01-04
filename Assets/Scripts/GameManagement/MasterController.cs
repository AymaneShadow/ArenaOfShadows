using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameUnit;
using Photon.Pun;
using UnityEngine;

namespace GameManagement
{
    public class MasterController : MonoBehaviourPunCallbacks
    {
        private Dictionary<GameData.Team, HashSet<MinionBehavior>> minions;
        private Dictionary<GameData.Team, GameData.Team> targets;

        private MinionValues minionValues;
        private GameObject minionPrefab;
        private GameObject spawnPointHolder;

        public bool IsPaused = false;
        
        public MasterController()
        {
            minions = new Dictionary<GameData.Team,  HashSet<MinionBehavior>>();
            targets = new Dictionary<GameData.Team, GameData.Team>();

            foreach (GameData.Team team in (GameData.Team[])Enum.GetValues(typeof(GameData.Team)))
            {
                minions.Add(team, new HashSet<MinionBehavior>());
                
                //Set default target to opposing team
                targets.Add(team, (GameData.Team)(((int)team + 2) % 4));
            }

            Debug.Log($"Init {minions.Count} teams");
        }

        public void Init(MinionValues minionValues, GameObject minionPrefab, GameObject spawnPointHolder, GameObject minionPaths)
        {
            this.minionValues = minionValues;
            this.minionPrefab = minionPrefab;
            this.spawnPointHolder = spawnPointHolder;

            MinionBehavior.Values = minionValues;
            MinionBehavior.Splines = minionPaths;
            
            Debug.Log("Starting Master Client Controller");
        }

        public void StartMinionSpawning(int startDelayInMs = 0)
        {
            Debug.Log($"Spawning Minions in {startDelayInMs}ms");
            StartCoroutine(SpawnMinions());
        }

        private IEnumerator SpawnMinions()
        {
            while (!IsPaused)
            {
                StartCoroutine(OnWaveSpawn());
                
                yield return new WaitForSeconds(minionValues.WaveDelayInMs / 1000);
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private IEnumerator OnWaveSpawn()
        {
            Debug.Log("Spawning Minion Wave");

            int wavesSpawned = 0;
            while (wavesSpawned++ < minionValues.WaveSize)
            {
                SpawnMinions(this, EventArgs.Empty);
                yield return new WaitForSeconds(minionValues.MinionOffsetInMs / 1000);
            }
        }

        void SpawnMinions(object o , EventArgs e)
        {
            foreach (GameData.Team team in (GameData.Team[]) Enum.GetValues(typeof(GameData.Team)))
            {
                Vector3 spawnPoint = spawnPointHolder.transform.Find(team.ToString()).transform.position;
                
                Debug.Log($"Spawning Minion at {spawnPoint}");
                GameObject go = PhotonNetwork.Instantiate(minionPrefab.name, spawnPoint,
                    Quaternion.LookRotation((Vector3.zero - transform.position).normalized));
                MinionBehavior behavior = go.GetComponent<MinionBehavior>();
                behavior.Init(go.GetInstanceID(), team, targets[team]);
                minions[team].Add(behavior);
            }
        }


        [PunRPC]
        void SetMinionTarget(GameData.Team team, GameData.Team target)
        {

            targets[team] = target;
            //For now, have all minions instantly switch agro. Maybe change this over so only future minions switch agro?
            foreach (MinionBehavior minionBehavior in minions[team])
            {
                minionBehavior.SetTarget(target);
            }
        }
    }
}
