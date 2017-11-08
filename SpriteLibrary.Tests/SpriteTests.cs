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
            Assert.Throws<Exception>(() => { var s = new Sprite(testSprite); });
        }

        [Fact]
        public void should_throw_exception_when_sprite_byte_array_is_too_short()
        {
            byte[] testSprite = { (byte)'Z', (byte)'S', (byte)'P', (byte)'R' };
            Assert.Throws<Exception>(() => { var s = new Sprite(testSprite); });
        }

        [Fact]
        public void should_create_new_sprite_from_byte_array()
        {
            byte[] testSprite = { (byte)'Z', (byte)'S', (byte)'P', (byte)'R', // header
                                    1, // version
                                    0x00, 0x00, 0xFF, 0xFF, // checksum
                                    0x25, 0x00, 0x00, 0x00, // pixel offset
                                    0x01, 0x00, // pixel length
                                    0x26, 0x00, 0x00, 0x00, // palette offset
                                    0x01, 0x00, // palette length
                                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // reserved
                                    0x65, 0x00, 0x00, 0x00, // display text
                                    0x41, 0x00, 0x00, 0x00, // author
                                    // pixels
                                    0x13,
                                    // palette
                                    0x31, 
                                };
            var s = new Sprite(testSprite);
            Assert.IsType<Sprite>(s);
            Assert.Equal(0x13, s.PixelData[0]);
            Assert.Equal(0x31, s.PaletteData[0]);
            Assert.Equal("e", s.DisplayText);
            Assert.Equal("A", s.Author);
        }

        [Fact]
        public void should_create_empty_sprite_with_version_1()
        {
            var s = new Sprite();
            Assert.Equal(1, s.Version);
        }
        
        [Fact]
        public void should_create_empty_sprite_with_display_text_Unknown()
        {
            var s = new Sprite();
            Assert.Equal("Unknown", s.DisplayText);
        }

        [Fact]
        public void should_create_empty_sprite_with_author_Unknown()
        {
            var s = new Sprite();
            Assert.Equal("Unknown", s.Author);
        }
    }
}
