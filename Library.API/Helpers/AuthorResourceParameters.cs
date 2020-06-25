using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public class AuthorResourceParameters
    {
        public const int MaxPageSize = 50;
        private int _pageSize = 10;
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }
        /// <summary>
        /// 过滤字段
        /// </summary>
        public string BirthPlace { get; set; }
        /// <summary>
        /// 搜索字段
        /// </summary>
        public string SearchQuery { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public string SortBy { get; set; } = "Name";

    }
}
