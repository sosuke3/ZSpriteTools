using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Xunit;

namespace SpriteLibrary.Tests
{
    public class PlayerSpriteTests
    {
        [Fact]
        public void should_update_main_palette()
        {
            var file = File.ReadAllBytes("data\\orb.new.spr");
            var s = new PlayerSprite(file);

            var c = Color.FromArgb(0, 0, 0);
            Assert.NotEqual(c, s.Palette[0]);

            s.GreenMailPalette[1] = c;
            Assert.Equal(c, s.Palette[0]);
        }
    }
}
