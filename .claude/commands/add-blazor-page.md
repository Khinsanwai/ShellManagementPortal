# Add Blazor Page

Add a new Blazor List + Form page for an existing entity.

## Input

Entity name (must already have API endpoints): `/add-blazor-page Product`

## Steps

### 1. Create List Page — `Web/Components/Pages/{Entity}/{Entity}List.razor`

```razor
@page "/{entities}"
@attribute [Authorize]
@inject ApiService ApiService
@inject NotificationService NotificationService
@inject DialogService DialogService
@inject NavigationManager NavigationManager

<PageTitle>{Entities} - Shell Management Portal</PageTitle>

<RadzenRow class="rz-p-4">
    <RadzenColumn Size="12">
        <RadzenStack Orientation="Orientation.Horizontal" JustifyContent="JustifyContent.SpaceBetween" AlignItems="AlignItems.Center">
            <RadzenStack Orientation="Orientation.Horizontal" AlignItems="AlignItems.Center" Gap="1rem">
                <RadzenButton Icon="arrow_back" ButtonStyle="ButtonStyle.Light" Click="@(() => NavigationManager.NavigateTo("/"))" Tooltip="Return to Dashboard" />
                <RadzenText TextStyle="TextStyle.H4">{Entities}</RadzenText>
            </RadzenStack>
            <RadzenButton Text="Add {Entity}" Icon="add" Click="@Create{Entity}" />
        </RadzenStack>
    </RadzenColumn>
</RadzenRow>

<RadzenDataGrid @ref="grid" Data="@items" TItem="{Entity}Dto" AllowSorting="true" AllowPaging="true" PageSize="10">
    <Columns>
        <RadzenDataGridColumn TItem="{Entity}Dto" Property="Name" Title="Name" />
        <!-- Add columns for each property -->
        <RadzenDataGridColumn TItem="{Entity}Dto" Title="Actions" Width="200px">
            <Template Context="item">
                <RadzenButton Icon="edit" Size="ButtonSize.Small" Click="@(() => Edit{Entity}(item))" />
                <RadzenButton Icon="delete" Size="ButtonSize.Small" ButtonStyle="ButtonStyle.Danger" Click="@(() => Delete{Entity}(item))" />
            </Template>
        </RadzenDataGridColumn>
    </Columns>
</RadzenDataGrid>

@code {
    RadzenDataGrid<{Entity}Dto>? grid;
    IEnumerable<{Entity}Dto>? items;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        var result = await ApiService.GetAsync<PagedList<{Entity}Dto>>("{entity}/get", AppConfig.AccessToken);
        items = result?.Items;
        if (grid != null) await grid.Reload();
    }

    private async Task Create{Entity}()
    {
        var result = await DialogService.OpenAsync<{Entity}Form>("Add {Entity}");
        if (result == true) await LoadData();
    }

    private async Task Edit{Entity}({Entity}Dto item)
    {
        var result = await DialogService.OpenAsync<{Entity}Form>("Edit {Entity}", new() { { "{Entity}Id", item.Id } });
        if (result == true) await LoadData();
    }

    private async Task Delete{Entity}({Entity}Dto item)
    {
        var confirmed = await DialogService.Confirm("Are you sure?", "Delete {Entity}");
        if (confirmed == true)
        {
            await ApiService.DeleteAsync("{entity}/delete/{item.Id}", AppConfig.AccessToken);
            NotificationService.Notify(NotificationSeverity.Success, "Deleted", "{Entity} deleted successfully");
            await LoadData();
        }
    }
}
```

### 2. Create Form Page — `Web/Components/Pages/{Entity}/{Entity}Form.razor`

```razor
@inject ApiService ApiService
@inject NotificationService NotificationService
@inject DialogService DialogService

<RadzenTemplateForm Data="@model" TItem="{Entity}Dto" Submit="@OnSubmit">
    <RadzenStack Gap="1rem">
        <RadzenFormField Text="Name">
            <RadzenTextBox @bind-Value="model.Name" Name="Name" />
            <RadzenRequiredValidator Component="Name" Text="Name is required" />
        </RadzenFormField>
        <!-- Add fields for each property -->
        <RadzenStack Orientation="Orientation.Horizontal" JustifyContent="JustifyContent.End">
            <RadzenButton Text="Cancel" ButtonStyle="ButtonStyle.Light" Click="@(() => DialogService.Close(false))" />
            <RadzenButton Text="Save" ButtonType="ButtonType.Submit" />
        </RadzenStack>
    </RadzenStack>
</RadzenTemplateForm>

@code {
    [Parameter] public Guid? {Entity}Id { get; set; }

    private {Entity}Dto model = new();

    protected override async Task OnInitializedAsync()
    {
        if ({Entity}Id.HasValue)
        {
            var result = await ApiService.GetAsync<PagedList<{Entity}Dto>>($"{{entity}}/get?Id={{{Entity}Id}}", AppConfig.AccessToken);
            model = result?.Items?.FirstOrDefault() ?? new {Entity}Dto();
        }
    }

    private async Task OnSubmit({Entity}Dto item)
    {
        if ({Entity}Id.HasValue)
        {
            await ApiService.PutAsync("{entity}/update", item, AppConfig.AccessToken);
            NotificationService.Notify(NotificationSeverity.Success, "Updated", "{Entity} updated");
        }
        else
        {
            await ApiService.PostAsync("{entity}/create", item, AppConfig.AccessToken);
            NotificationService.Notify(NotificationSeverity.Success, "Created", "{Entity} created");
        }
        DialogService.Close(true);
    }
}
```

### 3. Update `Web/Components/_Imports.razor`

Add: `@using ShellMgmt.Domain.{Entity}Models`

### 4. Add Menu Item (optional)

Insert a new menu item in the database so the page appears in the sidebar.
