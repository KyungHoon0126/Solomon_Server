using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon_Server.DataBase
{
    public abstract class ModelBase
    {
        public enum ItemStateEnum
        {
            None = 0,
            Modified = 1,
            Added = 2,
            Deleted = 3
        }

        public ItemStateEnum ItemState { get; set; } = ItemStateEnum.None;
    }
}