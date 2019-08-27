namespace Megazone.Cloud.Media.Domain
{
    public struct Pagination
    {
        public Pagination(int offset = 0, int limitPerPage = 10)
        {
            Offset = offset;
            LimitPerPage = limitPerPage;
        }

        public int Offset { get; }
        public int LimitPerPage { get; }
    }
}