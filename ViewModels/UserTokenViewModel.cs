using System;
using System.Collections.Generic;
using System.Text;

namespace ViewModels
{
    public abstract class UserTokenBaseViewModel
    {
        public int Id { get; set; }

        public string AccessTokenHash { get; set; }

        public DateTimeOffset AccessTokenExpiresDateTime { get; set; }

        public string RefreshTokenIdHash { get; set; }

        public string RefreshTokenIdHashSource { get; set; }

        public DateTimeOffset RefreshTokenExpiresDateTime { get; set; }

        public int UserId { get; set; }
    }

    public class AddUserTokenViewModel : UserTokenBaseViewModel
    {

    }

    public class ListUserTokenViewModel : UserTokenBaseViewModel
    {

    }
}
