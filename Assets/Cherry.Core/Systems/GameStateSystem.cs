using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class GameStateSystem : ComponentSystem
    {
        private EntityQuery _queryGameState;
        private EntityQuery _queryUser;
        private EntityQuery _queryPlayers;

        private Dictionary<string, object> metricaEventDict;
        
        protected override void OnCreate()
        {
            _queryGameState = GetEntityQuery(
                ComponentType.ReadOnly<GameStateData>());
            _queryUser = GetEntityQuery(
                ComponentType.ReadOnly<UserInputData>());
            _queryPlayers = GetEntityQuery(
                ComponentType.ReadOnly<AbilityActorPlayer>());
            metricaEventDict = new Dictionary<string, object>();
        }

        protected override void OnUpdate()
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var metrica = AppMetrica.Instance;
            
            Entities.With(_queryGameState).ForEach(
                (Entity entity, GameState state) =>
                {
                    if (state.userPlayer == null)
                    {
                        Entities.With(_queryUser).ForEach((AbilityActorPlayer player) =>
                        {
                            state.userPlayer = player;
                        });
                        if (state.userPlayer != null)
                        {
                            metricaEventDict.Clear();
                            metricaEventDict.Add("level",1);
                            state.metrica.ReportEvent("level_start", metricaEventDict);
                            state.metrica.SendEventsBuffer();
                            Debug.Log("[GAMESTATE] Appmetrica level start");
                            var panels = ActorSpawn.Spawn(state.sampleSpawner, state.userPlayer.Actor);

                            state.startTime = Time.ElapsedTime;
                            if (panels == null) return;
                            
                            ActorSpawn.RunSpawnActions(panels);
                            foreach (var panel in panels)
                            {
                                panel.transform.SetParent( state.rootCanvas.transform);
                                var r = panel.GetComponent<RectTransform>();
                                r.localScale = Vector3.one;
                                r.anchoredPosition = Vector2.zero;
                                r.anchorMax = Vector2.one;
                                r.anchorMin = Vector2.zero;
                                r.offsetMax = Vector2.one;
                                r.offsetMin = Vector2.zero;
                                r.sizeDelta = Vector2.one;
                                panel.SetActive(false);
                            }

                            state.respawnPanel = panels[0];
                            state.winPanel = panels[1];
                            state.losePanel = panels[2];

                            var resp = state.respawnPanel.GetComponent<UIGameStatePanel>();
                            resp.actionButton.onClick.AddListener(() =>
                            {
                                state.respawnPanel.SetActive(false);
                                dstManager.RemoveComponent<DeadActorData>(state.userPlayer.Actor.ActorEntity);
                                state.userPlayer.UpdateHealthData(state.userPlayer.MaxHealth);
                                var a = state.userPlayer.Actor.GameObject.GetComponent<Animator>();
                                a.SetBool("Dead", false);
                            });
                            resp.secondActionButton.onClick.AddListener((() =>
                            {
                                dstManager.DestroyEntity(dstManager.UniversalQuery);
                                SceneManager.LoadScene(0);
                            }));

                            var lose = state.losePanel.GetComponent<UIGameStatePanel>();
                            lose.actionButton.onClick.AddListener(() =>
                            {
                                dstManager.DestroyEntity(dstManager.UniversalQuery);
                                SceneManager.LoadScene(0);
                            });
                            
                            var win = state.winPanel.GetComponent<UIGameStatePanel>();
                            win.actionButton.onClick.AddListener(() =>
                            {
                                dstManager.DestroyEntity(dstManager.UniversalQuery);
                                SceneManager.LoadScene(0);
                            });
                        }
                    }
                    
                    if (state.userPlayer == null) return;
                    
                    state.players.Clear();
                    
                    Entities.With(_queryPlayers).ForEach((AbilityActorPlayer player) =>
                    {
                        state.players.Add(player);
                    });

                    if (!state.userPlayer.IsAlive)
                    {
                        if (state.userPlayer.deathCount <= state.maxDeathCount)
                        {
                            if (state.respawnPanel.activeSelf == false) state.respawnPanel.SetActive(true);
                        }
                        else 
                        {
                            if (state.losePanel.activeSelf == false)
                            {
                                metricaEventDict.Clear();
                                metricaEventDict.Add("level",1);
                                metricaEventDict.Add("result","lose");
                                metricaEventDict.Add("time", (int)(Time.ElapsedTime - state.startTime));
                                metricaEventDict.Add("progress", 100);
                                state.metrica.ReportEvent("level_finish", metricaEventDict);
                                state.metrica.SendEventsBuffer();
                                Debug.Log("[GAMESTATE] Appmetrica Finish event lose");
                                state.losePanel.SetActive(true);
                            }
                        }
                    }

                    if (state.players.Count == 1 && state.players.First() == state.userPlayer && state.winPanel.activeSelf == false)
                    {
                        metricaEventDict.Clear();
                        metricaEventDict.Add("level",1);
                        metricaEventDict.Add("result","win");
                        metricaEventDict.Add("time", (int)(Time.ElapsedTime - state.startTime));
                        metricaEventDict.Add("progress", 100);
                        state.metrica.ReportEvent("level_finish", metricaEventDict);
                        state.metrica.SendEventsBuffer();
                        Debug.Log("[GAMESTATE] Appmetrica Finish event win");
                        state.winPanel.SetActive(true);
                    }
                });
        }
    }
}