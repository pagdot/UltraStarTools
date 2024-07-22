using Microsoft.AspNetCore.Components;

namespace DataGrid;

[CascadingTypeParameter(nameof(TGridItem))]
public partial class DataGrid<TGridItem>
{
    [Parameter]
    public IEnumerable<TGridItem> Items
    {
        get => _items;
        set
        {
            _items = value;
            RefreshFilteredItems();
        }
    }

    [Parameter] public IEnumerable<TGridItem> FilteredItems { get; set; } = Enumerable.Empty<TGridItem>();
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public string? RowHeight { get; set; }

    private List<BaseColumn<TGridItem>> _columns = new();
    private IEnumerable<TGridItem> _items = Enumerable.Empty<TGridItem>();

    public void AddColumn(BaseColumn<TGridItem> column, bool isDefaultSort)
    {
        _columns.Add(column);
        if (isDefaultSort)
            ChangeOrder(column);
        InvokeAsync(StateHasChanged);
    }

    public void RefreshFilteredItems() => FilteredItems =
        Items.Where(x => _columns.Aggregate(true, (current, column) => current & column.Filter(x)));
}
