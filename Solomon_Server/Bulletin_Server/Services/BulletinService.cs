using Solomon_Server.Models.Bulletin;
using System;
using System.Threading.Tasks;

namespace Bulletin_Server.Service
{
    public partial class SolomonService : IService
    {
        public Task<Response<BulletinModel>> GetAllBulletins()
        {
            throw new NotImplementedException();
        }

        public Task<Response> WriteBulletin(string title, string content, string writer)
        {
            throw new NotImplementedException();
        }

        public Task<Response> DeleteBulletin(int idx, string id)
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateBulletin(string title, string content)
        {
            throw new NotImplementedException();
        }
    }
}
