using System;
using System.Collections.Generic;
using Character;
using GameManagement;
using Photon.Pun;
using UnityEngine;

namespace GameUnit
{
    public class BaseBehavior : MonoBehaviour, IGameUnit
    {
        public int NetworkID { get; set; }
        public GameData.Team Team { get; set; }
        
        public GameObject AttachtedObjectInstance { get; set; }

        
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }
        public float MaxHealth { get; set; } = 1000;
        public GameUnitType Type => GameUnitType.Structure;
        public float Health { get; set; }
        public float MoveSpeed { get; set; } = 0;
        public float RotationSpeed { get; set; }  = 0;
        public float AttackDamage { get; set; } = 0;
        public float AttackSpeed { get; set; } = 0;
        public float AttackRange { get; set; } = 0;
        public IGameUnit CurrentAttackTarget { get; set; } = null;
        public HashSet<IGameUnit> CurrentlyAttackedBy { get; set; }
        
        public int Pages { get; set; }
        
        // Start is called before the first frame update
        void Start()
        {
            GameObject o = gameObject;
            NetworkID = o.GetInstanceID();
            bool res = Enum.TryParse(o.name, out GameData.Team parsedTeam);
            if (res)
            {
                Team = parsedTeam;
            }
            else
            {
                Debug.LogError($"Could not init base {o.name}");
            }

            Health = MaxHealth;
            CurrentlyAttackedBy = new HashSet<IGameUnit>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        public bool IsDestroyed()
        {
            return !gameObject;
        }
        
        public void Damage(IGameUnit unit, float damage)
        {
            this.CurrentlyAttackedBy.Add(unit);
            this.Health -= damage;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Local player, send data
                stream.SendNext(this.NetworkID);
                stream.SendNext(this.Team);
                stream.SendNext(this.Health);
                stream.SendNext(this.MaxHealth);
                stream.SendNext(this.Pages);
            }
            else
            {
                // Network player, receive data
                this.NetworkID = (int)stream.ReceiveNext();
                this.Team = (GameData.Team)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
                this.MaxHealth = (float)stream.ReceiveNext();
                this.Pages = (int)stream.ReceiveNext();
            }
        }
    }
}
