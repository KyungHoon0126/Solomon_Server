namespace Solomon_Server.Models.Bulletin
{
    public class BulletinModel
    {
        public int bulletin_idx { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string writer { get; set; }
        public string written_time { get; set; }
    }
}
