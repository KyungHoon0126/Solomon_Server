namespace Bulletin_Server.Common
{
    public class ComDef
    {
        public static readonly string DATABASE_URL = $"SERVER=localhost;DATABASE=Bulletin;UID=root;PASSWORD=#kkh03kkh#;allow user variables=true";

        public static string GetIndexSortSQL(string tableName)
        {
            string sortSql = $@"
ALTER 
    TABLE bulletin.{tableName} AUTO_INCREMENT = 1;
SET 
    @COUNT = 0;
UPDATE
    bulletin.{tableName} SET idx = @COUNT:= @COUNT + 1
;";
            return sortSql;
        }
    }
}