using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("First Person")]
    [Category("First Person")]
    [Image(typeof(IconShotFirstPerson), ColorTheme.Type.Blue)]

    [Description("Moves with the head of a Character")]

    [Serializable]
    public class ShotTypeFirstPerson : TShotType
    {
        [SerializeField] private ShotSystemFirstPerson m_FirstPerson;
        [SerializeField] private ShotSystemHeadBobbing m_HeadBobbing;
        [SerializeField] private ShotSystemHeadLeaning m_HeadLeaning;
        [SerializeField] private ShotSystemNoise m_Noise;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override Args Args
        {
            get
            {
                this.m_Args ??= new Args(this.m_ShotCamera, null);
                this.m_Args.ChangeTarget(this.m_FirstPerson.GetTarget(this));
        
                return this.m_Args;
            }
        }

        public override Transform[] Ignore => Array.Empty<Transform>();

        public override bool UseSmoothPosition => false;
        public override bool UseSmoothRotation => false;

        public override Transform Target => null;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public ShotTypeFirstPerson()
        {
            this.m_FirstPerson = new ShotSystemFirstPerson();
            this.m_HeadBobbing = new ShotSystemHeadBobbing();
            this.m_HeadLeaning = new ShotSystemHeadLeaning();
            this.m_Noise = new ShotSystemNoise();
            
            this.m_ShotSystems.Add(this.m_FirstPerson.Id, this.m_FirstPerson);
            this.m_ShotSystems.Add(this.m_HeadBobbing.Id, this.m_HeadBobbing);
            this.m_ShotSystems.Add(this.m_HeadLeaning.Id, this.m_HeadLeaning);
            this.m_ShotSystems.Add(this.m_Noise.Id, this.m_Noise);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Character Character => this.m_FirstPerson.GetTarget(this);

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override void OnBeforeAwake(ShotCamera shotCamera)
        {
            base.OnBeforeAwake(shotCamera);
            this.m_FirstPerson?.OnAwake(this);
            this.m_HeadBobbing?.OnAwake(this);
            this.m_HeadLeaning?.OnAwake(this);
            this.m_Noise?.OnAwake(this);
        }

        protected override void OnBeforeStart(ShotCamera shotCamera)
        {
            base.OnBeforeStart(shotCamera);
            this.m_FirstPerson?.OnStart(this);
            this.m_HeadBobbing?.OnStart(this);
            this.m_HeadLeaning?.OnStart(this);
            this.m_Noise?.OnStart(this);
        }

        protected override void OnBeforeDestroy(ShotCamera shotCamera)
        {
            base.OnBeforeDestroy(shotCamera);
            this.m_FirstPerson?.OnDestroy(this);
            this.m_HeadBobbing?.OnDestroy(this);
            this.m_HeadLeaning?.OnDestroy(this);
            this.m_Noise?.OnDestroy(this);
        }

        protected override void OnBeforeEnable(TCamera camera)
        {
            base.OnBeforeEnable(camera);
            this.m_FirstPerson?.OnEnable(this, camera);
            this.m_HeadBobbing?.OnEnable(this, camera);
            this.m_HeadLeaning?.OnEnable(this, camera);
            this.m_Noise?.OnEnable(this, camera);
        }

        protected override void OnBeforeDisable(TCamera camera)
        {
            base.OnBeforeDisable(camera);
            this.m_FirstPerson?.OnDisable(this, camera);
            this.m_HeadBobbing?.OnDisable(this, camera);
            this.m_HeadLeaning?.OnDisable(this, camera);
            this.m_Noise?.OnDisable(this, camera);
        }
        
        protected override void OnBeforeUpdate()
        {
            base.OnBeforeUpdate();
            this.m_FirstPerson?.OnUpdate(this);
            this.m_HeadBobbing?.OnUpdate(this);
            this.m_HeadLeaning?.OnUpdate(this);
            this.m_Noise?.OnUpdate(this);
        }
    }
}