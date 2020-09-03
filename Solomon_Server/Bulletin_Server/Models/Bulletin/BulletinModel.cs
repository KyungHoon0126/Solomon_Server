using System;

namespace Solomon_Server.Models.Bulletin
{
    public class BulletinModel
    {
        public int idx { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string writer { get; set; }
        public DateTime written_time { get; set; }
        public string comment { get; set; }
    }
}
