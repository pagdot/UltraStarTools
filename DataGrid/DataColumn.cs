using Microsoft.AspNetCore.Components;

namespace DataGrid;

public class DataColumn<TGridItem, TField> : BaseColumn<TGridItem>
{
    [Parameter] public Func<TGridItem, TField>? Field { get; set; }
    [Parameter] public Func<TField?, string?> DisplayFunc { get; set; } = x => x?.ToString();

    public override IEnumerable<TGridItem> Sort(IEnumerable<TGridItem> items, bool ascending) => ascending
        ? items.OrderBy(x => Field is null ? default : Field.Invoke(x))
        : items.OrderByDescending(x => Field is null ? default : Field.Invoke(x));

    public override bool Filter(TGridItem item)
    {
        return Field?.Invoke(item)?.ToString()?.Contains(FilterText, StringComparison.InvariantCultureIgnoreCase) ?? string.IsNullOrEmpty(FilterText);
    }

    public override RenderFragment Render(TGridItem item) => x => x.AddMarkupContent(0, DisplayFunc(Field is null ? default : Field.Invoke(item)) ?? string.Empty);
}
