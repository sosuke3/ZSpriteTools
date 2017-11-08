using System;
using System.Linq;
using Xunit;

namespace SpriteLibrary.Tests
{
    public class SpriteTests
    {
        [Fact]
        public void should_throw_exception_when_sprite_doesnt_start_with_ZSPR()
        {
            byte[] testSprite = { (byte)'X', (byte)'S', (byte)'P', (byte)'R' };
            Assert.Throws<Exception>(() => { var t = new Sprite(testSprite); });
        }

        [Fact]
        public void should_create_new_sprite()
        {
            byte[] testSprite = { (byte)'Z', (byte)'S', (byte)'P', (byte)'R' };
            var t = new Sprite(testSprite);
            Assert.IsType<Sprite>(t);
        }
    }
}
