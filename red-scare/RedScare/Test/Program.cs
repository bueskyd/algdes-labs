using Xunit;

namespace RedScare.Test
{
    public class RedScareTests
    {
        [Fact]
        public void None_common_1_20()
        {
            var redScare = new RedScare();
            redScare.ReadInput(new StreamReader("../../../../../data/common-1-20.txt"));
            Assert.Equal(-1, redScare.None());
        }
        [Fact]
        public void Some_common_1_20()
        {
            var redScare = new RedScare();
            redScare.ReadInput(new StreamReader("../../../../../data/common-1-20.txt"));
            Assert.Equal(false, redScare.Some());
        }
        [Fact]
        public void Many_common_1_20()
        {
            var redScare = new RedScare();
            redScare.ReadInput(new StreamReader("../../../../../data/common-1-20.txt"));
            Assert.Equal(-1, redScare.Many());
        }
        [Fact]
        public void Few_common_1_20()
        {
            var redScare = new RedScare();
            redScare.ReadInput(new StreamReader("../../../../../data/common-1-20.txt"));
            Assert.Equal(-1, redScare.Few());
        }
        [Fact]
        public void Alternate_common_1_20()
        {
            var redScare = new RedScare();
            redScare.ReadInput(new StreamReader("../../../../../data/common-1-20.txt"));
            Assert.Equal(false, redScare.Alternate());
        }
        [Fact]
        public void None_dodecahedron()
        {
            var redScare = new RedScare();
            redScare.ReadInput(new StreamReader("../../../../../data/dodecahedron.txt"));
            Assert.Equal(-1, redScare.None());
        }

        [Fact]
        public void Some_dodecahedron()
        {
            var redScare = new RedScare();
            redScare.ReadInput(new StreamReader("../../../../../data/dodecahedron.txt"));
            Assert.Equal(true, redScare.Some());
        }
        [Fact]
        public void Many_dodecahedron()
        {
            var redScare = new RedScare();
            redScare.ReadInput(new StreamReader("../../../../../data/dodecahedron.txt"));
            Assert.Equal(true, redScare.Some());
        }
    }
}