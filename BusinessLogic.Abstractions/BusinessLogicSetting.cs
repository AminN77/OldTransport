namespace BusinessLogic.Abstractions
{
    public static class BusinessLogicSetting
    {
        public struct FilesPath
        {
        }

        public struct DefaultThumbnailSize
        {
            public struct Small
            {
                public const int Width = 32;
                public const int Height = 32;
            }
            public struct Medium
            {
                public const int Width = 128;
                public const int Height = 128;
            }
            public struct Large
            {
                public const int Width = 256;
                public const int Height = 256;
            }
        }

        public const string OrderByFullName = "FullName:Asc";
        public const string OrderByDate = "Date:Asc";
        public const string DescendingOrderByDate = "Date:Desc";
        public const string DescendingOrderById = "Id:Desc";
        public const int SmallDefaultPageSize = 50;
        public const int MediumDefaultPageSize = 80;
        public const int LargeDefaultPageSize = 120;
        public const int ExteraLargeDefaultPageSize = 300;
        public const int MaxDefaultPageSize = int.MaxValue;
        public const int DefaultRollbackMillisecondsTimeout = 30000;
        public const int DefaultSaltCount = 32;
        public const int DefaultPassworHashCount = 128;
        public const int DefaultMinIterations = 10000;
        public const int DefaultMaxIterations = 100000;
        public const string UserDisplayName = "کاربر";
        public const string RoleDisplayName = "نقش";
        public const string OrganizationLevelDisplayName = "سطح سازمانی";


        public struct City
        {
        }
    }
}
