using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MorSun.Model;
using System.Data.Entity;
using MorSun.Common;
using HOHO18.Common;

namespace MorSun.Bll
{
    /// <summary>
    /// 业务逻辑基类
    /// </summary>
    /// <typeparam name="Entity"></typeparam>
    public class BaseBll<Entity> : GenericEFDao<MorSunEntities, Entity>
        where Entity : class
    {
        
    }
}
