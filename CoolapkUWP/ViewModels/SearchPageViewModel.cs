﻿using CoolapkUWP.Helpers;
using CoolapkUWP.Helpers.Providers;
using CoolapkUWP.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolapkUWP.ViewModels.SearchPage
{
    internal static class Base
    {
        internal static int SearchFeedTypeComboBoxSelectedIndex { get; set; }
        internal static int SearchFeedSortTypeComboBoxSelectedIndex { get; set; }

        internal static readonly ImmutableArray<SearchListProvider> providers = new SearchListProvider[]
        {
            new SearchListProvider(
                    async (keyWord, page, lastItem) =>
                    {
                        string feedType = string.Empty;
                        string sortType = string.Empty;
                        switch (SearchFeedTypeComboBoxSelectedIndex)
                        {
                            case 0: feedType = "all"; break;
                            case 1: feedType = "feed"; break;
                            case 2: feedType = "feedArticle"; break;
                            case 3: feedType = "rating"; break;
                            case 4: feedType = "picture"; break;
                            case 5: feedType = "question"; break;
                            case 6: feedType = "answer"; break;
                            case 7: feedType = "video"; break;
                            case 8: feedType = "ershou"; break;
                            case 9: feedType = "vote"; break;
                        }
                        switch (SearchFeedSortTypeComboBoxSelectedIndex)
                        {
                            case 0: sortType = "default"; break;
                            case 1: sortType = "hot"; break;
                            case 2: sortType = "reply"; break;
                        }
                        return (JArray)await DataHelper.GetDataAsync(
                            DataUriType.SearchFeeds,
                            feedType,
                            sortType,
                            keyWord,
                            page,
                            page > 1 ? "&lastItem=" + lastItem : string.Empty);
                    },
                    (o) => new FeedModel(o),
                    "id"),

                new SearchListProvider(
                    async (keyWord, page, lastItem) =>
                        (JArray)await DataHelper.GetDataAsync(
                            DataUriType.SearchUsers,
                            keyWord,
                            page,
                            page > 1 ? "&lastItem=" + lastItem : string.Empty),
                    (o) => new UserModel(o),
                    "uid"),

                new SearchListProvider(
                    async (keyWord, page, lastItem) =>
                        (JArray)await DataHelper.GetDataAsync(
                            DataUriType.SearchTags,
                            keyWord,
                            page,
                            page > 1 ? "&lastItem=" + lastItem : string.Empty),
                    (o) => new TopicModel(o),
                    "id")
    }.ToImmutableArray();

}

    internal class ViewModel : IViewModel
    {
        public double[] VerticalOffsets { get; set; } = new double[1];
        public string Title { get; } = string.Empty;

        internal readonly ImmutableArray<SearchListProvider> providers = Base.providers;

        internal string KeyWord { get; private set; }
        internal int TypeComboBoxSelectedIndex { get; private set; }

        internal ViewModel(int selectedIndex, string keyWord)
        {
            if(selectedIndex < -1 && selectedIndex >= providers.Length)
            {
                throw new ArgumentException(nameof(selectedIndex));
            }
            TypeComboBoxSelectedIndex = selectedIndex;
            KeyWord = keyWord;
            foreach (var item in providers)
            {
                item.Reset();
            }
        }

        internal async Task ChangeWordAndSearch(string keyWord, int index)
        {
            KeyWord = keyWord;
            TypeComboBoxSelectedIndex = index;
            await providers[TypeComboBoxSelectedIndex].Search(KeyWord);
        }

        [Obsolete("使用ChangeWordAndSearch")]
        public Task Refresh(int p = -1) => Task.CompletedTask;
    }
}
