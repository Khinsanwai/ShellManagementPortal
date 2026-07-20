# Skills Reference — Shell Management Portal

Available slash commands for this project. Type `/skill-name` in Claude Code to use.

## Available Skills

| Command | Description |
|---------|-------------|
| `/new-entity` | Scaffold a complete new entity across all CQRS layers (Domain, Application, Persistence, API, Blazor) |
| `/add-blazor-page` | Add a Blazor List + Form page for an existing entity |
| `/add-api-endpoint` | Add a new API endpoint to an existing or new controller |
| `/add-migration` | Create and apply an EF Core migration |
| `/run-project` | Build and run both API and Web projects |
| `/seed-menu` | Insert a new menu item into the database sidebar |

## How Skills Work

Each skill is a `.md` file in `.claude/commands/`. When you type `/skill-name`, Claude reads the instructions and executes the steps defined in that file.

Skills contain:
- **Input format** — what to pass after the command
- **Steps** — exact file locations, code templates, and commands to run
- **Conventions** — naming patterns, namespaces, and folder structures matching this project

## Example Usage

```
/new-entity Product (Name:string:required, Price:decimal:required, Description:string:optional)
/add-blazor-page Product
/add-api-endpoint POST /api/v1/product/bulk-delete
/add-migration AddProductTable
/run-project
/seed-menu Products (route:/products, parent:Administration, icon:inventory_2)
```

## Creating New Skills

To add a new skill, create a `.md` file in `.claude/commands/`:

```
.claude/commands/my-skill.md
```

The filename (without `.md`) becomes the command name: `/my-skill`

Structure your skill file with:
1. **Title** — what the skill does
2. **Input** — what the user provides
3. **Steps** — numbered instructions with code templates
4. **Notes** — conventions, warnings, or references
