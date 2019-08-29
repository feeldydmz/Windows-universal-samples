namespace Megazone.Cloud.Media.Domain
{
    public struct Pagination
    {
        public Pagination(int index = 0, int limitPerPage = 10)
        {
            Index = index;
            LimitPerPage = limitPerPage;
            Offset = index * limitPerPage;
        }

        public int Offset { get; }
        public int Index { get; }
        public int LimitPerPage { get; }
    }
}