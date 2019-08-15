namespace Megazone.Cloud.Media.ServiceInterface.Model
{
    public class Pagination
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