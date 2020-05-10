using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModels
{
    public class RoleBaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ListRoleViewModel : RoleBaseViewModel
    {

    }
}
