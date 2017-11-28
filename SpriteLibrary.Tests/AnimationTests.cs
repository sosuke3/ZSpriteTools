using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Xunit;

namespace SpriteLibrary.Tests
{
    public class AnimationTests
    {
        [Fact]
        public void should_load_json_database()
        {
            var animations = Animations.Instance;

            Assert.True(true);
        }
    }
}
