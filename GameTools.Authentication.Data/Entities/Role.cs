using GameTools.Authentication.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTools.Authentication.Data.Entities
{
    public class Role : IRole
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
    }
}
