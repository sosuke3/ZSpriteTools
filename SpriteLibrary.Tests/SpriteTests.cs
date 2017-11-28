using System;
using System.IO;
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
        public void should_throw_exception_creating_new_sprite_from_byte_array()
        {
            byte[] testSprite = { (byte)'Z', (byte)'S', (byte)'P', (byte)'R', // header
                                    1, // version
                                    0x00, 0x00, 0xFF, 0xFF, // checksum
                                    0x25, 0x00, 0x00, 0x00, // pixel offset
                                    0x01, 0x00, // pixel length
                                    0x26, 0x00, 0x00, 0x00, // palette offset
                                    0x01, 0x00, // palette length
                                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // reserved
                                    0x65, 0x00, 0x00, 0x00, // display text (unicode)
                                    0x41, 0x00, 0x00, 0x00, // author (unicode)
                                    0x51, 0x00, // author rom display (ascii)
                                    // pixels
                                    0x13,
                                    // palette
                                    0x31
                                };
            Assert.Throws<Exception>(() => { var x = new Sprite(testSprite); });
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
                                    0x02, 0x00, // palette length
                                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // reserved
                                    0x65, 0x00, 0x00, 0x00, // display text (unicode)
                                    0x41, 0x00, 0x00, 0x00, // author (unicode)
                                    0x51, 0x00, // author rom display (ascii)
                                    // pixels
                                    0x13,
                                    // palette
                                    0x31, 0x20
                                };
            var s = new Sprite(testSprite);
            Assert.IsType<Sprite>(s);
            Assert.Equal(0x13, s.PixelData[0]);
            Assert.Equal(0x31, s.PaletteData[0]);
            Assert.Equal("e", s.DisplayText);
            Assert.Equal("A", s.Author);
            Assert.Equal("Q", s.AuthorRomDisplay);
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

        [Fact]
        public void should_create_empty_sprite_with_author_rom_display_Unknown()
        {
            var s = new Sprite();
            Assert.Equal("Unknown", s.AuthorRomDisplay);
        }

        [Fact]
        public void should_export_byte_array_that_reads_back_in()
        {
            var s = new Sprite();
            s.DisplayText = "This is a test";
            var bytes = s.ToByteArray();

            var s2 = new Sprite(bytes);

            Assert.Equal("This is a test", s2.DisplayText);
            Assert.Equal(s.DisplayText, s2.DisplayText);
            Assert.Equal(s.Author, s2.Author);
            Assert.Equal(s.AuthorRomDisplay, s2.AuthorRomDisplay);
            Assert.Equal(s.CheckSum, s2.CheckSum);
            Assert.Equal(s.PaletteData, s2.PaletteData);
            Assert.Equal(s.PaletteDataLength, s2.PaletteDataLength);
            Assert.Equal(s.PaletteDataOffset, s2.PaletteDataOffset);
            Assert.Equal(s.Reserved, s2.Reserved);
            Assert.Equal(s.PixelData, s2.PixelData);
            Assert.Equal(s.PixelDataLength, s2.PixelDataLength);
            Assert.Equal(s.PixelDataOffset, s2.PixelDataOffset);
            Assert.Equal(s.Version, s2.Version);
            Assert.True(s2.HasValidChecksum);
        }

        [Fact]
        public void should_import_from_file_bytes()
        {
            var file = File.ReadAllBytes("data\\orb.new.spr");
            var s = new Sprite(file);
            Assert.Equal(0x7000, s.PixelDataLength);
            Assert.Equal(0x7C, s.PaletteDataLength);
            Assert.Equal("Orb", s.DisplayText);
            Assert.False(s.HasValidChecksum);
        }

        [Fact]
        public void should_import_old_format_from_file_bytes()
        {
            var file = File.ReadAllBytes("data\\orb.old.spr");
            var s = new Sprite(file);
            Assert.Equal(0x7000, s.PixelDataLength);
            Assert.Equal(0x78, s.PaletteDataLength);
            Assert.False(s.HasValidChecksum);
        }
    }
}
