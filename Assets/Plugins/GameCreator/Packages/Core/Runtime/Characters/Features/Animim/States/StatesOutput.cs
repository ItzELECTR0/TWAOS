using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace GameCreator.Runtime.Characters.Animim
{
    public class StatesOutput : TAnimimOutput
    {
        [NonSerialized] private readonly SortedList<int, List<StatePlayableBehaviour>> m_Layers;

        // PROPERTIES: ----------------------------------------------------------------------------

        internal override float RootMotion
        {
            get
            {
                float motion = 0f;
                foreach (KeyValuePair<int, List<StatePlayableBehaviour>> entry in this.m_Layers)
                {
                    foreach (StatePlayableBehaviour state in entry.Value)
                    {
                        motion = Math.Max(motion, state.RootMotion);
                    }
                }

                return motion;
            }
        }
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public StatesOutput(AnimimGraph animimGraph) : base(animimGraph)
        {
            this.m_Layers = new SortedList<int, List<StatePlayableBehaviour>>();
        }

        public StatesOutput() : base(null)
        {
            this.m_Layers = new SortedList<int, List<StatePlayableBehaviour>>();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool IsAvailable(int layer)
        {
            if (!this.m_Layers.ContainsKey(layer)) return true;
            return !this.m_Layers.TryGetValue(layer, out List<StatePlayableBehaviour> states) || 
                   states.Count == 0;
        }
        
        public async Task SetState(StateData stateData, int layer,
            BlendMode blendMode, ConfigState config)
        {
            switch (stateData.Type)
            {
                case StateData.StateType.AnimationClip:
                    await this.SetState(
                        stateData.GetAnimationClip(this.m_AnimimGraph.Character.Args),
                        stateData.AvatarMask, 
                        layer, blendMode, config
                    );
                    break;

                case StateData.StateType.RuntimeController:
                    await this.SetState(
                        stateData.RuntimeController, stateData.AvatarMask, 
                        layer, blendMode, config
                    );
                    break;
                
                case StateData.StateType.State:
                    await this.SetState(
                        stateData.State, 
                        layer, blendMode, config
                    );
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public Task SetState(AnimationClip animationClip, AvatarMask avatarMask, int layer,
            BlendMode blendMode, ConfigState config)
        {
            StatePlayableBehaviour template = new StatePlayableBehaviour(
                animationClip, avatarMask, layer,
                blendMode, this.m_AnimimGraph, config
            );

            var statePlayable = ScriptPlayable<StatePlayableBehaviour>.Create(
                this.m_AnimimGraph.Graph, template, 1
            );

            this.SetPlayable(ref statePlayable, layer, config);
            this.RunStateChange();

            return TASK_COMPLETE;
        }
        
        public Task SetState(RuntimeAnimatorController rtc, AvatarMask avatarMask, int layer,
            BlendMode blendMode, ConfigState config)
        {
            StatePlayableBehaviour template = new StatePlayableBehaviour(
                rtc, avatarMask, layer, blendMode,
                this.m_AnimimGraph, config
            );

            var statePlayable = ScriptPlayable<StatePlayableBehaviour>.Create(
                this.m_AnimimGraph.Graph, template, 1
            );

            this.SetPlayable(ref statePlayable, layer, config);
            this.RunStateChange();

            return TASK_COMPLETE;
        }
        
        public async Task SetState(State state, int layer, BlendMode blendMode, ConfigState config)
        {
            StatePlayableBehaviour template = new StatePlayableBehaviour(
                state, layer, blendMode,
                this.m_AnimimGraph, config
            );

            var statePlayable = ScriptPlayable<StatePlayableBehaviour>.Create(
                this.m_AnimimGraph.Graph, template, 1
            );

            StatePlayableBehaviour behavior = this.SetPlayable(ref statePlayable, layer, config);
            this.RunStateChange();

            while (!behavior.IsComplete && !ApplicationManager.IsExiting)
            {
                await Task.Yield();
            }
        }

        public void Stop(int layer, float delay, float transitionOut)
        {
            this.StopPlayable(layer, delay, transitionOut);
            this.RunStateChange();
        }

        public void ChangeWeight(int layer, float weight, float transition)
        {
            if (this.m_Layers.TryGetValue(layer, out List<StatePlayableBehaviour> activeList))
            {
                int activeListCount = activeList.Count;
                if (activeListCount == 0) return;
                
                activeList[activeListCount - 1].ChangeWeight(weight, transition);
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private StatePlayableBehaviour SetPlayable(
            ref ScriptPlayable<StatePlayableBehaviour> statePlayable, 
            int layer, ConfigState config)
        {
            this.StopPlayable(
                layer, 
                config.DelayIn + config.TransitionIn,
                AnimimGraph.SAFE_TIME_OFFSET
            );
            
            int setLayerIndex = -1;
            foreach (KeyValuePair<int, List<StatePlayableBehaviour>> entry in this.m_Layers)
            {
                if (entry.Key > layer) break;
                setLayerIndex += 1;
            }

            Playable output;
            Playable input;

            if (this.m_Layers.Count == 0)
            {
                output = this.ScriptPlayable;
                input = this.ScriptPlayable.GetInput(0);
                this.ScriptPlayable.DisconnectInput(0);
            }
            else if (setLayerIndex < 0)
            {
                int prevLayerKey = this.m_Layers.Keys[0];
                int prevLayerListLast = this.m_Layers[prevLayerKey].Count - 1;
                StatePlayableBehaviour prevState = this.m_Layers[prevLayerKey][prevLayerListLast];

                output = prevState.mixerPlayable;
                input = prevState.mixerPlayable.GetInput(0);
                prevState.mixerPlayable.DisconnectInput(0);
            }
            else
            {
                int prevLayerKey = this.m_Layers.Keys[setLayerIndex];
                int prevLayerListLast = this.m_Layers[prevLayerKey].Count - 1;
                StatePlayableBehaviour prevState = this.m_Layers[prevLayerKey][prevLayerListLast];

                output = prevState.scriptPlayable.GetOutput(0);
                input = prevState.scriptPlayable;
                prevState.scriptPlayable.GetOutput(0).DisconnectInput(0);
            }

            statePlayable.ConnectInput(0, input, 0);
            statePlayable.SetInputWeight(0, 1f);
            
            output.ConnectInput(0, statePlayable, 0);
            output.SetInputWeight(0, 1f);
            
            StatePlayableBehaviour behaviour = statePlayable.GetBehaviour();

            if (!this.m_Layers.ContainsKey(layer))
            {
                this.m_Layers.Add(layer, new List<StatePlayableBehaviour>());
            }
            
            this.m_Layers[layer].Add(behaviour);
            behaviour.Create(this);

            return behaviour;
        }
        
        private void StopPlayable(int layer, float delay, float transitionOut)
        {
            if (!this.m_Layers.TryGetValue(layer, out List<StatePlayableBehaviour> activeList))
            {
                return;
            }
            
            foreach (StatePlayableBehaviour statePlayableBehaviour in activeList)
            {
                statePlayableBehaviour.Stop(delay, transitionOut);
            }
        }
        
        private void RunStateChange()
        {
            Args args = new Args(this.m_AnimimGraph.Character);
            foreach (KeyValuePair<int, List<StatePlayableBehaviour>> entry in this.m_Layers)
            {
                int index = entry.Value.Count - 1;
                if (index < 0) continue;

                StatePlayableBehaviour state = entry.Value[index];
                if (state.IsExiting || state.State == null) continue;
                
                state.State.RunChange(args);
            }
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        internal override void OnDeleteChild(TAnimimPlayableBehaviour playableBehaviour)
        {
            StatePlayableBehaviour stateBehaviour = playableBehaviour as StatePlayableBehaviour;
            if (stateBehaviour == null) return;

            int layer = stateBehaviour.Layer;
            if (this.m_Layers.TryGetValue(layer, out List<StatePlayableBehaviour> activeList))
            {
                activeList.Remove(stateBehaviour);
                
                if (activeList.Count == 0)
                {
                    this.m_Layers.Remove(layer);
                }
            }
        }
    }
}