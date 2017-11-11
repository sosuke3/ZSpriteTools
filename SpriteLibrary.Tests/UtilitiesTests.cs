using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Xunit;

namespace SpriteLibrary.Tests
{
    public class UtilitiesTests
    {
        [Fact]
        public void should_convert_black_to_zero_ushort()
        {
            Color c = Color.FromArgb(0, 0, 0);
            Assert.Equal(0, Utilities.GetUShortFromColor(c));
        }

        [Fact]
        public void should_convert_black_to_zero_bytes()
        {
            Color c = Color.FromArgb(0, 0, 0);
            var bytes = Utilities.GetBytesFromColor(c);

            Assert.Equal(0, bytes[0]);
            Assert.Equal(0, bytes[1]);
        }

        [Fact]
        public void should_convert_white_to_7FFF_ushort()
        {
            Color c = Color.FromArgb(255, 255, 255);
            Assert.Equal(0x7FFF, Utilities.GetUShortFromColor(c));
        }

        [Fact]
        public void should_convert_white_to_FF_7F_bytes()
        {
            Color c = Color.FromArgb(255, 255, 255);
            var bytes = Utilities.GetBytesFromColor(c);

            Assert.Equal(0xFF, bytes[0]); // little endian
            Assert.Equal(0x7F, bytes[1]);
        }

        [Fact]
        public void should_convert_7FFF_to_white()
        {
            ushort color = 0x7FFF;
            var c = Utilities.GetColorFromBytes(color);

            Assert.Equal(Color.FromArgb(255, 255, 255), c);
        }

        [Fact]
        public void should_convert_FF_7F_to_white()
        {
            var bytes = new byte[] { 0xFF, 0x7F }; // little endian
            var c = Utilities.GetColorFromBytes(bytes[0], bytes[1]);

            Assert.Equal(Color.FromArgb(255, 255, 255), c);
        }

        [Fact]
        public void should_convert_00_FF_to_R255_G57_B0()
        {
            var bytes = new byte[] { 0x00, 0xFF }; // little endian
            var c = Utilities.GetColorFromBytes(bytes[0], bytes[1]);

            Assert.Equal(Color.FromArgb(255, 198, 0), c);
        }

        [Fact]
        public void should_convert_FF_00_to_R0_G197_B255()
        {
            var bytes = new byte[] { 0xFF, 0x00 }; // little endian
            var c = Utilities.GetColorFromBytes(bytes[0], bytes[1]);

            Assert.Equal(Color.FromArgb(0, 58, 255), c);
        }

        [Fact]
        public void should_convert_R255_G57_B0_to_FF_00_bytes()
        {
            var bytes = Utilities.GetBytesFromColor(Color.FromArgb(255, 57, 0));

            Assert.Equal(0xFF, bytes[0]); // little endian
            Assert.Equal(0x00, bytes[1]);
        }

        [Fact]
        public void should_convert_R0_G197_B255_to_00_7F_bytes()
        {
            var bytes = Utilities.GetBytesFromColor(Color.FromArgb(0, 197, 255));

            Assert.Equal(0x00, bytes[0]); // little endian
            Assert.Equal(0x7F, bytes[1]);
        }

        [Fact]
        public void should_convert_multiple_of_8()
        {
            for(int i=0; i<0xFF; i+=8)
            {
                var bytes = Utilities.GetBytesFromColor(Color.FromArgb(i, i, i));

                byte b1 = (byte)((((i / 8) << 5) | ((i / 8) << 0)));
                byte b2 = (byte)((((i / 8) << 10) | ((i / 8) << 5)) >> 8);

                Assert.Equal(b1, bytes[0]); // little endian
                Assert.Equal(b2, bytes[1]);
            }
        }
    }
}
