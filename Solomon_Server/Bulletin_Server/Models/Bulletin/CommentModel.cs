namespace Solomon_Server.Models.Bulletin
{
    public class CommentModel
    {
        public int bulletin_idx { get; set; }
        public string writer { get; set; }
        public string content { get; set; }
        public int comment_idx { get; set; }
    }
}
