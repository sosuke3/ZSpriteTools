using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteLibrary
{
    public sealed class Animations
    {
        #region Singleton
        private Animations() : this("animationdata.json")
        {
        }
        public static Animations Instance { get { return Nested.instance; } }
        private class Nested
        {
            static Nested()
            {

            }

            internal static readonly Animations instance = new Animations();
        }
        #endregion

        public Dictionary<string, AnimationType> AnimationData { get; set; }

        private Animations(string filename)
        {
            var json = File.ReadAllText(filename);

            this.AnimationData = JsonConvert.DeserializeObject<Dictionary<string, AnimationType>>(json);
        }
    }

    public class AnimationType
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("steps")]
        public List<Step> Steps { get; set; }
    }

    public class Step
    {
        [JsonProperty("sprites")]
        public List<SpriteStep> Sprites { get; set; }
        [JsonProperty("length")]
        public int Length { get; set; }
        [JsonProperty("shadow")]
        public string Shadow { get; set; }
    }

    public class SpriteStep
    {
        [JsonProperty("row")]
        public string Row { get; set; }
        [JsonProperty("col")]
        public int Col { get; set; }
        public int[] pos { get; set; }
        public string size { get; set; }
        public string trans { get; set; }

        public Point Position
        {
            get
            {
                if(pos.Length < 2)
                {
                    return new Point(0, 0);
                }
                
                return new Point(pos[0], pos[1]);
            }
        }

        public TileDrawType Size
        {
            get
            {
                TileDrawType ret;
                if(Enum.TryParse(size, out ret))
                {
                    return ret;
                }
                return TileDrawType.EMPTY;
            }
        }

        public TileFlipType Flip
        {
            get
            {
                TileFlipType ret;
                if(Enum.TryParse(trans, out ret))
                {
                    return ret;
                }
                return TileFlipType.NO_FLIP;
            }
        }
    }

}
