﻿using Forums.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forums.Models.Search
{
    public class SearchResultModel
    {
        public IEnumerable<PostListingModel> Posts { get; set; }
        public string SearchQuery { get; set; }
        public bool EmptySearchResults { get; set; }
    }
}
