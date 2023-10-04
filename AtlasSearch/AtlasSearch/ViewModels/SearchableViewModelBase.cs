using CommunityToolkit.Mvvm.ComponentModel;

namespace AtlasSearch.ViewModels
{
    public abstract partial class SearchableViewModelBase<TItem, TQueryArgs> : ObservableObject
    {
        protected readonly Action<TQueryArgs> _searchDebouncer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasResults))]
        [NotifyPropertyChangedFor(nameof(IsSearching))]
        private TItem[] results = Array.Empty<TItem>();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSearching))]
        private string searchQuery = string.Empty;

        [ObservableProperty]
        private TItem? selectedItem;

        public bool HasResults => Results.Any();

        public bool IsSearching => !HasResults && !string.IsNullOrEmpty(SearchQuery);

        protected SearchableViewModelBase()
        {
            Action<TQueryArgs> search = (arg) => _ = Search(arg);

            _searchDebouncer = search.Debounce();
        }

        protected abstract Task Search(TQueryArgs args);

        protected abstract void TriggerSearch();

        partial void OnSearchQueryChanged(string value)
        {
            Results = Array.Empty<TItem>();
            SelectedItem = default;
            TriggerSearch();
        }
    }
}
