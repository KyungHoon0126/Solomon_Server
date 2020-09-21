using Solomon_Server.JWT.Models;
using Solomon_Server.JWT.Services;

namespace Solomon_Server.Common
{
    // TODO : Repository Public 시, JWT Secret Key, Meal API Key 정보 제외.
    public class ComDef
    {   
        // Token Verification
        public static JWTContainerModel jWTContainerModel = new JWTContainerModel();
        public static JWTService jwtService = new JWTService(jWTContainerModel.SecretKey);
    }
}