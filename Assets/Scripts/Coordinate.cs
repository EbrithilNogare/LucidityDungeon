using System;

namespace Assets.Scripts
{
    // chatGPT generated from: give me dictionary for 2d coordinates able to be comparable inside dictionary
    // minor functional edits
    struct Coordinate : IEquatable<Coordinate>, IComparable<Coordinate>
    {
        public int x { get; set; }
        public int y { get; set; }

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Coordinate(Coordinate coordinate)
        {
            this.x = coordinate.x;
            this.y = coordinate.y;
        }

        public override int GetHashCode()
        {
            return (x * 733).GetHashCode() ^ (y * 739).GetHashCode();
        }

        public bool Equals(Coordinate other)
        {
            return x == other.x && y == other.y;
        }

        public int CompareTo(Coordinate other)
        {
            int xComparison = x.CompareTo(other.x);
            return xComparison != 0 ? xComparison : y.CompareTo(other.y);
        }
    }
}
