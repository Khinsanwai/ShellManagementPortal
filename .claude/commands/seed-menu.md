# Seed Menu Item

Add a new menu item to the database so it appears in the Blazor sidebar.

## Input

Menu details: `/seed-menu Products (route:/products, parent:Administration, icon:inventory_2)`

## Steps

### 1. Get Parent Menu ID

Query the database to find the parent menu's ID:

```bash
sqlcmd -S "KHINSANWAI\\SQLEXPRESS" -d SMPortal -Q "SELECT Id, Name FROM appshell.MenuItem WHERE Name = '{ParentName}'"
```

### 2. Insert Menu Item

```bash
sqlcmd -S "KHINSANWAI\\SQLEXPRESS" -d SMPortal -Q "
INSERT INTO appshell.MenuItem (Id, Name, Route, Icon, ParentId, IsActive, SortOrder)
VALUES (NEWID(), '{MenuName}', '{Route}', '{Icon}', '{ParentId}', 1, {SortOrder})
"
```

### 3. Assign to Admin Group

To make the menu visible to admin users, assign it to the admin user menu:

```bash
sqlcmd -S "KHINSANWAI\\SQLEXPRESS" -d SMPortal -Q "
INSERT INTO appshell.UserMenuAssignment (Id, UserId, MenuItemId)
SELECT NEWID(), '{AdminUserId}', Id FROM appshell.MenuItem WHERE Name = '{MenuName}'
"
```

### 4. Verify

```bash
sqlcmd -S "KHINSANWAI\\SQLEXPRESS" -d SMPortal -Q "SELECT * FROM appshell.MenuItem WHERE Name = '{MenuName}'"
```

## Existing Seed Scripts

Reference the SQL files in `database/` folder for examples:
- `seed_all_menus.sql` — Seeds Dashboard, Users, Administration, Settings menus
- `seed_user_groups_menu.sql` — Seeds User Groups menu item

## Common Parent Menus

| Name | Typical Route |
|------|--------------|
| Dashboard | / |
| Administration | (container) |
| Settings | (container) |
| Users | /users |

## Notes

- `SortOrder` controls menu display order (lower = higher)
- `ParentId` is NULL for top-level menus
- `IsActive` must be 1 for the menu to appear
- After inserting, refresh the Blazor app to see the new menu
