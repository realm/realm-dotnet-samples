using Acr.UserDialogs;
using AtlasSearch.Models;
using AtlasSearch.Services;

namespace AtlasSearch.ViewModels
{
    public partial class AutocompleteViewModel : SearchableViewModelBase<Movie, string>
    {
        protected override async Task Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return;
            }

            try
            {
                var results = await SearchService.Autocomplete(query);
                if (query == SearchQuery)
                {
                    Results = results;
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert($"An error occurred while executing search: {ex}", "Failed to execute search");
            }
        }

        protected override void TriggerSearch()
        {
            _searchDebouncer(SearchQuery);
        }
    }
}
