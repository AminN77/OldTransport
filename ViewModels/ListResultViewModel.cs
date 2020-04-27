using System.Collections.Generic;

namespace ViewModels
{
    public class ListResultViewModel<TListEntityViewModel>
    {
        public IList<TListEntityViewModel> Results { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPagesCount { get; set; }
        public int TotalEntitiesCount { get; set; }
    }
}