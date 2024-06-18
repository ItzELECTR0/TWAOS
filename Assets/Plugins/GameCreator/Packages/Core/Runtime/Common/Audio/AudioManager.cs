using System.Threading.Tasks;
using GameCreator.Runtime.Common.Audio;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    public class AudioManager : Singleton<AudioManager>, IGameSave
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void OnSubsystemsInit()
        {
            Instance.WakeUp();
        }
        
        // PRIVATE PROPERTIES: --------------------------------------------------------------------

        protected override bool SurviveSceneLoads => true;
        
        // PUBLIC PROPERTIES: ---------------------------------------------------------------------

        /// <summary>
        /// Control the master volume or each audio system separately
        /// </summary>
        public Volume Volume { get; private set; } = new Volume();

        /// <summary>
        /// Sound effects that are played only once, like gun shots, footsteps, slashes
        /// or tension stings. 
        /// </summary>
        public SoundEffect SoundEffect { get; private set; }
        
        /// <summary>
        /// Background ambient sounds that are looped and will persist until they are
        /// instructed to fade out.
        /// </summary>
        public Ambient Ambient { get; private set; }
        
        /// <summary>
        /// Background music sounds that are looped and will persist until they are
        /// instructed to fade out.
        /// </summary>
        public Music Music { get; private set; }
        
        /// <summary>
        /// Sound effects that are played once by a character, such as a voiced dialogue
        /// line or a grunt. very similar to Sound Effects, but specifically designed for
        /// characters.
        /// </summary>
        public Speech Speech { get; private set; }
        
        /// <summary>
        /// Beeps and sounds played by buttons and other non-diegetic UI elements.
        /// </summary>
        public UserInterface UserInterface { get; private set; }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected override void OnCreate()
        {
            base.OnCreate();

            Transform soundEffectsParent = this.CreateParent("Sound Effects");
            Transform ambientParent = this.CreateParent("Ambient");
            Transform musicParent = this.CreateParent("Music");
            Transform speechParent = this.CreateParent("Speech");
            Transform userInterfacesParent = this.CreateParent("User Interface");

            this.SoundEffect = new SoundEffect(soundEffectsParent.transform);
            this.Ambient = new Ambient(ambientParent.transform);
            this.Music = new Music(musicParent.transform);
            this.Speech = new Speech(speechParent.transform);
            this.UserInterface = new UserInterface(userInterfacesParent.transform);

            _ = SaveLoadManager.Subscribe(this);
        }

        // UPDATE METHOD: -------------------------------------------------------------------------

        private void Update()
        {
            this.Volume.Update();
            
            this.SoundEffect.Update();
            this.Ambient.Update();
            this.Music.Update();
            this.Speech.Update();
            this.UserInterface.Update();
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private async Task StopAll(float duration)
        {
            await Task.WhenAll(
                this.SoundEffect.StopAll(duration),
                this.Ambient.StopAll(duration),
                this.Music.StopAll(duration),
                this.Speech.StopAll(duration),
                this.UserInterface.StopAll(duration)
            );
        }
        
        private Transform CreateParent(string id)
        {
            GameObject instance = new GameObject(id);
            instance.transform.SetParent(this.transform);

            return instance.transform;
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public string SaveID => "volumes";
        public bool IsShared => true;

        public System.Type SaveType => typeof(Volume);
        
        public object GetSaveData(bool includeNonSavable)
        {
            return this.Volume;
        }

        public LoadMode LoadMode => LoadMode.Greedy;
        
        public Task OnLoad(object value)
        {
            _ = this.StopAll(0.5f);
            
            this.Volume = value as Volume ?? new Volume();
            return Task.FromResult(true);
        }
    }
}
