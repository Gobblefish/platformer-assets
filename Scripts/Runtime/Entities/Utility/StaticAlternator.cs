/* --- Libraries --- */
// System.
using System.Collections;
using System.Collections.Generic;
// Unity.
using UnityEngine;
using UnityEngine.Events;
// Gobblefish.
using Gobblefish.Audio;
// Platformer.
using Platformer.Physics;

namespace Platformer.Entities.Utility {

    public class StaticAlternator : MonoBehaviour {

        public enum AlternatingType {
            A, B, C, D, Count,
        }

        public enum AlternatingState {
            Stable, Changing
        }

        [System.Serializable]
        public class AlternatingEntity {
            public Entity entity;
            public int index;
            public bool flipped;
            public AlternatingEntity(Entity e, int x, int y) { 
                this.entity=e; 
                this.index=x; 
                this.flipped= y == 0 ? false : true;
            }
        }

        [System.Serializable]
        public class AlternatingSettings {
            public int index;
            public float period;
            public float offset;
            
            public AudioSnippet preChangeSound;
            public AudioSnippet changeSound;

            public Timer changeTimer = new Timer(0f, 0f);
            public Timer prechangeTimer = new Timer(0f, 0f);

            bool on;

            public void Start() {
                changeTimer.Start(offset);
                on = true;
            }

            public void Update(float dt, List<AlternatingEntity> entityList) {
                bool changeFin = changeTimer.TickDown(dt) || !changeTimer.Active;
                if (changeFin) {
                    prechangeTimer.Start(PRE_CHANGE_DURATION);
                    changeTimer.Start(period);

                    // preChangeSound.Play();
                }

                bool prechangeFin = prechangeTimer.TickDown(dt);
                
                if (prechangeFin) {
                    List<AlternatingEntity> curatedOn = entityList.FindAll(e => e.index == index && !e.flipped);
                    List<AlternatingEntity> curatedOff = entityList.FindAll(e => e.index == index && e.flipped);
                    foreach (AlternatingEntity e in curatedOn) {
                        e.entity.gameObject.SetActive(on);
                    }
                    foreach (AlternatingEntity e in curatedOff) {
                        e.entity.gameObject.SetActive(!on);
                    }
                    on = !on;

                    // changeSound.Play();
                }
            }
        }

        public List<AlternatingEntity> entityList = new List<AlternatingEntity>();

        public List<AlternatingSettings> settingsList = new List<AlternatingSettings>();

        // The duration before changing that we indicate a change is happening.
        private const float PRE_CHANGE_DURATION = 0.3f;

        public int index;
        public int offIndex => (index + (int)Mathf.Floor((float)(int)AlternatingType.Count / 2f)) % (int)AlternatingType.Count;

        void Start() {
            foreach (AlternatingSettings s in settingsList) {
                s.Start();
            }
        }
        
        public void Refresh() {
            entityList = new List<AlternatingEntity>();
        }


        public void Add(Entity entity, int x, int y) {
            if (entity == null) { return; }
            entityList.Add(new AlternatingEntity(entity, x, y));
        }


        void FixedUpdate() {

            float dt = Time.fixedDeltaTime;
            foreach (AlternatingSettings s in settingsList) {
                s.Update(dt, entityList);
            }
            
        }

    }

}
