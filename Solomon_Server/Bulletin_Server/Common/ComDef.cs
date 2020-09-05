using Bulletin_Server.JWT.Models;
using Bulletin_Server.JWT.Services;

namespace Bulletin_Server.Common
{
    public class ComDef
    {
        public static JWTContainerModel jWTContainerModel = new JWTContainerModel();
        public static JWTService jwtService = new JWTService(jWTContainerModel.SecretKey);

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

        // Repository Public 시 주의 사항 : JWT Secret Key, Meal API Key 제외
    }
}