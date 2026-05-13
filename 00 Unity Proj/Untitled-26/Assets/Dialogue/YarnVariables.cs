namespace Yarn.Unity.Variables {

    using Yarn.Unity;

    [System.CodeDom.Compiler.GeneratedCode("YarnSpinner", "3.1.3.0")]
    public partial class YarnVariables : Yarn.Unity.InMemoryVariableStorage, Yarn.Unity.IGeneratedVariableStorage {
        // Accessor for Number $flowers
        public float Flowers {
            get => this.GetValueOrDefault<float>("$flowers");
            set => this.SetValue<float>("$flowers", value);
        }

        // Accessor for Bool $flowerFinished
        public bool FlowerFinished {
            get => this.GetValueOrDefault<bool>("$flowerFinished");
            set => this.SetValue<bool>("$flowerFinished", value);
        }

        // Accessor for Bool $iceFinished
        public bool IceFinished {
            get => this.GetValueOrDefault<bool>("$iceFinished");
            set => this.SetValue<bool>("$iceFinished", value);
        }

        // Accessor for Bool $hasTalkedPenguin1
        public bool HasTalkedPenguin1 {
            get => this.GetValueOrDefault<bool>("$hasTalkedPenguin1");
            set => this.SetValue<bool>("$hasTalkedPenguin1", value);
        }

        // Accessor for Bool $hasTalkedPenguin2
        public bool HasTalkedPenguin2 {
            get => this.GetValueOrDefault<bool>("$hasTalkedPenguin2");
            set => this.SetValue<bool>("$hasTalkedPenguin2", value);
        }

        // Accessor for Bool $motherFinished
        public bool MotherFinished {
            get => this.GetValueOrDefault<bool>("$motherFinished");
            set => this.SetValue<bool>("$motherFinished", value);
        }

        // Accessor for Bool $oasisFinished
        public bool OasisFinished {
            get => this.GetValueOrDefault<bool>("$oasisFinished");
            set => this.SetValue<bool>("$oasisFinished", value);
        }

        // Accessor for Bool $talkedWithJulien
        public bool TalkedWithJulien {
            get => this.GetValueOrDefault<bool>("$talkedWithJulien");
            set => this.SetValue<bool>("$talkedWithJulien", value);
        }

        // Accessor for Bool $capsley_likes_you
        /// <summary>
        /// Whether Capsley like you or not. This starts true, but may change.
        /// </summary>
        public bool CapsleyLikesYou {
            get => this.GetValueOrDefault<bool>("$capsley_likes_you");
            set => this.SetValue<bool>("$capsley_likes_you", value);
        }

        // Accessor for String $player_name
        /// <summary>
        /// The player's name. The player chooses this. It starts empty.
        /// </summary>
        public string PlayerName {
            get => this.GetValueOrDefault<string>("$player_name");
            set => this.SetValue<string>("$player_name", value);
        }

    }
}
