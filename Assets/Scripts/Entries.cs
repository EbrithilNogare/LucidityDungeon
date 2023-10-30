namespace Assets.Scripts
{
    struct Entries
    {
        public bool up { get; }
        public bool left { get; }
        public bool down { get; }
        public bool right { get; }

        public Entries(bool up, bool left, bool down, bool right)
        {
            this.up = up;
            this.left = left;
            this.down = down;
            this.right = right;
        }
    }
}
